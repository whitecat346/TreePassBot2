using System.Security.Cryptography;

namespace TreePassBot2.BotEngine.Utils;

public static class VerificationCodeGenerator
{
    public static string GenerateNumericCode(int length = 6)
    {
        Span<char> chars = stackalloc char[length];

        for (var i = 0; i < length; i++)
        {
            chars[i] = (char)('0' + RandomNumberGenerator.GetInt32(0, 10));
        }

        return new string(chars);
    }
}
