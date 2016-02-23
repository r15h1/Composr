using System;

namespace Composr.Core
{
    /// <summary>
    /// to be thrown when data input does not meet expectations
    /// </summary>
    public class SpecificationException : Exception
    {
        public SpecificationException(string message)
            : base(message)
        {
        }
    }
}
