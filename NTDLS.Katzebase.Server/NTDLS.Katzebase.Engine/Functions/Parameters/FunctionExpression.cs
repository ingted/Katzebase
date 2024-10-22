using fs;

namespace NTDLS.Katzebase.Engine.Functions.Parameters
{
    internal class FunctionExpression : FunctionParameterBase
    {
        public enum FunctionExpressionType
        {
            Text,
            Numeric
        }

        public FunctionExpressionType ExpressionType { get; set; }

        public fstring Value { get; set; } = fstring.SEmpty;
        public List<FunctionParameterBase> Parameters { get; private set; } = new();
    }
}
