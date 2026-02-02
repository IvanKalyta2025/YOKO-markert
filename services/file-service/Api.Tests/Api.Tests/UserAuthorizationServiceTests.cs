using Api.Services;
using Api.Interfaces;
using Api.Contracts.Requests;
using Api.Domain.Entities;
using NSubstitute;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

//dotnet add package NSubstitute.Analyzers.CSharp



namespace Api.Tests;

public class UserAuthorizationServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly UserAuthorizationService _service;

    public UserAuthorizationServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "test_key_test_key_test_key_test_key_1234"
            })
            .Build();
        var jwtService = new JwtTokenService(config);
        _service = new UserAuthorizationService(_userRepository, jwtService);
    }

    [Theory]
    [InlineData("test@example.com", "Password123")]
    public async Task Register_ShouldReturnFailure_WhenEmailAlreadyExists(string email, string password)
    {
        //Arrange
        var existingUser = new User { Email = email };
        _userRepository.GetByEmailAsync(email).Returns(existingUser);
        var request = new RegisterRequest(email, password);
        //ACT
        var result = await _service.RegisterAsync(request);

        //ASSERT
        Assert.False(result.Success);
        Assert.Equal("Email already in use.", result.Message);
        await _userRepository.Received(0).AddAsync(Arg.Any<User>());
    }
}
