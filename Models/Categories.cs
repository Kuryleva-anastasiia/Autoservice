namespace Autoservice.Models
{
    public class Categories
    {
        public int id { get; set; }
        public string? name { get; set; }
        //public ICollection<Books> Books { get; } = new List<Books>();
        public ICollection<Services> Services { get; } = new List<Services>();
    }
}
