using System;

namespace FHIRConverter.Exceptions
{
    public class ResourceIdPathException : Exception
    {
        public ResourceIdPathException()
        {
        }

        public ResourceIdPathException(string prop)
        {
            InvalidProp = prop;
        }

        public string InvalidProp { get; set; }
    }
}