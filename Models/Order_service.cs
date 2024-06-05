namespace Autoservice.Models
{
    public class Order_service
    {
        public Order_service() { }
        public int id { get; set; }
        public int order_id { get; set; }
        public Orders? Orders { get; set; } = null!;
        public int service_id { get; set; }
        public Services? Services { get; set; } = null!;
        public Order_service(int order, int service)
        {
            order_id = order;
            service_id = service;
        }
    }
}
