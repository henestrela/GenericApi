using Mapster;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Utils.Attributes;
using Utils.Interfaces;
using Utils.Mapper;

namespace GenericApi.Config
{
    public class MapperConfig
    {
        private static readonly List<string> displayNames = new List<string>();


        public TypeAdapterConfig Register()
        {
            var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;

            IEnumerable<System.Reflection.Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName != null && (
            a.FullName.StartsWith("Service.DTO")));


            //MethodInfo method = typeof(ICustomMapperDTO).GetMethod("AutomapperConfig");



            foreach (System.Reflection.Assembly assembly in assemblies)
            {
                List<Type> theList = assembly.GetTypes()
                     .Where(t => (
                     t.Namespace.Contains("Service.DTO")) &&
                     t.GetInterfaces().Any(x => x == typeof(IBaseIdDTO) || x == typeof(ICustomMapperDTO) || x == typeof(IBaseDTO) ||
                                      (x.IsGenericType &&
                                       (x.GetGenericTypeDefinition() == typeof(IBaseIdDTO) || x.GetGenericTypeDefinition() == typeof(ICustomMapperDTO)))))
                     .ToList();



                foreach (Type t in theList)
                {
                    System.Reflection.PropertyInfo[] props = t.GetProperties();

                    foreach (System.Reflection.PropertyInfo p in props)
                    {
                        IEnumerable<DisplayAttribute> display = p.GetCustomAttributes(true).Where(x => x is DisplayAttribute).Cast<DisplayAttribute>();
                        foreach (DisplayAttribute d in display)
                        {
                            if (displayNames.Contains(d.Name))
                            {
                                continue;
                            }

                            displayNames.Add(d.Name);
                        }

                    }
                }


                foreach (Type ty in theList)
                {
                    GrMapperAttribute attr = ty.GetCustomAttributes(typeof(GrMapperAttribute), false).FirstOrDefault() as GrMapperAttribute;
                    if (attr == null)
                    {
                        continue;
                    }

                    //mc.CreateMap<byte, char>();

                    //mc.CreateMap<byte[]?, string>().ConvertUsing(new ByteToString());
                    //mc.CreateMap<string, byte[]?>().ConvertUsing(new StringToByte());

                    typeAdapterConfig.NewConfig(typeof(IList<>).MakeGenericType(ty), typeof(IList<>).MakeGenericType(attr.Type));

                    typeAdapterConfig.NewConfig(typeof(IList<>).MakeGenericType(attr.Type), typeof(IList<>).MakeGenericType(ty));

                    if (ty.GetInterfaces().Any(x => x == typeof(ICustomMapperDTO)))
                    {
                        typeAdapterConfig.NewConfig(ty, attr.Type);
                        object classInstance = Activator.CreateInstance(ty, null);



                        ((ICustomMapperDTO)classInstance).MapsterConfig(new TypeAdapterSetterCustom(typeAdapterConfig.NewConfig(attr.Type,ty)));
                    }
                    else
                    {
                        typeAdapterConfig.NewConfig(ty, attr.Type);
                        typeAdapterConfig.NewConfig(attr.Type, ty);
                    }
                }
            }

            return typeAdapterConfig;
        }

        //public MapperConfig()
        //{

        //}
        //public MapperConfig(IMapperConfigurationExpression mc)
        //{

        //    //TODO: See other alternatives to load assembly but this is Only to load Assembly before get


        //    IEnumerable<System.Reflection.Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName != null && (
        //    a.FullName.StartsWith("MedRoom.DTO")));


        //    //MethodInfo method = typeof(ICustomMapperDTO).GetMethod("AutomapperConfig");



        //    foreach (System.Reflection.Assembly assembly in assemblies)
        //    {
        //        List<Type> theList = assembly.GetTypes()
        //             .Where(t => (
        //             t.Namespace.Contains("MedRoom.DTO") ) &&
        //             t.GetInterfaces().Any(x => x == typeof(IBaseIdDTO) || x == typeof(ICustomMapperDTO) || x == typeof(IBaseDTO) ||
        //                              (x.IsGenericType &&
        //                               (x.GetGenericTypeDefinition() == typeof(IBaseIdDTO) || x.GetGenericTypeDefinition() == typeof(ICustomMapperDTO)))))
        //             .ToList();



        //        foreach (Type t in theList)
        //        {
        //            System.Reflection.PropertyInfo[] props = t.GetProperties();

        //            foreach (System.Reflection.PropertyInfo p in props)
        //            {
        //                IEnumerable<DisplayAttribute> display = p.GetCustomAttributes(true).Where(x => x is DisplayAttribute).Cast<DisplayAttribute>();
        //                foreach (DisplayAttribute d in display)
        //                {
        //                    if (displayNames.Contains(d.Name))
        //                    {
        //                        continue;
        //                    }

        //                    displayNames.Add(d.Name);
        //                }

        //            }
        //        }


        //        foreach (Type ty in theList)
        //        {
        //            MrMapperAttribute attr = ty.GetCustomAttributes(typeof(MrMapperAttribute), false).FirstOrDefault() as MrMapperAttribute;
        //            if (attr == null)
        //            {
        //                continue;
        //            }

        //            //mc.CreateMap<byte, char>();

        //            //mc.CreateMap<byte[]?, string>().ConvertUsing(new ByteToString());
        //            //mc.CreateMap<string, byte[]?>().ConvertUsing(new StringToByte());

        //            mc.CreateMap(typeof(IList<>).MakeGenericType(ty), typeof(IList<>).MakeGenericType(attr.Type));

        //            mc.CreateMap(typeof(IList<>).MakeGenericType(attr.Type), typeof(IList<>).MakeGenericType(ty));

        //            if (ty.GetInterfaces().Any(x => x == typeof(ICustomMapperDTO)))
        //            {
        //                mc.CreateMap(ty, attr.Type);
        //                object classInstance = Activator.CreateInstance(ty, null);
        //                //((ICustomMapperDTO)classInstance).AutomapperConfig(new MappingExpressionCustom(mc.CreateMap(attr.Type, ty)));
        //            }
        //            else
        //            {
        //                mc.CreateMap(ty, attr.Type);
        //                mc.CreateMap(attr.Type, ty);
        //            }
        //        }
        //    }
        //}


        //public class ByteToString : ITypeConverter<byte[]?, string>
        //{
        //    public string Convert(byte[] source, string destination, ResolutionContext context)
        //    {
        //        return System.Text.Encoding.UTF8.GetString(source);
        //    }
        //}

        //public class StringToByte : ITypeConverter<string, byte[]?>
        //{

        //    public byte[] Convert(string source, byte[] destination, ResolutionContext context)
        //    {
        //        return System.Text.Encoding.UTF8.GetBytes(source);
        //    }
        //}
    }
}
