namespace WonderfulRabbitsApi.Services;

using AutoMapper;
using BCrypt.Net;
using WonderfulRabbitsApi.Authorization;
using WonderfulRabbitsApi.DatabaseContext;
using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Models.Users;
using WonderfulRabbitsApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using WonderfulRabbitsApi.Models.Rabbits;
using System.Collections.Generic;

public class RabbitService : IRabbitService
{
    private RabbitDbContext _context;
    private IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private IJwtUtils _jwtUtils;

    public RabbitService(
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


    public async Task<int> RegisterRabbitAsync(RegisterRabbitModel model)
    {
        var rabbit = _mapper.Map<Rabbit>(model);

        if (model.UserId != null) rabbit.User = await GetUserByIdAsync(model.UserId);
        else rabbit.User = GetCurrentUser();

        _context.Rabbits.Add(rabbit);
        await _context.SaveChangesAsync();

        return rabbit.Id;
    }

    public async Task<Rabbit> GetRabbitAsync(int id)
    {
        return await GetRabbitByIdAsync(id);
    }

    public Task<List<Rabbit>> GetRabbitsAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateRabbitAsync(int id, UpdateRabbitModel model)
    {
        throw new NotImplementedException();
    }

    public Task DeleteRabbitAsync(int id)
    {
        throw new NotImplementedException();
    }

    //****************
    //Helper methods
    //****************

    private async Task<Rabbit> GetRabbitByIdAsync(int id)
    {
        var rabbit = await _context.Rabbits.FindAsync(id);
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
