﻿namespace NTDLS.Katzebase.Engine.Query
{
    public class UpsertKeyValue
    {
        public PrefixedField Field { get; private set; }
        public SmartValue Value { get; private set; }

        public UpsertKeyValue(PrefixedField field, SmartValue value)
        {
            Field = field;
            Value = value;
        }
    }
}
