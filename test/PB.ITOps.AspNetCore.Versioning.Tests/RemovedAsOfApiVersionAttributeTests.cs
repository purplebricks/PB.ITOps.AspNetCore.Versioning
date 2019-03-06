using Xunit;

namespace PB.ITOps.AspNetCore.Versioning.Tests
{
    public class RemovedAsOfApiVersionAttributeTests
    {
        [Theory]
        [InlineData(0, "0.0")]
        [InlineData(1, "1.0")]
        [InlineData(10, "10.0")]
        public void GivenARemovedAsOfApiVersion_WhenInstantiated_ThenMajorVersionIsSet(ushort version, string expected) 
        {
            var actual = new IntroducedInApiVersionAttribute(version);
            
            Assert.Equal(expected, actual.Version.ToString());
        }
    }
}