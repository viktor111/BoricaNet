using System.Security.Cryptography;
using System.Text;

namespace BoricaNet.Helpers;

internal static class Generator
{
    public static string GenerateHexStringFromByteArray(byte[] byteArray)
    {
        return BitConverter.ToString(byteArray).Replace("-","");
    }
    
    public static byte[] GenerateRandomByteArray(int minLength, int maxLength)
    {
        var random = RandomNumberGenerator.Create();
        var length = new byte[4];
        random.GetBytes(length);
        var arrayLength = BitConverter.ToInt32(length, 0);
        arrayLength = Math.Abs(arrayLength % (maxLength - minLength + 1)) + minLength;
        var array = new byte[arrayLength];
        random.GetBytes(array);
        return array;
    }
    
    public static int GenerateOrderNumber(string nonce)
    {
        var data = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
        var nonceBytes = Encoding.UTF8.GetBytes(nonce);
        var inputData = new byte[data.Length + nonceBytes.Length];

        Buffer.BlockCopy(data, 0, inputData, 0, data.Length);
        Buffer.BlockCopy(nonceBytes, 0, inputData, data.Length, nonceBytes.Length);

        byte[] hash;

        using (var hmac = new HMACSHA256(GetKey()))
        {
            hash = hmac.ComputeHash(inputData);
        }

        var randomNumber = BitConverter.ToInt32(hash, 0) % 900000 + 100000;

        
        var str = Math.Abs(randomNumber);

        if (str.ToString().Length < 6)
        {
            return GenerateOrderNumber(nonce);
        }
        
        return Math.Abs(randomNumber);
    }

    
    private static byte[] GetKey()
    {
        var secretKey = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(secretKey);

        return secretKey;
    }
}