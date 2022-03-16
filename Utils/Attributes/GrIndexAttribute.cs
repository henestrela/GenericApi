using System;

namespace Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class GrIndexAttribute : Attribute
    {
        public GrIndexAttribute(string[] index)
        {
            Index = index;
        }

        public string[] Index { get; }
    }
}
