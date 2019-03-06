using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Moq;
using PB.ITOps.AspNetCore.Versioning.Tests.Builders;
using Xunit;

namespace PB.ITOps.AspNetCore.Versioning.Tests.IntroducedApiVersionConventionBuilderTests
{
    public class IntroducedApiVersionConventionBuilderTests
    {
        private readonly Mock<ApiVersionConventionBuilder> _apiVersionConventionBuilder;
        private readonly Mock<StubControllerApiVersionConventionBuilder<StubController>> _controllerApiConventionBuilder;
        private readonly Mock<StubActionApiVersionConventionBuilder<StubController>> _actionApiVersionConventionBuilder;
        
        public IntroducedApiVersionConventionBuilderTests()
        {
            _apiVersionConventionBuilder = new Mock<ApiVersionConventionBuilder>();
            _controllerApiConventionBuilder = new Mock<StubControllerApiVersionConventionBuilder<StubController>>();
            _actionApiVersionConventionBuilder = new Mock<StubActionApiVersionConventionBuilder<StubController>>();
            
            _apiVersionConventionBuilder.Setup(a => a.Controller(typeof(StubController)))
                .Returns(_controllerApiConventionBuilder.Object);
        }
        
        [Fact]
        public void GivenControllerDoesNotHaveIntroducedVersion_ThenUseApiConvention()
        {
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            var controllerModel = ControllerModelBuilder.ForController<StubController>().Build();

            var result = builder.ApplyTo(controllerModel);
            
            _apiVersionConventionBuilder.Verify(b => b.ApplyTo(controllerModel), Times.Once);
            _apiVersionConventionBuilder.Verify(b => b.Controller(It.IsAny<TypeInfo>()), Times.Never);
        }

        [Fact]
        public void GivenControllerIsIntroducedInFutureVersion_ThenUseApiConvention()
        {
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            var controllerModel = ControllerModelBuilder.ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(4)
                .Build();

            var result = builder.ApplyTo(controllerModel);
            
            _apiVersionConventionBuilder.Verify(b => b.ApplyTo(controllerModel), Times.Once);
            _apiVersionConventionBuilder.Verify(b => b.Controller(It.IsAny<TypeInfo>()), Times.Never);
        }

        [Fact]
        public void GivenControllerIsRemovedBeforeStartVersion_ThenUseApiConvention()
        {
            var builder = new IntroducedApiVersionConventionBuilder(3, 3, _apiVersionConventionBuilder.Object);
            var controllerModel = ControllerModelBuilder.ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithRemovedAsOfApiVersionAttribute(2)
                .Build();

            var result = builder.ApplyTo(controllerModel);
            
            _apiVersionConventionBuilder.Verify(b => b.ApplyTo(controllerModel), Times.Once);
            _apiVersionConventionBuilder.Verify(b => b.Controller(It.IsAny<TypeInfo>()), Times.Never);
        }

        [Fact]
        public void GivenControllerIsIntroduced_ThenControllerSupportedApiVersionSetToCurrentVersion()
        {
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            var controllerModel = ControllerModelBuilder.ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .Build();
            
            var result = builder.ApplyTo(controllerModel);
            
            Assert.True(result);
            _controllerApiConventionBuilder.Verify(b => b.HasApiVersion(new ApiVersion(3, 0)));
            _controllerApiConventionBuilder.Verify(b => b.HasApiVersion(It.IsAny<ApiVersion>()), Times.Once);
            _apiVersionConventionBuilder.Verify(b => b.ApplyTo(controllerModel), Times.Once);
        }
        
        [Fact]
        public void GivenControllerIsIntroduced_ThenControllerDeprecatedApiVersionSetToPreviousVersions()
        {
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            var controllerModel = ControllerModelBuilder.ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .Build();
            
            var result = builder.ApplyTo(controllerModel);
            
            Assert.True(result);
            _controllerApiConventionBuilder.Verify(b => b.HasDeprecatedApiVersion(new ApiVersion(1, 0)));
            _controllerApiConventionBuilder.Verify(b => b.HasDeprecatedApiVersion(new ApiVersion(2, 0)));
            _controllerApiConventionBuilder.Verify(b => b.HasDeprecatedApiVersion(It.IsAny<ApiVersion>()), Times.Exactly(2));
            _apiVersionConventionBuilder.Verify(b => b.ApplyTo(controllerModel), Times.Once);
        }
        
