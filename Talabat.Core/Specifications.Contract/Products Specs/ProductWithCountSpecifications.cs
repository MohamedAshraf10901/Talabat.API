using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Products_Specs
{
    public class ProductWithCountSpecifications :BaseSpecifications<Product>
    {
        public ProductWithCountSpecifications(ProductSpecParams productSpec) : base(p =>

            (!productSpec.BrandId.HasValue || p.ProductBrandId == productSpec.BrandId.Value)
            &&
            (!productSpec.TypeId.HasValue || p.ProductTypeId == productSpec.TypeId.Value)

            )
        {
            
        }
    }
}
