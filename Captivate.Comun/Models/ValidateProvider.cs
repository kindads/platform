using Captivate.Comun.Interfaces;
using Captivate.Comun.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models
{
    public class ValidateProvider : IValidateProvider
    {
        public HttpStatusCode StatusCode { set; get; }

        public string Message { set; get; }

        public bool IsValid { set; get; }

        public EnumMailProviders enumMailProvider { set; get; }

        public ValidateProvider(EnumMailProviders enumMailProvider)
        {
            Message = string.Empty;
            IsValid = false;
            this.enumMailProvider = enumMailProvider;
        }

        public string Validate(bool IsValid, HttpStatusCode Status)
        {
            this.StatusCode = Status;
            this.IsValid = IsValid;
            MakeMessage();
            return Message;
        }

        public void MakeMessage()
        {

            switch(enumMailProvider)
            {
                case EnumMailProviders.IContact:
                    {
                        MakeIContactMessage();
                    }break;
            }
        }

        public void MakeIContactMessage()
        {
            switch(StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    {
                        Message = "You are not logged in";
                    }break;
                case HttpStatusCode.BadRequest:
                    {
                        Message = "Your data could not be parsed or your request contained invalid data";
                    }
                    break;
                case HttpStatusCode.OK:
                    {
                        Message = "Your request was processed successfully";
                    }
                    break;
                case HttpStatusCode.PaymentRequired:
                    {
                        Message = "You must pay your iContact bill before we can process your request";
                    }
                    break;
                case HttpStatusCode.Forbidden:
                    {
                        Message = "YYou are logged in, but do not have permission to perform that action";
                    }
                    break;
                case HttpStatusCode.NotFound:
                    {
                        Message = "You have requested a resource that cannot be found";
                    }
                    break;
                case HttpStatusCode.MethodNotAllowed:
                    {
                        Message = "You cannot perform that method on the requested resource";
                    }
                    break;
                case HttpStatusCode.NotAcceptable:
                    {
                        Message = "You have requested that iContact generate data in an unsupported format. The iContact API can only return data in XML or JSON";
                    }
                    break;
                case HttpStatusCode.UnsupportedMediaType:
                    {
                        Message = "Your request was not in a supported format. You can make requests in XML or JSON.";
                    }
                    break;
                case HttpStatusCode.InternalServerError:
                    {
                        Message = "An error occurred in iContact’s code";
                    }
                    break;
                case HttpStatusCode.NotImplemented:
                    {
                        Message = "You have requested a resource that has not been implemented or you have specified an incorrect version of the iContact API";
                    }
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    {
                        Message = "You cannot perform the action because the system is experiencing extremely high traffic or you cannot perform the action because the system is down for maintenance";
                    }
                    break;
            }
        }
    }
}
