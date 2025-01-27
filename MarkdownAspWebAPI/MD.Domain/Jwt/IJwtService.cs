using Ardalis.Result;

namespace MD.Domain;

/// <summary>
/// Интерфейс для работы с JWT токенами. Обеспечивает операции генерации, обновления и аннулирования токенов.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Выпускает новую пару токенов (AccessToken и RefreshToken) для указанного пользователя.
    /// </summary>
    /// <param name="user">Пользователь, для которого генерируются токены.</param>
    /// <returns>Пара токенов (AccessToken и RefreshToken).</returns>
    Task<JwtTokenPair> IssueAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет токены, используя предоставленный RefreshToken, если он действителен.
    /// </summary>
    /// <param name="refreshToken">Обновляющий токен, используемый для обновления AccessToken.</param>
    /// <returns>
    /// Результат операции обновления. Содержит новую пару токенов (AccessToken и RefreshToken) 
    /// в случае успеха или ошибку в случае сбоя.
    /// </returns>
    Task<Result<JwtTokenPair>> RefreshAsync(JwtToken refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Аннулирует предоставленный RefreshToken, делая его недействительным.
    /// </summary>
    /// <param name="refreshToken">Обновляющий токен, который требуется аннулировать.</param>
    void InvalidateRefreshToken(JwtToken refreshToken);
}
