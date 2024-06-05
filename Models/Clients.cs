using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace Autoservice.Models
{
    public class Clients
    {
        [HiddenInput(DisplayValue = false)]
        public int id { get; set; }

        [DisplayName("Телефон")]
        [DataType(DataType.PhoneNumber)]
        [Required]
        public string login { get; set; } = "";
        
        [Required]
        [DisplayName("Пароль")]
        [DataType(DataType.Password)]
        public string? password { get; set; }


        [DisplayName("Клиент")]
        public string? name { get; set; }

        public ICollection<Orders> Orders { get; } = new List<Orders>();

        public ICollection<Cart> Cart { get; } = new List<Cart>();
        //public ICollection<Order_Rent> Order_Rent { get; } = new List<Order_Rent>();
        //public ICollection<Order_Buy> Order_Buy { get; } = new List<Order_Buy>();
    }


}
