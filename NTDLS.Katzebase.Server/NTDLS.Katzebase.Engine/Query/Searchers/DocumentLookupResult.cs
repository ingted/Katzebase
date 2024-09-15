using fs;

namespace NTDLS.Katzebase.Engine.Query.Searchers
{
    public class DocumentLookupResult
    {
        public List<fstring?> Values { get; private set; } = new();

        public List<string?> AggregateFields { get; private set; } = new();

        public DocumentLookupResult(List<fstring?> values)
        {
            Values.AddRange(values);
        }

        public DocumentLookupResult(List<fstring?> values, List<string?> aggregateFields)
        {
            Values.AddRange(values);
            AggregateFields.AddRange(aggregateFields);
        }
    }
}
