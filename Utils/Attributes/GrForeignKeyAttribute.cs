using System;

namespace Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class GrForeignKeyAttribute : Attribute
    {
        public GrForeignKeyAttribute(Type type, string nameReference = null, string navigationProperty = null)
        {
            Type = type;
            NameReference = nameReference;
            NavigationProperty = navigationProperty;
        }

        public Type Type { get; }
        public string NameReference { get; } = null;
        public string NavigationProperty { get; } = null;
    }
}