        [Fact]
        public void GivenControllerIsRemoved_ThenControllerSupportedVersionsEmpty()
        {
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            var controllerModel = ControllerModelBuilder.ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithRemovedAsOfApiVersionAttribute(2)
                .Build();
            
            var result = builder.ApplyTo(controllerModel);
            
            Assert.True(result);
            _controllerApiConventionBuilder.Verify(b => b.HasApiVersion(It.IsAny<ApiVersion>()), Times.Never);
            _apiVersionConventionBuilder.Verify(b => b.ApplyTo(controllerModel), Times.Once);
        }
        
        [Fact]
        public void GivenControllerIsRemoved_ThenControllerDeprecatedVersions()
        {
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            var controllerModel = ControllerModelBuilder.ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithRemovedAsOfApiVersionAttribute(2)
                .Build();
            
            var result = builder.ApplyTo(controllerModel);
            
            Assert.True(result);
            _controllerApiConventionBuilder.Verify(b => b.HasDeprecatedApiVersion(new ApiVersion(1, 0)));
            _controllerApiConventionBuilder.Verify(b => b.HasDeprecatedApiVersion(It.IsAny<ApiVersion>()), Times.Once);
            _apiVersionConventionBuilder.Verify(b => b.ApplyTo(controllerModel), Times.Once);
        }
        
        [Fact]
        public void GivenControllerIsIntroducedTwice_ThenInvalidOperationExceptionIsThrown()
        {
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            var controllerModel = ControllerModelBuilder.ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithIntroducedInApiVersionAttribute(2)
                .Build();
            
            Assert.Throws<InvalidOperationException>(() => builder.ApplyTo(controllerModel));
        }
        
        [Fact]
        public void GivenControllerIsRemovedTwice_ThenInvalidOperationExceptionIsThrown()
        {
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            var controllerModel = ControllerModelBuilder.ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithRemovedAsOfApiVersionAttribute(2)
                .WithRemovedAsOfApiVersionAttribute(3)
                .Build();
            
            Assert.Throws<InvalidOperationException>(() => builder.ApplyTo(controllerModel));
        }
        
        [Fact]
        public void GivenControllerIsRemovedInSameVersionAsIntroduced_ThenInvalidOperationExceptionIsThrown()
        {
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            var controllerModel = ControllerModelBuilder.ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithRemovedAsOfApiVersionAttribute(1)
                .Build();
            
            Assert.Throws<InvalidOperationException>(() => builder.ApplyTo(controllerModel));
        }
        
        [Fact]
        public void GivenControllerIsRemovedBeforeIntroduced_ThenInvalidOperationExceptionIsThrown()
        {
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            var controllerModel = ControllerModelBuilder.ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(2)
                .WithRemovedAsOfApiVersionAttribute(1)
                .Build();
            
            Assert.Throws<InvalidOperationException>(() => builder.ApplyTo(controllerModel));
        }

        [Fact]
        public void GivenControllerIsIntroducedAndActionHasNoAttribute_ThenActionSupportedSetToControllerSupported()
        {
            // Arrange
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            
            var action = ActionModelBuilder
                .WithAction<StubController>(c => c.Get())
                .Build();
            
            var controllerModel = ControllerModelBuilder
                .ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithAction(action)
                .Build();

            _controllerApiConventionBuilder
                .Setup(b => b.Action(action.ActionMethod))
                .Returns(_actionApiVersionConventionBuilder.Object);
            
            // Act
            var result = builder.ApplyTo(controllerModel);
            
            // Assert
            _actionApiVersionConventionBuilder.Verify(b => b.HasApiVersion(new ApiVersion(3, 0)), Times.Once);
        }
        
        [Fact]
        public void GivenControllerIsIntroducedAndActionHasNoAttribute_ThenActionDeprecatedApiVersionSetToPreviousVersions()
        {
            // Arrange
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            
            var action = ActionModelBuilder
                .WithAction<StubController>(c => c.Get())
                .Build();
            
            var controllerModel = ControllerModelBuilder
                .ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithAction(action)
                .Build();

            _controllerApiConventionBuilder
                .Setup(b => b.Action(action.ActionMethod))
                .Returns(_actionApiVersionConventionBuilder.Object);
            
            // Act
            var result = builder.ApplyTo(controllerModel);
            
            // Assert
            _actionApiVersionConventionBuilder.Verify(b => b.HasDeprecatedApiVersion(new ApiVersion(1, 0)), Times.Once);
            _actionApiVersionConventionBuilder.Verify(b => b.HasDeprecatedApiVersion(new ApiVersion(2, 0)), Times.Once);
            _actionApiVersionConventionBuilder.Verify(b => b.HasDeprecatedApiVersion(It.IsAny<ApiVersion>()), Times.Exactly(2));
        }
        
