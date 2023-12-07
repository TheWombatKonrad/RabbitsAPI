using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace WonderfulRabbitsApi.Models.Images;

public class UpdateImageModel
{
    public string Title { get; set; }
}