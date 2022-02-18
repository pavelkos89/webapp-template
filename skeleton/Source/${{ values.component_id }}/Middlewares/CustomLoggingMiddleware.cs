using Elements.Logging.AspNetCore;
using Elements.LoggingContract;
using ${{ values.component_id }}.Configuration;
using Microsoft.AspNetCore.Http;
using System;

namespace ${{ values.component_id }}.Middlewares
{
	public class CustomLoggingMiddleware : AspNetCoreLoggerMiddleware
	{
		private readonly IConfigProvider _configProvider;

		public CustomLoggingMiddleware(IConfigProvider configProvider, RequestDelegate next)
			: base(next)
		{
			_configProvider = configProvider ?? throw new ArgumentNullException(nameof(configProvider));
		}

		protected override LogLevel GetLogLevel(HttpContext context) => _configProvider.GetLogLevelAsync(context).Result;

		protected override string GetTenantId(HttpContext context) => _configProvider.GetTenantIdAsync(context).Result;
	}
}
