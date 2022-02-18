using Elements.Logging.Tracing;
using ${{ values.component_id }}.Configuration;
using System;
using System.Collections.Generic;

namespace ${{ values.component_id }}.Helpers
{
	public static class LoggingHelper
	{
		public static void InitializeLogging()
		{
			Tracer.EnableLogging(Constants.ServiceName);

			AppDomain.CurrentDomain.UnhandledException += (o, arg) =>
			{
				var errorMessage = $"Unhandled exception occured in '{AppDomain.CurrentDomain.FriendlyName}'{Environment.NewLine}";
				errorMessage += arg.IsTerminating ? "Terminate application" : "Application will not terminate";

				Tracer.TraceErrorMessage((Exception)arg.ExceptionObject, errorMessage);
			};
		}

		public static void StartLoggingOperation(ConfigWrapper config, IDictionary<string, string[]> headers)
		{
			// TODO: Fix implementation
			//Tracer.StartLoggingOperation(new ElementsLogContext(config?.Id ?? "(no tenant)", config?.MyMainSchema?.LogLevel ?? Tracer.DefaultLogLevel, headers));
			Tracer.StartLoggingOperation(new ElementsLogContext(config?.Id ?? "(no tenant)", Tracer.DefaultLogLevel, headers));
		}
	}
}
