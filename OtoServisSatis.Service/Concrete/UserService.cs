using OtoServisSatis.Data;
using OtoServisSatis.Data.Concrete;
using OtoServisSatis.Service.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace OtoServisSatis.Service.Concrete
{
    public class UserService : UserRepository, IUserService
    {
        public UserService(DatabaseContext context) : base(context)
        {
        }
    }
}
