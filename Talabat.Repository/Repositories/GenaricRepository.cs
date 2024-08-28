using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;
using Talabat.Repository.Specifications;

namespace Talabat.Repository.Repositories
{
    public class GenaricRepository<T> : IGenaricRepository<T> where T : BaseEntity
    {
        public readonly StoreDbContext _context;
        public GenaricRepository(StoreDbContext context) // ask clr create object from storedbcontext implicitly
        {
            _context = context;
        }

        //GetAll
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
            {
                return (IReadOnlyList<T>)await _context.Products.Include(p=>p.ProductBrand).Include(p=>p.ProductType).ToListAsync();
            }
            return await _context.Set<T>().ToListAsync();            
        }



        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
        {
            //return await SpecificationsEvaluator<T>.GetQuery(_context.Set<T>(),spec).ToListAsync();
            return await ApplySpecifications(spec).ToListAsync();
        }




        //GetById
        public async Task<T?> GetAsync(int id)
        {
            if (typeof(T) == typeof(Product))
            {
                return await _context.Products.Where(p=>p.Id==id).Include(p => p.ProductBrand).Include(p => p.ProductType).FirstOrDefaultAsync()as T;
            }
            return await _context.Set<T>().FindAsync(id);
        }


        public Task<T?> GetWithSpecAsync(ISpecifications<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_context.Set<T>(), spec).FirstOrDefaultAsync();

        }


        public async Task<int> GetCountAsync(ISpecifications<T> spec)
        {
            return await ApplySpecifications(spec).CountAsync();
        }


        private IQueryable<T> ApplySpecifications(ISpecifications<T> spec)
        {
            return SpecificationsEvaluator<T>.GetQuery(_context.Set<T>(),spec);
        }

        public async Task AddAsync(T item)  =>  await _context.Set<T>().AddAsync(item);

        public void Delete(T item) =>  _context.Set<T>().Remove(item);
        
        public void Update(T item) => _context.Set<T>().Update(item);

    }
}
