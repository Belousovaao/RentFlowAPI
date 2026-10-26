using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RentFlow.Api.Services.Auth;
using RentFlow.Api.Services.Requests;
using RentFlow.Api.Services.Responses;
using RentFlow.Application.Auth.Otp;
using RentFlow.Application.Auth.Registration;
using RentFlow.Application.Customers.Dtos;
using RentFlow.Application.Customers.Queries;
using RentFlow.Application.Interfaces;

namespace RentFlow.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IOtpService _otpService;
    private readonly IOtpAuthenticationService _otpAuthenticationService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRegistrationService _registrationService;

    public AuthController(
        IMediator mediator, 
        IOtpService otpService, 
        IOtpAuthenticationService otpAuthenticationService,
        IJwtTokenService jwtTokenService,
        IRegistrationService registrationService)
    {
        _mediator = mediator;
        _otpService = otpService;
        _otpAuthenticationService = otpAuthenticationService;
        _jwtTokenService = jwtTokenService;
        _registrationService = registrationService;
    }

    [HttpPost("otp/send")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<OtpChannel>(
                request.Channel,
                ignoreCase: true,
                out var channel))
        {
            return BadRequest(new
            {
                message = "Unsupported OTP channel."
            });
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return BadRequest(new
            {
                message = "Unable to determine client IP address."
            });
        }

        var result = await _otpService.SendAsync(
            channel,
            request.Destination,
            ipAddress,
            ct);

        if (result.Status == OtpSendResultStatus.RateLimited)
        {
            Response.Headers.RetryAfter = result.RetryAfter.ToString();

            return StatusCode(StatusCodes.Status429TooManyRequests,
                new
                {
                    message = "Too many OTP requests.",
                    retryAfter = result.RetryAfter
                });
        }

        return Ok(new SendOtpResponse(
            result.ChallengeId!.Value,
            result.ExpiresIn,
            result.RetryAfter));
    }

    [HttpPost("otp/verify")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request, CancellationToken ct)
    {
        if (!Enum.TryParse<OtpChannel>(
                request.Channel,
                ignoreCase: true,
                out var channel))
        {
            return BadRequest(new
            {
                message = "Unsupported OTP channel."
            });
        }

        var result = await _otpAuthenticationService.VerifyAsync(
            request.ChallengeId,
            channel,
            request.Destination,
            request.Code,
            ct);

        return result.VerificationResult switch
        {
            OtpVerificationResult.Success
                when result.Status == OtpAuthenticationStatus.Authenticated
                    && result.User is not null =>
                Ok(new VerifyOtpResponse(
                    true,
                    OtpAuthenticationStatus.Authenticated,
                    _jwtTokenService.CreateToken(result.User),
                    null)),

            OtpVerificationResult.Success
                when result.Status == OtpAuthenticationStatus.RegistrationRequired
                    && result.User is null =>
                Ok(new VerifyOtpResponse(
                    true,
                    OtpAuthenticationStatus.RegistrationRequired,
                    null,
                    result.RegistrationTicket)),

            OtpVerificationResult.InvalidCode =>
                BadRequest(new
                {
                    message = "Invalid OTP code."
                }),

            OtpVerificationResult.NotFound =>
                BadRequest(new
                {
                    message = "Invalid or expired OTP challenge."
                }),

            OtpVerificationResult.TooManyAttempts =>
                StatusCode(StatusCodes.Status429TooManyRequests,
                new
                {
                    message = "Too many verification attempts."
                }),

            _ => throw new InvalidOperationException(
                $"Unexpected OTP verification result: {result}")
        };
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _registrationService.RegisterAsync(
            request.RegistrationTicket,
            new RegisterCustomerData(
                request.CustomerType,
                request.Email,
                request.Phone,
                request.FirstName,
                request.LastName,
                request.Inn,
                request.ConsentDocumentId,
                request.PersonalDataConsentAccepted),
            cancellationToken);

        return result.Status switch
        {
            RegistrationResultStatus.Success
                when result.User is not null =>
                Ok(new RegisterResponse(
                    true,
                    _jwtTokenService.CreateToken(result.User))),

            RegistrationResultStatus.InvalidTicket =>
                BadRequest(new
                {
                    message = "Invalid or expired registration ticket."
                }),

            RegistrationResultStatus.ContactMismatch =>
                BadRequest(new
                {
                    message = "Registration contact does not match the verified contact."
                }),

            RegistrationResultStatus.InvalidRequest =>
                BadRequest(new
                {
                    message = "Invalid registration data."
                }),

            RegistrationResultStatus.AlreadyExists =>
                Conflict(new
                {
                    message = "A user with the specified contact already exists."
                }),

            _ => throw new InvalidOperationException(
                $"Unsupported registration result: {result.Status}.")
        };
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserDto>> Me(CancellationToken ct)
    {
        var userId = User.GetUserId();
        return Ok(await _mediator.Send(new GetCurrentUserQuery(userId), ct));
    }
}
