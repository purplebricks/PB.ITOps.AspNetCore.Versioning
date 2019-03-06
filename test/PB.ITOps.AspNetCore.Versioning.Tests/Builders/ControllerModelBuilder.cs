using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace PB.ITOps.AspNetCore.Versioning.Tests.Builders
{
    public class ControllerModelBuilder
    {
        private readonly List<object> _attributes = new List<object>();
        private readonly List<ActionModel> _actions = new List<ActionModel>();
        private readonly TypeInfo _typeInfo;

        private ControllerModelBuilder(TypeInfo typeInfo)
        {
            _typeInfo = typeInfo;
        }
        
        public static ControllerModelBuilder ForController<T>()
        {
            return new ControllerModelBuilder(typeof(T).GetTypeInfo());
        }

        public ControllerModelBuilder WithIntroducedInApiVersionAttribute(ushort version)
        {
            _attributes.Add(new IntroducedInApiVersionAttribute(version));
            return this;
        }
        
        public ControllerModelBuilder WithRemovedAsOfApiVersionAttribute(ushort version)
        {
            _attributes.Add(new RemovedAsOfApiVersionAttribute(version));
            return this;
        }

        public ControllerModelBuilder WithAction(ActionModel action)
        {
            _actions.Add(action);
            return this;
        }

        public ControllerModel Build()
        {
            var model = new ControllerModel(_typeInfo, _attributes);
            foreach (var action in _actions)
            {
                model.Actions.Add(action);
            }

            return model;
        }
    }
}