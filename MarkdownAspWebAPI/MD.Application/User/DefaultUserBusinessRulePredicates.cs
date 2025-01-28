using MD.Domain;

namespace MD.Application;

public sealed class DefaultUserBusinessRulePredicates : IUserBusinessRulePredicates
{
    private readonly IUserRepository _repository;

    public DefaultUserBusinessRulePredicates(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<bool> IsUserEmailFree(string email)
    {
        return await _repository.GetUserByEmailAsync(email) is { IsSuccess: false};
    }
}