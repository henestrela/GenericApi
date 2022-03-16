using System;

namespace Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GrMapperAttribute : Attribute
    {
        public GrMapperAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }


    }
}
