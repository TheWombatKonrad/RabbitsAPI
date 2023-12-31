namespace WonderfulRabbitsApiTests;

using Bogus;
using Bogus.Extensions;
using WonderfulRabbitsApi.Entities;
using BCrypt.Net;
using WonderfulRabbitsApi.Models.Users;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;
using WonderfulRabbitsApi.Models.Images;
using WonderfulRabbitsApi.Models.Rabbits;
using System.Net.Http.Headers;
using AutoMapper;
using WonderfulRabbitsApi.Controllers;

public class TestDataHelper
{
    IMapper _mapper;

    public TestDataHelper(IMapper mapper = null)
    {
        _mapper = mapper;
    }

    public RegisterUserModel GetRegisterUserModel()
    {
        var faker = new Faker<RegisterUserModel>()
            .RuleFor(u => u.Username, f => f.Internet.UserName().ClampLength(max: 16))
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => f.Internet.Password());

        var user = faker.Generate(1)[0];

        return user;
    }

    public List<User> GetUsers(int amount)
    {
        var faker = new Faker<User>()
            .RuleFor(u => u.Username, f => f.Internet.UserName().ClampLength(max: 16))
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => BCrypt.HashPassword(f.Internet.Password()));

        return faker.Generate(amount);
    }

    public List<User> GetFullUsers(int amount)
    {
        var faker = new Faker<User>()
            .RuleFor(u => u.Username, f => f.Internet.UserName().ClampLength(max: 16))
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => BCrypt.HashPassword(f.Internet.Password()));

        var users = faker.Generate(amount);

        foreach (var user in users)
        {
            var rabbits = GetRabbits(2);
            var images = GetImages(2);

            rabbits[0].Images.Add(images[0]);
            rabbits[1].Images.Add(images[1]);

            user.Rabbits.Add(rabbits[0]);
            user.Rabbits.Add(rabbits[1]);

        }

        return users;
    }

    public List<Rabbit> GetRabbits(int amount)
    {
        var faker = new Faker<Rabbit>()
            .RuleFor(r => r.Name, f => f.Name.FirstName().ClampLength(max: 16))
            .RuleFor(r => r.Birthdate, f => f.Date.Past());

        return faker.Generate(amount);
    }

    public string HashPassword(string password)
    {
        return BCrypt.HashPassword(password);
    }

    public async Task<string> GetAuthenticationToken(HttpClient client, AuthenticateRequestModel model = null)
    {
        if (model == null) model = GetAuthenticateRequestModel();

        var json = JsonSerializer.Serialize(model);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/users/authenticate")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(request);
        var responseModel = await response.Content.ReadFromJsonAsync<AuthenticateResponseModel>();

        return responseModel.Token;
    }

    private AuthenticateRequestModel GetAuthenticateRequestModel()
    {
        return new AuthenticateRequestModel()
        {
            Username = "Wombats",
            Password = "password1234"
        };
    }

    public async Task<UserModel> GetUserFromClient(HttpClient client, int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{id}");

        var response = await client.SendAsync(request);
        var userModel = await response.Content.ReadFromJsonAsync<UserModel>();

        return userModel;
    }

    public HttpRequestMessage GetRegisterRabbitRequest(RegisterRabbitModel model, string token)
    {
        var json = JsonSerializer.Serialize(model);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/rabbits/register")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return request;
    }

    public async Task<RabbitModel> GetRabbitFromClientAsync(HttpClient client, int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/rabbits/{id}");

        var response = await client.SendAsync(request);
        var model = await response.Content.ReadFromJsonAsync<RabbitModel>();

        return model;
    }

    public List<Image> GetImages(int amount)
    {
        var faker = new Faker<Image>()
            .RuleFor(r => r.Title, f => f.Lorem.Sentence().ClampLength(max: 16))
            .RuleFor(r => r.DateAdded, f => f.Date.Past())
            .RuleFor(r => r.FileName, Path.GetRandomFileName)
            .RuleFor(r => r.ImageData, GetFakeImageData())
            .RuleFor(r => r.FileExtension, ".jpg");

        var image = faker.Generate(amount);

        return image;
    }

    public UploadImageModel GetUploadImagesModel()
    {
        var model = _mapper.Map<UploadImageModel>(GetImages(1)[0]);

        return model;
    }

    private byte[] GetFakeImageData()
    {
        Random rnd = new Random();
        byte[] bytes = new byte[1000];
        rnd.NextBytes(bytes);

        return bytes;
    }

    public HttpRequestMessage GetUploadImageRequest(UploadImageModel model, string token)
    {
        var json = JsonSerializer.Serialize(model);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/images/upload")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return request;
    }

    public async Task<ImageModel> GetImageFromClient(HttpClient client, int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/images/{id}");

        var response = await client.SendAsync(request);
        var model = await response.Content.ReadFromJsonAsync<ImageModel>();

        return model;
    }

    public async Task<int> UploadImageToClientAsync(HttpClient client, string token, UploadImageModel model)
    {
        var uploadRequest = GetUploadImageRequest(model, token);
        var uploadResponse = await client.SendAsync(uploadRequest);

        var id = (await uploadResponse.Content.ReadFromJsonAsync<ImageModel>()).Id;

        return id;
    }
}