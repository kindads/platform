using captivate_express_webapp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System;

namespace captivate_express_webapp.Services
{
  public class PublisherService
  {
    private KindadsEntities _context;

    public PublisherService()
    {
      _context = new KindadsEntities();
    }

    public async Task<List<PRODUCT>> GetSitesByIdUser(string idUser)
    {
      return await (from s in _context.SITES
                    from p in s.PRODUCTs
                    join pt in _context.PRODUCT_TYPE on p.PRODUCT_TYPE.IdProductType equals pt.IdProductType
                    where p.AspNetUsers_Id.Equals(idUser)
                    select p).ToListAsync();
    }

    public List<PARTNER> GetPartnersByIdProductType(string id)
    {
      if (!string.IsNullOrEmpty(id))
      {
        var idProductType = new System.Guid(id);
        List<PARTNER> partners= (from prtn in _context.PARTNERS from pt in prtn.PRODUCT_TYPE where pt.IdProductType.Equals(new Guid(id)) select prtn).ToList();

        return (partners);

      }
      return new List<PARTNER>();
    }

    public bool ShowCreateProduct(string idUser)
    {
      var result = (from s in _context.SITES
                    from p in s.PRODUCTs
                    join pt in _context.PRODUCT_TYPE on p.PRODUCT_TYPE.IdProductType equals pt.IdProductType
                    where p.AspNetUsers_Id.Equals(idUser)
                    select p).ToList();

      return result != null && result.Any();
    }

    public bool ShowCreateSite(string idUser)
    {
      return !((from r in _context.SITES where r.AspNetUsers_Id.Equals(idUser) select r).Count() > 0);
    }

    public async Task<List<SITE>> GetSitesListByIdUser(string idUser)
    {
      return await (from s in _context.SITES
                    where s.AspNetUsers_Id.Equals(idUser) && s.Verified == true
                    select s).ToListAsync();
    }

    public string GetPublisherBalance(string idUser)
    {
      AspNetUser _aspNetUser = (AspNetUser)((from d in _context.AspNetUsers where d.Id.Equals(idUser) select d).FirstOrDefault());

      string wallet = _aspNetUser.WalletAddress;

      var _balance = Helpers.AsyncHelpers.RunSync<string>(() => Helpers.NethereumHelper.GetBalance(wallet, true));

      return _balance.ToString();
    }

    public string GetPublisherBalanceAsync(string idUser)
    {
      AspNetUser _aspNetUser = ((from d in _context.AspNetUsers where d.Id.Equals(idUser) select d).FirstOrDefault());

      string wallet = _aspNetUser.WalletAddress;
      //System.Threading.Thread.Sleep(15000);
      return Helpers.AsyncHelpers.RunSync<string>(() => Helpers.NethereumHelper.GetBalance(wallet, true));
    }

    public async Task<List<Models.Wallet.walletTransactions>> GetTransactionsByIdUser(string idUser)
    {
      string sinout = "IN";
      AspNetUser _user = (from _u in _context.AspNetUsers where _u.Id.Equals(idUser) select _u).FirstOrDefault();
      List<Models.Wallet.walletTransactions> _walletTransactions = new List<Models.Wallet.walletTransactions>();

      List<TRANSACTIONS_CAPT> _tcap = await (from t in _context.TRANSACTIONS_CAPT
                                             where t.HashTo.Equals(_user.WalletAddress) || t.HashFrom.Equals(_user.WalletAddress)
                                             select t).ToListAsync();

      List<TRANSACTIONS_EXTERNAL> _text = await (from t in _context.TRANSACTIONS_EXTERNAL
                                                 where t.HashTo.Equals(_user.WalletAddress) || t.HashFrom.Equals(_user.WalletAddress)
                                                 select t).ToListAsync();

      foreach (TRANSACTIONS_CAPT d in _tcap)
      {
        if (d.HashFrom == _user.WalletAddress)
        { sinout = "<img src='https://wakindads.azurewebsites.net/Img/UI/kads_out.png' width='24px' /> "; }
        else
        { sinout = "<img src='https://wakindads.azurewebsites.net/Img/UI/kads_in.png' width='24px' /> "; }

        _walletTransactions.Add(new Models.Wallet.walletTransactions() { hashfrom = d.HashFrom, hashto = d.HashTo, hashtransaction = d.HashTransaction, date = d.RegisterDate, value = (Convert.ToDouble(d.Amount) / 100000000), inout = sinout });
      }

      foreach (TRANSACTIONS_EXTERNAL xt in _text)
      {
        if (xt.HashFrom == _user.WalletAddress)
        { sinout = "<img src='https://wakindads.azurewebsites.net/Img/UI/kads_out.png' width='24px' /> "; }
        else
        { sinout = "<img src='https://wakindads.azurewebsites.net/Img/UI/kads_in.png' width='24px' /> "; }

        _walletTransactions.Add(new Models.Wallet.walletTransactions() { hashfrom = xt.HashFrom, hashto = xt.HashTo, hashtransaction = xt.HashTransaction, date = xt.RegisterDate, value = (Convert.ToDouble(xt.Amount) / 100000000), inout = sinout });
      }

      return (from r in _walletTransactions orderby r.date descending select r).ToList();
    }

    public List<PRODUCT> GetProductsByIdSite(Guid idSite)
    {
      return (from p in _context.PRODUCTS
              where p.SITE.IdSite.Equals(idSite)
              select p).ToList();
    }

  }
}
