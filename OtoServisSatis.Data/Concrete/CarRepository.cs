using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OtoServisSatis.Data.Abstract;
using OtoServisSatis.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace OtoServisSatis.Data.Concrete
{
    public class CarRepository : Repository<Arac>, ICarRepository
    {
        public CarRepository(DatabaseContext context) : base(context)
        {
        }

        public async Task<Arac> GetCustomCar(int id)
        {
            return await _dbSet.AsNoTracking().Include(a => a.Marka).FirstOrDefaultAsync(b=>b.Id == id);
        }

        public async Task<List<Arac>> GetCustomCarList()
        {
            return await _dbSet.AsNoTracking().Include(a => a.Marka).ToListAsync();
        }

        public async Task<List<Arac>> GetCustomCarList(Expression<Func<Arac, bool>> expression)
        {
            return await _dbSet.Where(expression).AsNoTracking().Include(a => a.Marka).ToListAsync();
        }
    }
}
