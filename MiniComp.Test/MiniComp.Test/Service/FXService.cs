using MiniComp.Autofac;

namespace MiniComp.Test.Service;

[AutofacDependency(typeof(IFXService<>), ServiceLifetime = ServiceLifetime.Scoped, ServiceKey = "")]
public class FXService<T> : IFXService<T>
    where T : class { }
