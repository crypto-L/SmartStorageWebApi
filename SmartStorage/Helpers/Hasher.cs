using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

namespace Helpers;

public class Hasher
{
    public static string HashString(string toHash, string salt = "")
    {
        if (String.IsNullOrEmpty(toHash))
        {
            return String.Empty;
        }

        using var sha = SHA256.Create();
        // Convert the string to a byte array first, to be processed
        byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(toHash + salt);
        byte[] hashBytes = sha.ComputeHash(textBytes);
        
        // Convert back to a string, removing the '-' that BitConverter adds
        string hash = BitConverter
            .ToString(hashBytes)
            .Replace("-", String.Empty);

        return hash;
    }
}