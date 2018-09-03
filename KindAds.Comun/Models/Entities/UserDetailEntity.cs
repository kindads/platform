using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.Entities
{   
    public class UserDetailEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public System.DateTime RegistrationDate { get; set; }
        public bool IsPremium { get; set; }
        public string UserId { get; set; }
        public bool IsMetamask { get; set; }
    }
}
