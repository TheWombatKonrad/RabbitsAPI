namespace WonderfulRabbitsApiTests.ServiceTests;

using WonderfulRabbitsApi.Authorization;
using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Entities;
using Xunit;
using AutoMapper;
using WonderfulRabbitsApi.Services;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
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
        var result = await sut.GetUserAsync(id);
        result.Should().BeEquivalentTo(user,
            options => options
                .Excluding(x => x.Id)
                .Excluding(x => x.PasswordHash));

        result.PasswordHash.Should().NotBeEquivalentTo(userModel.Password);
        result.PasswordHash.Should().NotBeNull();
    }

    [Fact]
    public async void RegisterUserAsync_WhenTheUsernameAlreadyExists_ThenAnExceptionShouldBeSent()
    {
        //Arrange
        using var context = fixture.CreateContext();
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), mapper, jwtUtils);

        var userModel = helper.GetRegisterUserModel();
        var id = await sut.RegisterUserAsync(userModel);

        context.ChangeTracker.Clear();

        //Act & Assert
        await sut.Invoking(y => y.RegisterUserAsync(userModel)).Should()
            .ThrowAsync<AppException>()
            .WithMessage("Username '" + userModel.Username + "' is already taken");
    }

    [Fact]
    public async void AuthenticateUser_WhenUsernameAndPasswordIsValid_ThenAWorkingTokenShouldBeReturned()
    {
        //Arrange
        using var context = fixture.CreateContext();
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), mapper, jwtUtils);

        var userModel = helper.GetRegisterUserModel();
        var userId = await sut.RegisterUserAsync(userModel);

        context.ChangeTracker.Clear();

        //Act
        var result = sut.AuthenticateUser(new AuthenticateRequestModel()
        {
            Username = userModel.Username,
            Password = userModel.Password
        });

        //Assert
        var id = jwtUtils.ValidateToken(result.Token);
        id.Equals(userId);
    }

    [Fact]
    public async void AuthenticateUser_WhenPasswordIsInvalid_ThenAnExceptionShouldBeSent()
    {
        //Arrange
        using var context = fixture.CreateContext();
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), mapper, jwtUtils);

        var userModel = helper.GetRegisterUserModel();
        await sut.RegisterUserAsync(userModel);

        context.ChangeTracker.Clear();

        //Act & Assert
        sut.Invoking(y => y.AuthenticateUser(new AuthenticateRequestModel()
        {
            Username = userModel.Username,
            Password = "wrongPassword"
        })).Should()
            .Throw<AppException>()
            .WithMessage("Username or password is incorrect");
    }

    [Fact]
    public async void GetUser_WhenUserExists_ThenItShouldBeReturned()
    {
        //Arrange
        using var context = fixture.CreateContext();
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), mapper, jwtUtils);

        var userModel = helper.GetRegisterUserModel();
        var id = await sut.RegisterUserAsync(userModel);

        context.ChangeTracker.Clear();

        var expected = mapper.Map<User>(userModel);
        expected.Id = id;

        //Act
        var result = await sut.GetUserAsync(id);

        //Assert
        result.Should().BeEquivalentTo(expected, options =>
            options.Excluding(x => x.PasswordHash));
    }

    [Fact]
    public async void GetUsers_WhenUsersExist_ThenTheyShouldBeReturned()
    {
        //Arrange
        using var context = fixture.CreateContext();
        context.Database.BeginTransaction();

        var users = helper.GetUsers(3);
        context.Users.AddRange(users);
        context.SaveChanges();

        context.ChangeTracker.Clear();

        var sut = new UserService(context, new HttpContextAccessor(), mapper, jwtUtils);

        //Act
        var result = await sut.GetUsersAsync();

        //Assert
        result.Should().BeEquivalentTo(users, options =>
            options.Excluding(x => x.PasswordHash));
    }

    [Fact]
    public async void UpdateUser_WhenUserExists_ThenItShouldBeUpdated()
    {
        //Arrange
        using var context = fixture.CreateContext();
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), mapper, jwtUtils);

        var userModel = helper.GetRegisterUserModel();
        var id = await sut.RegisterUserAsync(userModel);
        context.ChangeTracker.Clear();

        UpdateUserModel updateModel = new UpdateUserModel()
        {
            Username = "new",
            Password = userModel.Password,
            Email = userModel.Email
        };

        var expected = mapper.Map<User>(updateModel);
        expected.Id = id;

        //Act
        await sut.UpdateUserAsync(id, updateModel);
        context.ChangeTracker.Clear();

        //Assert
        var result = context.Users.FirstOrDefault(x => x.Id == id);

        result.Should().BeEquivalentTo(expected, options =>
            options.Excluding(x => x.PasswordHash));
    }

    [Fact]
    public async void DeleteUser_WhenUserExists_ThenItShouldBeDeleted()
    {
        //Arrange
        using var context = fixture.CreateContext();
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), mapper, jwtUtils);

        var userModel = helper.GetRegisterUserModel();
        var id = await sut.RegisterUserAsync(userModel);
        context.ChangeTracker.Clear();

        //Act
        await sut.DeleteUserAsync(id);
        context.ChangeTracker.Clear();

        //Assert
        var result = context.Users.FirstOrDefault(x => x.Id == id);
        result.Should().BeNull();
    }
}