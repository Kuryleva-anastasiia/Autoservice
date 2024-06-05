using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Autoservice.Models
{
    public class Services
    {
        [HiddenInput(DisplayValue = false)]
        public int id { get; set; }

        [DisplayName("Название")]
        [Required]
        public string? name { get; set; }
        
        [DisplayName("Описание")]
        [DataType(DataType.MultilineText)]
        [UIHint("MultilineText")]
        public string? description { get; set; }

        [DisplayName("Цена")]
        [HiddenInput(DisplayValue = false)]
        [UIHint("Decimal")]
        public decimal? price { get; set; }

        [DisplayName("Время выполнения")]
        [Required]
        public int? duration { get; set; }

        [DisplayName("Категория")]
        [Required]
        public int category_id { get; set; }

        public ICollection<Cart> Cart { get; } = new List<Cart>();
        //public ICollection<Order_Rent_Books> RentBooks { get; } = new List<Order_Rent_Books>();
        public ICollection<Order_service> Order_service { get; } = new List<Order_service>();
        public Categories Categories { get; set; } = null!;


    }
}
