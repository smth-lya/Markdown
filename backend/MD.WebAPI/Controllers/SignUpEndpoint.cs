using FluentValidation;
using MD.Application;
using MD.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MD.WebAPI;

[ApiController]
[Route("api/auth")]
public sealed class SignUpEndpoint : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserBusinessRulePredicates _rulePredicates;

    public SignUpEndpoint(IUserService userService, IUserBusinessRulePredicates rulePredicates)
    {
        _userService = userService;
        _rulePredicates = rulePredicates;
    }

    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser([FromBody] SignUpRequest request, CancellationToken cancellationToken)
    {
        var validator = new SingUpRequestValidator(_rulePredicates);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return BadRequest(errors);
        }
        
        var result = await _userService.SignUpAsync(request.Email, request.Password, cancellationToken);

        var response = new SignUpResponse(result.Value.Id);
        return Ok(response);
    }

    public record SignUpRequest(string Email, string Password);

    public record SignUpResponse(Guid? UserId);

    public class SingUpRequestValidator : AbstractValidator<SignUpRequest>
    {
        public SingUpRequestValidator(IUserBusinessRulePredicates rulePredicates)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email cannot be empty")
                .EmailAddress()
                .WithMessage("Invalid email format")
                .MustAsync(async (email, _) => await rulePredicates.IsUserEmailFree(email))
                .WithMessage("Email is not free");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password cannot be empty")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long");
        }
    }
}
