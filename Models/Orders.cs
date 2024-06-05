using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Autoservice.Models
{
    public class Orders
    {
        public Orders() { }

        [DisplayName("Номер заказа")]
        [HiddenInput(DisplayValue = false)]
        public int id { get; set; }

        [DisplayName("Клиент")]
        [Required]
        public int client_id { get; set; }

        [DisplayName("Менеджер")]
        [Required]
        public int employee_id { get; set; }

        [DisplayName("Дата покупки")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        [Required]
        public DateTime date { get; set; }

        [DisplayName("Время выполнения")]
        public int? completeInDays { get; set; }


        [UIHint("Decimal")]
        [DisplayName("Сумма")]
        public decimal? sum { get; set; }

        [DisplayName("Статус")]
        [Required]
        public string? status { get; set; }

        public Clients Clients { get; set; } = null!;
        public Employees Employees { get; set; } = null!;

        public ICollection<Order_service> Order_service { get; } = new List<Order_service>();
        public Orders(int id_client, int employee_id, DateTime date, int completeInDays, decimal? sum, string? status)
        {
            this.client_id = id_client;
            this.employee_id = employee_id;
            this.date = date;
            this.completeInDays = completeInDays;
            this.sum = sum;
            this.status = status;
        }
    }
}

