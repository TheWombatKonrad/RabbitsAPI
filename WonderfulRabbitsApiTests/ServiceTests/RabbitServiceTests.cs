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
using WonderfulRabbitsApi.Models.Rabbits;

public class RabbitServiceTests : IClassFixture<TestDatabaseFixture>
{
    private TestDataHelper _helper;
    private IMapper _mapper;
    private IJwtUtils jwtUtils;
    private TestDatabaseFixture _fixture;

    public RabbitServiceTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;

        _helper = new TestDataHelper();
        _mapper = new Mapper(AutoMapperConfiguration.Configure());
        jwtUtils = new JwtUtils(new OptionsWrapper<AppSettings>(new AppSettings() { Secret = "myBestTestingsecret1234skskskksa" }));
    }

    [Fact]
    public async Task RegisterRabbitAsync_WhenANewRabbitIsAdded_ThenItShouldBeAddedToTheDb()
    {
        //Arrange
        using var _context = _fixture.CreateContext();

        _context.Database.BeginTransaction();

        var user = _helper.GetUsers(1)[0];
        _context.Users.Add(user);
        _context.SaveChanges();

        var rabbit = _helper.GetRabbits(1)[0];
        var model = _mapper.Map<RegisterRabbitModel>(rabbit);
        // model.UserId = _context.Users.First(x => x.Username == user.Username).Id;
        model.UserId = user.Id;

        var sut = new RabbitService(_context, new HttpContextAccessor(), _mapper, jwtUtils);

        //Act
        var id = await sut.RegisterRabbitAsync(model);

        _context.ChangeTracker.Clear();

        //Assert
        var result = await sut.GetRabbitAsync(id);

        result.Should().BeEquivalentTo(rabbit,
            options => options
                .Excluding(x => x.Id));
    }

    [Fact]
    public void GetRabbitAsync_WhenTheRabbitExists_ThenItShouldBeReturned()
    {

    }

    [Fact]
    public void GetRabbitsAsync_WhenRabbitsExist_ThenItShouldBeReturned()
    {

    }

    [Fact]
    public void UpdateRabbitAsync_WhenTheRabbitExists_ThenItShouldBeUpdatedInTheDb()
    {

    }

    [Fact]
    public void DeleteRabbitAsync_WhenTheRabbitExists_ThenItShouldBeDeleted()
    {

    }
}