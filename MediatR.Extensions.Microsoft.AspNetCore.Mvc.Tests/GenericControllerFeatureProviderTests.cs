using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Extensions.Microsoft.AspNetCore.Mvc.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc.Tests
{
    [TestClass]
    public class GenericControllerFeatureProviderTests
    {
        private readonly Func<Type, Type> _provideGenericControllerType = type => (Type)null;
        private readonly Func<Type, Type> _provideGenericControllerType2 = type => typeof(MediatrMvcGenericController<,>);
        private readonly Func<Type, Type> _provideGenericControllerTypeConstructed = type => typeof(MediatrMvcGenericController<IRequest<string>, string>);
        private readonly Func<Type, Type> _provideGenericControllerTypeExtended = type => typeof(ExtendedMediatrMvcGenericController<,>);
        private readonly Func<Type, Type> _provideGenericControllerTypeReverted = type => typeof(RevertedParamsMediatrMvcGenericController<,>);
        private readonly Func<Type, Type> _provideGenericControllerTypeSingleParam = type => typeof(SingleParamMediatrMvcGenericController<>);
        private readonly Func<Type, Type> _provideGenericControllerTypeBad = type => typeof(BadMediatrMvcGenericController<,,>);
        private readonly Func<Type, Type> _provideGenericControllerTypeExtendedConstructed = type => typeof(ExtendedMediatrMvcGenericController<IRequest<string>, string>);
        private readonly Func<Type, Type> _provideNonGenericControllerType = type => typeof(Controller);
        private readonly Func<Type, Type> _provideGenericControllerInterface = type => typeof(IMediatrMvcGenericController<>);

        [TestMethod]
        public void GenericControllerTypeProvisionValidatedCorrectly()
        {
            foreach (var @case in new[] {
                new { callback = _provideGenericControllerType, shouldFail = false },
                new { callback = _provideGenericControllerType2, shouldFail = false },
                new { callback = _provideGenericControllerTypeConstructed, shouldFail = true },
                new { callback = _provideGenericControllerTypeExtended, shouldFail = false },
                new { callback = _provideGenericControllerTypeExtendedConstructed, shouldFail = true },
                new { callback = _provideNonGenericControllerType, shouldFail = true },
                new { callback = _provideGenericControllerInterface, shouldFail = true }
            })
            {
                var services = GetServiceCollection();
                var featureProvider = new MediatrMvcFeatureProvider(services, @case.callback);
                var controllerFeature = new ControllerFeature();

                if (@case.shouldFail)
                    Assert.ThrowsException<InvalidTypeException>(() => featureProvider.PopulateFeature(null, controllerFeature));
                else
                {
                    featureProvider.PopulateFeature(null, controllerFeature);
                    Assert.AreEqual(5, controllerFeature.Controllers.Count);
                }
            }
        }

        [TestMethod]
        public void GenericControllerPolymorphicTypeProvisionValidatedCorrectly()
        {
            var services = GetServiceCollection();
            var featureProvider = new ExtendedGenericControllerFeatureProvider2(services, _provideGenericControllerType2);
            var controllerFeature = new ControllerFeature();

            featureProvider.PopulateFeature(null, controllerFeature);
            Assert.AreEqual(5, controllerFeature.Controllers.Count);
        }

        [TestMethod]
        public void GenericControllersConstructedCorrectly()
        {
            foreach (var @case in new[] {
                new { callback = _provideGenericControllerType, expectedType = typeof(MediatrMvcGenericController<,>) },
                new { callback = _provideGenericControllerType2, expectedType = typeof(MediatrMvcGenericController<,>) },
                new { callback = _provideGenericControllerTypeExtended, expectedType = typeof(ExtendedMediatrMvcGenericController<,>) },
                //new { callback = _provideGenericControllerTypeReverted, expectedType = typeof(RevertedParamsMediatrMvcGenericController<,>) }
            })
            {
                var services = GetServiceCollection();
                var featureProvider = new MediatrMvcFeatureProvider(services, @case.callback);
                var controllerFeature = new ControllerFeature();

                featureProvider.PopulateFeature(null, controllerFeature);
                Assert.AreEqual(5, controllerFeature.Controllers.Count);

                Assert.IsTrue(controllerFeature.Controllers.All(c =>
                    c.IsConstructedGenericType && c.GetGenericTypeDefinition() == @case.expectedType));

                Assert.IsTrue(controllerFeature.Controllers.Any(c =>
                    c.GenericTypeArguments[0] == typeof(GetTestDataRequest) &&
                    c.GenericTypeArguments[1] == typeof(string)));

                Assert.IsTrue(controllerFeature.Controllers.Any(c =>
                    c.GenericTypeArguments[0] == typeof(GetTestDataRequest2) &&
                    c.GenericTypeArguments[1] == typeof(int)));

                Assert.IsTrue(controllerFeature.Controllers.Any(c =>
                    c.GenericTypeArguments[0] == typeof(GetTestDataRequest3) &&
                    c.GenericTypeArguments[1] == typeof(object)));

                Assert.IsTrue(controllerFeature.Controllers.Any(c =>
                    c.GenericTypeArguments[0] == typeof(GetTestDataRequest4) &&
                    c.GenericTypeArguments[1] == typeof(Unit)));

                Assert.IsTrue(controllerFeature.Controllers.Any(c =>
                    c.GenericTypeArguments[0] == typeof(GetTestDataRequest5) &&
                    c.GenericTypeArguments[1] == typeof(Unit)));
            }
        }

        [TestMethod]
        public void GenericControllersConstructedCorrectlyForIRequest()
        {
            var services = new ServiceCollection();

            services.AddTransient<IRequestHandler<GetTestDataRequest5, Unit>, GetTestDataRequest5Handler>();

            var featureProvider = new MediatrMvcFeatureProvider(services, _provideGenericControllerTypeSingleParam);
            var controllerFeature = new ControllerFeature();

            featureProvider.PopulateFeature(null, controllerFeature);
            Assert.AreEqual(1, controllerFeature.Controllers.Count);

            Assert.IsTrue(controllerFeature.Controllers.All(c =>
                c.IsConstructedGenericType && c.GetGenericTypeDefinition() == typeof(SingleParamMediatrMvcGenericController<>)));
        }

        [TestMethod]
        public void GenericControllersConstructionFailsGracefully()
        {
            foreach (var @case in new[] {
                new { callback = _provideGenericControllerTypeSingleParam, expectedType = typeof(MediatrMvcGenericController<,>) },
                new { callback = _provideGenericControllerTypeBad, expectedType = typeof(RevertedParamsMediatrMvcGenericController<,>) }
            })
            {
                var services = GetServiceCollection();
                var featureProvider = new MediatrMvcFeatureProvider(services, @case.callback);
                var controllerFeature = new ControllerFeature();

                Assert.ThrowsException<InvalidTypeException>(() => featureProvider.PopulateFeature(null, controllerFeature));
            }
        }

        [TestMethod]
        public void HandledRequestDiscoveryEnabledByDefault()
        {
            foreach(var @case in new[] {
                new { controllerType = typeof(ControllerWithHandledRequest), requestToBeSkipped = typeof(GetTestDataRequest3) },
                new { controllerType = typeof(ControllerWithHandledRequestAttr), requestToBeSkipped = typeof(GetTestDataRequest2) }
            })
            {
                var services = GetServiceCollection();
                var featureProvider = new MediatrMvcFeatureProvider(services);
                var controllerFeature = new ControllerFeature();

                controllerFeature.Controllers.Add(@case.controllerType.GetTypeInfo());

                featureProvider.PopulateFeature(null, controllerFeature);

                Assert.AreEqual(5, controllerFeature.Controllers.Count);
                Assert.IsTrue(controllerFeature.Controllers.All(c => !c.IsGenericType || c.GenericTypeArguments[0] != @case.requestToBeSkipped));
            }
        }

        [TestMethod]
        public void HandledRequestDiscoveryDisabledWhenConfigured()
        {
            foreach (var @case in new[]
            {
                new { controllerType = typeof(ControllerWithHandledRequest), requestToBeSkipped = typeof(GetTestDataRequest3) },
                new { controllerType = typeof(ControllerWithHandledRequestAttr), requestToBeSkipped = typeof(GetTestDataRequest2) }
            })
            {
                var services = GetServiceCollection();
                var featureProvider = new MediatrMvcFeatureProvider(services, applySettings: settings => settings.DisableHandledRequestDiscovery());
                var controllerFeature = new ControllerFeature();

                controllerFeature.Controllers.Add(@case.controllerType.GetTypeInfo());

                featureProvider.PopulateFeature(null, controllerFeature);
                Assert.AreEqual(6, controllerFeature.Controllers.Count);
            }
        }

        [TestMethod]
        public void RequestsAreSkippedWhenConfigured()
        {
            var services = GetServiceCollection().AddTransient<IRequestHandler<BadTestDataRequest3, object>, BadTestDataRequest3Handler>();
            var featureProvider = new ExtendedGenericControllerFeatureProvider(services, type => typeof(MediatrMvcGenericController<,>));
            var controllerFeature = new ControllerFeature();

            featureProvider.PopulateFeature(null, controllerFeature);
            Assert.AreEqual(5, controllerFeature.Controllers.Count);
        }

        private IServiceCollection GetServiceCollection()
        {
            var services = new ServiceCollection();

            services.AddTransient<IRequestHandler<GetTestDataRequest, string>, GetTestDataRequestHandler>();
            services.AddTransient<IRequestHandler<GetTestDataRequest2, int>, GetTestDataRequest2Handler>();
            services.AddTransient<IRequestHandler<GetTestDataRequest3, object>, GetTestDataRequest3Handler>();
            services.AddTransient<IRequestHandler<GetTestDataRequest4, Unit>, GetTestDataRequest4Handler>();
            services.AddTransient<IRequestHandler<GetTestDataRequest5, Unit>, GetTestDataRequest5Handler>();

            return services;
        }

        #region Dummies

        private class ExtendedGenericControllerFeatureProvider : MediatrMvcFeatureProvider
        {
            public ExtendedGenericControllerFeatureProvider(IServiceCollection services, Func<Type, Type> provideGenericControllerType) : base(services, provideGenericControllerType,
                settings => settings.DisableHandledRequestDiscovery())
            { }

            protected override bool ShouldSkip(Type requestType)
            {
                return requestType.Name.StartsWith("Bad");
            }
        }

        private class ExtendedGenericControllerFeatureProvider2 : MediatrMvcFeatureProvider
        {
            private readonly Func<Type, Type> _provideGenericControllerType;

            public ExtendedGenericControllerFeatureProvider2(IServiceCollection services, Func<Type, Type> provideGenericControllerType) : base(services)
            {
                _provideGenericControllerType = provideGenericControllerType;
            }

            protected override Type ProvideGenericControllerType(Type requestType)
            {
                return _provideGenericControllerType(requestType);
            }
        }

        private class ExtendedMediatrMvcGenericController<TRequest, TResponse> : MediatrMvcGenericController<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            public ExtendedMediatrMvcGenericController(IMediator mediator) : base(mediator)
            { }
        }

        private class RevertedParamsMediatrMvcGenericController<TResponse, TRequest> : MediatrMvcGenericController<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            public RevertedParamsMediatrMvcGenericController(IMediator mediator) : base(mediator)
            { }
        }

        private class SingleParamMediatrMvcGenericController<TRequest> : MediatrMvcGenericController<TRequest, Unit>
            where TRequest : IRequest
        {
            public SingleParamMediatrMvcGenericController(IMediator mediator) : base(mediator)
            {
            }
        }

        private class BadMediatrMvcGenericController<TRequest, TResponse, TSomethingElse> : MediatrMvcGenericController<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            public BadMediatrMvcGenericController(IMediator mediator) : base(mediator)
            {
            }
        }

        private class ControllerWithHandledRequest : Controller
        {
            public IActionResult RandomAction(GetTestDataRequest3 request)
            {
                return new EmptyResult();
            }
        }

        private class ControllerWithHandledRequestAttr : Controller
        {
            public ControllerWithHandledRequestAttr()
            { }

            [HandlesRequest(typeof(GetTestDataRequest2))]
            public IActionResult RandomAction()
            {
                return new EmptyResult();
            }
        }

        private interface IMediatrMvcGenericController<TRequest> : IRequest<TRequest> { }

        private class GetTestDataRequest : IRequest<string>
        {
        }

        private class GetTestDataRequest2 : IRequest<int>
        {
        }

        private class GetTestDataRequest3 : IRequest<object>
        {
        }

        private class GetTestDataRequest4 : IRequest<Unit>
        {
        }

        private class GetTestDataRequest5 : IRequest
        {
        }

        private class BadTestDataRequest3 : IRequest<object>
        {
        }

        private class GetTestDataRequestHandler : IRequestHandler<GetTestDataRequest, string>
        {
            public Task<string> Handle(GetTestDataRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult("From Handle!");
            }
        }

        private class GetTestDataRequest2Handler : IRequestHandler<GetTestDataRequest2, int>
        {
            public Task<int> Handle(GetTestDataRequest2 request, CancellationToken cancellationToken)
            {
                return Task.FromResult(1);
            }
        }

        private class GetTestDataRequest3Handler : IRequestHandler<GetTestDataRequest3, object>
        {
            public Task<object> Handle(GetTestDataRequest3 request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new object());
            }
        }

        private class GetTestDataRequest4Handler : IRequestHandler<GetTestDataRequest4, Unit>
        {
            public Task<Unit> Handle(GetTestDataRequest4 request, CancellationToken cancellationToken)
            {
                return Task.FromResult(Unit.Value);
            }
        }

        private class GetTestDataRequest5Handler : IRequestHandler<GetTestDataRequest5, Unit>
        {
            public Task<Unit> Handle(GetTestDataRequest5 request, CancellationToken cancellationToken)
            {
                return Task.FromResult(Unit.Value);
            }
        }

        private class BadTestDataRequest3Handler : IRequestHandler<BadTestDataRequest3, object>
        {
            public Task<object> Handle(BadTestDataRequest3 request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new object());
            }
        }
        #endregion
    }
}
