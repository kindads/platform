using Captivate.Azure;
using Captivate.Business;
using Captivate.Common.Interfaces;
using Captivate.Common.Models.Entities;
using Captivate.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Business
{
    public class CategoryManager : ITelemetria
    {
        CategoryRepository repository { set; get; }
        public ITrace telemetria { set; get; }
        public CategoryManager()
        {
     
            telemetria = new Trace();
            repository = new CategoryRepository();
        }

        public List<CategoryEntity> GetByIdSite(Guid idSite)
        {
            return repository.GetByIdSite(idSite);
        }
    }
}
