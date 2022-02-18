module.exports = function (grunt) {
	const solutionPath = "Source/${{ values.component_id }}.sln";
	const packageName = "${{ values.component_id }}";
	const appPath = "Source/${{ values.component_id }}/${{ values.component_id }}.csproj";
	const coberturaReportFiles = grunt.file.expand({ filter: "isFile" }, ["Source/*.Tests/TestResults/coverage.cobertura.xml"]).join(';');
	const getBuildContainerParams = () => {
		return {
			options: { stdout: true },
			command: `powershell -NonInteractive -ExecutionPolicy Bypass Source\\Docker\\container-local.ps1 -operation "build" < NUL`
		}
	}

	const getPushContainerParams = () => {
		return {
			options: { stdout: true },
			command: `powershell -NonInteractive -ExecutionPolicy Bypass Source\\Docker\\container-local.ps1 -operation "push" < NUL`
		}
	}

	grunt.initConfig({
		pkg: grunt.file.readJSON("package.json"),
		cleanfiles: {
			basic: [
				"**/TestResults/**",
				"CoverageReport/**",
				["Source/**/bin/**/*.*", "!Source/**/bin/**/*vshost*"],  // sometimes VS opens and locks some vshost files, so we don't try to delete them
				"Source/**/obj/**",
				"Source/Docker/install/choco/**",
				"Source/Chocolatey/Artifacts/**",
				"Source/Chocolatey/Temp/**",
				"dist/**"
			],
			options: {
				folders: true
			}
		},
		shell: {
			restore: {
				command: `dotnet restore ${solutionPath}`
			},
			test: {
				command: `dotnet test ${solutionPath} --filter Category!=Live --logger:trx;LogFileName=TestResultVSTest.trx;verbosity=minimal /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/`
			},
			publishLinux: {
				command:
					`dotnet publish ${appPath} --configuration Release -p:MvcRazorCompileOnPublish=false --runtime debian.10-x64 --self-contained=true /p:PackageAsSingleFile=false --output Source/${packageName}/bin/Release/PublishOutput/Linux`
			},
			publishWebdeploy: {
				command:
					`dotnet publish ${appPath} --configuration Release -p:MvcRazorCompileOnPublish=false /p:PublishProfile=WebDeployPackage /p:WebPublishMethod=Package /p:PackageAsSingleFile=true /p:PackageLocation=../WebDeployPackages --output Source/${packageName}/bin/Release/PublishOutput/WebDeploy/${packageName}`
			},
			buildDebug: {
				command: `dotnet build ${solutionPath} --configuration Debug`
			},
			buildRelease: {
				command: `dotnet build ${solutionPath} --configuration Release`
			},
			chocoBuild: {
				options: { stdout: true },
				command:
					`powershell -ExecutionPolicy Bypass Source/Shared.Chocolatey/CreatePackage.ps1  -PackageName ${packageName} -DeployType 'WebApp' < NUL`
			},
			buildContainer: getBuildContainerParams(),
			pushContainer: getPushContainerParams(),
			generateCoverageReport: {
				command:
					"reportgenerator.exe -reports:" + coberturaReportFiles + " -targetdir:CoverageReport"
			}
		},
		nucheck: {
			all: {
				src: solutionPath
			}
		},
		copy: {
			dist: {
				files: [
					{
						expand: true,
						cwd: "Source/Chocolatey/Artifacts",
						src: ["**"],
						dest: "dist/Chocolatey/"
					},
					{
						expand: true,
						cwd: "Source/Chocolatey/InstallationHelpers",
						src: ["**"],
						dest: "dist/Chocolatey/"
					},
					{
						expand: true,
						cwd: "Documentation",
						src: ["**"],
						dest: "dist/"
					},
					{
						expand: true,
						cwd: "Source/Docker",
						src: ["Dockerfile"],
						dest: "dist/"
					},
					{
						expand: true,
						cwd: `Source/${packageName}/bin/Release/PublishOutput/Linux`,
						src: ["**/*"],
						dest: "dist/PublishOutput/Linux"
					}
				]
			}
		}
	});

	// Every package defined in package.json, also have to be loaded here
	grunt.loadNpmTasks("grunt-nucheck");
	grunt.loadNpmTasks("grunt-shell");
	grunt.loadNpmTasks("grunt-contrib-copy");
	grunt.loadNpmTasks("grunt-contrib-clean");

	grunt.renameTask("clean", "cleanfiles"); //Rename so that task-name "clean" can be used as "main" clean

	// Default task(s).
	grunt.registerTask("clean", ["cleanfiles"]);
	grunt.registerTask("chocoBuild", ["shell:publishWebdeploy", "shell:chocoBuild"]);
	grunt.registerTask("buildContainer", ["shell:buildContainer"]);
	grunt.registerTask("pushContainer", ["shell:pushContainer"]);
	grunt.registerTask("publish", ["shell:publishLinux", "chocoBuild"]);
	grunt.registerTask("test", ["clean", "nucheck", "shell:buildDebug", "shell:test"]);
	grunt.registerTask("default", ["clean", "test", "publish", "copy:dist"]);
	grunt.registerTask("coverage", ["test", "shell:generateCoverageReport"]);
};
