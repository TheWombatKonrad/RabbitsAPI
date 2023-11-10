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
using WonderfulRabbitsApi.Models.Images;

public class ImageServiceTests
{
    private TestDataHelper _helper;
    private IMapper _mapper;
    private SqliteConnectionFactory _factory;

    public ImageServiceTests()
    {
        _factory = new SqliteConnectionFactory();
        _mapper = new Mapper(AutoMapperConfiguration.Configure());
        _helper = new TestDataHelper(_mapper);
    }

    [Fact]
    public async void RegisterImageAsync_WhenAnImageIsRegistered_ThenItShouldBeAddedToTheDb()
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

        _context.ChangeTracker.Clear();

        var model = _helper.GetRegisterImagesModel();
        model.RabbitId = rabbit.Id;

        var sut = new ImageService(_context, _mapper);
        var image = _mapper.Map<Image>(model);
        image.Rabbit = rabbit;

        //Act
        image.Id = await sut.UploadImageAsync(model);

        //Assert
        var result = _context.Images
            .Include(i => i.Rabbit)
            .FirstOrDefault(x => x.Id == image.Id);

        result.Should().BeEquivalentTo(image, option => option
            .Excluding(x => x.DateAdded)
            .Excluding(x => x.FileName)
            .Excluding(x => x.Rabbit.Images)
            .IgnoringCyclicReferences());
    }

    [Fact]
    public async void GetImageAsync_WhenTheImageExists_ThenItShouldBeReturned()
    {
        //Arrange
        using var _context = _factory.CreateContextForSQLite();

        _context.Database.BeginTransaction();

        var user = _helper.GetUsers(1)[0];
        _context.Users.Add(user);

        var rabbit = _helper.GetRabbits(1)[0];
        rabbit.User = user;
        _context.Rabbits.Add(rabbit);

        var image = _helper.GetImages(1)[0];
        image.Rabbit = rabbit;
        _context.Images.Add(image);
        _context.SaveChanges();

        _context.ChangeTracker.Clear();

        var sut = new ImageService(_context, _mapper);

        //Act
        var result = await sut.GetImageAsync(image.Id);

        //Assert
        result.Should().BeEquivalentTo(image, opt => opt.IgnoringCyclicReferences());
    }

    [Fact]
    public async void GetImagesAsync_WhenImagesExist_ThenTheyShouldBeReturned()
    {
        //Arrange
        using var _context = _factory.CreateContextForSQLite();

        _context.Database.BeginTransaction();

        var user = _helper.GetUsers(1)[0];
        _context.Users.Add(user);

        var rabbit = _helper.GetRabbits(1)[0];
        rabbit.User = user;
        _context.Rabbits.Add(rabbit);

        var images = _helper.GetImages(3);

        foreach (var image in images)
        {
            image.Rabbit = rabbit;
        }

        _context.Images.AddRange(images);
        _context.SaveChanges();

        _context.ChangeTracker.Clear();

        var sut = new ImageService(_context, _mapper);

        //Act
        var result = await sut.GetImagesAsync();

        //Assert
        result.Should().BeEquivalentTo(images, opt => opt.IgnoringCyclicReferences());
    }

    [Fact]
    public async void DeleteImageAsync_WhenTheImageExists_ThenItShouldBeDeleted()
    {
        //Arrange
        using var _context = _factory.CreateContextForSQLite();

        _context.Database.BeginTransaction();

        var user = _helper.GetUsers(1)[0];
        _context.Users.Add(user);

        var rabbit = _helper.GetRabbits(1)[0];
        rabbit.User = user;
        _context.Rabbits.Add(rabbit);

        var image = _helper.GetImages(1)[0];
        image.Rabbit = rabbit;
        _context.Images.Add(image);
        _context.SaveChanges();

        _context.ChangeTracker.Clear();

        var sut = new ImageService(_context, _mapper);

        //Act
        await sut.DeleteImageAsync(image.Id);

        //Assert
        var result = _context.Images.FirstOrDefault(x => x.Id == image.Id);
        result.Should().BeNull();
    }

    [Fact]
    public async void UpdateImageAsync_WhenTheImageExists_ThenItShouldBeUpdated()
    {
        //Arrange
        using var _context = _factory.CreateContextForSQLite();

        _context.Database.BeginTransaction();

        var user = _helper.GetUsers(1)[0];
        _context.Users.Add(user);

        var rabbit = _helper.GetRabbits(1)[0];
        rabbit.User = user;
        _context.Rabbits.Add(rabbit);

        var image = _helper.GetImages(1)[0];
        image.Rabbit = rabbit;
        _context.Images.Add(image);
        _context.SaveChanges();

        _context.ChangeTracker.Clear();

        var sut = new ImageService(_context, _mapper);

        var model = new UpdateImageModel()
        {
            Title = "newTitle"
        };
        image.Title = "newTitle";

        //Act
        await sut.UpdateImageAsync(image.Id, model);

        //Assert
        var result = _context.Images
            .Include(i => i.Rabbit)
            .ThenInclude(i => i.User)
            .FirstOrDefault(x => x.Id == image.Id);

        result.Should().BeEquivalentTo(image, opt => opt.IgnoringCyclicReferences());

    }
}