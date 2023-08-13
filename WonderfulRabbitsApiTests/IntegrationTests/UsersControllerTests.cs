using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;
using System.Text.Json;
using System.Text;
using WonderfulRabbitsApi.Models.Users;
using System.Net.Http.Headers;

namespace WonderfulRabbitsApiTests.IntegrationTests;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient client;
    private readonly CustomWebApplicationFactory<Program> factory;
    private readonly TestDataHelper _helper;

    public UsersControllerTests()
    {
        factory = new CustomWebApplicationFactory<Program>();
        client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        _helper = new TestDataHelper();
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
        var userModel = _helper.GetRegisterUserModel();
        await _helper.RegisterUserAndGetId(client, userModel);

        var requestModel = new AuthenticateRequestModel()
        {
            Username = userModel.Username,
            Password = userModel.Password
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
        var userModel = _helper.GetRegisterUserModel();
        await _helper.RegisterUserAndGetId(client, userModel);

        var requestModel = new AuthenticateRequestModel()
        {
            Username = userModel.Username,
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
        var userModel = _helper.GetRegisterUserModel();
        var id = await _helper.RegisterUserAndGetId(client, userModel);
        var token = await _helper.GetAuthenticationToken(client, new AuthenticateRequestModel()
        {
            Username = userModel.Username,
            Password = userModel.Password
        });

        var requestModel = new UpdateUserModel()
        {
            Username = "newUsername"
        };

        var json = JsonSerializer.Serialize(requestModel);

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/users/{id}")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await client.SendAsync(request);
        var updatedUser = await _helper.GetUserFromClient(client, id);

        //Assert
        response.Should().BeSuccessful();
        updatedUser.Username.Should().Be("newUsername");

    }

    [Fact]
    public async Task GetUser_WhenRequestIsSent_ThenTheUserShouldBeReturned()
    {
        //Arrange 
        var registerModel = _helper.GetRegisterUserModel();
        var id = await _helper.RegisterUserAndGetId(client, registerModel);

        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{id}");

        //Act
        var getResponse = await client.SendAsync(getRequest);
        var result = await getResponse.Content.ReadFromJsonAsync<UserModel>();

        //Assert
        getResponse.Should().BeSuccessful();
        result.Should().BeEquivalentTo(registerModel, options => options
            .Excluding(x => x.Password));
    }

    [Fact]
    public async Task GetUsers_WhenRequestIsSent_ThenAllUsersShouldBeReturned()
    {
        //Arrange 
        List<RegisterUserModel> registerModels = new List<RegisterUserModel>()
        {
            _helper.GetRegisterUserModel(),
            _helper.GetRegisterUserModel()
        };

        foreach (var model in registerModels)
        {
            var json = JsonSerializer.Serialize(model);

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/users/register")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var registerUserResponse = await client.SendAsync(request);
        }

        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/users/");

        //Act
        var getResponse = await client.SendAsync(getRequest);
        var result = await getResponse.Content.ReadFromJsonAsync<List<UserModel>>();

        //Assert
        getResponse.Should().BeSuccessful();
        result.Should().BeEquivalentTo(registerModels, options => options
            .Excluding(x => x.Password));
    }

    [Fact]
    public async Task DeleteUser_WhenTheRequestIsSent_ThenTheUserShouldBeDeleted()
    {
        //Arrange
        var userModel = _helper.GetRegisterUserModel();
        var id = await _helper.RegisterUserAndGetId(client, userModel);
        var token = await _helper.GetAuthenticationToken(client, new AuthenticateRequestModel()
        {
            Username = userModel.Username,
            Password = userModel.Password
        });


        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/users/{id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await client.SendAsync(request);
        var result = await _helper.GetUserFromClient(client, id);

        //Assert
        response.Should().BeSuccessful();
        result.Username.Should().BeNull();
    }
}