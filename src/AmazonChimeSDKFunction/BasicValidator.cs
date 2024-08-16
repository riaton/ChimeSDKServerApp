using ChimeApp.Models;
using Google.Protobuf;
using System.Collections;
using System.Text.RegularExpressions;

namespace ChimeApp
{
    internal static class BasicValidator
    {
        internal static List<string> Validate(IMessage message)
        {
            var results = new List<string>();
            foreach (var field in message.Descriptor.Fields.InDeclarationOrder())
            {
                var prop = message.GetType().GetProperty(field.PropertyName);
                if(prop == null) continue;

                dynamic? val = prop.GetValue(message);

                //valがIMessageの場合、再帰的にValidation
                if (val is IMessage)
                {
                    var errors = Validate(val);
                    foreach(var error in errors)
                    {
                        results.Add(error);
                    }
                    continue;
                }

                var fieldOption = field.GetOptions();
                var required = fieldOption?.GetExtension(ValidationRulesExtensions.Required);
                var strLen = fieldOption?.GetExtension(ValidationRulesExtensions.Strlen);
                var regex = fieldOption?.GetExtension(ValidationRulesExtensions.Regex);
                var minVal = fieldOption?.GetExtension(ValidationRulesExtensions.Minval);
                var maxVal = fieldOption?.GetExtension(ValidationRulesExtensions.Maxval);

                //valがstring型の場合
                if (prop.PropertyType == typeof(string))
                {
                    string? v = val;
                    //必須チェック(string)
                    if (required != null && string.IsNullOrEmpty(v))
                    {
                        results.Add($"Failed to Required Check. FieldName = {field.PropertyName}");
                    }
                    //文字列長チェック
                    if(strLen != null && v != null && v.Length > strLen)
                    {
                        results.Add($"Failed to StrLen Check. FieldName = {field.PropertyName}");
                    }
                    //正規表現チェック
                    if (regex != null && v != null && !Regex.IsMatch(v, regex))
                    {
                        results.Add($"Failed to Regex Check. FieldName = {field.PropertyName}");
                    }
                    continue;
                }
                //必須チェック(string以外)
                if (required != null && val == null)
                {
                    results.Add($"Failed to Required Check. FieldName = {field.PropertyName}");
                }
                //valが値型の場合
                if (prop.PropertyType.IsValueType)
                {
                    //minチェック
                    if (minVal != null && val < minVal)
                    {
                        results.Add($"Failed to MinVal Check. FieldName = {field.PropertyName}");
                    }
                    //maxチェック
                    if (maxVal != null && val > maxVal)
                    {
                        results.Add($"Failed to MaxVal Check. FieldName = {field.PropertyName}");
                    }
                    continue;
                }
                //listの場合、再帰的にValidation
                if (val is IEnumerable)
                {
                    foreach(var v in val)
                    {
                        if(v == null) results.Add($"{field.PropertyName} Contains null value.");
                        if (v is IMessage)
                        {
                            var errors = Validate(v);
                            foreach (var error in errors)
                            {
                                results.Add(error);
                            }
                        }
                    }
                    continue;
                }
            }
            return results;
        }
    }
}
