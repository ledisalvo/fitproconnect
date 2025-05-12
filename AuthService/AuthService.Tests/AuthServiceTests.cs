//using AuthService.Application.Dtos;
//using AuthService.Infrastructure.Data;
//using AuthService.Infrastructure.Services;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Moq;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;
//using AuthService.Domain.Entities;
//using System;
//using EntityFrameworkCore.Testing.Moq;


//namespace AuthService.Tests;

//public class AuthServiceTests
//{
//    [Fact]
//    public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
//    {
//        // Arrange
//        var fakeEmail = "admin@fitpro.com";
//        var fakePassword = "123456";
//        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(fakePassword);

//        var fakeUser = new User
//        {
//            Id = Guid.NewGuid(),
//            Email = fakeEmail,
//            PasswordHash = hashedPassword,
//            Role = "Admin"
//        };

//        // Mock DbContext con soporte para async queries
//        var dbContextMock = Create.MockedDbContextFor<AuthDbContext>();
//        dbContextMock.Set<User>().Add(fakeUser);
//        await dbContextMock.SaveChangesAsync();


//        // Mock IConfiguration
//        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
//        {
//            { "Jwt:Key", "esto-es-una-clave-super-segura-123456" },
//            { "Jwt:Issuer", "FitProConnect" },
//            { "Jwt:Audience", "FitProConnectUsers" }
//        }).Build();

//        var authService = new Infrastructure.Services.AuthService(dbContextMock, config);

//        var request = new LoginRequestDto
//        {
//            Email = fakeEmail,
//            Password = fakePassword
//        };

//        // Act
//        var result = await authService.LoginAsync(request);

//        // Assert
//        Assert.Equal(fakeEmail, result.Email);
//        Assert.False(string.IsNullOrEmpty(result.Token));
//    }

//    [Fact]
//    public async Task LoginAsync_ThrowsException_WhenPasswordIsInvalid()
//    {
//        // Arrange
//        var email = "admin@fitpro.com";
//        var correctPassword = "123456";
//        var wrongPassword = "abc123";

//        var user = new User
//        {
//            Id = Guid.NewGuid(),
//            Email = email,
//            PasswordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword),
//            Role = "Admin"
//        };

//        var dbContextMock = Create.MockedDbContextFor<AuthDbContext>();
//        dbContextMock.Set<User>().Add(user);
//        await dbContextMock.SaveChangesAsync();

//        var config = new ConfigurationBuilder()
//            .AddInMemoryCollection(new Dictionary<string, string>
//            {
//            { "Jwt:Key", "esto-es-una-clave-super-segura-123456" },
//            { "Jwt:Issuer", "FitProConnect" },
//            { "Jwt:Audience", "FitProConnectUsers" }
//            })
//            .Build();

//        var authService = new Infrastructure.Services.AuthService(dbContextMock, config);

//        var request = new LoginRequestDto
//        {
//            Email = email,
//            Password = wrongPassword
//        };

//        // Act & Assert
//        var ex = await Assert.ThrowsAsync<Exception>(() => authService.LoginAsync(request));
//        Assert.Equal("Credenciales inválidas", ex.Message);
//    }

//}
