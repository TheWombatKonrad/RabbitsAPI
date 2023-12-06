namespace WonderfulRabbitsApi.Services;

using AutoMapper;
using WonderfulRabbitsApi.DatabaseContext;
using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using WonderfulRabbitsApi.Models.Rabbits;
using System.Collections.Generic;

public class RabbitService : IRabbitService
{
    private RabbitDbContext _context;
    private IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public RabbitService(
        RabbitDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<int> RegisterRabbitAsync(RegisterRabbitModel model)
    {
        var rabbit = _mapper.Map<Rabbit>(model);
        rabbit.User = GetCurrentUser();

        if (rabbit.User == null) throw new AppException("Error: The current user could not be retrieved.");

        _context.Rabbits.Add(rabbit);
        await _context.SaveChangesAsync();

        return rabbit.Id;
    }

    public async Task<Rabbit> GetRabbitAsync(int id)
    {
        return await GetRabbitByIdAsync(id);
    }

    public async Task<List<Rabbit>> GetRabbitsAsync()
    {
        return await _context.Rabbits
            .Include(i => i.User)
            .Include(i => i.Images)
            .ToListAsync();
    }

    public async Task UpdateRabbitAsync(int id, UpdateRabbitModel model)
    {
        var rabbit = await GetRabbitByIdAsync(id);
        _mapper.Map(model, rabbit);
        _context.Rabbits.Update(rabbit);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRabbitAsync(int id)
    {
        var rabbit = _context.Rabbits.FirstOrDefault(x => x.Id == id);

        _context.Remove(rabbit);
        await _context.SaveChangesAsync();
    }

    //****************
    //Helper methods
    //****************

    private async Task<Rabbit> GetRabbitByIdAsync(int id)
    {
        var rabbit = await _context.Rabbits
            .Include(i => i.User)
            .Include(i => i.Images)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (rabbit == null) throw new KeyNotFoundException("Rabbit not found");
        return rabbit;
    }

    private async Task<User> GetUserByIdAsync(int? id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) throw new KeyNotFoundException("The user could not be found.");

        return user;
    }

    private User GetCurrentUser()
    {
        try
        {
            return _httpContextAccessor.HttpContext.Items["User"] as User;
        }
        catch
        {
            throw new KeyNotFoundException("No current user could be found.");
        }
    }

}
