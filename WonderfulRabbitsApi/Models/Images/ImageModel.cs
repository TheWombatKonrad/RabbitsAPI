using WonderfulRabbitsApi.Models.Rabbits;

namespace WonderfulRabbitsApi.Models.Images
{
    public class ImageModel
    {
        public int Id { get; set; }
        public DateTime DateAdded { get; set; }
        public virtual RabbitDataModel Rabbit { get; set; }
        public string Title { get; set; }
        public string Base64ImageData { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }
}