namespace WonderfulRabbitsApiTests.ServiceTests;

using Microsoft.Extensions.Options;
using WonderfulRabbitsApi.Authorization;
using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Entities;
using Xunit;
using AutoMapper;
using BCrypt.Net;
using MockQueryable.Moq;
using Moq;
using WonderfulRabbitsApi.DatabaseContext;
using WonderfulRabbitsApi.Services;
using Microsoft.AspNetCore.Http;
using FluentAssertions;

public class UserServiceTests
{
    private TestDataHelper helper;
    private IMapper mapper;
    private IJwtUtils jwtUtils;
    private IOptions<AppSettings> appSettings;

    public UserServiceTests()
    {
        helper = new TestDataHelper();
        mapper = new Mapper(AutoMapperConfiguration.Configure());
        appSettings = new OptionsWrapper<AppSettings>(new AppSettings() { Secret = "thisISaTESTINGsecret28282" });
        jwtUtils = new JwtUtils(appSettings);
    }

    [Fact]
    public async void RegisterUser_WhenANewUserIsRegistered_ThenItShouldBeAddedToTheDB()
    {
        //Arrange
        var userModel = helper.GetRegisterUserModel();
        var user = mapper.Map<User>(userModel);
        user.PasswordHash = BCrypt.HashPassword(userModel.Password);
        var users = new List<User>() { user };

        var mock = users.BuildMock().BuildMockDbSet();
        var dbContextMock = new Mock<RabbitDbContext>(null);
        dbContextMock.Setup(x => x.Users).Returns(mock.Object);

        var sut = new UserService(dbContextMock.Object, new HttpContextAccessor(), mapper, jwtUtils);

        //Act
        userModel = helper.GetRegisterUserModel();
        var id = await sut.RegisterUserAsync(userModel);
        var result = await sut.GetUser(id);

        //Assert
        result.Should().BeEquivalentTo(user);
    }
}