using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;

namespace Talabat.Repository.Data
{
    public static class StoreDbContextSeed
    {
        // Data Seeding

        public static async Task SeedAsync(StoreDbContext _context)
        {

            //**** Brands ****

            if (_context.Brands.Count() == 0)
            {
                // 1.Reed Data from Json File
                var brandData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/brands.json");
                //Console.WriteLine(brandData);

                // 2.convert json string to the needed type
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);

                if(brands?.Count() > 0)
                {
                    foreach (var brand in brands)
                    {
                        _context.Brands.Add(brand);
                    }
                    await _context.SaveChangesAsync();
                }
            }



            //*******************************************************************

            //**** Categories ****

            if (_context.Types.Count() == 0)
            {

                // 1.Reed Data from Json File
                var categoryData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/categories.json");
                //Console.WriteLine(categoryData);

                // 2.convert json string to the needed type
                var Categories = JsonSerializer.Deserialize<List<ProductType>>(categoryData);

                if (Categories?.Count() > 0)
                {
                    foreach (var category in Categories)
                    {
                        _context.Types.Add(category);
                    }
                    await _context.SaveChangesAsync();
                }
            }

            //*************************************************************************

            //**** Products ****

            if(_context.Products.Count() == 0) 
            { 
                // 1.Reed Data from Json File
                var productData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/products.json");
                //Console.WriteLine(productData);

                // 2.convert json string to the needed type
                var products = JsonSerializer.Deserialize<List<Product>>(productData);

                if (products?.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        _context.Set<Product>().Add(product);
                    }
                    await _context.SaveChangesAsync();
                }
            }

            //*************************************************************************

            //**** delivary ****

            if (_context.DeliveryMethods.Count() == 0)
            {
                // 1.Reed Data from Json File
                var deliveryData = File.ReadAllText("../Talabat.Repository/Data/DataSeed/delivery.json");
                //Console.WriteLine(deliveryData);

                // 2.convert json string to the needed type => List<DeliveryMethod>
                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);

                if (deliveryMethods?.Count() > 0)
                {
                    foreach (var deliveryMethod in deliveryMethods)
                    {
                        _context.Set<DeliveryMethod>().Add(deliveryMethod);
                    }
                    await _context.SaveChangesAsync();
                }
            }

        }

    }
}
