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
            telemetria = new Trace();
            repository = new ProductRepository();
        }

        public  List<ProductEntity> GetAllActive()
        {
            List<ProductEntity> productos = new List<ProductEntity>();
            productos = repository.GetActiveProducts();
            return productos;
        }


        public ProductViewModel GetBlock(string idUser,int page)
        {
            ProductViewModel model = new ProductViewModel();
            var products = repository.GetActiveProductsByUserId(Guid.Parse(idUser)); // GetAllActive();
            //Todo
            int pageSize = 4;

            model.PageSize = pageSize;
            model.TotalRecord = products.Count();
            model.NoOfPages = (model.TotalRecord / model.PageSize) + ((model.TotalRecord % model.PageSize) > 0 ? 1 : 0);
            model.ListProducts = (from product in products
             orderby product.ShortDescription ascending
             select product)
            .ToList()
            .Skip((page - 1) * model.PageSize)
            .Take(model.PageSize).ToList();

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
