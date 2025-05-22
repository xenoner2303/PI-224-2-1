using System.Text;
using System.Security.Cryptography;

namespace BLL.Services;

public static class PasswordHasher
{
    private const int saltLenght = 16; //16 байтів (128 біт)
    private const int keyLenght = 32;  //32 байти (256 біт)
    private const int iterationsNumber = 1000; //кількість ітерацій перехешування

    public static string HashPassword(string password)
    {
        //генерим випадкову сіль
        byte[] salt = RandomNumberGenerator.GetBytes(saltLenght);

        //хешуємо пароль разом із сіллю
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterationsNumber,
            HashAlgorithmName.SHA256,
            keyLenght
        );

        //повертаємо сіль + пароль(взагаліто можна форматувати масив байтів через розміри вище та потім їх перевіряти, але для наявності зроблю через точку)
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2)
        {
            return false;
        }

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] hash = Convert.FromBase64String(parts[1]);

        byte[] newHash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterationsNumber,
            HashAlgorithmName.SHA256,
            keyLenght
        );

        return CryptographicOperations.FixedTimeEquals(hash, newHash);
    }
}
