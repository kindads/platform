using Captivate.Common.Models;
using Captivate.Common.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Common.Interfaces
{
    public interface ICategorySiteRepository : IDapperGenericRepository<CategorySiteEntity>
    {
        CategorySiteEntity GetSingle(int CATEGORY_IdCategory, Guid SITEs_IdSite);
    }
}
