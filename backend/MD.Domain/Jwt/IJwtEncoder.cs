using System.Security.Claims;

namespace MD.Domain;

/// <summary>
/// Интерфейс для кодирования и декодирования JWT токенов. 
/// Обеспечивает методы для создания, проверки и извлечения данных из токенов.
/// </summary>
public interface IJwtEncoder
{
    /// <summary>
    /// Создаёт JWT токен на основе предоставленных пользовательских утверждений (claims) 
    /// и указанного времени действия.
    /// </summary>
    /// <param name="userClaims">Список утверждений (claims), которые будут включены в токен.</param>
    /// <param name="expirationTime">Время действия токена.</param>
    /// <returns>Сгенерированный JWT токен.</returns>
    JwtToken CreateToken(IEnumerable<Claim> userClaims, TimeSpan expirationTime);

    /// <summary>
    /// Декодирует предоставленный JWT токен, извлекая из него утверждения (claims).
    /// </summary>
    /// <param name="token">JWT токен, который нужно декодировать.</param>
    /// <returns>Коллекция утверждений (claims), содержащихся в токене.</returns>
    IEnumerable<Claim> DecodeToken(JwtToken token);

    /// <summary>
    /// Проверяет корректность предоставленного JWT токена, включая его подпись, срок действия и структуру.
    /// </summary>
    /// <param name="token">JWT токен, который необходимо проверить.</param>
    /// <exception cref="SecurityTokenException">Выбрасывается, если токен недействителен.</exception>
    ClaimsPrincipal ValidateToken(JwtToken token);
}
