using System.Collections.Generic;

namespace Cassette.DependencyGraphInteration.InterationResults
{
    public class EnumerableInterationResult<T> : SimpleInteractionResult
    {
        public IEnumerable<T> Enumerable { get; set; }
    }
}
