﻿using NTDLS.Katzebase.Engine.Functions.Procedures.Persistent;
using NTDLS.Katzebase.Engine.Schemas;

namespace NTDLS.Katzebase.Engine.Functions.Procedures
{
    internal class AppliedProcedurePrototype
    {
        public string Name { get; set; } = string.Empty;
        public PhysicalSchema? PhysicalSchema { get; set; }
        public PhysicalProcedure? PhysicalProcedure { get; set; }
        public ProcedureParameterValueCollection Parameters { get; set; } = new();
        public bool IsSystem { get; set; }
    }
}
