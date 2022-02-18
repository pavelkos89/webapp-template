using ${{ values.component_id }}.Configuration;
using Xunit;

namespace ${{ values.component_id }}.Tests.Configuration
{
	public class ConfigWrapperTests
	{

		[Fact]
		public void Validate_ShouldSucceed_ForValidConfiguration()
		{
			var config = CreateDummyConfiguration();
			var (isValid, errors) = config.IsValid();

			Assert.True(isValid);
			Assert.Empty(errors);
		}

		private static ConfigWrapper CreateDummyConfiguration()
		{
			// TODO: Implement
			return new ConfigWrapper();
		}
	}
}
