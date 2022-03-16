

using Microsoft.Extensions.DependencyInjection;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Utils.Interfaces;

namespace GenericApi.Config
{
    public class InjectionServices
    {
        public void getServices(IServiceCollection services)
        {

            //TODO: See other alternatives to load assembly but this is Only to load Assembly before get

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Service.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();

            toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

            IEnumerable<Assembly> assemblies = loadedAssemblies.Where(a => a.FullName != null && (
                a.FullName.StartsWith("Service") 
            ));


            var interfaces = loadedAssemblies.Where(a => a.FullName != null && a.FullName.StartsWith("Service.Interface")).FirstOrDefault();


            foreach (System.Reflection.Assembly assembly in assemblies)
            {
                List<Type> theList = interfaces.GetTypes()
                     .Where(t => 
                     t.Namespace == "Service.Interface"  &&
                     t.GetInterfaces().Any(x => x == typeof(IGrService) ||
                                      (x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IGrGenericService<>))))
                     .ToList();


                foreach (Type ty in theList)
                {
                    Type definedType = assembly.GetTypes()
                         .Where(t => t.GetInterfaces().Contains(ty)).FirstOrDefault();
                    if (definedType == null) 
                        continue; 
                    services.AddScoped(ty, definedType);
                }
            }
        }
    }
}