using System.Security.Cryptography;
using System.Xml.Linq;

class Program
{
    static void Main(string[] args)
    {
        string filePath = "customer_info.xml";
        string encryptedFilePath = "encrypted_data.xml";

        byte[] aesKey = GenerateRandomBytes(32);
        byte[] aesIV = GenerateRandomBytes(16);

        XDocument doc = XDocument.Load(filePath);

        foreach (var customer in doc.Descendants("customer"))
        {
            string creditCard = customer.Element("creditcard")?.Value;
            string password = customer.Element("password")?.Value;

            string encryptedCreditCard = Convert.ToBase64String(EncryptStringToBytes(creditCard, aesKey, aesIV));
            customer.Element("creditcard").Value = encryptedCreditCard;

            byte[] salt = GenerateRandomBytes(16);
            byte[] hashedPassword = HashPasswordWithSalt(password, salt);
            string passwordHashBase64 = Convert.ToBase64String(hashedPassword);
            string saltBase64 = Convert.ToBase64String(salt);
            customer.Element("password").Value = $"{saltBase64}:{passwordHashBase64}";
        }

        doc.Save(encryptedFilePath);

        Console.WriteLine("XML File Output To: " + encryptedFilePath);
    }

    static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = key;
        aesAlg.IV = iv;

        using MemoryStream msEncrypt = new();
        using CryptoStream csEncrypt = new(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write);
        using (StreamWriter swEncrypt = new(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        return msEncrypt.ToArray();
    }

    static byte[] HashPasswordWithSalt(string password, byte[] salt)
    {
        using Rfc2898DeriveBytes pbkdf2 = new(password, salt, 10000, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(32); 
    }

    static byte[] GenerateRandomBytes(int length)
    {
        byte[] bytes = new byte[length];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return bytes;
    }
}
