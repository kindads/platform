using Captivate.Business;
using Captivate.Common.Interfaces;
using Captivate.Comun.Models;
using Captivate.Comun.Models.Entities;
using Captivate.Comun.Models.ViewModel;
using Captivate.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Negocio
{
    public  class ProductManager : ITelemetria
    {
        public ITrace telemetria { set; get; }
        public ProductRepository repository { set; get; }

        public ProductManager()
        {
            KindadsContext context = new KindadsContext();
            telemetria = new Trace();
            repository = new ProductRepository { Context=context};
        }

        public  List<ProductEntity> GetAllActive()
        {
            List<ProductEntity> productos = new List<ProductEntity>();
            productos = repository.FindBy( o=>o.IsActive==true).ToList();
            return productos;
        }


        public ProductViewModel GetBlock(string idUser,int page)
        {
            ProductViewModel model = new ProductViewModel();
            var products = GetAllActive();
            //Todo
            int pageSize = 4;

            model.PageSize = pageSize;
            model.TotalRecord = (from r in products where r.AspNetUsers_Id.Equals(idUser) select r).Count();
            model.NoOfPages = (model.TotalRecord / model.PageSize) + ((model.TotalRecord % model.PageSize) > 0 ? 1 : 0);
            model.ListProducts= (from product in products
                                 orderby product.ShortDescription ascending
                                 where product.AspNetUsers_Id==idUser
                                 select product).ToList().Skip((page - 1) * model.PageSize).Take(model.PageSize).ToList();

            return model;
        }

        public bool LogicalDelete(Guid Id)
        {
            bool result = false;
            ProductEntity producto = repository.FindById(Id);
            if (ContainCampaignActive(producto) == false)
            {
                result = repository.DeleteById(Id);
            }
            return result;
        }

        public bool AddProduct(ProductEntity product)
        {
            repository.Add(product);
            return true;
        }

        public bool ContainCampaignActive(ProductEntity product)
        {
            bool result = false;
            result = product.CampaignEntitys.Count() > 0 ? true : false;
            return result;
        }
    }
}
