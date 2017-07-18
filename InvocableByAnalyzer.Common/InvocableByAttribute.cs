using System;

namespace InvocableByAnalyzer.Common
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