        [Fact]
        public void GivenActionIsRemovedInFutureVersion_ThenActionSupportedApiVersionsSetToCurrent()
        {
            // Arrange
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            
            var action = ActionModelBuilder
                .WithAction<StubController>(c => c.Get())
                .WithRemovedAsOfApiVersionAttribute(4)
                .Build();
            
            var controllerModel = ControllerModelBuilder
                .ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithAction(action)
                .Build();

            _controllerApiConventionBuilder
                .Setup(b => b.Action(action.ActionMethod))
                .Returns(_actionApiVersionConventionBuilder.Object);
            
            // Act
            var result = builder.ApplyTo(controllerModel);
            
            // Assert
            _actionApiVersionConventionBuilder.Verify(b => b.HasApiVersion(new ApiVersion(3, 0)), Times.Once);
            _actionApiVersionConventionBuilder.Verify(b => b.HasApiVersion(It.IsAny<ApiVersion>()), Times.Once);
        }
        
        [Fact]
        public void GivenActionIsIntroducedInNewerVersionThanController_ThenActionSupportedApiVersionsSetToCurrent()
        {
            // Arrange
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            
            var action = ActionModelBuilder
                .WithAction<StubController>(c => c.Get())
                .WithIntroducedInApiVersionAttribute(2)
                .Build();
            
            var controllerModel = ControllerModelBuilder
                .ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithAction(action)
                .Build();

            _controllerApiConventionBuilder
                .Setup(b => b.Action(action.ActionMethod))
                .Returns(_actionApiVersionConventionBuilder.Object);

            // Act
            var result = builder.ApplyTo(controllerModel);
            
            // Assert
            _actionApiVersionConventionBuilder.Verify(b => b.HasApiVersion(new ApiVersion(3, 0)), Times.Once);
            _actionApiVersionConventionBuilder.Verify(b => b.HasApiVersion(It.IsAny<ApiVersion>()), Times.Once);
        }
        
        [Fact]
        public void GivenActionIsIntroducedInNewerVersionThanController_ThenActionDeprecatedApiVersionsSet()
        {
            // Arrange
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            
            var action = ActionModelBuilder
                .WithAction<StubController>(c => c.Get())
                .WithIntroducedInApiVersionAttribute(2)
                .Build();
            
            var controllerModel = ControllerModelBuilder
                .ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithAction(action)
                .Build();

            _controllerApiConventionBuilder
                .Setup(b => b.Action(action.ActionMethod))
                .Returns(_actionApiVersionConventionBuilder.Object);
            
            // Act
            var result = builder.ApplyTo(controllerModel);
            
            // Assert
            _actionApiVersionConventionBuilder.Verify(b => b.HasDeprecatedApiVersion(new ApiVersion(2, 0)), Times.Once);
            _actionApiVersionConventionBuilder.Verify(b => b.HasDeprecatedApiVersion(It.IsAny<ApiVersion>()), Times.Once);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        public void GivenActionIsRemovedInPreviousOrCurrentVersion_ThenActionSupportedApiVersionsSetToEmpty(ushort versionRemoved)
        {
            // Arrange
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            
            var action = ActionModelBuilder
                .WithAction<StubController>(c => c.Get())
                .WithRemovedAsOfApiVersionAttribute(versionRemoved)
                .Build();
            
            var controllerModel = ControllerModelBuilder
                .ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithAction(action)
                .Build();

            _controllerApiConventionBuilder
                .Setup(b => b.Action(action.ActionMethod))
                .Returns(_actionApiVersionConventionBuilder.Object);
            
            // Act
            var result = builder.ApplyTo(controllerModel);
            
            // Assert
            _actionApiVersionConventionBuilder.Verify(b => b.HasApiVersion(It.IsAny<ApiVersion>()), Times.Never);
        }
        
        [Theory]
        [InlineData(2, 1)]
        [InlineData(3, 1, 2)]
        public void GivenActionIsRemovedInPreviousOrCurrentVersion_ThenActionDeprecatedApiVersionsSet(ushort versionRemoved, params int[] deprecatedVersion)
        {
            // Arrange
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            
            var action = ActionModelBuilder
                .WithAction<StubController>(c => c.Get())
                .WithRemovedAsOfApiVersionAttribute(versionRemoved)
                .Build();
            
            var controllerModel = ControllerModelBuilder
                .ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithAction(action)
                .Build();

            _controllerApiConventionBuilder
                .Setup(b => b.Action(action.ActionMethod))
                .Returns(_actionApiVersionConventionBuilder.Object);
            
            // Act
            var result = builder.ApplyTo(controllerModel);
            
            // Assert
            _actionApiVersionConventionBuilder.Verify(b => b.HasDeprecatedApiVersion(It.IsAny<ApiVersion>()), Times.Exactly(deprecatedVersion.Length));
            foreach (var deprecated in deprecatedVersion)
            {
                _actionApiVersionConventionBuilder.Verify(b => b.HasDeprecatedApiVersion(new ApiVersion(deprecated, 0)), Times.Once);    
            }
        }
        
        [Fact]
        public void GivenActionIsRemovedBeforeIntroducedInController_ThenInvalidOperationExceptionIsThrown()
        {
            // Arrange
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            
            var action = ActionModelBuilder
                .WithAction<StubController>(c => c.Get())
                .WithRemovedAsOfApiVersionAttribute(1)
                .Build();
            
            var controllerModel = ControllerModelBuilder
                .ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(2)
                .WithAction(action)
                .Build();

            _controllerApiConventionBuilder
                .Setup(b => b.Action(action.ActionMethod))
                .Returns(_actionApiVersionConventionBuilder.Object);
            
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => builder.ApplyTo(controllerModel));
        }
        
