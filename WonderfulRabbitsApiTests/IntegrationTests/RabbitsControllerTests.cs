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
using WonderfulRabbitsApi.DatabaseContext;

namespace WonderfulRabbitsApiTests.IntegrationTests;

public class RabbitsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly TestDataHelper _helper;
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitsControllerTests()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        _helper = new TestDataHelper();
        _mapper = new Mapper(AutoMapperConfiguration.Configure());
        _scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task RegisterRabbit_WhenARabbitIsRegistered_ThenTheCallIsSuccessful()
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

        var token = await _helper.GetAuthenticationToken(_client, new AuthenticateRequestModel()
        {
            Username = user.Username,
            Password = "password1234"
        });

        //create request
        var model = _mapper.Map<RegisterRabbitModel>(_helper.GetRabbits(1)[0]);
        model.UserId = user.Id;

        var request = _helper.GetRegisterRabbitRequest(model, token);

        //Act
        var response = await _client.SendAsync(request);

        //Assert
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task UpdateRabbit_WhenUpdateRequestIsSent_ThenTheRabbitShouldBeUpdated()
    {
        //Arrange
        var user = _helper.GetFullUsers(1)[0];
        user.PasswordHash = _helper.HashPassword("password1234");

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<RabbitDbContext>();

            context.Users.Add(user);
            context.SaveChanges();
        }

        var token = await _helper.GetAuthenticationToken(_client, new AuthenticateRequestModel()
        {
            Username = user.Username,
            Password = "password1234"
        });

        //create update request
        var requestModel = new UpdateRabbitModel() { Name = "newName" };
        var json = JsonSerializer.Serialize(requestModel);

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/rabbits/{1}")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.SendAsync(request);
        var updatedRabbit = await _helper.GetRabbitFromClientAsync(_client, 1);

        //Assert
        response.Should().BeSuccessful();
        updatedRabbit.Name.Should().Be("newName");
    }

    [Fact]
    public async Task GetRabbit_WhenRequestIsSent_ThenTheRabbitShouldBeReturned()
    {
        //Arrange
        var user = _helper.GetFullUsers(1)[0];
        var rabbit = user.Rabbits.First();

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<RabbitDbContext>();

            context.Users.Add(user);
            context.SaveChanges();
        }

        var rabbitModel = _mapper.Map<RabbitModel>(rabbit);
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/rabbits/{rabbit.Id}");

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
        var user = _helper.GetFullUsers(1)[0];

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<RabbitDbContext>();

            context.Users.AddRange(user);
            context.SaveChanges();
        }

        var rabbitModels = _mapper.Map<List<RabbitModel>>(user.Rabbits);
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
        var user = _helper.GetFullUsers(1)[0];
        user.PasswordHash = _helper.HashPassword("password1234");

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetService<RabbitDbContext>();

            context.Users.Add(user);
            context.SaveChanges();
        }

        var token = await _helper.GetAuthenticationToken(_client, new AuthenticateRequestModel()
        {
            Username = user.Username,
            Password = "password1234"
        });

        //create delete request
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/rabbits/1");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.SendAsync(request);
        var result = await _helper.GetRabbitFromClientAsync(_client, 1); //from context instead?

        //Assert
        response.Should().BeSuccessful();
        result.Name.Should().BeNull();
    }
}