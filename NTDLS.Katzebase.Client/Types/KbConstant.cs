using static NTDLS.Katzebase.Client.KbConstants;
using fs;
using Microsoft.Extensions.Primitives;
namespace NTDLS.Katzebase.Client.Types
{
    public class KbConstant
    {
        public fstring? Value { get; set; }
        public KbBasicDataType DataType { get; set; }

        public KbConstant(fstring? value, KbBasicDataType dataType)
        {
            Value = value;
            DataType = dataType;
        }

        public KbConstant(string value)
        {
            Value = fstring.NewS(value);
            DataType = KbBasicDataType.String;
        }
    }
}
