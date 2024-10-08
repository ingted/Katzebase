﻿using NTDLS.Katzebase.Engine.Functions.Parameters;
using NTDLS.Katzebase.Engine.Library;
using NTDLS.Katzebase.Engine.Query.Constraints;
using static NTDLS.Katzebase.Engine.Library.EngineConstants;

namespace NTDLS.Katzebase.Engine.Query
{
    /// <summary>
    /// Contains a parsed query via StaticQueryParser.PrepareQuery();
    /// </summary>
    internal class PreparedQuery
    {
        internal enum QueryAttribute
        {
            IndexName,
            IsUnique,
            ProcessId,
            SpecificSchemaPrefix,
            IncludePhysicalPages,
            TargetSchema,
            Schema,
            ObjectName,
            Parameters,
            Batches,
            Partitions,
            PageSize,

            //----------Configuration (BEGIN) ----------
            BaseAddress,
            DataRootPath,
            TransactionDataPath,
            LogDirectory,
            FlushLog,
            DefaultDocumentPageSize,
            UseCompression,
            HealthMonitoringEnabled,
            HealthMonitoringCheckpointSeconds,
            HealthMonitoringInstanceLevelEnabled,
            HealthMonitoringInstanceLevelTimeToLiveSeconds,
            MaxIdleConnectionSeconds,
            DefaultIndexPartitions,
            DeferredIOEnabled,
            WriteTraceData,
            CacheEnabled,
            CacheMaxMemory,
            CacheScavengeInterval,
            CachePartitions,
            CacheSeconds
            //----------Configuration (END)----------
        }

        /// <summary>
        /// When this is non-null, it will cause the searcher methods to pull fields from all schemas,
        ///     unless it also contains a list of schema aliases, in which case they will be used to filter
        ///     which schemas we pull fields from.
        /// </summary>
        public List<string>? DynamicSchemaFieldFilter { get; set; }

        public Dictionary<QueryAttribute, object> Attributes { get; private set; } = new();
        public List<QuerySchema> Schemas { get; private set; } = new();
        public int RowLimit { get; set; }
        public QueryType QueryType { get; set; }
        public SubQueryType SubQueryType { get; set; }
        public Conditions Conditions { get; set; } = new();
        //public PrefixedFields SelectFields { get; set; } = new();

        public PrefixedFields CreateFields { get; set; } = new();

        public FunctionParameterBaseCollection SelectFields = new();
        public FunctionParameterBase ProcedureCall = new();
        public FunctionParameterBaseCollection GroupFields { get; set; } = new();
        public SortFields SortFields { get; set; } = new();

        /// <summary>
        /// List of key/values used for insert statements.
        /// </summary>
        //public List<UpsertKeyValues> UpsertValues { get; set; } = new();
        public List<NamedFunctionParameterBaseCollection> UpsertValues { get; set; } = new();

        /// <summary>
        /// List of values for updates by field name.
        /// </summary>
        public NamedFunctionParameterBaseCollection UpdateValues { get; set; } = new();

        public List<KbNameValuePair<string, string>> VariableValues { get; set; } = new();

        public T Attribute<T>(QueryAttribute attribute, T defaultValue)
        {
            if (Attributes.TryGetValue(attribute, out object? value))
            {
                return (T)value;
            }
            return defaultValue;
        }

        public T Attribute<T>(QueryAttribute attribute)
        {
            return (T)Attributes[attribute];
        }

        public void AddAttribute(QueryAttribute key, object value)
        {
            if (!Attributes.TryAdd(key, value))
            {
                Attributes[key] = value;
            }
        }
    }
}
