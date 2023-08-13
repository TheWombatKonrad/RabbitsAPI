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
        // _db.Users.AddRange(
        //    new User()
        //    {
        //        Username = "Wombats",
        //        PasswordHash = _helper.HashPassword("password1234"),
        //        Email = "email@email.com"
        //    }
        // );

        // _db.SaveChanges(true);
    }
}