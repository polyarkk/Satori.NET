using System.Text.RegularExpressions;

namespace Satori.Protocol.Elements;

public static partial class ElementSerializer {
    private readonly static Regex PascalToKebabSplitRegex = new("(?=[A-Z])");

    internal static string ConvertPascalToKebab(string input) {
        return string.Join('-', PascalToKebabSplitRegex.Split(input)[1..]).ToLower();
    }

    internal static string ConvertKebabToPascal(string input) {
        IEnumerable<string>? parts = input.Split('-').Select(part => {
                char[]? arr = part.ToCharArray();
                arr[0] = char.ToUpper(arr[0]);
                return new string(arr);
            }
        );
        return string.Join("", parts);
    }
}
