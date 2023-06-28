using Xunit;
using WonderfulRabbitsApi.Helpers;

namespace WonderfulRabbitsApiTests
{
    public class AutoMapperTests
    {
        [Fact]
        public void Configure_WhenAutoMapperIsConfigured_AllProfilesShouldBeAddedCorrectly()
        {
            var config = AutoMapperConfiguration.Configure();

            config.AssertConfigurationIsValid();
        }
    }
}