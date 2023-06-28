namespace WonderfulRabbitsApiTests.ServiceTests;

using Microsoft.Extensions.Options;
using WonderfulRabbitsApi.Authorization;
using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Entities;
using Xunit;
using AutoMapper;
using WonderfulRabbitsApi.Services;
using Microsoft.AspNetCore.Http;
using FluentAssertions;

public class UserServiceTests : IClassFixture<TestDatabaseFixture>
{
    private TestDataHelper helper;
    private IMapper mapper;
    private IJwtUtils jwtUtils;
    private IOptions<AppSettings> appSettings;
    private TestDatabaseFixture fixture;

    public UserServiceTests(TestDatabaseFixture fixture)
    {
        this.fixture = fixture;
        helper = new TestDataHelper();
        mapper = new Mapper(AutoMapperConfiguration.Configure());
        appSettings = new OptionsWrapper<AppSettings>(new AppSettings() { Secret = "thisISaTESTINGsecret28282" });
        jwtUtils = new JwtUtils(appSettings);
    }

    [Fact]
    public async void RegisterUser_WhenANewUserIsRegistered_ThenItShouldBeAddedToTheDB()
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
}