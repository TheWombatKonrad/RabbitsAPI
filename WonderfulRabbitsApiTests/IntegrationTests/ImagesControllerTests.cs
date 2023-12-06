using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;
using System.Text.Json;
using System.Text;
using WonderfulRabbitsApi.Models.Users;
using System.Net.Http.Headers;
using AutoMapper;
using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Models.Images;
using WonderfulRabbitsApi.DatabaseContext;

namespace WonderfulRabbitsApiTests.IntegrationTests;

public class ImagesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly TestDataHelper _helper;
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _scopeFactory;

    public ImagesControllerTests()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        _mapper = new Mapper(AutoMapperConfiguration.Configure());
        _helper = new TestDataHelper(_mapper);
        _scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task RegisterImage_WhenTheImageIsRegistered_ThenItsIdIsReturned()
    {
        //Arrange
        var user = _helper.GetUsers(1)[0];
        user.PasswordHash = _helper.HashPassword("password1234");

        var rabbit = _helper.GetRabbits(1)[0];
        user.Rabbits.Add(rabbit);

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

        //create image
        var imageModel = _helper.GetUploadImagesModel();
        imageModel.RabbitId = 1;

        var request = _helper.GetUploadImageRequest(imageModel, token);

        //Act
        var response = await _client.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<ImageModel>();

        //Assert
        response.Should().BeSuccessful();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task UpdateImage_WhenTheImageExists_ThenItShouldBeUpdated()
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
        var updateModel = new UpdateImageModel() { Title = "newTitle" };
        var json = JsonSerializer.Serialize(updateModel);

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/images/1")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.SendAsync(request);
        var updatedImage = await _helper.GetImageFromClient(_client, 1);

        //Assert
        response.Should().BeSuccessful();
        updatedImage.Title.Should().Be("newTitle");
    }

    [Fact]
    public async Task GetImage_WhenTheImageExists_ThenItShouldBeReturned()
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

        var imageModel = _mapper.Map<ImageModel>(user.Rabbits.First().Images.First());
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/images/{1}");

        //Act
        var getResponse = await _client.SendAsync(getRequest);
        var result = await getResponse.Content.ReadFromJsonAsync<ImageModel>();

        //Assert
        getResponse.Should().BeSuccessful();
        result.Should().BeEquivalentTo(imageModel, opt => opt
            .Excluding(x => x.DateAdded) //these are set by the service
            .Excluding(x => x.FileName));
    }

    [Fact]
    public async Task GetImages_WhenImagesExist_ThenTheyShouldBeReturned()
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

        var images = user.Rabbits.SelectMany(x => x.Images).ToList();

        var imageModels = _mapper.Map<List<ImageModel>>(images);
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/images/");

        //Act
        var getResponse = await _client.SendAsync(getRequest);
        var result = await getResponse.Content.ReadFromJsonAsync<List<ImageModel>>();

        //Assert
        getResponse.Should().BeSuccessful();
        result.Should().BeEquivalentTo(imageModels, opt => opt
            .Excluding(x => x.DateAdded) //these are set by the service
            .Excluding(x => x.FileName));
    }

    [Fact]
    public async Task DeleteImage_WhenTheImageExists_ThenItShouldShouldBeDeleted()
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
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/images/1");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.SendAsync(request);
        var result = await _helper.GetImageFromClient(_client, 1);

        //Assert
        response.Should().BeSuccessful();
        result.Title.Should().BeNull();
    }
}