namespace WonderfulRabbitsApi.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public DateTime DateAdded { get; set; }
        public virtual Rabbit Rabbit { get; set; }
        public string Title { get; set; }
        public byte[] ImageData { get; set; }


        // public int Id { get; set; }
        // public byte[] Bytes { get; set; }
        // public string Description { get; set; }
        // public string FileExtension { get; set; }
        // public decimal Size { get; set; }
        // public int ProductId { get; set; }
        // [ForeignKey("ProductId")]
        // public Product Product { get; set; }
    }
}