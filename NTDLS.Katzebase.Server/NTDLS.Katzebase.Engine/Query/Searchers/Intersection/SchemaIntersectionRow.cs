﻿using fs;
using NTDLS.Katzebase.Client.Exceptions;
using NTDLS.Katzebase.Client.Types;
using NTDLS.Katzebase.Engine.Documents;

namespace NTDLS.Katzebase.Engine.Query.Searchers.Intersection
{
    internal class SchemaIntersectionRow
    {
        public KbInsensitiveDictionary<DocumentPointer> SchemaDocumentPointers { get; private set; } = new();

        public List<fstring?> Values { get; set; } = new();

        /// <summary>
        /// The schemas that were used to make up this row.
        /// </summary>
        public HashSet<string> SchemaKeys { get; set; } = new();


        /// <summary>
        /// Auxiliary fields are values that may be used for method calls, sorting, grouping, etc.
        ///     where the fields value may not necessarily be returned directly in the results.
        /// </summary>
        public KbInsensitiveDictionary<fstring?> AuxiliaryFields { get; private set; } = new();

        public void InsertValue(string fieldNameForException, int ordinal, fstring? value)
        {
            if (Values.Count <= ordinal)
            {
                int difference = (ordinal + 1) - Values.Count;
                if (difference > 0)
                {
                    var r = new string[difference];
                    Values.AddRange(fstring.fromStringArr(r));
                }
            }
            //if (Values[ordinal] != null)
            //{
            //    throw new KbEngineException($"Ambiguous field [{fieldNameForException}].");
            //}

            //Values[ordinal] = value;
        }

        public void AddSchemaDocumentPointer(string schemaPrefix, DocumentPointer documentPointer)
        {
            SchemaDocumentPointers.Add(schemaPrefix, documentPointer);
        }

        public SchemaIntersectionRow Clone()
        {
            var newRow = new SchemaIntersectionRow
            {
                SchemaKeys = new HashSet<string>(SchemaKeys)
            };

            newRow.Values.AddRange(Values);

            newRow.AuxiliaryFields = AuxiliaryFields.Clone();
            newRow.SchemaDocumentPointers = SchemaDocumentPointers.Clone();

            return newRow;
        }
    }
}
