using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.Products_Specs
{
    public class ProductWithBrandAndCategorySpecifications : BaseSpecifications<Product>
    {

        //This Ctor will be used for creating Object for GetAll Products
        public ProductWithBrandAndCategorySpecifications(ProductSpecParams productSpec)
            : base(p =>             
                    (string.IsNullOrEmpty(productSpec.Search)||p.Name.ToLower().Contains(productSpec.Search))
                    &&
                    (!productSpec.BrandId.HasValue || p.ProductBrandId == productSpec.BrandId.Value) 
                    && 
                    (!productSpec.TypeId.HasValue || p.ProductTypeId == productSpec.TypeId.Value)
        
            )
        
        {
            Includes.Add(p=>p.ProductBrand);
            Includes.Add(p => p.ProductType);

            if (!string.IsNullOrEmpty(productSpec.Sort))
            {
                switch (productSpec.Sort)
                {
                    case "priceAsc":
                        //OrderBy=p=>p.Price;
                        AddOrderBy(p=>p.Price);
                        break;

                    case "priceDesc":
                        //OrderByDesc = p => p.Price;
                        AddOrderByDesc(p => p.Price);
                        break;

                    default:
                        AddOrderBy(p => p.Name);
                        break;

                }
            }
            else
            {
                AddOrderBy(p => p.Name);
            }

            // Total = 1000
            // PageIndex = 9
            // PageSize = 50
            // ====>>>> Skip 400  ,  Take 50

            ApplyPagination(productSpec.PageSize * (productSpec.PageIndex - 1), productSpec.PageSize);

        }

        //This Ctor will be used for creating Object for GetById Products
        public ProductWithBrandAndCategorySpecifications(int id) :base(p => p.Id ==id)
        {
            Includes.Add(p => p.ProductBrand);
            Includes.Add(p => p.ProductType);
        }

    
    }
}
