namespace WonderfulRabbitsApiTests.ServiceTests;

using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Entities;
using Xunit;
using AutoMapper;
using WonderfulRabbitsApi.Services;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using WonderfulRabbitsApi.Models.Rabbits;
using Moq;
using Microsoft.EntityFrameworkCore;

public class RabbitServiceTests : IClassFixture<TestDatabaseFixture>
{
    private TestDataHelper _helper;
    private IMapper _mapper;
    private SqliteConnectionFactory _factory;

    public RabbitServiceTests()
    {
        _factory = new SqliteConnectionFactory();
        _helper = new TestDataHelper();
        _mapper = new Mapper(AutoMapperConfiguration.Configure());
    }

    [Fact]
    public async Task RegisterRabbitAsync_WhenANewRabbitIsAdded_ThenItShouldBeAddedToTheCurrentUser()
    {
        //Arrange
        using var _context = _factory.CreateContextForSQLite();

        _context.Database.BeginTransaction();

        var user = _helper.GetUsers(1)[0];
        _context.Users.Add(user);
        _context.SaveChanges();

        var rabbit = _helper.GetRabbits(1)[0];
        var model = _mapper.Map<RegisterRabbitModel>(rabbit);
        rabbit.User = user; //user is retrieved from context later

        var httpContextAccessor = GetMockIHttpContextAccessorForCurrentUser(user);
        var sut = new RabbitService(_context, httpContextAccessor, _mapper);

        //Act
        var id = await sut.RegisterRabbitAsync(model);

        _context.ChangeTracker.Clear();

        //Assert
        var result = _context.Rabbits
            .Include(i => i.User)
            .FirstOrDefault(x => x.Id == id);

        result.Should().BeEquivalentTo(rabbit,
            options => options
                .Excluding(x => x.Id)
                .IgnoringCyclicReferences());
    }

    [Fact]
    public async void GetRabbitAsync_WhenTheRabbitExists_ThenItShouldBeReturned()
    {
        //Arrange
        using var _context = _factory.CreateContextForSQLite();
        var sut = new RabbitService(_context, new HttpContextAccessor(), _mapper);

        _context.Database.BeginTransaction();

        var user = _helper.GetUsers(1)[0];
        _context.Users.Add(user);

        var rabbit = _helper.GetRabbits(1)[0];
        rabbit.User = user;
        _context.Rabbits.Add(rabbit);
        _context.SaveChanges();

        _context.ChangeTracker.Clear();

        //Act
        var result = await sut.GetRabbitAsync(rabbit.Id);

        //Assert
        result.Should().BeEquivalentTo(rabbit, option =>
            option.IgnoringCyclicReferences());
    }

    [Fact]
    public async void GetRabbitsAsync_WhenRabbitsExist_ThenItShouldBeReturned()
    {
        //Arrange
        using var _context = _factory.CreateContextForSQLite();
        var sut = new RabbitService(_context, new HttpContextAccessor(), _mapper);

        _context.Database.BeginTransaction();

        var user = _helper.GetUsers(1)[0];
        _context.Users.Add(user);

        var rabbits = _helper.GetRabbits(2);
        foreach (var rabbit in rabbits) rabbit.User = user;
        _context.Rabbits.AddRange(rabbits);
        _context.SaveChanges();

        _context.ChangeTracker.Clear();

        //Act
        var result = await sut.GetRabbitsAsync();

        //Assert
        result.Should().BeEquivalentTo(rabbits, option =>
            option.IgnoringCyclicReferences());
    }

    [Fact]
    public async void UpdateRabbitAsync_WhenTheRabbitExists_ThenItShouldBeUpdatedInTheDb()
    {
        //Arrange
        using var _context = _factory.CreateContextForSQLite();
        var sut = new RabbitService(_context, new HttpContextAccessor(), _mapper);

        _context.Database.BeginTransaction();

        var user = _helper.GetUsers(1)[0];
        _context.Users.Add(user);

        var rabbit = _helper.GetRabbits(1)[0];
        rabbit.User = user;
        _context.Rabbits.Add(rabbit);
        _context.SaveChanges();

        _context.ChangeTracker.Clear();

        var updateModel = new UpdateRabbitModel() { Name = "newName" };
        rabbit.Name = "newName";

        //Act
        await sut.UpdateRabbitAsync(rabbit.Id, updateModel);

        //Assert
        var result = _context.Rabbits
            .Include(i => i.User)
            .FirstOrDefault(x => x.Id == rabbit.Id);

        result.Should().BeEquivalentTo(rabbit, option =>
            option.IgnoringCyclicReferences());

    }

    [Fact]
    public async void DeleteRabbitAsync_WhenTheRabbitExists_ThenItShouldBeDeleted()
    {
        //Arrange
        using var _context = _factory.CreateContextForSQLite();
        var sut = new RabbitService(_context, new HttpContextAccessor(), _mapper);

        _context.Database.BeginTransaction();

        var user = _helper.GetUsers(1)[0];
        _context.Users.Add(user);

        var rabbit = _helper.GetRabbits(1)[0];
        rabbit.User = user;
        _context.Rabbits.Add(rabbit);
        _context.SaveChanges();

        _context.ChangeTracker.Clear();

        //Act
        await sut.DeleteRabbitAsync(rabbit.Id);
        _context.ChangeTracker.Clear();

        //Assert
        var result = _context.Rabbits.FirstOrDefault(x => x.Id == rabbit.Id);
        result.Should().BeNull();
    }

    //********************
    //Helper Methods
    //********************

    private IHttpContextAccessor GetMockIHttpContextAccessorForCurrentUser(User user)
    {
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();

        context.Items["User"] = user;
        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

        return mockHttpContextAccessor.Object;

    }
}