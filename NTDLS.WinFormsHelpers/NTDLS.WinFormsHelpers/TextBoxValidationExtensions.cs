using NTDLS.Helpers;

namespace NTDLS.WinFormsHelpers
{
    /// <summary>
    /// Various functions for getting and validating values from WinForms textbox controls.
    /// </summary>
    public static class TextBoxValidationExtensions
    {
        /// <summary>
        /// Gets a value from a windows textbox and converts it to the specified type.
        /// </summary>
        public static T ValueAs<T>(this TextBox textBox)
            => Converters.ConvertTo<T>(textBox.Text);

        /// <summary>
        /// Gets an integer value from a windows textbox. Ensures that the value falls within the given ranges.
        /// </summary>
        /// <param name="textBox">Text box to get the value from.</param>
        /// <param name="minValue">The minimum value for the parsed control text validation.</param>
        /// <param name="maxValue">The maximum value for the parsed control text validation.</param>
        /// <param name="message">Message for the exception which is thrown when validation fails. Use [min] and [max] for place holders of the given minValue and maxValue.</param>
        /// <returns>Returns the parsed integer.</returns>
        public static int GetAndValidateNumeric(this TextBox textBox, int minValue, int maxValue, string message)
        {
            message = message.Replace("[min]", $"{minValue:n0}", StringComparison.InvariantCultureIgnoreCase);
            message = message.Replace("[max]", $"{maxValue:n0}", StringComparison.InvariantCultureIgnoreCase);

            if (int.TryParse(textBox.Text.Replace(",", ""), out var value))
            {
                if (value < minValue || value > maxValue)
                {
                    throw new Exception(message);
                }
                return value;
            }
            throw new Exception(message);
        }

        /// <summary>
        /// Gets a double floating value from a windows textbox. Ensures that the value falls within the given ranges.
        /// </summary>
        /// <param name="textBox">Text box to get the value from.</param>
        /// <param name="minValue">The minimum value for the parsed control text validation.</param>
        /// <param name="maxValue">The maximum value for the parsed control text validation.</param>
        /// <param name="message">Message for the exception which is thrown when validation fails. Use [min] and [max] for place holders of the given minValue and maxValue.</param>
        /// <returns>Returns the parsed double.</returns>
        public static double GetAndValidateNumeric(this TextBox textBox, double minValue, double maxValue, string message)
        {
            message = message.Replace("[min]", $"{minValue:n0}", StringComparison.InvariantCultureIgnoreCase);
            message = message.Replace("[max]", $"{maxValue:n0}", StringComparison.InvariantCultureIgnoreCase);

            if (double.TryParse(textBox.Text.Replace(",", ""), out var value))
            {
                if (value < minValue || value > maxValue)
                {
                    throw new Exception(message);
                }
                return value;
            }
            throw new Exception(message);
        }

        /// <summary>
        /// Gets a float floating value from a windows textbox. Ensures that the value falls within the given ranges.
        /// </summary>
        /// <param name="textBox">Text box to get the value from.</param>
        /// <param name="minValue">The minimum value for the parsed control text validation.</param>
        /// <param name="maxValue">The maximum value for the parsed control text validation.</param>
        /// <param name="message">Message for the exception which is thrown when validation fails. Use [min] and [max] for place holders of the given minValue and maxValue.</param>
        /// <returns>Returns the parsed float.</returns>
        public static float GetAndValidateNumeric(this TextBox textBox, float minValue, float maxValue, string message)
        {
            message = message.Replace("[min]", $"{minValue:n0}", StringComparison.InvariantCultureIgnoreCase);
            message = message.Replace("[max]", $"{maxValue:n0}", StringComparison.InvariantCultureIgnoreCase);

            if (float.TryParse(textBox.Text.Replace(",", ""), out var value))
            {
                if (value < minValue || value > maxValue)
                {
                    throw new Exception(message);
                }
                return value;
            }
            throw new Exception(message);
        }

        /// <summary>
        /// Gets a string value from a windows textbox. Ensures that the length falls within the given ranges.
        /// </summary>
        /// <param name="textBox">Text box to get the value from.</param>
        /// <param name="minLength">The minimum length of the control text.</param>
        /// <param name="maxLength">The maximum length of the control text.</param>
        /// <param name="message">Message for the exception which is thrown when validation fails. Use [min] and [max] for place holders of the given minLength and maxLength.</param>
        /// <returns>Returns the control text.</returns>
        public static string GetAndValidateText(this TextBox textBox, int minLength, int maxLength, string message)
        {
            message = message.Replace("[min]", $"{minLength:n0}", StringComparison.InvariantCultureIgnoreCase);
            message = message.Replace("[max]", $"{maxLength:n0}", StringComparison.InvariantCultureIgnoreCase);

            int length = textBox.Text.Length;

            if (length < minLength || length > maxLength)
            {
                throw new Exception(message);
            }
            return textBox.Text.Trim();

            throw new Exception(message);
        }

        /// <summary>
        /// Gets a string value from a windows textbox. Ensures that it contains a value.
        /// </summary>
        /// <param name="textBox">Text box to get the value from.</param>
        /// <param name="message">Message for the exception which is thrown when validation fails.</param>
        /// <returns>Returns the control text.</returns>
        public static string GetAndValidateText(this TextBox textBox, string message)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text.Trim()))
            {
                throw new Exception(message);
            }
            return textBox.Text.Trim();

            throw new Exception(message);
        }
    }
}
