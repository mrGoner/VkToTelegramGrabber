using System;
using System.Text;
using System.Collections.Generic;

namespace VkApi.Extensions;

public static class EnumExtensions
{
    public static string ConvertToSnakeCase<T>(this T enumValue) where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException($"{typeof(T)} must be enum!");

        var enumString = enumValue.ToString();

        var sb = new StringBuilder();

        sb.Append(char.ToLowerInvariant(enumString[0]));

        for (int i = 1; i < enumString.Length; ++i)
        {
            char c = enumString[i];
            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    public static IEnumerable<Enum> GetFlags<T>(this T input) where T : Enum
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException($"{typeof(T)} must be enum!");

        foreach (Enum value in Enum.GetValues(input.GetType()))
            if (input.HasFlag(value))
                yield return value;
    }
}