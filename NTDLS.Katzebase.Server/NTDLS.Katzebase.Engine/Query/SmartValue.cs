using fs;
using NTDLS.Katzebase.Client.Exceptions;

namespace NTDLS.Katzebase.Engine.Query
{
    public class SmartValue
    {
        private fstring? _value = null;

        /// <summary>
        /// This value is a constant string.
        /// </summary>
        public bool IsString { get; private set; }
        /// <summary>
        /// This value is a constant (string or numeric). If false, then this value is a field name.
        /// </summary>
        public bool IsConstant { get; private set; }
        /// <summary>
        /// This value is numeric and does not contain string characters.
        /// </summary>
        public bool IsNumeric { get; private set; }
        /// <summary>
        /// This value has been set.
        /// </summary>
        public bool IsSet { get; private set; }
        public string Prefix { get; private set; } = string.Empty;

        /// <summary>
        /// The schema.field key for the field. Can be parsed to PrefixedField via PrefixedField.Parse(this.Key).
        /// </summary>
        public string Key
            => string.IsNullOrEmpty(Prefix) ? (_value?.s ?? "") : $"{Prefix}.{_value?.s}";

        public SmartValue()
        {
        }

        public SmartValue(string value)
        {
            Value = value.toF();
        }

        public SmartValue(fstring value)
        {
            Value = value;
        }

        public SmartValue Clone()
        {
            return new SmartValue()
            {
                IsConstant = IsConstant,
                IsNumeric = IsNumeric,
                IsString = IsString,
                Prefix = Prefix,
                IsSet = IsSet,
                _value = _value
            };
        }

        public override string ToString()
        {
            return _value?.ToString() ?? string.Empty;
        }

        public fstring? Value
        {
            get { return _value; }
            set
            {
                IsConstant = false;
                IsNumeric = false;
                IsString = false;

                _value = value?.ToLowerInvariant();

                if (_value?.s == "null")
                {
                    IsConstant = true;
                    _value = null;
                }

                if (_value != null)
                {
                    if (_value.s.StartsWith('\'') && _value.s.EndsWith('\''))
                    {
                        //Handle escape sequences:
                        _value = _value.s.Replace("\\'", "\'").toF();

                        _value = value?.s.Substring(1, _value.s.Length - 2).toF();
                        IsString = true;
                        IsConstant = true;
                    }
                    else
                    {
                        if (_value.s.Contains('.') && double.TryParse(_value.s, out _) == false)
                        {
                            var parts = _value.s.Split('.');
                            if (parts.Length != 2)
                            {
                                throw new KbParserException("Invalid query. Found [" + _value.s + "], Expected a multi-part condition field.");
                            }

                            Prefix = parts[0];
                            _value = parts[1].toF();
                        }
                    }

                    if (_value != null && double.TryParse(_value.s, out _))
                    {
                        IsConstant = true;
                        IsNumeric = true;
                    }
                    else if (_value != null && _value.s.Contains(':'))
                    {
                        //Check to see if this is a "between" expression "number:number" e.g. 5:10
                        var parts = _value.s.Split(':');
                        if (parts.Length == 2)
                        {
                            if (double.TryParse(parts[0], out _) && double.TryParse(parts[1], out _))
                            {
                                IsConstant = true;
                            }
                        }
                    }
                }

                IsSet = true;
            }
        }
    }
}
