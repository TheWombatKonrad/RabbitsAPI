namespace WonderfulRabbitsApiTests.ServiceTests;

using WonderfulRabbitsApi.Authorization;
using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Entities;
using Xunit;
using AutoMapper;
using WonderfulRabbitsApi.Services;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using Bogus;
using Bogus.Extensions;
using BCrypt.Net;
using WonderfulRabbitsApi.Models.Users;
using Microsoft.Extensions.Options;

public class UserServiceTests : IClassFixture<TestDatabaseFixture>
{
    private TestDataHelper helper;
    private IMapper mapper;
    private IJwtUtils jwtUtils;
    private TestDatabaseFixture fixture;

    public UserServiceTests(TestDatabaseFixture fixture)
    {
        this.fixture = fixture;

        helper = new TestDataHelper();
        mapper = new Mapper(AutoMapperConfiguration.Configure());
        jwtUtils = new JwtUtils(new OptionsWrapper<AppSettings>(new AppSettings() { Secret = "myBestTestingsecret1234skskskksa" }));
    }

    [Fact]
    public async void RegisterUserAsync_WhenANewUserIsRegistered_ThenItShouldBeAddedToTheDB()
    {
        //Arrange
        using var context = fixture.CreateContext();

        //the database isn't actually updated, avoiding test interference
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), mapper, jwtUtils);

        var userModel = helper.GetRegisterUserModel();
        var user = mapper.Map<User>(userModel);

        //Act
        var id = await sut.RegisterUserAsync(userModel);

        //to make sure that the user is loaded from the database
        context.ChangeTracker.Clear();

        //Assert
        var result = await sut.GetUser(id);
        result.Should().BeEquivalentTo(user,
            options => options
                .Excluding(x => x.Id)
                .Excluding(x => x.PasswordHash));

        result.PasswordHash.Should().NotBeEquivalentTo(userModel.Password);
        result.PasswordHash.Should().NotBeNull();
    }

    [Fact]
    public async void RegisterUserAsync_WhenTheUsernameAlreadyExists_TheUserShouldNotBeAdded()
    {
        //Arrange
        using var context = fixture.CreateContext();
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), mapper, jwtUtils);

        var userModel = helper.GetRegisterUserModel();
        var id = await sut.RegisterUserAsync(userModel);

        //Act & Assert
        await sut.Invoking(y => y.RegisterUserAsync(userModel)).Should()
            .ThrowAsync<AppException>()
            .WithMessage("Username '" + userModel.Username + "' is already taken");
    }

    [Fact]
    public async void AuthenticateUser_WhenUsernameAndPasswordIsValid_AWorkingTokenShouldBeReturned()
    {
        //Arrange
        using var context = fixture.CreateContext();
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), mapper, jwtUtils);

        var userModel = helper.GetRegisterUserModel();
        var userId = await sut.RegisterUserAsync(userModel);

        //Act
        var result = sut.AuthenticateUser(new AuthenticateRequest()
        {
            Username = userModel.Username,
            Password = userModel.Password
        });

        //Assert
        var id = jwtUtils.ValidateToken(result.Token);
        id.Equals(userId);
    }
}