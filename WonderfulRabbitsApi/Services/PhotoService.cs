namespace WonderfulRabbitsApi.Services;

using AutoMapper;
using WonderfulRabbitsApi.DatabaseContext;
using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using WonderfulRabbitsApi.Models.Rabbits;
using System.Collections.Generic;
using WonderfulRabbitsApi.Models.Photos;

public class PhotoService : IPhotoService
{
    private RabbitDbContext _context;
    private IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public PhotoService(
        RabbitDbContext context,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<int> RegisterPhoto(RegisterPhotoModel model)
    {
        var photo = _mapper.Map<Photo>(model);
        photo.DateAdded = DateTime.Now;
        photo.Rabbit = await GetRabbitByIdAsync(model.RabbitId);

        _context.Photos.Add(photo);
        await _context.SaveChangesAsync();

        return photo.Id;

    }

    //****************
    //Helper methods
    //****************

    private async Task<Rabbit> GetRabbitByIdAsync(int id)
    {
        var rabbit = await _context.Rabbits
            .Include(i => i.User)
            .Include(i => i.Photos)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (rabbit == null) throw new KeyNotFoundException("Rabbit not found");
        return rabbit;
    }

}