using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Models.ViewModels
{
    public class ProductsViewModel
    {
        public Products Product { get; set; }

        public IEnumerable<ProductTypes> ProductTypes { get; set; }
        
        public IEnumerable<SpecialTags> SpecialTags { get; set; }

    }
}
