using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace PB.ITOps.AspNetCore.Versioning.Tests.Builders
{
    public class ActionModelBuilder
    {
        private readonly List<object> _attributes = new List<object>();
        private readonly MethodInfo _methodInfo;

        private ActionModelBuilder(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
        }

        public static ActionModelBuilder WithAction<T>(Expression<Action<T>> expression)
        {
            if (expression.Body is MethodCallExpression member)
                return new ActionModelBuilder(member.Method);

            throw new ArgumentException("Expression is not a method", nameof(expression));
        }
            
        public ActionModelBuilder WithIntroducedInApiVersionAttribute(ushort version)
        {
            _attributes.Add(new IntroducedInApiVersionAttribute(version));
            return this;
        }
            
        public ActionModelBuilder WithRemovedAsOfApiVersionAttribute(ushort version)
        {
            _attributes.Add(new RemovedAsOfApiVersionAttribute(version));
            return this;
        }

        public ActionModel Build()
        {
            return new ActionModel(_methodInfo, _attributes);
        }
    }
}