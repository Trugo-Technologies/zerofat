using System.Text;
using Microsoft.Extensions.Localization;

namespace ZeroFat.Application.Common.Extensions;
public static class StringExtensions
{
    public static bool HasValue(this string? text) => !string.IsNullOrWhiteSpace(text);
    public static bool IsEmpty(this string? text) => !HasValue(text);
    public static string GenerateDigitCode(int length)
    {
        const string digits = "0123456789";
        StringBuilder codeBuilder = new StringBuilder();
        Random random = new Random();

        for (int i = 0; i < length; i++)
        {
            int index = random.Next(digits.Length);
            codeBuilder.Append(digits[index]);
        }

        return codeBuilder.ToString();
    }
}
