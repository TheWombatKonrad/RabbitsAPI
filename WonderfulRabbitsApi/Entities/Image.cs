using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace WonderfulRabbitsApi.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public DateTime DateAdded { get; set; }
        public virtual Rabbit Rabbit { get; set; }
        public string Title { get; set; }
        public byte[] ImageData { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }

        // public string Description { get; set; }
        // public decimal Size { get; set; }
    }
}