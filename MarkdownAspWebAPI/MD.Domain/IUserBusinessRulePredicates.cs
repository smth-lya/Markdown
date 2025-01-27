namespace MD.Domain;

public interface IUserBusinessRulePredicates
{
    public Task<bool> IsUserEmailFree(string email);   
}