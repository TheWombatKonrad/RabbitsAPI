namespace WonderfulRabbitsApiTests;

using Bogus;
using WonderfulRabbitsApi.Entities;
using BCrypt.Net;
using WonderfulRabbitsApi.Models.Users;

public class TestDataHelper
{
    public RegisterUserModel GetRegisterUserModel()
    {
        var faker = new Faker<RegisterUserModel>()
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => f.Internet.Password());

        var user = faker.Generate(1)[0];

        return user;
    }

    public List<User> GetUsers()
    {
        var faker = new Faker<User>()
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => BCrypt.HashPassword(f.Internet.Password()));

        return faker.Generate(3);
    }
}