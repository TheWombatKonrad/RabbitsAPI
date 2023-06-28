namespace WonderfulRabbitsApi.Entities
{
    public class Photo
    {
        public int Id { get; set; }
        public DateTime DateAdded { get; set; }
        public virtual Rabbit Rabbit { get; set; }
    }
}