        [Fact]
        public void GivenActionIsRemovedSameVersionIntroducedInController_ThenInvalidOperationExceptionIsThrown()
        {
            // Arrange
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            
            var action = ActionModelBuilder
                .WithAction<StubController>(c => c.Get())
                .WithRemovedAsOfApiVersionAttribute(1)
                .Build();
            
            var controllerModel = ControllerModelBuilder
                .ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithAction(action)
                .Build();

            _controllerApiConventionBuilder
                .Setup(b => b.Action(action.ActionMethod))
                .Returns(_actionApiVersionConventionBuilder.Object);
            
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => builder.ApplyTo(controllerModel));
        }
        
        [Fact]
        public void GivenActionIsRemovedSameVersionIntroducedInAction_ThenInvalidOperationExceptionIsThrown()
        {
            // Arrange
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            
            var action = ActionModelBuilder
                .WithAction<StubController>(c => c.Get())
                .WithIntroducedInApiVersionAttribute(2)
                .WithRemovedAsOfApiVersionAttribute(2)
                .Build();
            
            var controllerModel = ControllerModelBuilder
                .ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithAction(action)
                .Build();

            _controllerApiConventionBuilder
                .Setup(b => b.Action(action.ActionMethod))
                .Returns(_actionApiVersionConventionBuilder.Object);
            
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => builder.ApplyTo(controllerModel));
        }
        
        [Fact]
        public void GivenActionIsRemovedBeforeIntroducedInAction_ThenInvalidOperationExceptionIsThrown()
        {
            // Arrange
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            
            var action = ActionModelBuilder
                .WithAction<StubController>(c => c.Get())
                .WithRemovedAsOfApiVersionAttribute(2)
                .WithIntroducedInApiVersionAttribute(3)
                .Build();
            
            var controllerModel = ControllerModelBuilder
                .ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(1)
                .WithAction(action)
                .Build();

            _controllerApiConventionBuilder
                .Setup(b => b.Action(action.ActionMethod))
                .Returns(_actionApiVersionConventionBuilder.Object);
            
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => builder.ApplyTo(controllerModel));
        }

        [Fact]
        public void GivenActionIsIntroducedBeforeIntroducedInController_ThenInvalidOperationExceptionIsThrown()
        {
            // Arrange
            var builder = new IntroducedApiVersionConventionBuilder(1, 3, _apiVersionConventionBuilder.Object);
            
            var action = ActionModelBuilder
                .WithAction<StubController>(c => c.Get())
                .WithIntroducedInApiVersionAttribute(1)
                .Build();
            
            var controllerModel = ControllerModelBuilder
                .ForController<StubController>()
                .WithIntroducedInApiVersionAttribute(2)
                .WithAction(action)
                .Build();

            _controllerApiConventionBuilder
                .Setup(b => b.Action(action.ActionMethod))
                .Returns(_actionApiVersionConventionBuilder.Object);
            
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => builder.ApplyTo(controllerModel));
        }
    }

    public class StubControllerApiVersionConventionBuilder<T> : ControllerApiVersionConventionBuilder
    {
        public StubControllerApiVersionConventionBuilder() : base(typeof(T))
        {
        }
    }

    public class StubActionApiVersionConventionBuilder<T> : ActionApiVersionConventionBuilder
    {
        public StubActionApiVersionConventionBuilder() : base(new ControllerApiVersionConventionBuilder(typeof(T)))
        {
        }
    }

    public class StubController : ControllerBase
    {
        public ActionResult Get()
        {
            return Ok();
        }
    }
}