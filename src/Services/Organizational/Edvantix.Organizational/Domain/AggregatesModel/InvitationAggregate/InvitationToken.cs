using System.Security.Cryptography;
using System.Text;

namespace Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

/// <summary>
/// Value object для токена приглашения.
/// Токен генерируется один раз при создании приглашения;
/// в БД хранится только хэш (SHA-256), сам токен передаётся пользователю единожды.
/// </summary>
public sealed record InvitationToken
{
    private InvitationToken(string value, string hash)
    {
        Value = value;
        Hash = hash;
    }

    /// <summary>Сырое значение токена (base64url, 64 символа). Хранится только в памяти.</summary>
    public string Value { get; }

    /// <summary>SHA-256 хэш токена в виде HEX-строки. Хранится в БД.</summary>
    public string Hash { get; }

    /// <summary>Генерирует новый криптографически стойкий токен.</summary>
    public static InvitationToken Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(48);
        var value = Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');

        return new InvitationToken(value, ComputeHash(value));
    }

    /// <summary>Вычисляет SHA-256 хэш для заданного значения токена.</summary>
    public static string ComputeHash(string tokenValue) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(tokenValue)));
}
