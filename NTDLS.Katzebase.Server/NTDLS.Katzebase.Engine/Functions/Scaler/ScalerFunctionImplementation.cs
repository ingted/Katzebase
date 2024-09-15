using fs;
using NTDLS.Katzebase.Client.Exceptions;
using NTDLS.Katzebase.Client.Types;
using NTDLS.Katzebase.Engine.Atomicity;
using NTDLS.Katzebase.Engine.Documents;
using NTDLS.Katzebase.Engine.Functions.Parameters;
using NTDLS.Katzebase.Engine.Query.Constraints;
using System.Globalization;
using System.Text;
using static NTDLS.Katzebase.Engine.Library.EngineConstants;

namespace NTDLS.Katzebase.Engine.Functions.Scaler
{
    /// <summary>
    /// Contains all function protype definitions, function implementations and expression collapse functionality.
    /// </summary>
    internal class ScalerFunctionImplementation
    {
        internal static string[] PrototypeStrings = {
                //"Function_Name:ReturnType:param_type/Param_Name=default_value,param_type/Param_Name=default_value
                "Guid:string:",
                "IsGreater:boolean:numeric/value1,numeric/value2",
                "IsLess:boolean:numeric/value1,numeric/value2",
                "IsGreaterOrEqual:boolean:numeric/value1,numeric/value2",
                "IsLessOrEqual:boolean:numeric/value1,numeric/value2",
                "IsBetween:boolean:numeric/value,numeric/rangeLow,numeric/rangeHigh",
                "IsNotBetween:boolean:numeric/value,numeric/rangeLow,numeric/rangeHigh",
                "IsEqual:boolean:string/text1,string/text2",
                "IsNotEqual:boolean:string/text1,string/text2",
                "IsLike:boolean:string/text,string/pattern",
                "IsNotLike:boolean:string/text,string/pattern",
                "DocumentUID:string:string/schemaAlias",
                "DocumentPage:string:string/schemaAlias",
                "DocumentID:string:string/schemaAlias",
                "DateTimeUTC:string:optional_string/format=yyyy-MM-dd HH:mm:ss.ms",
                "DateTime:string:optional_string/format=yyyy-MM-dd HH:mm:ss.ms",
                "ToProper:string:string/text",
                "ToLower:string:string/text",
                "ToUpper:string:string/text",
                "Length:numeric:string/text",
                "SubString:string:string/text,numeric/startIndex,numeric/length",
                "Coalesce:string:infinite_string/text",
                "Concat:string:infinite_string/text",
                "Trim:string:string/text",
                "Checksum:numeric:string/text",
                "Sha1:string:string/text",
                "IndexOf:string:string/textToFind,string/textToSearch",
                "LastIndexOf:numeric:string/textToFind,string/textToSearch",
                "Sha256:string:string/text",
                "Right:string:string/text,numeric/length",
                "Left:string:string/text,numeric/length",
                "IIF:string:boolean/condition,string/whenTrue,string/whenFalse",
            };

        internal static fstring? CollapseAllFunctionParameters(Transaction transaction, FunctionParameterBase param, KbInsensitiveDictionary<fstring?> rowFields)
        {
            if (param is FunctionConstantParameter functionConstantParameter)
            {
                var value = functionConstantParameter.RawValue.s;
                if (value.StartsWith('\'') && value.EndsWith('\''))
                {
                    return fstring.NewS(value.Substring(1, value.Length - 2));
                }
                return fstring.NewS(value);
            }
            else if (param is FunctionDocumentFieldParameter functionDocumentFieldParameter)
            {
                var result = rowFields.SingleOrDefault(o => o.Key == functionDocumentFieldParameter.Value.Key).Value
                    ?? throw new KbFunctionException($"Field was not found when processing function: {functionDocumentFieldParameter.Value.Key}.");

                if (result.IsS)
                {
                    return result; //((fstring.S)result).Item;
                }
                else
                {
                    throw new KbFunctionException($"fstring.S Expected.");
                }
            }
            else if (param is FunctionExpression functionExpression)
            {
                var expression = new NCalc.Expression(functionExpression.Value.s.Replace("{", "(").Replace("}", ")"));

                foreach (var subParam in functionExpression.Parameters)
                {
                    if (subParam is FunctionWithParams)
                    {
                        string variable = ((FunctionNamedWithParams)subParam).ExpressionKey.Replace("{", "").Replace("}", "");
                        var value = CollapseAllFunctionParameters(transaction, subParam, rowFields);
                        if (value != null)
                        {
                            expression.Parameters.Add(variable, decimal.Parse(value.s));
                        }
                        else
                        {
                            expression.Parameters.Add(variable, null);
                        }
                    }
                    else if (subParam is FunctionDocumentFieldParameter)
                    {
                        string variable = ((FunctionDocumentFieldParameter)subParam).ExpressionKey.Replace("{", "").Replace("}", "");
                        var value = rowFields.FirstOrDefault(o => o.Key == ((FunctionDocumentFieldParameter)subParam).Value.Key).Value;
                        if (value != null)
                        {
                            expression.Parameters.Add(variable, decimal.Parse(value.s));
                        }
                        else
                        {
                            expression.Parameters.Add(variable, null);
                        }
                    }
                    else
                    {
                        throw new KbNotImplementedException();
                    }
                }

                return fstring.NewS(expression.Evaluate()?.ToString() ?? string.Empty);
            }
            else if (param is FunctionWithParams functionWithParams)
            {
                var subParams = new List<fstring?>();

                foreach (var subParam in functionWithParams.Parameters)
                {
                    subParams.Add(CollapseAllFunctionParameters(transaction, subParam, rowFields));
                }

                return ExecuteFunction(transaction, functionWithParams.Function, subParams, rowFields);
            }
            else
            {
                //What is this?
                throw new KbNotImplementedException();
            }
        }


