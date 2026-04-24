using OtoServisSatis.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace OtoServisSatis.Data.Abstract
{
    public interface ICarRepository :IRepository<Arac>
    {
        Task<List<Arac>> GetCustomCarList();
        Task<List<Arac>> GetCustomCarList(Expression<Func<Arac, bool>> expression);
        Task<Arac> GetCustomCar(int id);
    }
}
