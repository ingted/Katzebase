using fs;

namespace NTDLS.Helpers
{
    /// <summary>
    /// Helper functions for type conversions.
    /// </summary>
    public class Converters
    {
        /// <summary>
        /// Makes a best effort conversion from a string to the given type.
        /// </summary>
        public static T ConvertTo<T>(fstring? value, T defaultValue)
            => ConvertToNullable<T>(value) ?? defaultValue;

        /// <summary>
        /// Makes a best effort conversion from a string to the given type.
        /// </summary>
        public static T? ConvertToNullable<T>(fstring? value)
        {
            if (value == null)
            {
                return default;
            }

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            else if (typeof(T) == typeof(int))
            {
                if (int.TryParse(value.s.Replace(",", ""), out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to integer.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T));
            }
            else if (typeof(T) == typeof(ulong))
            {
                if (ulong.TryParse(value.s.Replace(",", ""), out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to integer.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T));
            }
            else if (typeof(T) == typeof(float))
            {
                if (float.TryParse(value.s.Replace(",", ""), out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to float.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T));
            }
            else if (typeof(T) == typeof(double))
            {
                if (double.TryParse(value.s.Replace(",", ""), out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to double.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T));
            }
            else if (typeof(T) == typeof(bool))
            {
                value = value.s.Replace(",", "").ToLower().toF();

                if (value.s.All(char.IsNumber))
                {
                    value = int.Parse(value.s) != 0 ? "true".toF() : "false".toF();
                }

                if (bool.TryParse(value.s, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to boolean.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T));
            }
            else
            {
                throw new Exception($"Unsupported conversion type.");
            }
        }

        /// <summary>
        /// Makes a best effort conversion from a string to the given type.
        /// </summary>
        public static T ConvertTo<T>(fstring value)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            else if (typeof(T) == typeof(int))
            {
                if (int.TryParse(value.s, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to integer.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T));
            }
            else if (typeof(T) == typeof(ulong))
            {
                if (ulong.TryParse(value.s, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to integer.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T));
            }
            else if (typeof(T) == typeof(float))
            {
                if (float.TryParse(value.s, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to float.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T));
            }
            else if (typeof(T) == typeof(double))
            {
                if (double.TryParse(value.s, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to double.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T));
            }
            else if (typeof(T) == typeof(bool))
            {
                value = value.s.ToLower().toF();

                if (value.s.All(char.IsNumber))
                {
                    if (int.Parse(value.s) != 0)
                        value = "true".toF();
                    else
                        value = "false".toF();
                }

                if (bool.TryParse(value.s, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to boolean.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T));
            }
            else
            {
                throw new Exception($"Unsupported conversion type.");
            }
        }
    }
}
