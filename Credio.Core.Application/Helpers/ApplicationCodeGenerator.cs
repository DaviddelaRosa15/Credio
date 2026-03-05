using System.Security.Cryptography;
using System.Text;

namespace Credio.Core.Application.Helpers;

public class ApplicationCodeGenerator
{
    private const string Prefix = "SOL";
    private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    public static string Generate()
    {
        string datePart = DateTime.UtcNow.ToString("yyMM");
        string randomPart = GenerateRandomString(4);

        return $"{Prefix}-{datePart}-{randomPart}";
    }
    
    private static string GenerateRandomString(int length)
    {
        StringBuilder result = new StringBuilder(length);
        
        byte[] buffer = new byte[length];

        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        
        // fill the buffer array with random sequences of values (0-255)
        rng.GetBytes(buffer);

        for (int i = 0; i < length; i++)
        {
            int index = buffer[i] % AllowedChars.Length; // module result base on the buffer number and allowed Chars length
            
            result.Append(AllowedChars[index]);  // base on the result select a random chars 
        }

        return result.ToString();
    }
}