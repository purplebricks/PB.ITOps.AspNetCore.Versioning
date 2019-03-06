using System;
using Xunit;

namespace PB.ITOps.AspNetCore.Versioning.Tests.IntroducedApiVersionConventionBuilderTests
{
    public class ConstructorTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void GivenApiStartVersionGteZero_ThenArgumentExceptionNotThrown(ushort startVersion)
        {
            var actual = new IntroducedApiVersionConventionBuilder(startVersion, 1);
        }

        [Fact]
        public void GivenCurrentApiVersionLtStartApiVersion_ThenArgumentExceptionThrown()
        {
            Assert.Throws<ArgumentException>(() => new IntroducedApiVersionConventionBuilder(1, 0));
        }
        
        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        public void GivenCurrentApiVersionGteStartApiVersion_ThenArgumentExceptionNotThrown(ushort startApiVersion, ushort currentApiVersion)
        {
            var actual = new IntroducedApiVersionConventionBuilder(startApiVersion, currentApiVersion);
        }

        [Theory]
        [InlineData(0, 0, 1)]
        [InlineData(0, 1, 2)]
        [InlineData(1, 5, 5)]
        public void GivenValidVersions_ThenAllVersionsIsSet(ushort startApiVersion, ushort currentApiVersion, ushort expectedCount)
        {
            var actual = new IntroducedApiVersionConventionBuilder(startApiVersion, currentApiVersion);
            
            Assert.Equal(expectedCount, actual.AllVersions.AllVersions.Count);
        }
    }
}