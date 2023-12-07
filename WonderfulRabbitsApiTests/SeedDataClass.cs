namespace WonderfulRabbitsApiTests;

using WonderfulRabbitsApi.DatabaseContext;
using WonderfulRabbitsApi.Entities;

public interface ISeedDataClass
{
    void InitializeDbForTests();
}

public class SeedDataClass : ISeedDataClass
{
    private readonly RabbitDbContext _db;
    private readonly TestDataHelper _helper;

    public SeedDataClass(RabbitDbContext db)
    {
        _db = db;
        _helper = new TestDataHelper();
    }

    public void InitializeDbForTests()
    {
        var user = new User()
        {
            Username = "Wombats",
            PasswordHash = _helper.HashPassword("password1234"),
            Email = "email@email.com"
        };

        _db.Users.Add(user);

        var rabbit = _helper.GetRabbits(1)[0];
        rabbit.User = user;

        _db.Rabbits.Add(rabbit);

        var image = _helper.GetImages(1)[0];
        image.Rabbit = rabbit;

        _db.Images.Add(image);

        _db.SaveChanges(true);
    }
}