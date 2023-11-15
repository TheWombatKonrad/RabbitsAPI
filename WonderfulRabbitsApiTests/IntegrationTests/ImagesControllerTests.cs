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
using WonderfulRabbitsApi.Models.Images;
using WonderfulRabbitsApi.Entities;

namespace WonderfulRabbitsApiTests.IntegrationTests;

public class ImagesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> factory;
    private readonly TestDataHelper _helper;
    private readonly IMapper _mapper;

    public ImagesControllerTests()
    {
        factory = new CustomWebApplicationFactory<Program>();
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        _mapper = new Mapper(AutoMapperConfiguration.Configure());
        _helper = new TestDataHelper(_mapper);
    }

    [Fact]
    public async Task RegisterImage_WhenTheImageIsRegistered_ThenItsIdIsReturned()
    {
        //Arrange
        var token = await _helper.GetAuthenticationToken(_client);

        //create image
        var imageModel = _helper.GetUploadImagesModel();
        imageModel.RabbitId = 1;

        var request = _helper.GetUploadImageRequest(imageModel, token);

        //Act
        var response = await _client.SendAsync(request);
        var result = await response.Content.ReadFromJsonAsync<ImageModel>();

        //Assert
        response.Should().BeSuccessful();
        result.Id.Should().Be(2);
    }

    [Fact]
    public async Task UpdateImage_WhenTheImageExists_ThenItShouldBeUpdated()
    {
        //Arrange
        var token = await _helper.GetAuthenticationToken(_client);

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
        var token = await _helper.GetAuthenticationToken(_client);

        //create image
        var image = _helper.GetImages(1)[0];
        var uploadModel = _mapper.Map<UploadImageModel>(image);
        uploadModel.RabbitId = 1;
        var id = await _helper.UploadImageToClientAsync(_client, token, uploadModel);

        //updating image for comparing
        image.Id = id;
        var rabbit = await _helper.GetRabbitFromClientAsync(_client, 1);
        image.Rabbit = _mapper.Map<Rabbit>(rabbit);

        //create request
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/images/{id}");

        //Act
        var getResponse = await _client.SendAsync(getRequest);
        var resultModel = await getResponse.Content.ReadFromJsonAsync<ImageModel>();
        var result = _mapper.Map<Image>(resultModel);

        //Assert
        getResponse.Should().BeSuccessful();
        result.Should().BeEquivalentTo(image, opt => opt
            .Excluding(x => x.DateAdded) //these are set by the service
            .Excluding(x => x.FileName));
    }

    [Fact]
    public async Task GetImages_WhenImagesExist_ThenTheyShouldBeReturned()
    {
        //Arrange
        var token = await _helper.GetAuthenticationToken(_client);

        //create image
        var images = _helper.GetImages(2);
        var rabbitModel = await _helper.GetRabbitFromClientAsync(_client, 1);
        var rabbit = _mapper.Map<Rabbit>(rabbitModel);

        foreach (var image in images)
        {
            var model = _mapper.Map<UploadImageModel>(image);
            model.RabbitId = 1;
            image.Id = await _helper.UploadImageToClientAsync(_client, token, model);
            image.Rabbit = rabbit;
        }

        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/images/");

        //Act
        var getResponse = await _client.SendAsync(getRequest);
        var resultModel = await getResponse.Content.ReadFromJsonAsync<List<ImageModel>>();
        var result = _mapper.Map<List<Image>>(resultModel).Skip(1); //from seed

        //Assert
        getResponse.Should().BeSuccessful();
        result.Should().BeEquivalentTo(images, opt => opt
            .Excluding(x => x.DateAdded) //these are set by the service
            .Excluding(x => x.FileName));
    }

    [Fact]
    public async Task DeleteImage_WhenTheImageExists_ThenItShouldShouldBeDeleted()
    {
        //Arrange
        //create user
        var token = await _helper.GetAuthenticationToken(_client);

        //create image
        var model = _helper.GetUploadImagesModel();
        model.RabbitId = 1;
        var id = await _helper.UploadImageToClientAsync(_client, token, model);

        //create delete request
        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/images/{id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await _client.SendAsync(request);
        var result = await _helper.GetImageFromClient(_client, id);

        //Assert
        response.Should().BeSuccessful();
        result.Title.Should().BeNull();
    }
}