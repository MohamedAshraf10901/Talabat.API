using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository.Specifications
{
    public class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
    {


        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,ISpecifications<TEntity> spec)
        {
            var query = inputQuery; // _context.Set<Product>()

            if(spec.Criteira is not null)
            {
                query = query.Where(spec.Criteira);
            }// _context.Set<Product>().Where(p=>p.Id ==10)
            //include
            // 1. p => p.Brands
            // 2. p => p.category


            if(spec.OrderBy is not null)
                query = query.OrderBy(spec.OrderBy);
            // _context.Set<Product>().OrderBy ( p => p.Name)
            
            if (spec.OrderByDesc is not null)
                query =query.OrderByDescending(spec.OrderByDesc);
            // _context.Set<Product>().OrderByDescending ( p => p.Name)


            if (spec.IsPaginationEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }


            // ahmed  ali  omar mahmoud
            query = spec.Includes.Aggregate(query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));
            //_context.Set<Product>().Where(p => p.Id == 10).include(p => p.Brands).include(p => p.category)
            // _context.Set<Product>().OrderBy ( p => p.Name).include(p => p.Brands).include(p => p.category)
            // _context.Set<Product>().OrderByDescending ( p => p.Name).include(p => p.Brands).include(p => p.category)


            return query;
        }


    }
}

//_context.Products.Where(p=>p.Id==id).Include(p => p.Brand).Include(p => p.Category).FirstOrDefaultAsync()as T;
//_context.Products.Include(p=>p.Brand).Include(p=>p.Category).ToListAsync();
