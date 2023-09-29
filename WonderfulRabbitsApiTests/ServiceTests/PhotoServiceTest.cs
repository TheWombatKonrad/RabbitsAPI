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

public class PhotoServiceTests
{
    private TestDataHelper _helper;
    private IMapper _mapper;
    private SqliteConnectionFactory _factory;

    public PhotoServiceTests()
    {
        _factory = new SqliteConnectionFactory();
        _helper = new TestDataHelper();
        _mapper = new Mapper(AutoMapperConfiguration.Configure());
    }

    [Fact]
    public async void RegisterPhoto_WhenAPhotoIsRegistered_ThenItShouldBeAddedToTheDb()
    {
        //Arrange
        using var _context = _factory.CreateContextForSQLite();

        _context.Database.BeginTransaction();

        var user = _helper.GetUsers(1)[0];
        _context.Users.Add(user);

        var rabbit = _helper.GetRabbits(1)[0];
        rabbit.User = user;
        _context.Rabbits.Add(rabbit);
        _context.SaveChanges();

        var model = _helper.GetRegisterPhotoModel();
        model.RabbitId = rabbit.Id;

        var sut = new PhotoService(_context, new HttpContextAccessor(), _mapper);
        var photo = _mapper.Map<Photo>(model);
        photo.Rabbit = rabbit;

        //Act
        photo.Id = await sut.RegisterPhoto(model);

        //Assert
        var result = _context.Photos
            .Include(i => i.Rabbit)
            .FirstOrDefault(x => x.Id == photo.Id);

        result.Should().BeEquivalentTo(photo, option => option
            .Excluding(x => x.DateAdded)
            .IgnoringCyclicReferences());
    }
}