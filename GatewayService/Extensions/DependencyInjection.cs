using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GatewayService.Extensions
{
    public static class DependencyInjection
    {
        private static bool ShouldSkip(MemberInfo type, IEnumerable<string> exclude)
        {
            var exists = exclude.Any(it => type.Name.Contains(it));
            return exists;
        }

        public static void ConfigureDependencyInjection(this IServiceCollection services, params Assembly[] assemblies)
        {
            var skip = new HashSet<string>
            {
                "ServiceBase",
                "GenericRepository"
            };

            foreach (var assembly in assemblies)
            {
                var types = assembly
                    .GetExportedTypes()
                    .Where(it => it.IsClass && !ShouldSkip(it, skip))
                    .Select(it =>
                    {
                        // get interface class, with similar name
                        var inter = it.GetInterfaces()
                            ?.Where(ot => ot.Name.Contains(it.Name))
                            .FirstOrDefault();
                        // Return a pair
                        return (_class: it, _interface: inter);
                    })
                    //Remove types without interfaces
                    .Where(it => it._interface != null);

                foreach (var (_class, _interface) in types)
                {
                    Console.WriteLine($@"Service: {_class.FullName} impl:{_interface.FullName}");
                    services.AddScoped(_interface, _class);
                }
            }
        }
    }
}