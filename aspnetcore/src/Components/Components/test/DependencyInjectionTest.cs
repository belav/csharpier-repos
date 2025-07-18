// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components.Test.Helpers;

namespace Microsoft.AspNetCore.Components.Test;

public class DependencyInjectionTest
{
    private readonly TestRenderer _renderer;
    private readonly TestServiceProvider _serviceProvider;

    public DependencyInjectionTest()
    {
        _serviceProvider = new TestServiceProvider();
        _renderer = new TestRenderer(_serviceProvider);
    }

    [Fact]
    public void IgnoresPropertiesWithoutInjectAttribute()
    {
        // Arrange/Act
        var component = InstantiateComponent<HasPropertiesWithoutInjectAttribute>();

        // Assert
        Assert.Null(component.SomeProperty);
        Assert.Null(component.PrivatePropertyValue);
    }

    [Fact]
    public void IgnoresStaticProperties()
    {
        // Arrange/Act
        var component = InstantiateComponent<HasStaticProperties>();

        // Assert
        Assert.Null(HasStaticProperties.StaticPropertyWithInject);
        Assert.Null(HasStaticProperties.StaticPropertyWithoutInject);
    }

    [Fact]
    public void ThrowsForInjectablePropertiesWithoutSetter()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            InstantiateComponent<HasGetOnlyPropertyWithInject>();
        });

        Assert.Equal(
            $"Cannot provide a value for property '{nameof(HasInjectableProperty.MyService)}' "
                + $"on type '{typeof(HasGetOnlyPropertyWithInject).FullName}' because the property "
                + $"has no setter.",
            ex.Message
        );
    }

    [Fact]
    public void ThrowsIfNoSuchServiceIsRegistered()
    {
        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            InstantiateComponent<HasInjectableProperty>();
        });

        Assert.Equal(
            $"Cannot provide a value for property '{nameof(HasInjectableProperty.MyService)}' "
                + $"on type '{typeof(HasInjectableProperty).FullName}'. There is no registered service "
                + $"of type '{typeof(IMyService).FullName}'.",
            ex.Message
        );
    }

    [Fact]
    public void ThrowsIfNoSuchKeyedServiceIsRegistered()
    {
        var serviceInstance = new MyServiceImplementation();
        _serviceProvider.AddKeyedService<IMyService>(serviceInstance, "mismatched-key");

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            InstantiateComponent<HasKeyedInjectableProperty>();
        });

        Assert.Equal(
            $"Cannot provide a value for property '{nameof(HasKeyedInjectableProperty.MyService)}' "
                + $"on type '{typeof(HasKeyedInjectableProperty).FullName}'. There is no registered keyed service "
                + $"of type '{typeof(IMyService).FullName}' with key '{HasKeyedInjectableProperty.ServiceKey}'.",
            ex.Message
        );
    }

    [Fact]
    public void SetsInjectablePropertyValueIfServiceIsRegistered()
    {
        // Arrange
        var serviceInstance = new MyServiceImplementation();
        _serviceProvider.AddService<IMyService>(serviceInstance);

        // Act
        var instance = InstantiateComponent<HasInjectableProperty>();

        // Assert
        Assert.Same(serviceInstance, instance.MyService);
    }

    [Fact]
    public void SetsKeyedInjectablePropertyValueIfKeyedServiceIsRegistered()
    {
        // Arrange
        var serviceInstance = new MyServiceImplementation();
        _serviceProvider.AddKeyedService<IMyService>(
            serviceInstance,
            HasKeyedInjectableProperty.ServiceKey
        );

        // Act
        var instance = InstantiateComponent<HasKeyedInjectableProperty>();

        // Assert
        Assert.Same(serviceInstance, instance.MyService);
    }

    [Fact]
    public void HandlesInjectablePropertyScenarios()
    {
        // Arrange
        var serviceInstance = new MyServiceImplementation();
        var otherServiceInstance = new MyOtherServiceImplementation();
        var concreteServiceInstance = new MyConcreteService();
        _serviceProvider.AddService<IMyService>(serviceInstance);
        _serviceProvider.AddService<IMyOtherService>(otherServiceInstance);
        _serviceProvider.AddService(concreteServiceInstance);
        _serviceProvider.AddKeyedService<IMyService>(
            serviceInstance,
            HasManyInjectableProperties.ServiceKey
        );

        // Act
        var instance = InstantiateComponent<HasManyInjectableProperties>();

        // Assert
        Assert.Same(serviceInstance, instance.PublicReadWrite);
        Assert.Same(serviceInstance, instance.PublicReadOnly);
        Assert.Same(serviceInstance, instance.PrivateValue);
        Assert.Same(otherServiceInstance, instance.DifferentServiceType);
        Assert.Same(concreteServiceInstance, instance.ConcreteServiceType);
        Assert.Same(serviceInstance, instance.KeyedService);
    }

    [Fact]
    public void SetsInheritedInjectableProperties()
    {
        // Arrange
        var serviceInstance = new MyServiceImplementation();
        _serviceProvider.AddService<IMyService>(serviceInstance);

        // Act
        var instance = InstantiateComponent<HasInheritedInjectedProperty>();

        // Assert
        Assert.Same(serviceInstance, instance.MyService);
    }

    [Fact]
    public void SetsPrivateInheritedInjectableProperties()
    {
        // Arrange
        var serviceInstance = new MyServiceImplementation();
        _serviceProvider.AddService<IMyService>(serviceInstance);

        // Act
        var instance = InstantiateComponent<HasInheritedPrivateInjectableProperty>();

        // Assert
        Assert.Same(serviceInstance, instance.PrivateMyService);
    }

    private T InstantiateComponent<T>()
        where T : IComponent => (T)_renderer.InstantiateComponent<T>();

    class HasPropertiesWithoutInjectAttribute : TestComponent
    {
        public IMyService SomeProperty { get; set; }
        public IMyService PrivatePropertyValue => PrivateProperty;
        private IMyService PrivateProperty { get; set; }
    }

    class HasStaticProperties : TestComponent
    {
        [Inject]
        public static IMyService StaticPropertyWithInject { get; set; }
        public static IMyService StaticPropertyWithoutInject { get; set; }
    }

    class HasGetOnlyPropertyWithInject : TestComponent
    {
        [Inject]
        public IMyService MyService { get; }
    }

    class HasInjectableProperty : TestComponent
    {
        [Inject]
        public IMyService MyService { get; set; }
    }

    class HasKeyedInjectableProperty : TestComponent
    {
        public const string ServiceKey = "my-service";

        [Inject(Key = ServiceKey)]
        public IMyService MyService { get; set; }
    }

    class HasPrivateInjectableProperty : TestComponent
    {
        [Inject]
        private IMyService MyService { get; set; }

        public IMyService PrivateMyService => MyService;
    }

    class HasInheritedPrivateInjectableProperty : HasPrivateInjectableProperty { }

    class HasManyInjectableProperties : TestComponent
    {
        public const string ServiceKey = "my-service";

        [Inject]
        public IMyService PublicReadWrite { get; set; }

        [Inject]
        public IMyService PublicReadOnly { get; private set; }

        [Inject]
        private IMyService Private { get; set; }

        [Inject]
        public IMyOtherService DifferentServiceType { get; set; }

        [Inject]
        public MyConcreteService ConcreteServiceType { get; set; }

        [Inject(Key = ServiceKey)]
        public IMyService KeyedService { get; set; }

        public IMyService PrivateValue => Private;
    }

    class HasInheritedInjectedProperty : HasInjectableProperty { }

    interface IMyService { }

    interface IMyOtherService { }

    class MyServiceImplementation : IMyService { }

    class MyOtherServiceImplementation : IMyOtherService { }

    class MyConcreteService { }

    class TestComponent : IComponent
    {
        // IMPORTANT: The fact that these throw demonstrates that the injection
        // happens before any of the lifecycle methods. If you change these to
        // not throw, then be sure also to add a test to verify that injection
        // occurs before lifecycle methods.

        public void Attach(RenderHandle renderHandle) => throw new NotImplementedException();

        public Task SetParametersAsync(ParameterView parameters) =>
            throw new NotImplementedException();
    }
}
