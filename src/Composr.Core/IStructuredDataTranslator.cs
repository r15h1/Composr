using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Core
{
    public interface IStructuredDataTranslator
    {
        /// <summary>
        /// translate a post into a different type, e.g. recipe
        /// useful for converting a post into a structured data type as described at http://schema.org/
        /// Will be implemented using the visitor pattern
        /// </summary>
        /// <typeparam name="T">target type, which is reflected in the Output property</typeparam>
        /// <param name="post">source</param>        
        void Translate(Post post);

        /// <summary>
        /// the output of the translation
        /// </summary>
        IStructuredData Output { get; }
    }
}
