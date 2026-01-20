using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Services;
using NSubstitute.Routing.Handlers;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserAuthorizationService _userService;

    public AuthController(UserAuthorizationService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await _userService.RegisterAsync(request);

        if (!result.Success)
            return BadRequest(new { error = result.Message });

        return Ok(new
        {
            message = result.Message,
            userId = result.User?.Id
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _userService.LoginAsync(request);

        if (!result.Success)
            return BadRequest(new { error = result.Message });
        else
            return Ok(new
            {
                message = result.Message,
                userId = result.User?.Id
            });
    }
    [HttpPost("change")]
    public async Task<IActionResult> Change(ChangeRequest request)
    {
        var result = await _userService.ChangePasswordAsync(request);

        if (!result.Success)
            return BadRequest(new { error = result.Message });

        else
            return Ok(new
            {
                message = result.Message,
                userId = result.User?.Id
            }
            );
    }
}


