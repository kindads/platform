using Captivate.Comun.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Captivate.Negocio.Partners.IContact
{
  public class IContactGetListResponse : IResponse
  {
    public string Id { set; get; }
    public HttpStatusCode StatusCode { set; get; }

    public string listId { set; get; }

    public string name { set; get; }

    public string publicname { set; get; }

    public string description { set; get; }

    public string emailOwnerOnChange { set; get; }

    public string welcomeOnManualAdd { set; get; }

    public string welcomeOnSignupAdd { set; get; }

    public string welcomeMessageId { set; get; }
  }
}
