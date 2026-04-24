using OtoServisSatis.Data;
using OtoServisSatis.Data.Concrete;
using OtoServisSatis.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace OtoServisSatis.Service.Concrete
{
    public class CarService : CarRepository, ICarService
    {
        public CarService(DatabaseContext context) : base(context)
        {
        }
    }
}
