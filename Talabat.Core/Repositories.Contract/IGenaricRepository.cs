using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repositories.Interfaces
{
    public interface IGenaricRepository<T> where T : BaseEntity
    {
        //GetAll
        Task<IReadOnlyList<T>> GetAllAsync();

        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec);



        //GetById
        Task<T?> GetAsync(int id);


        Task<T?> GetWithSpecAsync(ISpecifications<T> spec);



        Task<int> GetCountAsync(ISpecifications<T> spec);



        Task AddAsync(T item);
        void Delete(T item);
        void Update(T item);



    }
}
