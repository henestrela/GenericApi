using Mapster;
using System;
using System.Linq.Expressions;
using Utils.LinqHelpers;

namespace Utils.Mapper
{

    public class TypeAdapterSetterCustom
    {
        private readonly TypeAdapterSetter Config;
        public TypeAdapterSetterCustom(TypeAdapterSetter config)
        {
            Config = config;
        }
        public void Map<TSetter, TSource, TSourceMember>(Expression<Func<TSetter, TSourceMember>> memberAccess, Expression<Func<TSource, TSourceMember>> source)
        {
            Config.Map(LinqMethods.GetMemberName(memberAccess), source);
        }

    }
}
