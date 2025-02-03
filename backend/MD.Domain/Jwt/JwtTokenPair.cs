namespace MD.Domain;

/// <summary>
/// Представляет пару JWT токенов: AccessToken и RefreshToken.
/// </summary>
/// <param name="AccessToken">Токен доступа, используемый для аутентификации запросов.</param>
/// <param name="RefreshToken">Обновляющий токен, используемый для получения новой пары токенов.</param>
public sealed record JwtTokenPair(JwtToken AccessToken, JwtToken RefreshToken);
