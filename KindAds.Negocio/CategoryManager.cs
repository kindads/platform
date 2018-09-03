using KindAds.Azure;
using KindAds.Business;
using KindAds.Common.Interfaces;
using KindAds.Common.Models.Entities;
using KindAds.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Business
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
