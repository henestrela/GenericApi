using System;

namespace Utils.Attributes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GrClassAttribute : Attribute
    {
        public string UniqueKeyOrder { get; set; } = null;
    }
}
