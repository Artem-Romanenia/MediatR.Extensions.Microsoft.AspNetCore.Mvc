using Mediatr.Extensions.Microsoft.AspNetCore.Mvc;
using Mediatr.Extensions.Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc.Tests
{
    [TestClass]
    public class ConventionTests
    {
        private Func<Type, string> _provideControllerName;
        private Func<Type, RequestType> _classifyRequestTypeQuery;
        private Func<Type, RequestType> _classifyRequestTypeCommand;
        private Func<Type, RequestType> _classifyRequestTypeCommandDelete;

        public ConventionTests()
        {
            _provideControllerName = (requestType) =>
            {
                if (requestType.Name.EndsWith("Request"))
                {
                    return requestType.Name.Replace("Request", string.Empty);
                }

                return requestType.Name;
            };

            _classifyRequestTypeQuery = (requestType) => RequestType.Query;
            _classifyRequestTypeCommand = (requestType) => RequestType.Command;
            _classifyRequestTypeCommandDelete = (requestType) => RequestType.DeleteCommand;
        }

        [TestMethod]
        public void ControllerNameSetCorrectly()
        {
            foreach(var @case in new[] {
                new { callback = (Func<Type, string>)null, Name = "GetTestDataRequest" },
                new { callback = _provideControllerName, Name = "GetTestData" }
            })
            {
                var controllerModel = GetControllerModel();

                var convention = new Convention(@case.callback, null);
                convention.Apply(controllerModel);
                Assert.AreEqual(@case.Name, controllerModel.ControllerName);
            }
        }

        [TestMethod]
        public void HttpMethodConstraintsAppliedCorrectly()
        {
            foreach (var @case in new[] {
                new { Verb = "GET", callback = _classifyRequestTypeQuery, existingConstraints = new string[0] },
                new { Verb = "POST", callback = _classifyRequestTypeCommand, existingConstraints = new string[0] },
                new { Verb = "GET", callback = _classifyRequestTypeQuery, existingConstraints = new [] { "DELETE" } },
                new { Verb = "POST", callback = _classifyRequestTypeCommand, existingConstraints = new [] { "DELETE" } },
                new { Verb = "DELETE", callback = _classifyRequestTypeCommand, existingConstraints = new [] { "DELETE" } },
            })
            {
                var controllerModel = GetControllerModel(@case.existingConstraints);

                var convention = new Convention(null, @case.callback);
                convention.Apply(controllerModel);
                Assert.AreEqual(1, controllerModel.Actions.Count);

                var actionModel = controllerModel.Actions.Single();
                Assert.AreEqual(1, actionModel.Selectors.Count);

                var selector = actionModel.Selectors.Single();
                Assert.AreEqual(1, selector.ActionConstraints.Count(c => c.GetType() == typeof(HttpMethodActionConstraint)));

                var actionConstraint = (HttpMethodActionConstraint)selector.ActionConstraints.Single(c => c.GetType() == typeof(HttpMethodActionConstraint));
                Assert.AreEqual(1, actionConstraint.HttpMethods.Count(m => m == @case.Verb));

                foreach(var httpConstraint in @case.existingConstraints)
                {
                    Assert.AreEqual(1, actionConstraint.HttpMethods.Count(m => m == httpConstraint));
                }
            }
        }

        private ControllerModel GetControllerModel(params string[] httpMethods)
        {
            var controllerModel = new ControllerModel(typeof(MediatrMvcGenericController<GetTestDataRequest, string>).GetTypeInfo(), new List<object>());
            var actionModel = new ActionModel(typeof(MediatrMvcGenericController<GetTestDataRequest, string>).GetMethod("Index"), new List<object>());
            var selectorModel = new SelectorModel();

            if (httpMethods != null && httpMethods.Count() > 0)
            {
                selectorModel.ActionConstraints.Add(new HttpMethodActionConstraint(httpMethods));
            }

            actionModel.Selectors.Add(selectorModel);
            controllerModel.Actions.Add(actionModel);
            return controllerModel;
        }
    }
}
