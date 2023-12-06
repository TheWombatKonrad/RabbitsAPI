using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;
using System.Text.Json;
using System.Text;
using WonderfulRabbitsApi.Models.Users;
using System.Net.Http.Headers;
using WonderfulRabbitsApi.DatabaseContext;
using AutoMapper;
using WonderfulRabbitsApi.Helpers;

namespace WonderfulRabbitsApiTests.IntegrationTests;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TestDataHelper _helper;
    private readonly IMapper _mapper;

    public UsersControllerTests()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        _helper = new TestDataHelper();
        _scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
        _mapper = new Mapper(AutoMapperConfiguration.Configure());
    }

    [Fact]
    public async Task RegisterUser_WhenAUserIsRegistered_ThenAnIdIsReturned()
    {
        //Arrange
        var userModel = _helper.GetRegisterUserModel();
        var json = JsonSerializer.Serialize(userModel);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/users/register")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        //Act
        var response = await client.SendAsync(request);
        var responseUser = await response.Content.ReadFromJsonAsync<UserModel>();

        //Assert
        response.Should().BeSuccessful();
        responseUser.Id.Should().Be(1);
    }

    [Fact]
    public async Task AuthenticateUser_WhenUsernameAndPasswordIsValid_ThenTheRequestShouldBeSuccessful()
    {
        //Arrange
        var user = _helper.GetUsers(1)[0];
        user.PasswordHash = _helper.HashPassword("password1234");

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<RabbitDbContext>();

            context.Users.Add(user);
            context.SaveChanges();
        }

        var requestModel = new AuthenticateRequestModel()
        {
            Username = user.Username,
            Password = "password1234"
        };

        var json = JsonSerializer.Serialize(requestModel);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/users/authenticate")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        //Act
        var response = await client.SendAsync(request);

        //Assert
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task AuthenticateUser_WhenUsernameAndPasswordIsInvalid_ThenTheRequestShouldNotBeSuccesful()
    {
        //Arrange
        var user = _helper.GetUsers(1)[0];
        user.PasswordHash = _helper.HashPassword("password1234");

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<RabbitDbContext>();

            context.Users.Add(user);
            context.SaveChanges();
        }

        var requestModel = new AuthenticateRequestModel()
        {
            Username = user.Username,
            Password = "wrongPassword1234"
        };

        var json = JsonSerializer.Serialize(requestModel);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/users/authenticate")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        //Act
        var response = await client.SendAsync(request);

        //Assert
        response.Should().NotHaveStatusCode(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateUser_WhenUpdateRequestIsSent_ThenTheUserShouldBeUpdated()
    {
        //Arrange
        var user = _helper.GetUsers(1)[0];
        user.PasswordHash = _helper.HashPassword("password1234");

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<RabbitDbContext>();

            context.Users.Add(user);
            context.SaveChanges();
        }

        var token = await _helper.GetAuthenticationToken(client, new AuthenticateRequestModel()
        {
            Username = user.Username,
            Password = "password1234"
        });

        var requestModel = new UpdateUserModel()
        {
            Username = "newUsername"
        };

        var json = JsonSerializer.Serialize(requestModel);

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/users/{user.Id}")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await client.SendAsync(request);
        var updatedUser = await _helper.GetUserFromClient(client, user.Id);

        //Assert
        response.Should().BeSuccessful();
        updatedUser.Username.Should().Be("newUsername");

    }

    [Fact]
    public async Task GetUser_WhenRequestIsSent_ThenTheUserShouldBeReturned()
    {
        //Arrange 
        var user = _helper.GetFullUsers(1)[0];

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<RabbitDbContext>();

            context.Users.Add(user);
            context.SaveChanges();
        }

        var userModel = _mapper.Map<UserModel>(user);
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{user.Id}");

        //Act
        var getResponse = await client.SendAsync(getRequest);
        var result = await getResponse.Content.ReadFromJsonAsync<UserModel>();

        //Assert
        getResponse.Should().BeSuccessful();
        result.Should().BeEquivalentTo(userModel);
    }

    [Fact]
    public async Task GetUsers_WhenRequestIsSent_ThenAllUsersShouldBeReturned()
    {
        //Arrange 
        var users = _helper.GetFullUsers(2);

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<RabbitDbContext>();

            context.Users.AddRange(users);
            context.SaveChanges();
        }

        var userModels = _mapper.Map<List<UserModel>>(users);
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users/");

        //Act
        var getResponse = await client.SendAsync(getRequest);
        var result = await getResponse.Content.ReadFromJsonAsync<List<UserModel>>();

        //Assert
        getResponse.Should().BeSuccessful();
        result.Should().BeEquivalentTo(userModels);
    }

    [Fact]
    public async Task DeleteUser_WhenTheRequestIsSent_ThenTheUserShouldBeDeleted()
    {
        //Arrange
        var user = _helper.GetUsers(1)[0];
        user.PasswordHash = _helper.HashPassword("password1234");

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<RabbitDbContext>();

            context.Users.Add(user);
            context.SaveChanges();
        }


        var token = await _helper.GetAuthenticationToken(client, new AuthenticateRequestModel()
        {
            Username = user.Username,
            Password = "password1234"
        });


        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/users/{user.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await client.SendAsync(request);
        var result = await _helper.GetUserFromClient(client, user.Id);

        //Assert
        response.Should().BeSuccessful();
        result.Username.Should().BeNull();
    }
}