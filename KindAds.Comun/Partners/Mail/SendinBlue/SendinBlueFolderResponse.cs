using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Partners.Mail.SendinBlue
{
    public class SendinBlueFolderResponse
    {
        public Folder[] folders { set; get; }

        public SendinBlueFolderResponse()
        {
            folders = new Folder[] { };
        }
    }

    public class Folder
    {
       public int id { set; get; }
       public string name { set; get; }

       public List<Lista> Listas { set; get; }
       public dynamic lists { set; get; }
       public int  unique_subscribers { set; get; }
       public int total_blacklisted { set; get; }

        public int total_subscribers { set; get; }

        public Folder()
        {
            id = 0;
            name = string.Empty;
            unique_subscribers = 0;
            total_subscribers = 0;
            total_blacklisted = 0;
            Listas = new List<Lista>();
        }
    }

    public class Lista
    {
         public int id { set; get; }
         public string name { set; get; }
         public int total_subscribers { set; get; }
         public int? total_blacklisted { set; get; }

        public Lista()
        {
            id = 0;
            name = string.Empty;
            total_blacklisted = 0;
            total_subscribers = 0;
        }
    }
}
