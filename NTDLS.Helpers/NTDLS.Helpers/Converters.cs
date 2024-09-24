﻿using fs;
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
        public static T ConvertTo<T>(string? value, T defaultValue)
            => ConvertToNullable<T>(value) ?? defaultValue;

        public static T ConvertTo<T>(fstring? value)
            => ConvertToNullable<T>(value);


        public static T ConvertTo<T>(fstring? value, T defaultValue)
            => ConvertToNullable<T>(value.s) ?? defaultValue;

        public static T? ConvertToNullable<T>(fstring? value)
            => ConvertToNullable<T>(value.s);

        /// <summary>
        /// Makes a best effort conversion from a string to the given type.
        /// </summary>
        public static T? ConvertToNullable<T>(string? value)
        {
            if (value == null)
            {
                return default;
            }

            var targetType = typeof(T);
            if (Nullable.GetUnderlyingType(targetType) != null)
            {
                targetType = Nullable.GetUnderlyingType(targetType);
            }

            if (typeof(T) == typeof(string))
            {
                return (T?)Convert.ChangeType(value, targetType.EnsureNotNull());
            }
            else if (typeof(T) == typeof(int))
            {
                if (int.TryParse(value.Replace(",", ""), out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to integer.");
                }
                return (T?)Convert.ChangeType(parsedResult, targetType.EnsureNotNull());
            }
            else if (typeof(T) == typeof(ulong?))
            {
                if (ulong.TryParse(value.Replace(",", ""), out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to integer.");
                }
                return (T?)Convert.ChangeType(parsedResult, targetType.EnsureNotNull());
            }
            else if (typeof(T) == typeof(float))
            {
                if (float.TryParse(value.Replace(",", ""), out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to float.");
                }
                return (T?)Convert.ChangeType(parsedResult, targetType.EnsureNotNull());
            }
            else if (typeof(T) == typeof(double))
            {
                if (double.TryParse(value.Replace(",", ""), out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to double.");
                }
                return (T?)Convert.ChangeType(parsedResult, targetType.EnsureNotNull());
            }
            else if (typeof(T) == typeof(bool))
            {
                value = value.Replace(",", "").ToLower();

                if (value.All(char.IsNumber))
                {
                    value = int.Parse(value) != 0 ? "true" : "false";
                }

                if (bool.TryParse(value, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to boolean.");
                }
                return (T?)Convert.ChangeType(parsedResult, targetType.EnsureNotNull());
            }
            else
            {
                throw new Exception($"Unsupported conversion type.");
            }
        }

        /// <summary>
        /// Makes a best effort conversion from a string to the given type.
        /// </summary>
        public static T ConvertTo<T>(string value)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            else if (typeof(T) == typeof(int))
            {
                if (int.TryParse(value, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to integer.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T));
            }
            else if (typeof(T) == typeof(ulong))
            {
                if (ulong.TryParse(value, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to integer.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T));
            }
            else if (typeof(T) == typeof(float))
            {
                if (float.TryParse(value, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to float.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T));
            }
            else if (typeof(T) == typeof(double))
            {
                if (double.TryParse(value, out var parsedResult) == false)
                {
                    throw new Exception($"Error converting value [{value}] to double.");
                }
                return (T)Convert.ChangeType(parsedResult, typeof(T));
            }
            else if (typeof(T) == typeof(bool))
            {
                value = value.ToLower();

                if (value.All(char.IsNumber))
                {
                    if (int.Parse(value) != 0)
                        value = "true";
                    else
                        value = "false";
                }

                if (bool.TryParse(value, out var parsedResult) == false)
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
