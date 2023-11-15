using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;
using System.Text.Json;
using System.Text;
using WonderfulRabbitsApi.Models.Users;
using System.Net.Http.Headers;
using AutoMapper;
using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Models.Rabbits;

namespace WonderfulRabbitsApiTests.IntegrationTests;

public class RabbitsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> factory;
    private readonly TestDataHelper _helper;
    private readonly IMapper _mapper;

    public RabbitsControllerTests()
    {
        factory = new CustomWebApplicationFactory<Program>();
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        _helper = new TestDataHelper();
        _mapper = new Mapper(AutoMapperConfiguration.Configure());
    }

    [Fact]
    public async Task RegisterRabbit_WhenARabbitIsRegistered_ThenAnIdIsReturned()
    {
        //Arrange
        //create user
        var userModel = _helper.GetRegisterUserModel();
        var userId = await _helper.RegisterUserAndGetIdAsync(_client, userModel);
        var token = await _helper.GetAuthenticationToken(_client, new AuthenticateRequestModel()
        {
            Username = userModel.Username,
            Password = userModel.Password
        });

        //create request
        var rabbitModel = _mapper.Map<RegisterRabbitModel>(_helper.GetRabbits(1)[0]);
        rabbitModel.UserId = userId;

        var json = JsonSerializer.Serialize(rabbitModel);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/rabbits/register")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.SendAsync(request);
        var responseRabbit = await response.Content.ReadFromJsonAsync<RabbitModel>();

        //Assert
        response.Should().BeSuccessful();
        responseRabbit.Id.Should().Be(1);
    }

    [Fact]
    public async Task UpdateRabbit_WhenUpdateRequestIsSent_ThenTheRabbitShouldBeUpdated()
    {
        //Arrange
        //create user
        var userModel = _helper.GetRegisterUserModel();
        var userId = await _helper.RegisterUserAndGetIdAsync(_client, userModel);
        var token = await _helper.GetAuthenticationToken(_client, new AuthenticateRequestModel()
        {
            Username = userModel.Username,
            Password = userModel.Password
        });

        //create rabbit
        var id = await _helper.GetNewRabbitIdAsync(_client, userId, token);

        //create update request
        var requestModel = new UpdateRabbitModel() { Name = "newName" };
        var json = JsonSerializer.Serialize(requestModel);

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/rabbits/{id}")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.SendAsync(request);
        var updatedRabbit = await _helper.GetRabbitFromClientAsync(_client, id);

        //Assert
        response.Should().BeSuccessful();
        updatedRabbit.Name.Should().Be("newName");
    }

    [Fact]
    public async Task GetRabbit_WhenRequestIsSent_ThenTheRabbitShouldBeReturned()
    {
        //Arrange
        //create user
        var userModel = _helper.GetRegisterUserModel();
        var userId = await _helper.RegisterUserAndGetIdAsync(_client, userModel);
        var token = await _helper.GetAuthenticationToken(_client, new AuthenticateRequestModel()
        {
            Username = userModel.Username,
            Password = userModel.Password
        });

        //create rabbit
        var rabbit = _helper.GetRabbits(1)[0];
        var registerRabbitModel = new RegisterRabbitModel()
        {
            Name = rabbit.Name,
            UserId = userId,
            Birthdate = rabbit.Birthdate
        };
        var registerRequest = _helper.GetRegisterRabbitRequest(registerRabbitModel, token);
        var registerResponse = await _client.SendAsync(registerRequest);
        var id = (await registerResponse.Content.ReadFromJsonAsync<RabbitModel>()).Id;
        rabbit.Id = id;

        //create rabbitModel to compare to
        var rabbitModel = _mapper.Map<RabbitModel>(rabbit);
        rabbitModel.User = new UserDataModel()
        {
            Id = userId,
            Username = userModel.Username,
            Email = userModel.Email
        };

        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/rabbits/{id}");

        //Act
        var getResponse = await _client.SendAsync(getRequest);
        var result = await getResponse.Content.ReadFromJsonAsync<RabbitModel>();

        //Assert
        getResponse.Should().BeSuccessful();
        result.Should().BeEquivalentTo(rabbitModel);
    }

    [Fact]
    public async Task GetRabbits_WhenRequestIsSent_ThenAllRabbitsShouldBeReturned()
    {
        //Arrange
        //create user
        var userModel = _helper.GetRegisterUserModel();
        var userId = await _helper.RegisterUserAndGetIdAsync(_client, userModel);
        var token = await _helper.GetAuthenticationToken(_client, new AuthenticateRequestModel()
        {
            Username = userModel.Username,
            Password = userModel.Password
        });

        //create rabbits
        var rabbits = _helper.GetRabbits(2);
        var rabbitIds = new List<int>();

        foreach (var rabbit in rabbits)
        {
            var model = new RegisterRabbitModel()
            {
                Name = rabbit.Name,
                UserId = userId,
                Birthdate = rabbit.Birthdate
            };

            var registerRequest = _helper.GetRegisterRabbitRequest(model, token);
            var registerResponse = await _client.SendAsync(registerRequest);
            var id = (await registerResponse.Content.ReadFromJsonAsync<RabbitModel>()).Id;

            rabbitIds.Add(id);
            rabbit.Id = id;
        }

        //create rabbitModels to compare to
        var rabbitModels = new List<RabbitModel>();

        foreach (var rabbit in rabbits)
        {
            var model = _mapper.Map<RabbitModel>(rabbit);
            model.User = new UserDataModel()
            {
                Id = userId,
                Username = userModel.Username,
                Email = userModel.Email
            };

            rabbitModels.Add(model);
        }

        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/rabbits/");

        //Act
        var getResponse = await _client.SendAsync(getRequest);
        var result = await getResponse.Content.ReadFromJsonAsync<List<RabbitModel>>();

        //Assert
        getResponse.Should().BeSuccessful();
        result.Should().BeEquivalentTo(rabbitModels);
    }

    [Fact]
    public async Task DeleteRabbit_WhenTheRequestIsSent_ThenTheRabbitShouldBeDeleted()
    {
        //Arrange
        //create user
        var userModel = _helper.GetRegisterUserModel();
        var userId = await _helper.RegisterUserAndGetIdAsync(_client, userModel);
        var token = await _helper.GetAuthenticationToken(_client, new AuthenticateRequestModel()
        {
            Username = userModel.Username,
            Password = userModel.Password
        });

        //create rabbit
        var id = await _helper.GetNewRabbitIdAsync(_client, userId, token);

        //create delete request
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/rabbits/{id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.SendAsync(request);
        var result = await _helper.GetRabbitFromClientAsync(_client, id);

        //Assert
        response.Should().BeSuccessful();
        result.Name.Should().BeNull();
    }
}