using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace WonderfulRabbitsApi.Models.Images;

public class RegisterImageModel
{
    public int RabbitId { get; set; }
    public string Title { get; set; }
    public string Base64ImageData { get; set; }
    public string FileName { get; set; }
    public string FileExtension { get; set; }

}