namespace NTDLS.Katzebase.Engine.Functions.Aggregate.Parameters
{
    using fs;
    internal class AggregateSingleParameter : AggregateGenericParameter
    {
        public fstring Value { get; set; } = fstring.Empty;
    }
}
