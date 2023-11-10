namespace WonderfulRabbitsApi.Services;

using AutoMapper;
using WonderfulRabbitsApi.DatabaseContext;
using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using WonderfulRabbitsApi.Models.Rabbits;
using System.Collections.Generic;
using WonderfulRabbitsApi.Models.Images;
using System.Collections.Immutable;

public class ImageService : IImageService
{
    private RabbitDbContext _context;
    private readonly IMapper _mapper;

    public ImageService(
        RabbitDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> UploadImageAsync(UploadImageModel model)
    {
        var image = _mapper.Map<Image>(model);
        image.DateAdded = DateTime.Now;
        image.Rabbit = await GetRabbitByIdAsync(model.RabbitId);
        image.FileName = Path.GetRandomFileName();

        _context.Images.Add(image);
        await _context.SaveChangesAsync();

        return image.Id;

    }
    public async Task<Image> GetImageAsync(int id)
    {
        var image = await _context.Images
            .Include(i => i.Rabbit)
            .ThenInclude(i => i.User)
            .FirstOrDefaultAsync(x => x.Id == id);

        return image;
    }

    public async Task<List<Image>> GetImagesAsync()
    {
        return await _context.Images
            .Include(i => i.Rabbit)
            .ThenInclude(i => i.User)
            .ToListAsync();
    }

    public async Task DeleteImageAsync(int id)
    {
        var image = await _context.Images.FirstOrDefaultAsync(x => x.Id == id);

        _context.Images.Remove(image);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateImageAsync(int id, UpdateImageModel model)
    {
        var image = await GetImageAsync(id);
        _mapper.Map(model, image);

        _context.Images.Update(image);
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

    private async Task<Image> GetImageByIdAsync(int id)
    {
        var image = await _context.Images
            .Include(i => i.Rabbit)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (image == null) throw new KeyNotFoundException("Image not found");
        return image;
    }
}