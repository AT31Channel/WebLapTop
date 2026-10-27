using System.Collections.Generic;
using WebLapTop.Models.Entities;

namespace WebLapTop.Models.ViewModels
{
    public class AdminProductsViewModel
    {
        public List<Product> Products { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; } = 10;
    }
} 