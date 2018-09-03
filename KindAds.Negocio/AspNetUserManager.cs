using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KindAds.Business;
using KindAds.Common.Interfaces;
using KindAds.Common.Models.Entities;
using KindAds.Comun.Models.Entities;
using KindAds.DataAccess;

namespace KindAds.Negocio
{
    public class AspNetUserManager : ITelemetria
    {
        public ITrace telemetria { set; get; }
        public AspNetUserRepository repository { set; get; }

        public AspNetUserManager()
        {
            repository = new AspNetUserRepository();
        }

        public AspNetUserEntity GetById(Guid id)
        {
            return repository.GetById(id);
        }

        public bool Update(AspNetUserEntity aspNetUserEntity)
        {
            repository.Edit(aspNetUserEntity);
            return true;
        }
    }

}
