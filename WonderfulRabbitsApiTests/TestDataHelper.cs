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

public class TestDataHelper
{
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

    public async Task<string> GetAuthenticationToken(HttpClient client, AuthenticateRequestModel model)
    {
        var json = JsonSerializer.Serialize(model);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/users/authenticate")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(request);
        var responseModel = await response.Content.ReadFromJsonAsync<AuthenticateResponseModel>();

        return responseModel.Token;
    }

    public async Task<UserModel> GetUserFromClient(HttpClient client, int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/users/{id}");

        var response = await client.SendAsync(request);
        var userModel = await response.Content.ReadFromJsonAsync<UserModel>();

        return userModel;
    }

    public async Task<int> RegisterUserAndGetIdAsync(HttpClient client, RegisterUserModel model)
    {
        var json = JsonSerializer.Serialize(model);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/users/register")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(request);

        var user = await response.Content.ReadFromJsonAsync<UserModel>();

        return user.Id;
    }

    public async Task<int> GetNewRabbitIdAsync(HttpClient client, int userId, string token)
    {
        var rabbit = GetRabbits(1)[0];
        var model = new RegisterRabbitModel()
        {
            Name = rabbit.Name,
            UserId = userId
        };

        var request = GetRegisterRabbitRequest(model, token);
        var response = await client.SendAsync(request);
        var responseRabbit = await response.Content.ReadFromJsonAsync<RabbitModel>();

        return responseRabbit.Id;
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

    public async Task<RabbitModel> GetRabbitFromClient(HttpClient client, int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/rabbits/{id}");

        var response = await client.SendAsync(request);
        var model = await response.Content.ReadFromJsonAsync<RabbitModel>();

        return model;
    }

    public RegisterImageModel GetRegisterImagesModel()
    {
        var faker = new Faker<RegisterImageModel>()
            .RuleFor(r => r.Title, f => f.Lorem.Sentence().ClampLength(max: 16));

        var model = faker.Generate(1)[0];
        model.Base64ImageData = GetFakeImageData();

        return model;
    }

    private string GetFakeImageData()
    {
        Random rnd = new Random();
        byte[] bytes = new byte[1000];
        rnd.NextBytes(bytes);

        var imageData = Convert.ToBase64String(bytes);

        return imageData;
    }
}