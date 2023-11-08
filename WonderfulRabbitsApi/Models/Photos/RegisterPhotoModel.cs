using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace WonderfulRabbitsApi.Models.Photos;

public class RegisterPhotoModel
{
    public int RabbitId { get; set; }
    public string Title { get; set; }

    // [ModelBinder(BinderType = typeof(ByteArrayModelBinder))]
    public string Base64ImageData { get; set; }

}