        private static fstring? ExecuteFunction(Transaction transaction, string functionName, List<fstring?> parameters, KbInsensitiveDictionary<fstring?> rowFields)
        {
            var proc = ScalerFunctionCollection.ApplyFunctionPrototype(functionName, parameters);

            switch (functionName.ToLowerInvariant())
            {
                case "documentuid":
                    {
                        var rowId = rowFields.FirstOrDefault(o => o.Key == $"{proc.Get<string>("schemaAlias")}.{UIDMarker}");
                        return rowId.Value;
                    }
                case "documentid":
                    {
                        var rowId = rowFields.FirstOrDefault(o => o.Key == $"{proc.Get<string>("schemaAlias")}.{UIDMarker}");
                        if (rowId.Value == null)
                        {
                            return null;
                        }
                        return DocumentPointer.Parse(rowId.Value.s).DocumentId.ToString().toF();
                    }
                case "documentpage":
                    {
                        var rowId = rowFields.FirstOrDefault(o => o.Key == $"{proc.Get<string>("schemaAlias")}.{UIDMarker}");
                        if (rowId.Value == null)
                        {
                            return null;
                        }
                        return DocumentPointer.Parse(rowId.Value.s).PageNumber.ToString().toF();
                    }

                case "isgreater":
                    return (Condition.IsMatchGreater(transaction, proc.Get<int>("value1"), proc.Get<int>("value2")) == true).ToString().toF();
                case "isless":
                    return (Condition.IsMatchLesser(transaction, proc.Get<int>("value1"), proc.Get<int>("value2")) == true).ToString().toF();
                case "isgreaterorequal":
                    return (Condition.IsMatchGreater(transaction, proc.Get<int>("value1"), proc.Get<int>("value2")) == true).ToString().toF();
                case "islessorequal":
                    return (Condition.IsMatchLesserOrEqual(transaction, proc.Get<int>("value1"), proc.Get<int>("value2")) == true).ToString().toF();
                case "isbetween":
                    return (Condition.IsMatchBetween(transaction, proc.Get<int>("value"), proc.Get<int>("rangeLow"), proc.Get<int>("rangeHigh")) == true).ToString().toF();
                case "isnotbetween":
                    return (Condition.IsMatchBetween(transaction, proc.Get<int>("value"), proc.Get<int>("rangeLow"), proc.Get<int>("rangeHigh")) == false).ToString().toF();
                case "isequal":
                    return (Condition.IsMatchEqual(transaction, proc.Get<string>("text1").toF(), proc.Get<string>("text2").toF()) == true).ToString().toF();
                case "isnotequal":
                    return (Condition.IsMatchEqual(transaction, proc.Get<string>("text1").toF(), proc.Get<string>("text2").toF()) == false).ToString().toF();
                case "islike":
                    return (Condition.IsMatchLike(transaction, proc.Get<string>("text"), proc.Get<string>("pattern")) == true).ToString().toF();
                case "isnotlike":
                    return (Condition.IsMatchLike(transaction, proc.Get<string>("text"), proc.Get<string>("pattern")) == false).ToString().toF();

                case "guid":
                    return Guid.NewGuid().ToString().toF();

                case "datetimeutc":
                    return DateTime.UtcNow.ToString(proc.Get<string>("format")).toF();
                case "datetime":
                    return DateTime.Now.ToString(proc.Get<string>("format")).toF();

                case "checksum":
                    return Library.Helpers.Checksum(proc.Get<string>("text")).ToString().toF();
                case "sha1":
                    return Library.Helpers.GetSHA1Hash(proc.Get<string>("text")).toF();
                case "sha256":
                    return Library.Helpers.GetSHA256Hash(proc.Get<string>("text")).toF();
                case "indexof":
                    return proc.Get<string>("textToSearch").IndexOf(proc.Get<string>("textToFind")).ToString().toF();
                case "lastindexof":
                    return proc.Get<string>("textToSearch").LastIndexOf(proc.Get<string>("textToFind")).ToString().toF();
                case "right":
                    return proc.Get<string>("text").Substring(proc.Get<string>("text").Length - proc.Get<int>("length")).toF();
                case "left":
                    return proc.Get<string>("text").Substring(0, proc.Get<int>("length")).toF();
                case "iif":
                    {
                        if (proc.Get<bool>("condition"))
                            return proc.Get<string>("whenTrue").toF();
                        else return proc.Get<string>("whenFalse").toF();
                    }
                case "toproper":
                    return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(proc.Get<string>("text")).toF();
                case "tolower":
                    return proc.Get<string>("text").ToLowerInvariant().toF();
                case "toupper":
                    return proc.Get<string>("text").ToUpperInvariant().toF();
                case "length":
                    return proc.Get<string>("text").Length.ToString().toF();
                case "trim":
                    return proc.Get<string>("text").Trim().toF();
                case "substring":
                    return proc.Get<string>("text").Substring(proc.Get<int>("startIndex"), proc.Get<int>("length")).toF();
                case "concat":
                    {
                        var builder = new StringBuilder();
                        foreach (var p in parameters)
                        {
                            builder.Append(p);
                        }
                        return builder.ToString().toF();
                    }
                case "coalesce":
                    {
                        foreach (var p in parameters)
                        {
                            if (p != null)
                            {
                                return p;
                            }
                        }
                        return null;
                    }

            }

            throw new KbFunctionException($"Undefined function: {functionName}.");
        }
    }
}
