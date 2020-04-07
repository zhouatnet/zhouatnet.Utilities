using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace zhouatnet.Utilities
{
    /// <summary>
    ///  IServiceCollection 容器的扩展类
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 程序集依赖注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblyName"></param>
        /// <param name="serviceLifetime"></param>
        public static void AddAssembly(this IServiceCollection services, string assemblyName, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services) + "为空");

            if (string.IsNullOrEmpty(assemblyName))
                throw new ArgumentNullException(nameof(assemblyName) + "为空");

            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly) + ".dll不存在");

            var types = assembly.GetTypes();
            //不是抽象，不是泛型的类型
            var list = types.Where(m => m.IsClass && !m.IsAbstract && !m.IsGenericType).ToList();
            if (list == null || (list != null && !list.Any()))
                return;

            foreach (var type in list)
            {
                var interfacesList = type.GetInterfaces();
                if (interfacesList == null || (interfacesList != null && !interfacesList.Any()))
                    continue;

                var inter = interfacesList.First();
                switch (serviceLifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(inter, type);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(inter, type);
                        break;
                    default:
                        services.AddScoped(inter, type);
                        break;
                }
            }
        }
    }
}
