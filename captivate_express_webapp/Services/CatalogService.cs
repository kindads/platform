using captivate_express_webapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web;

namespace captivate_express_webapp.Services
{
  public class CatalogService
  {
    private KindadsEntities _context;

    public CatalogService()
    {
      _context = new KindadsEntities();
    }

    public Task<List<CATEGORY>> GetAllCategoryAsync()
    {
      return (from r in _context.CATEGORIES orderby r.Description select r).ToListAsync();
    }

    public async Task<List<PRODUCT_TYPE>> GetAllProductTypeAsync()
    {
      return await (from r in _context.PRODUCT_TYPE select r).ToListAsync();
    }


  }
}
