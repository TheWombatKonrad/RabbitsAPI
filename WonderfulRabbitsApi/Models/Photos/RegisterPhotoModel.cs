namespace WonderfulRabbitsApi.Models.Photos;

public class RegisterPhotoModel
{
    public int RabbitId { get; set; }
    public string Title { get; set; }
    public byte[] ImageData { get; set; }

}