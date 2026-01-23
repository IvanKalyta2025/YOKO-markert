using Api.Services;
using Api.Repositories;
using Api.Models;
using NSubstitute;
using FluentAssertions;
using Xunit;
using System.Reactive.Concurrency;
using Moq;
using Microsoft.AspNetCore.Identity; //dotnet add package Moq 



namespace Api.Tests;

public class UserAuthorizationServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserAuthorizationService _userAuthorizationService;

    public UserAuthorizationServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userAuthorizationService = new UserAuthorizationService(_userRepositoryMock.Object);
    }

    [Theory]
    [InlineData("test@example.com", "Password123")]
    public async Task Register_ShouldReturnFailure_WhenEmailAlreadyExists(string email, string password)
    {
        //Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = password,
        };
        //ACT

        //ACCERT
    }
}
