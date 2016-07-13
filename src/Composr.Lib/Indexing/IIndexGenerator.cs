using Composr.Core;

namespace Composr.Lib.Indexing
{
    public interface IIndexGenerator {
        void BuildIndex(Blog blog);
    }
}