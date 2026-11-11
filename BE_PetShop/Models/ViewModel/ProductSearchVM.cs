using System.Collections.Generic;
using BE_PetShop.Models;
using PagedList;

namespace BE_PetShop.Models.ViewModel
{
    public class ProductSearchVM
    {
        public string SearchTerm { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortOrder { get; set; }
        public IPagedList<Product> Products { get; set; }
    }
}