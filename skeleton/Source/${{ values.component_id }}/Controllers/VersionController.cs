using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ${{ values.component_id }}.Helpers;
using System.Reflection;

namespace ${{ values.component_id }}.Controllers
{
	[Route("api/version")]
	[AllowAnonymous]
	public class VersionController : ControllerBase
	{
		private readonly VersionInfo _versionInfo = new(Assembly.GetExecutingAssembly());

		[HttpGet]
		public VersionInfo Get()
		{
			return _versionInfo;
		}
	}
}
