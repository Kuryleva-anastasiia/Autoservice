using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Autoservice.Models
{
    public class Cart
    {
        public Cart() { }
        public int id { get; set; }
        public int client_id { get; set; }
        public Clients? Clients { get; set; } = null!;
        public int service_id { get; set; }
        public Services? Services { get; set; } = null!;

        public Cart(int client, int service)
        {
            client_id = client;
            service_id = service;
        }
    
    }
}

