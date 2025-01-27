namespace MD.Domain;

/// <summary>
/// Интерфейс для хранения и проверки валидности RefreshToken'ов.
/// </summary>
public interface IJwtRefreshTokenStorage
{
    /// <summary>
    /// Проверяет, является ли предоставленный RefreshToken действительным.
    /// </summary>
    /// <param name="refreshToken">Обновляющий токен для проверки.</param>
    /// <returns>True, если токен действителен, иначе false.</returns>
    Task<bool> IsValidAsync(JwtToken refreshToken);

    /// <summary>
    /// Удаляет RefreshToken из хранилища, делая его недействительным.
    /// </summary>
    /// <param name="refreshToken">Обновляющий токен для удаления.</param>
    Task RemoveAsync(JwtToken refreshToken);

    /// <summary>
    /// Сохраняет RefreshToken в хранилище с указанием времени действия.
    /// </summary>
    /// <param name="refreshToken">Обновляющий токен для сохранения.</param>
    /// <param name="expirationTime">Время действия токена.</param>
    Task StoreAsync(JwtToken refreshToken, TimeSpan expirationTime);
}