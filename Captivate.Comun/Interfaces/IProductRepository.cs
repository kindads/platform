﻿using Captivate.Comun.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Interfaces
{   
    public interface IProductRepository : IGenericRepository<ProductEntity>
    {

        ProductEntity GetSingle(Guid fooId);
    }
}