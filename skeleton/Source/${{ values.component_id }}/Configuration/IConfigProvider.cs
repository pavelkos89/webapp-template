using Elements.LoggingContract;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ${{ values.component_id }}.Configuration
{
	public interface IConfigProvider
	{
		Task<LogLevel> GetLogLevelAsync(HttpContext context);
		Task<string> GetTenantIdAsync(HttpContext context);
		Task<ConfigWrapper> GetConfigurationAsync(HttpContext context);
		Task<ConfigWrapper> GetConfigurationAsync(string tenant);
	}
}
