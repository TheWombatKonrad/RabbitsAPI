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
using System.Collections.Immutable;

public class PhotoService : IPhotoService
{
    private RabbitDbContext _context;
    private readonly IMapper _mapper;

    public PhotoService(
        RabbitDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> RegisterPhotoAsync(RegisterPhotoModel model)
    {
        var photo = _mapper.Map<Photo>(model);
        photo.DateAdded = DateTime.Now;
        photo.Rabbit = await GetRabbitByIdAsync(model.RabbitId);

        _context.Photos.Add(photo);
        await _context.SaveChangesAsync();

        return photo.Id;

    }
    public async Task<Photo> GetPhotoAsync(int id)
    {
        var photo = await _context.Photos
            .Include(i => i.Rabbit)
            .FirstOrDefaultAsync(x => x.Id == id);

        return photo;
    }

    public async Task<List<Photo>> GetPhotosAsync()
    {
        return await _context.Photos.Include(i => i.Rabbit).ToListAsync();
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