namespace WonderfulRabbitsApi.Services;

using AutoMapper;
using BCrypt.Net;
using WonderfulRabbitsApi.Authorization;
using WonderfulRabbitsApi.DatabaseContext;
using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Models.Users;
using WonderfulRabbitsApi.Services.Interfaces;

public class UserService : IUserService
{
    private RabbitDbContext _context;
    private IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private IJwtUtils _jwtUtils;

    public UserService(
        RabbitDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        IJwtUtils jwtUtils)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _jwtUtils = jwtUtils;
    }

    public AuthenticateResponse AuthenticateUser(AuthenticateRequest model)
    {
        var user = _context.Users.SingleOrDefault(x => x.Username == model.Username);

        // validate
        if (user == null || !BCrypt.Verify(model.Password, user.PasswordHash))
            throw new AppException("Username or password is incorrect");

        // authentication successful
        var response = _mapper.Map<AuthenticateResponse>(user);
        response.Token = _jwtUtils.GenerateToken(user);

        return response;
    }

    public List<User> GetUsers()
    {
        return _context.Users.ToList();
    }

    public async Task<User> GetUser(int id)
    {
        return await getById(id);
    }

    // public User GetCurrentUser()
    // {
    //     //doesn't allow anonymous so didn't add exception...
    //     var user = (User)_httpContextAccessor.HttpContext.Items["User"];
    //     return createUserView(user);
    // }

    public async Task<int> RegisterUserAsync(RegisterUserModel model)
    {
        // validate
        if (_context.Users.Any(x => x.Username == model.Username))
            throw new AppException("Username '" + model.Username + "' is already taken");

        // map model to new user object
        var user = _mapper.Map<User>(model);

        // hash password
        user.PasswordHash = BCrypt.HashPassword(model.Password);

        // save user
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return _context.Users.First(x => x.Username == model.Username).Id;
    }

    public async void UpdateUser(int id, UpdateUser model)
    {
        var user = await getById(id);

        // validate
        if (model.Username != user.Username && _context.Users.Any(x => x.Username == model.Username))
            throw new AppException("Username '" + model.Username + "' is already taken");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = BCrypt.HashPassword(model.Password);

        // copy model to user and save
        _mapper.Map(model, user);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async void DeleteUser(int id)
    {
        var user = await getById(id);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    //********************************
    //Helper Methods
    //********************************

    private async Task<User> getById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }
}