using fs;

namespace NTDLS.Katzebase.Engine.Functions.Procedures
{
    internal class ProcedureParameterValue
    {
        public ProcedureParameterPrototype Parameter { get; private set; }
        public fstring? Value { get; private set; } = null;

        public ProcedureParameterValue(ProcedureParameterPrototype parameter, fstring? value)
        {
            Parameter = parameter;
            Value = value;
        }

        public ProcedureParameterValue(ProcedureParameterPrototype parameter)
        {
            Parameter = parameter;
        }
    }
}
