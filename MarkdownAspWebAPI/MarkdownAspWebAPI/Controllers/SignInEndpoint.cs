using Ardalis.Result;
using FluentValidation;
using MD.Application;
using MD.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MD.WebAPI;

[ApiController]
[Route("auth")]
public sealed class SignInEndpoint : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserBusinessRulePredicates _rulePredicates;

    public SignInEndpoint(IUserService userService, IUserBusinessRulePredicates rulePredicates)
    {
        _userService = userService;
        _rulePredicates = rulePredicates;
    }

    [HttpPost("signin")]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request, CancellationToken ct)
    {
        var validator = new SignInValidator(_rulePredicates);
        var validationResult = await validator.ValidateAsync(request, ct);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return BadRequest(errors);
        }

        Result<JwtTokenPair> result = request switch
        {
            { Email: not null } => await _userService.SignInAsync(request.Email, request.Password, ct),
            _ => Result<JwtTokenPair>.Error("Username or email must be set")
        };

        if (!result.IsSuccess)
        {
            return BadRequest(string.Join("; ", result.Errors));
        }

        var response = new SignInResponse(result.Value.AccessToken.Token, result.Value.RefreshToken.Token);
        return Ok(response);
    }

    public record SignInRequest(string Email, string Password);

    public record SignInResponse(string AccessToken, string RefreshToken);

    public sealed class SignInValidator : AbstractValidator<SignInRequest>
    {
        public SignInValidator(IUserBusinessRulePredicates rulePredicates)
        {
            RuleFor(x => x.Email)
                    .NotEmpty()
                    .WithMessage("Email is required if username is empty")
                    .EmailAddress()
                    .MustAsync(async (email, _) => !await rulePredicates.IsUserEmailFree(email))
                    .WithMessage("User with this email doesn't exist");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password cannot be empty");
        }
    }
}
