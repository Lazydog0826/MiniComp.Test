using MiniComp.Autofac;

namespace MiniComp.Test.Service;

[AutofacDependency(typeof(IService), ServiceLifetime = ServiceLifetime.Scoped, ServiceKey = "")]
public class Service : IService { }
