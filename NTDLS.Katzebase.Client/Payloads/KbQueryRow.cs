using fs;

namespace NTDLS.Katzebase.Client.Payloads
{
    public class KbQueryRow
    {
        public List<fstring?> Values { get; set; }

        public KbQueryRow(List<fstring?> values)
        {
            Values = values;
        }

        public KbQueryRow()
        {
            Values = new();
        }

        public void AddValue(fstring? value)
        {
            Values.Add(value);
        }
    }
}
