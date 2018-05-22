﻿using Captivate.Business;
using Captivate.Common.Interfaces;
using Captivate.Comun.Models.Entities;
using Captivate.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Negocio
{
    public class CategoryManager : ITelemetria
    {
        CategoryRepository repository { set; get; }
        public ITrace telemetria { set; get; }
        public CategoryManager()
        {
            KindadsContext context = new KindadsContext();
            telemetria = new Trace();
            repository = new CategoryRepository { Context = context };
        }

        public List<CategoryEntity> GetByIdSite(Guid idSite)
        {
            return repository.GetByIdSite(idSite);
        }
    }
}