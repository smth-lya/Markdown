namespace MD.Domain;

/// <summary>
/// Представляет JWT токен.
/// </summary>
/// <param name="Token">Строковое значение токена.</param>
public sealed record JwtToken(string Token);
