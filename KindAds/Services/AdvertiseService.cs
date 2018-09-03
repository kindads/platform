using KindAds.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using KindAds.Models.Advertiser;

namespace KindAds.Services
{

  public class AdvertiseService
  {
    private KindadsEntities _context;
    public AdvertiseService()
    {
      _context = new KindadsEntities();
    }

    public List<PRODUCT_TYPE> GetAllProductType()
    {
      return (from r in _context.PRODUCT_TYPE orderby r.Name select r).ToList();
    }

    public List<CATEGORY> GetAllCategory()
    {
      return (from r in _context.CATEGORIES orderby r.Description select r).ToList();
    }

    public List<TAG> GetAllTag()
    {
      return (from r in _context.TAGS orderby r.Description select r).ToList();
    }

    public List<PARTNER> GetAllPartner()
    {
      return (from r in _context.PARTNERS orderby r.Name select r).ToList();
    }

    public async Task<List<PRODUCT_TYPE>> GetAllProductTypeAsync()
    {
      return await (from r in _context.PRODUCT_TYPE select r).ToListAsync();
    }

    public Task<List<CATEGORY>> GetAllCategoryAsync()
    {
      return (from r in _context.CATEGORIES orderby r.Description select r).ToListAsync();
    }

    public Task<List<TAG>> GetAllTagAsync()
    {
      return (from r in _context.TAGS orderby r.Description select r).ToListAsync();
    }

    public Task<List<PARTNER>> GetAllPartnerAsync()
    {
      return (from r in _context.PARTNERS orderby r.Name select r).ToListAsync();
    }

    public Task<List<ProductPurchasedViewModel>> GetAllProductsPurchasedAsync()
    {
      return (from p in _context.PRODUCTS
              join pt in _context.PRODUCT_TYPE on p.PRODUCT_TYPE.IdProductType equals pt.IdProductType
              orderby p.RegistrationDate descending
              select new ProductPurchasedViewModel { Product = p, ProductType = pt, Site = p.SITE, ListCategory = p.SITE.CATEGORY }).ToListAsync();
    }

    public List<PARTNER> GetPartnersByIdProductType(string id)
    {
      if (!string.IsNullOrEmpty(id))
      {
        var idProductType = new System.Guid(id);
        return (from p in _context.PRODUCTS
                join ptnr in _context.PARTNERS on p.PARTNER.IdPartner equals ptnr.IdPartner
                join pt in _context.PRODUCT_TYPE on p.PRODUCT_TYPE.IdProductType equals pt.IdProductType
                where pt.IdProductType.Equals(idProductType)
                select ptnr).Distinct().ToList();
      }
      return new List<PARTNER>();
    }

    public List<TAG> GetTagsByIdCategory(string id)
    {
      short idCategory;
      bool isNumber = short.TryParse(id, out idCategory);

      if (isNumber)
      {
        return (from t in _context.TAGS
                where t.CATEGORies.All(c => c.IdCategory.Equals(idCategory))
                select t).Distinct().ToList();
      }
      return null;
    }

    public List<ProductPurchasedViewModel> GetProductsByFilters(string idProduct, string idCategory, string idTag, string idPartner, string price)
    {
      List<ProductPurchasedViewModel> list = new List<ProductPurchasedViewModel>();
      IQueryable<PRODUCT> query = _context.PRODUCTS;

      if (idProduct != null && idProduct != string.Empty)
      {
        query = query.Where(en => en.PRODUCT_TYPE.IdProductType.Equals(new System.Guid(idProduct)));
      }

      if (idPartner != null && idPartner != string.Empty)
      {
        query = query.Where(r => r.PARTNER.IdPartner.Equals(new System.Guid(idPartner)));
      }

      if (price != null && double.TryParse(price, out double priceNum))
      {
        query = query.Where(r => r.Price.Equals(priceNum));
      }


      if (idCategory != null && short.TryParse(idCategory, out short idCategoryNum))
      {

        query = (from p in query
                 join s in _context.SITES on p.SITE.IdSite equals s.IdSite
                 from c in s.CATEGORY
                 where c.IdCategory.Equals(idCategoryNum)
                 select p).Distinct();
      }


      if (idTag != null && short.TryParse(idTag, out short idTagNum))
      {
        query = (from p in query
                 join s in _context.SITES on p.SITE.IdSite equals s.IdSite
                 from c in s.CATEGORY
                 from t in c.TAG
                 where t.IdTag.Equals(idTagNum)
                 select p).Distinct();
      }

      query = (from p in query orderby p.RegistrationDate descending select p);
      var queryResult = query.ToList();

      if (queryResult != null && queryResult.Any())
      {
        foreach (var item in queryResult)
        {
          list.Add(new ProductPurchasedViewModel { Product = item, ProductType = item.PRODUCT_TYPE, Site = item.SITE, ListCategory = item.SITE.CATEGORY });
        }
      }
      return list;
    }
  }


}
