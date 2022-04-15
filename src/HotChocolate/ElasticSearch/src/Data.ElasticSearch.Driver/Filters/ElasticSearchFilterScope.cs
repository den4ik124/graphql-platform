using System.Collections.Generic;
using HotChocolate.Data.Filters;

namespace HotChocolate.Data.ElasticSearch.Filters
{
    /// <inheritdoc />
    public class ElasticSearchFilterScope
        : FilterScope<ISearchOperation >
    {
        public ElasticSearchFilterScope()
        {
            // TODO this needs to be solved nicer
            Path.Push("document");

        }
        /// <summary>
        /// The path from the root to the current position in the input object
        /// </summary>
        public Stack<string> Path { get; } = new();
    }
}
