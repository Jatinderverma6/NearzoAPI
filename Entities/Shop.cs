namespace NearzoAPI.Entities
{
    public class Shop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UPIId { get; set; }
        public string Address { get; set; }

        public string OwnerId { get; set; }
        public User Owner { get; set; }
    }
}
