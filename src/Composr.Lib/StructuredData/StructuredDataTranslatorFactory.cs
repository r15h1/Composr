using Composr.Core;
using System;

namespace Composr.Lib.StructuredData
{
    public class StructuredDataTranslatorFactory
    {
        public static IStructuredDataTranslator CreateTranslator(string type)
        {
            Type t = Type.GetType(type);
            return (IStructuredDataTranslator)Activator.CreateInstance(t);
        }
    }
}
