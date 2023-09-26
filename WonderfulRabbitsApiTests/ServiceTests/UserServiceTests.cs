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
using Microsoft.EntityFrameworkCore;

public class UserServiceTests : IClassFixture<TestDatabaseFixture>
{
    private TestDataHelper _helper;
    private IMapper _mapper;
    private IJwtUtils _jwtUtils;
    private SqliteConnectionFactory _factory;

    public UserServiceTests()
    {
        _factory = new SqliteConnectionFactory();

        _helper = new TestDataHelper();
        _mapper = new Mapper(AutoMapperConfiguration.Configure());
        _jwtUtils = new JwtUtils(new OptionsWrapper<AppSettings>(new AppSettings() { Secret = "myBestTestingsecret1234skskskksa" }));
    }

    [Fact]
    public async void RegisterUserAsync_WhenANewUserIsRegistered_ThenItShouldBeAddedToTheDB()
    {
        //Arrange
        using var context = _factory.CreateContextForSQLite();

        //the database isn't actually updated, avoiding test interference
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), _mapper, _jwtUtils);

        var userModel = _helper.GetRegisterUserModel();
        var user = _mapper.Map<User>(userModel);

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
        using var context = _factory.CreateContextForSQLite();
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), _mapper, _jwtUtils);

        var userModel = _helper.GetRegisterUserModel();
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
        using var context = _factory.CreateContextForSQLite();
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), _mapper, _jwtUtils);

        var userModel = _helper.GetRegisterUserModel();
        var userId = await sut.RegisterUserAsync(userModel);

        context.ChangeTracker.Clear();

        //Act
        var result = sut.AuthenticateUser(new AuthenticateRequestModel()
        {
            Username = userModel.Username,
            Password = userModel.Password
        });

        //Assert
        var id = _jwtUtils.ValidateToken(result.Token);
        id.Equals(userId);
    }

    [Fact]
    public async void AuthenticateUser_WhenPasswordIsInvalid_ThenAnExceptionShouldBeSent()
    {
        //Arrange
        using var context = _factory.CreateContextForSQLite();
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), _mapper, _jwtUtils);

        var userModel = _helper.GetRegisterUserModel();
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
        using var context = _factory.CreateContextForSQLite();
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), _mapper, _jwtUtils);

        var user = _helper.GetUsers(1)[0];
        user.Rabbits.Add(_helper.GetRabbits(1)[0]);
        context.Users.Add(user);

        context.SaveChanges();

        context.ChangeTracker.Clear();

        //Act
        var result = await sut.GetUserAsync(user.Id);

        //Assert
        result.Should().BeEquivalentTo(user, options => options.IgnoringCyclicReferences());
    }

    [Fact]
    public async void GetUser_WhenTheUserDoesntExist_ThenAnExceptionShouldBeThrown()
    {
        //Arrange
        var context = _factory.CreateContextForSQLite();
        var sut = new UserService(context, new HttpContextAccessor(), _mapper, _jwtUtils);
        context.Database.BeginTransaction();

        //Act & Assert
        await sut.Invoking(y => y.GetUserAsync(1)).Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async void GetUsers_WhenUsersExist_ThenTheyShouldBeReturned()
    {
        //Arrange
        using var context = _factory.CreateContextForSQLite();
        context.Database.BeginTransaction();

        var users = _helper.GetUsers(3);
        users[0].Rabbits.Add(_helper.GetRabbits(1)[0]);
        context.Users.AddRange(users);
        context.SaveChanges();

        context.ChangeTracker.Clear();

        var sut = new UserService(context, new HttpContextAccessor(), _mapper, _jwtUtils);

        //Act
        var result = await sut.GetUsersAsync();

        //Assert
        result.Should().BeEquivalentTo(users, options =>
            options.Excluding(x => x.PasswordHash).IgnoringCyclicReferences());
    }

    [Fact]
    public async void UpdateUser_WhenUserExists_ThenItShouldBeUpdated()
    {
        //Arrange
        using var context = _factory.CreateContextForSQLite();
        var sut = new UserService(context, new HttpContextAccessor(), _mapper, _jwtUtils);
        context.Database.BeginTransaction();

        var user = _helper.GetUsers(1)[0];
        user.Rabbits.Add(_helper.GetRabbits(1)[0]);
        context.Users.Add(user);
        context.SaveChanges();

        context.ChangeTracker.Clear();

        UpdateUserModel updateModel = new UpdateUserModel()
        {
            Username = "new",
        };
        user.Username = "new";

        //Act
        await sut.UpdateUserAsync(user.Id, updateModel);
        context.ChangeTracker.Clear();

        //Assert
        var result = context.Users
            .Include(i => i.Rabbits)
            .FirstOrDefault(x => x.Id == user.Id);

        result.Should().BeEquivalentTo(user, options =>
            options
                .Excluding(x => x.PasswordHash)
                .IgnoringCyclicReferences());
    }

    [Fact]
    public async void DeleteUser_WhenUserExists_ThenItShouldBeDeleted()
    {
        //Arrange
        using var context = _factory.CreateContextForSQLite();
        context.Database.BeginTransaction();

        var sut = new UserService(context, new HttpContextAccessor(), _mapper, _jwtUtils);

        var user = _helper.GetUsers(1)[0];
        context.Users.Add(user);
        context.SaveChanges();

        context.ChangeTracker.Clear();

        //Act
        await sut.DeleteUserAsync(user.Id);
        context.ChangeTracker.Clear();

        //Assert
        var result = context.Users.FirstOrDefault(x => x.Id == user.Id);
        result.Should().BeNull();
    }
}