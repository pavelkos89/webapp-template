using Elements.Security.Token.Jwt;
using ${{ values.component_id }}.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace ${{ values.component_id }}
{
	public class ValidationOptionsProvider : IValidationOptionsProvider

	{
		private readonly IConfigProvider _configProvider;

		public ValidationOptionsProvider(IConfigProvider configProvider)
		{
			_configProvider = configProvider;
		}

		public TokenValidationOptions GetTokenValidationOptions(JwtSecurityToken accessToken)
		{
			var config = GetConfiguration(accessToken, _configProvider);

			var issuers = config.Federation.GetAllIssuers(config.Scope, config.Id);
			var issuer = issuers.FirstOrDefault(x => x.Id == accessToken.Issuer);

			return new TokenValidationOptions
			{
				AllowedAudiences = issuer.AllowedAudiences.Select(x => x.Id),
				IssuerDiscoveryUrl = issuer.DiscoveryUrl
			};
		}

		private static ConfigWrapper GetConfiguration(JwtSecurityToken accessToken, IConfigProvider configProvider)
		{
			var tenantId = JwtTokenHelper.GetTenant(accessToken);
			return configProvider.GetConfigurationAsync(tenantId).Result;
		}
	}
}
