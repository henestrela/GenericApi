using System;

namespace Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GrReferenceAttribute : Attribute
    {
        public GrReferenceAttribute(string nameReference, Type entityReference, string nameForeignKey)
        {
            NameReference = nameReference;
            EntityReference = entityReference;
            NameForeignKey = nameForeignKey;
        }

        public string NameReference { get; }

        public string NameForeignKey { get; }
        public Type EntityReference { get; }
    }
}
