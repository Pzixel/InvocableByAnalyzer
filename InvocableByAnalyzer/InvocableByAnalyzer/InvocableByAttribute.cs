using System;

namespace InvocableByAnalyzer
{
    public class InvocableByAttribute : Attribute
    {
        public Type[] AllowedTypes { get; }

        public InvocableByAttribute(params Type[] allowedTypes)
        {
            AllowedTypes = allowedTypes;
        }
    }
}
