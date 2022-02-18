using ConfigServer.Schema.Federation;
using Elements.ConfigServer.Client;
using Elements.ConfigServer.Client.ConfigurationManager;
using Elements.ConfigServer.Client.Entities;
using Elements.ConfigServer.Client.Extensions;
using Elements.Logging;
using Elements.Logging.Tracing;
using Elements.LoggingContract;
using Elements.Security.Token.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ${{ values.component_id }}.Configuration
{
	public class ConfigProvider : IConfigProvider
	{
		private static readonly IConfigManager<ConfigWrapper> _configManager;
		private static readonly IConfigurationRoot _appsettings;

		static ConfigProvider()
		{
			_appsettings = new ConfigurationBuilder()
					.AddJsonFile("appsettings.json", false, true)
					.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
					.Build();

			_configManager = ConfigManagerFactory<ConfigWrapper>.CreateConfigManager(
					key => _appsettings[$"Appsettings:{key}"],
					Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"),
					new List<SchemaEntry>
					{
						SchemaEntry.Create<FederationConfig>()
						//SchemaEntry.Create<SomeSchema>() //TODO: Add schemas here
					},
					shouldSendSchemas: true,
					headersToUpdate: new Dictionary<string, Func<string>>
					{
						{ LoggingMiddleware.RelatedModulesHeaderName, () => Constants.ServiceName }
					},
					onCreationFailed: ex =>
						Tracer.TraceErrorMessage(ex, "Failed to get configuration from ConfigServer"),
					enableCaching: true);
		}

		public static string GetPathBase()
		{
			return _appsettings[$"Appsettings:PathBase"];
		}

		public async Task SubscribeAsync(Action<Config<ConfigWrapper>> onConfigChanged, Action<Exception> onError, CancellationToken token)
		{
			using var tracer = Tracer.TraceMethod(LogLevel.ExternalCall, $"{nameof(ConfigProvider)}.{nameof(SubscribeAsync)}");
			await Task.Run(() => _configManager.Subscribe(onConfigChanged, onError, token), token);
		}

		public async Task<ConfigWrapper> GetConfigurationAsync(string tenant)
		{
			return !string.IsNullOrEmpty(tenant)
				? await _configManager.GetTenantConfigAsync(tenant)
				: await _configManager.GetGlobalConfigAsync();
		}

		public async Task<ConfigWrapper> GetConfigurationAsync(HttpContext context)
		{
			return JwtTokenHelper.TryGetAccessToken(context?.Request, out var accessToken)
				? await GetConfigViaTokenAsync(accessToken)
				: await _configManager.GetGlobalConfigAsync();
		}

		public async Task<LogLevel> GetLogLevelAsync(HttpContext context)
		{
			var config = await GetConfigurationAsync(context);
			// TODO: Implement
			// return config.MyMainSchema.LogLevel ?? LogLevel.Error;
			return LogLevel.Error;
		}

		public async Task<string> GetTenantIdAsync(HttpContext context)
		{
			var config = await GetConfigurationAsync(context);
			return config.Id;
		}

		private async Task<ConfigWrapper> GetConfigViaTokenAsync(JwtSecurityToken accessToken)
		{
			var tenantId = JwtTokenHelper.GetTenant(accessToken);
			return await GetConfigurationAsync(tenantId);
		}
	}
}
