using System;

namespace Utils.Attributes
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class GrPropertyAttribute : Attribute
    {
        public bool Required { get; set; } = false;

        public object Default { get; set; } = null;

        public bool UniqueKey { get; set; } = false;
        public string[] UniqueKeyComposite { get; set; } = new string[] { };
        public int Length { get; set; } = -1;
        public int MinLength { get; set; } = 0;
        public int Precision { get; set; } = -1;
        public int Scale { get; set; } = 0;

    }
}
