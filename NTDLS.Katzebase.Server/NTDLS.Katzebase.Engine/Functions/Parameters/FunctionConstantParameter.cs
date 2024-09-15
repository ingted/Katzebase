using fs;

namespace NTDLS.Katzebase.Engine.Functions.Parameters
{
    internal class FunctionConstantParameter : FunctionParameterBase
    {
        public fstring RawValue { get; set; } = fstring.Empty;

        /// <summary>
        /// This is the value that should be used for "user code". It removes the quotes from constant parameters.
        /// </summary>
        public fstring FinalValue
        {
            get
            {
                if (RawValue.s.StartsWith('\'') && RawValue.s.EndsWith('\''))
                {
                    return fstring.NewS(RawValue.s.Substring(1, RawValue.s.Length - 2));
                }
                return RawValue;
            }
        }

        public FunctionConstantParameter(fstring value)
        {
            RawValue = value;
        }
    }
}
