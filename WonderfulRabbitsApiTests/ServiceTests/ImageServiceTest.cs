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

public class ImageServiceTests
{
    private TestDataHelper _helper;
    private IMapper _mapper;
    private SqliteConnectionFactory _factory;

    public ImageServiceTests()
    {
        _factory = new SqliteConnectionFactory();
        _helper = new TestDataHelper();
        _mapper = new Mapper(AutoMapperConfiguration.Configure());
    }

    [Fact]
    public async void RegisterImage_WhenAnImageIsRegistered_ThenItShouldBeAddedToTheDb()
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

        var model = _helper.GetRegisterImagesModel();
        model.RabbitId = rabbit.Id;

        var sut = new ImageService(_context, _mapper);
        var image = _mapper.Map<Image>(model);
        image.Rabbit = rabbit;

        //Act
        image.Id = await sut.RegisterImageAsync(model);

        //Assert
        var result = _context.Images
            .Include(i => i.Rabbit)
            .FirstOrDefault(x => x.Id == image.Id);

        result.Should().BeEquivalentTo(image, option => option
            .Excluding(x => x.DateAdded)
            .IgnoringCyclicReferences());
    }
}