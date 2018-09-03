using KindAds.Models.Advertiser;
using KindAds.Models.Campaign;
using KindAds.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KindAds.Utils.Security;
using KindAds.Utils.Enums;
using System.IO;
using Microsoft.AspNet.Identity;
using MailChimp.Campaigns;
using MailChimp;
using KindAds.Common.Interfaces;

using KindAds.Common.Models;
using KindAds.DataAccess;
using KindAds.Common.Models.Entities;

using KindAds.Business;
using KindAds.Azure;
using KindAds.Comun.Models;

namespace KindAds.Controllers {
    [AuthorizeRoles(Roles.Advertiser, Roles.Publisher)]
    public class CampaignController : Controller {
        private CampaignService _service;
        private SendGridService _sendGridService;
        private ActiveCampaignService _activeCampaignService;
        private GetResponseService _getResponseService;
        private AccessService _accessService;
        List<Models.CAMPAIGN_CHAT> listCampaignChats;

        public ITrace telemetria { set; get; }


        public CampaignController()
        {
            listCampaignChats = new List<Models.CAMPAIGN_CHAT>();
            _service = new CampaignService();
            _accessService = new AccessService();
            _sendGridService = new SendGridService();
            _activeCampaignService = new ActiveCampaignService();
            _getResponseService = new GetResponseService();

            telemetria = new Trace();
        }

        public async Task<ActionResult> Index(string idProduct)
        {
            ViewBag.IsModifyCampaign = false;
            FillProtocols();
            Models.PRODUCT product = new Models.PRODUCT();
            ViewBag.IdProduct = idProduct;
            if (idProduct != null && !String.IsNullOrEmpty(idProduct)) {
                Guid idProductParam = new Guid(idProduct);
                Session["ProductId"] = idProduct;
                if (!(Guid.Empty == idProductParam)) {
                    product = await _service.GetProductByIdProductAsync(idProductParam);
                    if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_SUBSCRIBERS))) {
                        GetProviderViewSubscribers();
                    }
                    else if ((product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_PUSH_CREW)))) {
                        GetProviderViewPushCrew();
                    }
                    else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_MAIL_CHIMP))) {
                        GetProviderViewMailChimp();
                    }
                    else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_CAMPAIGN_MONITOR))) {
                        GetProviderViewCampaignMonitor();
                    }
                    else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_AWEBER))) {
                        GetProviderViewAweber();
                    }
                    else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_SEND_GRID))) {
                        GetProviderSendGrid();
                    }
                    else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_ACTIVE_CAMPAIGN))) {
                        GetProviderActiveCampaign();
                    }
                    else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_GETRESPONSE))) {
                        GetProviderGetResponse();
                    }
                    else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_ICONTACT))) {
                        GetProviderIContact();
                    }
                    else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_PUSH_ENGAGE))) {
                        GetProviderPushEngage();
                    }
                    else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_SENDINBLUE))) {
                        GetProviderSendinBlue();
                    }
                    else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_ONE_SIGNAL))) {
                        GetProviderOneSignal();
                    }
                    else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_MAILJET))) {
                        GetProviderMailJet();
                    }
                    ViewBag.ProductPrice = product.Price;
                    ViewBag.Site = product.SITE.Name;
                }
                else {
                    ModelState.AddModelError("", "Error loading data");
                }
            }

            return View(new CreateCampaingModel { Product = product, StartDate = DateTime.Now });
        }

        [HttpPost]
        public ActionResult Index(CreateCampaingModel model)
        {
            try {
                ViewBag.IsModifyCampaign = false;
                bool operationResult = false;
                FillProtocols();
                string protocolSelecc = Session["protocolSelecc"]?.ToString();

                if (ModelState.IsValid) {
                    model.Campaign = new Models.CAMPAIGN();
                    model.Campaign.PRODUCT = new Models.PRODUCT();
                    model.Campaign.AspNetUser = new Models.AspNetUser();

                    //TODO QUITAR CAMPOS NO NECESARIOS EN UN FUTURO RELASE
                    model.Campaign.Name = model.TitleText;
                    model.Campaign.AdURL = ""; // Quitar
                    model.Campaign.AdImage = ""; //Quitar
                    model.Campaign.PRODUCT.IdProduct = new Guid(Session["ProductId"].ToString());
                    model.Campaign.UTM_Campaign = ""; //Quitar
                    model.Campaign.UTM_Source = ""; //Quitar
                    model.Campaign.UTM_Medium = ""; //Quitar
                    model.Campaign.AspNetUser.Email = User.Identity.Name;
                    model.Campaign.StartDate = model.StartDate;

                    model.UrlText = String.IsNullOrEmpty(model.UrlText) ? "-" :
                      (String.IsNullOrEmpty(protocolSelecc) ? "https://" : protocolSelecc) + model.UrlText;

                    if (Utils.Constants.PROVIDER_SUBSCRIBERS.Equals(TempData["TypeChannel"].ToString())) {
                        GetProviderViewSubscribers();
                        model.Campaign.AdText = model.MessageText;
                        operationResult = _service.RegisterCampaign(model.Campaign, GeneratePushNotifSettings(model), GetFileUpload());
                    }
                    else if (Utils.Constants.PROVIDER_PUSH_CREW.Equals(TempData["TypeChannel"].ToString())) {
                        GetProviderViewPushCrew();
                        model.Campaign.AdURL = model.UrlText;
                        model.Campaign.AdText = model.MessageText;
                        model.Campaign.StartDate = DateTime.Now;
                        model.Campaign.EndDate = null;
                        operationResult = _service.RegisterCampaign(model.Campaign, GeneratePushNotifSettings(model), GetFileUpload());
                    }
                    else if (Utils.Constants.PROVIDER_MAIL_CHIMP.Equals(TempData["TypeChannel"].ToString())) {
                        GetProviderViewMailChimp();
                        _service.ObtainDefaultValues(model.Campaign.PRODUCT.IdProduct, model);
                        model.Campaign.AdText = model.MessageTextHtml;
                        operationResult = _service.RegisterCampaign(model.Campaign, GenerateMailChimpSettings(model), GetFileUpload());
                    }
                    else if (Utils.Constants.PROVIDER_CAMPAIGN_MONITOR.Equals(TempData["TypeChannel"].ToString())) {
                        GetProviderViewCampaignMonitor();
                        _service.GetDataClientCampaignMonitor(model.Campaign.PRODUCT.IdProduct, model);
                        model.Campaign.AdText = model.MessageTextHtml;
                        operationResult = _service.RegisterCampaign(model.Campaign, GenerateCampaignMonitorSettings(model), GetFileUpload());
                    }
                    else if (Utils.Constants.PROVIDER_AWEBER.Equals(TempData["TypeChannel"].ToString())) {
                        GetProviderViewAweber();
                        model.Campaign.AdText = model.MessageTextHtml;
                        operationResult = _service.RegisterCampaign(model.Campaign, GenerateAweberSettings(model), GetFileUpload());
                    }
                    else if (Utils.Constants.PROVIDER_SEND_GRID.Equals(TempData["TypeChannel"].ToString())) {
                        GetProviderSendGrid();
                        model.Campaign.AdText = model.MessageTextHtml;
                        operationResult = _service.RegisterCampaign(model.Campaign, GenerateSendGridSettings(model), GetFileUpload());
                    }
                    else if (Utils.Constants.PROVIDER_ACTIVE_CAMPAIGN.Equals(TempData["TypeChannel"].ToString())) {
                        GetProviderActiveCampaign();
                        model.Campaign.AdText = model.MessageTextHtml;
                        operationResult = _service.RegisterCampaign(model.Campaign, GenerateActiveCampaignSettings(model), GetFileUpload());
                    }
                    else if (Utils.Constants.PROVIDER_GETRESPONSE.Equals(TempData["TypeChannel"].ToString())) {
                        GetProviderGetResponse();
                        model.Campaign.AdText = model.MessageTextHtml;
                        operationResult = _service.RegisterCampaign(model.Campaign, GenerateGetResponseSettings(model), GetFileUpload());
                    }
                    else if (Utils.Constants.PROVIDER_ICONTACT.Equals(TempData["TypeChannel"].ToString())) {
                        GetProviderIContact();
                        model.Campaign.AdText = model.MessageTextHtml;
                        operationResult = _service.RegisterCampaign(model.Campaign, GenerateIContactSettings(model), GetFileUpload());
                    }
                    else if (Utils.Constants.PROVIDER_SENDINBLUE.Equals(TempData["TypeChannel"].ToString())) {
                        GetProviderSendinBlue();
                        model.Campaign.AdText = model.MessageTextHtml;
                        operationResult = _service.RegisterCampaign(model.Campaign, GenerateGetResponseSettings(model), GetFileUpload());
                    }
                    else if (Utils.Constants.PROVIDER_PUSH_ENGAGE.Equals(TempData["TypeChannel"].ToString())) {
                        GetProviderPushEngage();
                        model.Campaign.AdURL = model.UrlText;
                        model.Campaign.AdText = model.MessageText;
                        model.Campaign.StartDate = DateTime.Now;
                        model.Campaign.EndDate = null;
                        operationResult = _service.RegisterCampaign(model.Campaign, GeneratePushEngageSettings(model), GetFileUpload());
                    }
                    else if (Utils.Constants.PROVIDER_ONE_SIGNAL.Equals(TempData["TypeChannel"].ToString())) {
                        GetProviderOneSignal();
                        model.Campaign.AdURL = model.UrlText;
                        model.Campaign.AdText = model.MessageText;
                        model.Campaign.StartDate = DateTime.Now;
                        model.Campaign.EndDate = null;
                        operationResult = _service.RegisterCampaign(model.Campaign, GenerateOneSignalSettings(model), GetFileUpload());
                    }
                    else if (Utils.Constants.PROVIDER_MAILJET.Equals(TempData["TypeChannel"].ToString())) {
                        GetProviderMailJet();
                        model.Campaign.AdText = model.MessageTextHtml;
                        operationResult = _service.RegisterCampaign(model.Campaign, GenerateMailJetSettings(model), GetFileUpload());
                    }

                    if (operationResult == true) {
                        //Mecanismo de notificaciones
                        string message = string.Format("The campaign '{0}' was created", model.Campaign.Name);
                        EnqueueMailNotification(model.Campaign.Name, message);
                    }

                    return operationResult ? Json(new { success = true, message = "Campaign created successfully" }) : Json(new { error = "Error creating campaign" });
                }
                else {
                    return View(model);
                }
            }
            catch (Exception ex) {
                return Json(new { error = "Error creating campaign" });
            }
        }

        private List<Models.CAMPAIGN_SETTINGS> GenerateMailChimpSettings(CreateCampaingModel model)
        {
            List<Models.CAMPAIGN_SETTINGS> listCampaignSettings = new List<Models.CAMPAIGN_SETTINGS>();

            Models.CAMPAIGN_SETTINGS campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "mailChimpMessageText",
                SettingValue = model.MessageTextHtml
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "mailChimpSubject",
                SettingValue = model.SubjectMC
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "mailChimpFromName",
                SettingValue = model.FromNameMC
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "mailChimpFromEmail",
                SettingValue = model.FromEmailMC
            };
            listCampaignSettings.Add(campaignSettings);

            return listCampaignSettings;
        }

        private List<Models.CAMPAIGN_SETTINGS> GenerateCampaignMonitorSettings(CreateCampaingModel model)
        {
            List<Models.CAMPAIGN_SETTINGS> listCampaignSettings = new List<Models.CAMPAIGN_SETTINGS>();

            Models.CAMPAIGN_SETTINGS campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "campaignMonitorMessageText",
                SettingValue = model.MessageTextHtml
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "campaignMonitorSubject",
                SettingValue = model.SubjectMC
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "campaignMonitorFromName",
                SettingValue = model.FromNameMC
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "campaignMonitorFromEmail",
                SettingValue = model.FromEmailMC
            };

            listCampaignSettings.Add(campaignSettings);

            return listCampaignSettings;
        }

        private List<Models.CAMPAIGN_SETTINGS> GenerateAweberSettings(CreateCampaingModel model)
        {
            List<Models.CAMPAIGN_SETTINGS> listCampaignSettings = new List<Models.CAMPAIGN_SETTINGS>();

            Models.CAMPAIGN_SETTINGS campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "aweberBodyHtml",
                SettingValue = model.MessageTextHtml
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "aweberSubject",
                SettingValue = model.SubjectMC
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "aweberIsArchived",
                SettingValue = "true"
            };
            listCampaignSettings.Add(campaignSettings);

            return listCampaignSettings;
        }

        private List<Models.CAMPAIGN_SETTINGS> GenerateSendGridSettings(CreateCampaingModel model)
        {
            List<Models.CAMPAIGN_SETTINGS> listCampaignSettings = new List<Models.CAMPAIGN_SETTINGS>();

            Models.CAMPAIGN_SETTINGS campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "sendGridBodyHtml",
                SettingValue = model.MessageTextHtml
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "sendGridSubject",
                SettingValue = model.Subject
            };
            listCampaignSettings.Add(campaignSettings);

            return listCampaignSettings;
        }


        private List<Models.CAMPAIGN_SETTINGS> GenerateActiveCampaignSettings(CreateCampaingModel model)
        {
            List<Models.CAMPAIGN_SETTINGS> listCampaignSettings = new List<Models.CAMPAIGN_SETTINGS>();

            Models.CAMPAIGN_SETTINGS campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "activeCampaignBodyHtml",
                SettingValue = model.MessageTextHtml
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "activeCampaignSubject",
                SettingValue = model.Subject
            };
            listCampaignSettings.Add(campaignSettings);

            return listCampaignSettings;
        }

        private List<Models.CAMPAIGN_SETTINGS> GenerateIContactSettings(CreateCampaingModel model)
        {
            List<Models.CAMPAIGN_SETTINGS> listCampaignSettings = new List<Models.CAMPAIGN_SETTINGS>();

            Models.CAMPAIGN_SETTINGS campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "iContactBodyHtml",
                SettingValue = model.MessageTextHtml
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "iContactSubject",
                SettingValue = model.Subject
            };
            listCampaignSettings.Add(campaignSettings);

            return listCampaignSettings;
        }

        private List<Models.CAMPAIGN_SETTINGS> GenerateGetResponseSettings(CreateCampaingModel model)
        {
            List<Models.CAMPAIGN_SETTINGS> listCampaignSettings = new List<Models.CAMPAIGN_SETTINGS>();

            Models.CAMPAIGN_SETTINGS campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "getResponseBodyHtml",
                SettingValue = model.MessageTextHtml
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "getResponseSubject",
                SettingValue = model.Subject
            };
            listCampaignSettings.Add(campaignSettings);

            return listCampaignSettings;
        }

        private List<Models.CAMPAIGN_SETTINGS> GenerateMailJetSettings(CreateCampaingModel model)
        {
            List<Models.CAMPAIGN_SETTINGS> listCampaignSettings = new List<Models.CAMPAIGN_SETTINGS>();

            Models.CAMPAIGN_SETTINGS campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "mailJetBodyHtml",
                SettingValue = model.MessageTextHtml
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "mailJetSubject",
                SettingValue = model.Subject
            };
            listCampaignSettings.Add(campaignSettings);

            return listCampaignSettings;
        }

        private List<Models.CAMPAIGN_SETTINGS> GeneratePushNotifSettings(CreateCampaingModel model)
        {
            List<Models.CAMPAIGN_SETTINGS> listCampaignSettings = new List<Models.CAMPAIGN_SETTINGS>();

            Models.CAMPAIGN_SETTINGS campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "pushNotifMessageText",
                SettingValue = model.MessageText
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "pushNotifUrl",
                SettingValue = model.UrlText
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "pushNotifUtmCampaign",
                SettingValue = String.IsNullOrEmpty(model.UTM_Campaign) ? "" : model.UTM_Campaign
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "pushNotifUtmMedium",
                SettingValue = String.IsNullOrEmpty(model.UTM_Medium) ? "" : model.UTM_Medium
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "pushNotifUtmSource",
                SettingValue = String.IsNullOrEmpty(model.UTM_Source) ? "" : model.UTM_Source
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "pushNotifImage",
                SettingValue = "",
            };
            listCampaignSettings.Add(campaignSettings);

            return listCampaignSettings;
        }

        private List<Models.CAMPAIGN_SETTINGS> GeneratePushEngageSettings(CreateCampaingModel model)
        {
            List<Models.CAMPAIGN_SETTINGS> listCampaignSettings = new List<Models.CAMPAIGN_SETTINGS>();

            Models.CAMPAIGN_SETTINGS campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "pushNotifMessageText",
                SettingValue = model.MessageText
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "pushNotifUrl",
                SettingValue = model.UrlText
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "pushNotifImage",
                SettingValue = "",
            };
            listCampaignSettings.Add(campaignSettings);

            return listCampaignSettings;
        }

        private List<Models.CAMPAIGN_SETTINGS> GenerateOneSignalSettings(CreateCampaingModel model)
        {
            List<Models.CAMPAIGN_SETTINGS> listCampaignSettings = new List<Models.CAMPAIGN_SETTINGS>();

            Models.CAMPAIGN_SETTINGS campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "pushNotifMessageText",
                SettingValue = model.MessageText
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "pushNotifUrl",
                SettingValue = model.UrlText
            };
            listCampaignSettings.Add(campaignSettings);

            campaignSettings = new Models.CAMPAIGN_SETTINGS() {
                SettingName = "pushNotifImage",
                SettingValue = "",
            };
            listCampaignSettings.Add(campaignSettings);

            return listCampaignSettings;
        }

        public ActionResult ModifyCampaign(string idCampaign)
        {
            ViewBag.IsModifyCampaign = true;
            Session["IdCampaign"] = idCampaign;
            CreateCampaingModel model = new CreateCampaingModel();
            FillProtocols();
            var campaign = _service.GetCampaignById(idCampaign);
            var product = campaign.PRODUCT;

            model.Campaign = campaign;
            model.TitleText = campaign.Name;
            model.MessageText = campaign.AdText;
            model.StartDate = campaign.StartDate ?? DateTime.Now;
            model.ListCampaingMessageChat = GetCampaignChat(idCampaign);

            if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_SUBSCRIBERS))) {
                GetProviderViewSubscribers();
                FillPushNotifSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_PUSH_CREW))) {
                GetProviderViewPushCrew();
                FillPushNotifSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_MAIL_CHIMP))) {
                GetProviderViewMailChimp();
                FillMailChimpSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_CAMPAIGN_MONITOR))) {
                GetProviderViewCampaignMonitor();
                FillCampaignMonitorSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_AWEBER))) {
                GetProviderViewAweber();
                FillAweberSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_SEND_GRID))) {
                GetProviderSendGrid();
                FillSendGridSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_ACTIVE_CAMPAIGN))) {
                GetProviderActiveCampaign();
                FillActiveCampaignSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_GETRESPONSE))) {
                GetProviderGetResponse();
                FillGetResponseSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_ICONTACT))) {
                GetProviderIContact();
                FillIContactSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_PUSH_ENGAGE))) {
                GetProviderPushEngage();
                FillPushNotifSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_ONE_SIGNAL))) {
                GetProviderOneSignal();
                FillPushNotifSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_MAILJET))) {
                GetProviderMailJet();
                FillMailJetSettings(model, campaign);
            }

            ViewBag.ProductPrice = product.Price;
            ViewBag.Site = product.SITE.Name;

            return View(model);
        }

        [HttpPost]
        public ActionResult ModifyCampaign(CreateCampaingModel model)
        {
            try {
                bool operationResult = false;
                ViewBag.IsModifyCampaign = true;
                FillProtocols();

                var campaign = _service.GetCampaignById(Session["IdCampaign"].ToString());
                var product = campaign.PRODUCT;
                string protocolSelecc = Session["protocolSelecc"]?.ToString();

                model.Campaign = campaign;
                model.Campaign.Name = model.TitleText;
                model.Campaign.AdText = model.MessageText;
                model.Campaign.CAT_CAMPAIGN_STATUS_IdStatus = model.IsSendRevision ? (int)CatCampaignStatusEnum.Waiting_For_Response : (int)CatCampaignStatusEnum.In_Review;


                if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_SUBSCRIBERS))) {
                    GetProviderViewSubscribers();
                    model.UrlText = String.IsNullOrEmpty(model.UrlText) ? "-" : (String.IsNullOrEmpty(protocolSelecc) ? "https://" : protocolSelecc) + model.UrlText;
                    GetPushNotifSettings(model, campaign.CAMPAIGN_SETTINGS);
                    operationResult = _service.ModifyCampaign(model.Campaign, campaign.CAMPAIGN_SETTINGS, GetFileUpload());
                }
                else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_PUSH_CREW))) {
                    GetProviderViewPushCrew();
                    model.UrlText = String.IsNullOrEmpty(model.UrlText) ? "-" : (String.IsNullOrEmpty(protocolSelecc) ? "https://" : protocolSelecc) + model.UrlText;
                    GetPushCrewSettings(model, campaign.CAMPAIGN_SETTINGS);
                    operationResult = _service.ModifyCampaign(model.Campaign, campaign.CAMPAIGN_SETTINGS, GetFileUpload());
                }
                else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_MAIL_CHIMP))) {
                    GetProviderViewMailChimp();
                    model.Campaign.AdText = model.MessageTextHtml;
                    _service.ObtainDefaultValues(model.Campaign.PRODUCT.IdProduct, model);
                    GetMailChimpSettings(model, campaign.CAMPAIGN_SETTINGS);
                    operationResult = _service.ModifyCampaign(model.Campaign, campaign.CAMPAIGN_SETTINGS, GetFileUpload());
                }
                else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_CAMPAIGN_MONITOR))) {
                    GetProviderViewCampaignMonitor();
                    model.Campaign.AdText = model.MessageTextHtml;
                    _service.GetDataClientCampaignMonitor(model.Campaign.PRODUCT.IdProduct, model);
                    GetCampaignMonitorSettings(model, campaign.CAMPAIGN_SETTINGS);
                    operationResult = _service.ModifyCampaign(model.Campaign, campaign.CAMPAIGN_SETTINGS, GetFileUpload());
                }
                else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_AWEBER))) {
                    GetProviderViewAweber();
                    model.Campaign.AdText = model.MessageTextHtml;
                    GetAweberSettings(model, campaign.CAMPAIGN_SETTINGS);
                    operationResult = _service.ModifyCampaign(model.Campaign, campaign.CAMPAIGN_SETTINGS, GetFileUpload());
                }
                else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_SEND_GRID))) {
                    GetProviderSendGrid();
                    model.Campaign.AdText = model.MessageTextHtml;
                    GetSendGridSettings(model, campaign.CAMPAIGN_SETTINGS);
                    operationResult = _service.ModifyCampaign(model.Campaign, campaign.CAMPAIGN_SETTINGS, GetFileUpload());
                }
                else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_ACTIVE_CAMPAIGN))) {
                    GetProviderActiveCampaign();
                    model.Campaign.AdText = model.MessageTextHtml;
                    GetActiveCampaignSettings(model, campaign.CAMPAIGN_SETTINGS);
                    operationResult = _service.ModifyCampaign(model.Campaign, campaign.CAMPAIGN_SETTINGS, GetFileUpload());
                }
                else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_GETRESPONSE))) {
                    GetProviderGetResponse();
                    model.Campaign.AdText = model.MessageTextHtml;
                    GetGetResponseSettings(model, campaign.CAMPAIGN_SETTINGS);
                    operationResult = _service.ModifyCampaign(model.Campaign, campaign.CAMPAIGN_SETTINGS, GetFileUpload());
                }
                else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_ICONTACT))) {
                    GetProviderIContact();
                    model.Campaign.AdText = model.MessageTextHtml;
                    GetIContactSettings(model, campaign.CAMPAIGN_SETTINGS);
                    operationResult = _service.ModifyCampaign(model.Campaign, campaign.CAMPAIGN_SETTINGS, GetFileUpload());
                }
                else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_PUSH_ENGAGE))) {
                    GetProviderPushEngage();
                    model.UrlText = String.IsNullOrEmpty(model.UrlText) ? "-" : (String.IsNullOrEmpty(protocolSelecc) ? "https://" : protocolSelecc) + model.UrlText;
                    GetPushEngageSettings(model, campaign.CAMPAIGN_SETTINGS);
                    operationResult = _service.ModifyCampaign(model.Campaign, campaign.CAMPAIGN_SETTINGS, GetFileUpload());
                }
                else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_ONE_SIGNAL))) {
                    GetProviderOneSignal();
                    model.UrlText = String.IsNullOrEmpty(model.UrlText) ? "-" : (String.IsNullOrEmpty(protocolSelecc) ? "https://" : protocolSelecc) + model.UrlText;
                    GetOneSignalSettings(model, campaign.CAMPAIGN_SETTINGS);
                    operationResult = _service.ModifyCampaign(model.Campaign, campaign.CAMPAIGN_SETTINGS, GetFileUpload());
                }
                else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_MAILJET))) {
                    GetProviderMailJet();
                    model.Campaign.AdText = model.MessageTextHtml;
                    GetMailJetSettings(model, campaign.CAMPAIGN_SETTINGS);
                    operationResult = _service.ModifyCampaign(model.Campaign, campaign.CAMPAIGN_SETTINGS, GetFileUpload());
                }

                return operationResult ? Json(new { success = true, message = "Campaign modify successfully" }) : Json(new { error = "Error modify campaign" });
            }
            catch (Exception ex) {
                return Json(new { error = "Error modify campaign" });
            }
        }

        public void EnqueueMailNotification(string CampaignName, string message)
        {
            string IdUser = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
            AspNetUserRepository aspNetUserRepository = new AspNetUserRepository();
            AspNetUserEntity userData = aspNetUserRepository.FindById(IdUser);
            //Enviamos la notificacion

            MailNotification mailNotification = new MailNotification();
            // Fill object
            MailMessage email = new MailMessage();
            Notification notification = new Notification();

            email.Body = message;
            email.Destination = userData.Email;
            email.Subject = message;

            notification.Label = NotificationLabels.EMail;
            notification.IdUser = new Guid(IdUser);
            notification.Message = message;
            notification.Title = string.Format("Campaign notification {0}", DateTime.Now);

            //Add objects to MailNotification
            mailNotification.EMail = email;
            mailNotification.notificacion = notification;

            NotificationManager notificationManager = new NotificationManager();
            notificationManager.EnqueueMailNotification(mailNotification);
        }

        public void EnqueueChatMessage(string message,string idUser)
        {           
            ChatNotification chatNotification = new ChatNotification(idUser);

            // fill object
            chatNotification.message = message;

            // enqueue chat notification
            NotificationManager notificationManager = new NotificationManager();
            notificationManager.EnqueueChatNotification(chatNotification);
        }

        public void EnqueueMailNotification(string CampaignName, string message, string IdUser)
        {
            AspNetUserRepository aspNetUserRepository = new AspNetUserRepository();
            AspNetUserEntity userData = aspNetUserRepository.FindById(IdUser);
            //Enviamos la notificacion

            MailNotification mailNotification = new MailNotification();
            // Fill object
            MailMessage email = new MailMessage();
            Notification notification = new Notification();

            email.Body = message;
            email.Destination = userData.Email;
            email.Subject = message;

            notification.Label = NotificationLabels.EMail;
            notification.IdUser = new Guid(IdUser);
            notification.Message = message;
            notification.Title = string.Format("Campaign chat {0}", DateTime.Now);

            //Add objects to MailNotification
            mailNotification.EMail = email;
            mailNotification.notificacion = notification;


            NotificationManager notificationManager = new NotificationManager();
            notificationManager.EnqueueMailNotification(mailNotification);
        }
        public JsonResult RejectCampaign(CreateCampaingModel model)
        {
            var campaign = _service.GetCampaignById(Session["IdCampaign"].ToString());
            campaign.CAT_CAMPAIGN_STATUS_IdStatus = (int)CatCampaignStatusEnum.In_Review;
            var operationResult = _service.ModifyCampaign(campaign, null, null);

            if (operationResult == true) {
                string message = string.Format("The campaign '{0}' was rejected", campaign.Name);
                EnqueueMailNotification(campaign.Name, message);
                return Json(new { success = true, message = "Campaign modify successfully" });
            }
            else {
                return Json(new { error = "Error modify campaign" });
            }
        }

        private void FillMailChimpSettings(CreateCampaingModel model, Models.CAMPAIGN campaign)
        {
            if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any()) {
                foreach (var setting in campaign.CAMPAIGN_SETTINGS) {
                    model.MessageTextHtml = setting.SettingName.Equals("mailChimpMessageText") ? setting.SettingValue : model.MessageTextHtml;
                    model.SubjectMC = setting.SettingName.Equals("mailChimpSubject") ? setting.SettingValue : model.SubjectMC;
                    model.FromNameMC = setting.SettingName.Equals("mailChimpFromName") ? setting.SettingValue : model.FromNameMC;
                    model.FromEmailMC = setting.SettingName.Equals("mailChimpFromEmail") ? setting.SettingValue : model.FromEmailMC;
                }
            }
        }

        private void FillCampaignMonitorSettings(CreateCampaingModel model, Models.CAMPAIGN campaign)
        {
            if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any()) {
                foreach (var setting in campaign.CAMPAIGN_SETTINGS) {
                    model.MessageTextHtml = setting.SettingName.Equals("campaignMonitorMessageText") ? setting.SettingValue : model.MessageTextHtml;
                    model.SubjectMC = setting.SettingName.Equals("campaignMonitorSubject") ? setting.SettingValue : model.SubjectMC;
                    model.FromNameMC = setting.SettingName.Equals("campaignMonitorFromName") ? setting.SettingValue : model.FromNameMC;
                    model.FromEmailMC = setting.SettingName.Equals("campaignMonitorFromEmail") ? setting.SettingValue : model.FromEmailMC;
                }
            }
        }

        private void FillAweberSettings(CreateCampaingModel model, Models.CAMPAIGN campaign)
        {
            if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any()) {
                foreach (var setting in campaign.CAMPAIGN_SETTINGS) {
                    model.MessageTextHtml = setting.SettingName.Equals("aweberBodyHtml") ? setting.SettingValue : model.MessageTextHtml;
                    model.SubjectMC = setting.SettingName.Equals("aweberSubject") ? setting.SettingValue : model.SubjectMC;
                    //model. = setting.SettingName.Equals("aweberIsArchived") ? setting.SettingValue : model.FromNameMC;        
                }
            }
        }

        private void FillSendGridSettings(CreateCampaingModel model, Models.CAMPAIGN campaign)
        {
            if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any()) {
                foreach (var setting in campaign.CAMPAIGN_SETTINGS) {
                    model.MessageTextHtml = setting.SettingName.Equals("sendGridBodyHtml") ? setting.SettingValue : model.MessageTextHtml;
                    model.Subject = setting.SettingName.Equals("sendGridSubject") ? setting.SettingValue : model.Subject;
                }
            }
        }

        private void FillActiveCampaignSettings(CreateCampaingModel model, Models.CAMPAIGN campaign)
        {
            if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any()) {
                foreach (var setting in campaign.CAMPAIGN_SETTINGS) {
                    model.MessageTextHtml = setting.SettingName.Equals("activeCampaignBodyHtml") ? setting.SettingValue : model.MessageTextHtml;
                    model.Subject = setting.SettingName.Equals("activeCampaignSubject") ? setting.SettingValue : model.Subject;
                }
            }
        }

        private void FillGetResponseSettings(CreateCampaingModel model, Models.CAMPAIGN campaign)
        {
            if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any()) {
                foreach (var setting in campaign.CAMPAIGN_SETTINGS) {
                    model.MessageTextHtml = setting.SettingName.Equals("getResponseBodyHtml") ? setting.SettingValue : model.MessageTextHtml;
                    model.Subject = setting.SettingName.Equals("getResponseSubject") ? setting.SettingValue : model.Subject;
                }
            }
        }

        private void FillIContactSettings(CreateCampaingModel model, Models.CAMPAIGN campaign)
        {
            if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any()) {
                foreach (var setting in campaign.CAMPAIGN_SETTINGS) {
                    model.MessageTextHtml = setting.SettingName.Equals("iContactBodyHtml") ? setting.SettingValue : model.MessageTextHtml;
                    model.Subject = setting.SettingName.Equals("iContactSubject") ? setting.SettingValue : model.Subject;
                }
            }
        }

        private void FillPushNotifSettings(CreateCampaingModel model, Models.CAMPAIGN campaign)
        {
            if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any()) {
                foreach (var setting in campaign.CAMPAIGN_SETTINGS) {
                    model.UrlText = setting.SettingName.Equals("pushNotifUrl") ? GetUrlWithoutProtocol(setting.SettingValue) : model.UrlText;
                    model.MessageText = setting.SettingName.Equals("pushNotifMessageText") ? setting.SettingValue : model.MessageText;
                    model.UTM_Source = setting.SettingName.Equals("pushNotifUtmSource") ? setting.SettingValue : model.UTM_Source;
                    model.UTM_Medium = setting.SettingName.Equals("pushNotifUtmMedium") ? setting.SettingValue : model.UTM_Medium;
                    model.UTM_Campaign = setting.SettingName.Equals("pushNotifUtmCampaign") ? setting.SettingValue : model.UTM_Campaign;
                    model.Image = setting.SettingName.Equals("pushNotifImage") ? setting.SettingValue : model.Image;
                    campaign.AdImage = model.Image;
                }
            }
        }

        private void FillMailJetSettings(CreateCampaingModel model, Models.CAMPAIGN campaign)
        {
            if (campaign.CAMPAIGN_SETTINGS != null && campaign.CAMPAIGN_SETTINGS.Any()) {
                foreach (var setting in campaign.CAMPAIGN_SETTINGS) {
                    model.MessageTextHtml = setting.SettingName.Equals("mailJetBodyHtml") ? setting.SettingValue : model.MessageTextHtml;
                    model.Subject = setting.SettingName.Equals("mailJetSubject") ? setting.SettingValue : model.Subject;
                }
            }
        }

        private void GetMailChimpSettings(CreateCampaingModel model, ICollection<Models.CAMPAIGN_SETTINGS> listCampaignSettings)
        {
            if (listCampaignSettings != null && listCampaignSettings.Any()) {
                foreach (var setting in listCampaignSettings) {
                    setting.SettingValue = setting.SettingName.Equals("mailChimpMessageText") ? model.MessageTextHtml : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("mailChimpSubject") ? model.SubjectMC : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("mailChimpFromName") ? model.FromNameMC : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("mailChimpFromEmail") ? model.FromEmailMC : setting.SettingValue;
                }
            }
        }

        private void GetCampaignMonitorSettings(CreateCampaingModel model, ICollection<Models.CAMPAIGN_SETTINGS> listCampaignSettings)
        {
            if (listCampaignSettings != null && listCampaignSettings.Any()) {
                foreach (var setting in listCampaignSettings) {
                    setting.SettingValue = setting.SettingName.Equals("campaignMonitorMessageText") ? model.MessageTextHtml : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("campaignMonitorSubject") ? model.SubjectMC : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("campaignMonitorFromName") ? model.FromNameMC : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("campaignMonitorFromEmail") ? model.FromEmailMC : setting.SettingValue;
                }
            }
        }

        private void GetAweberSettings(CreateCampaingModel model, ICollection<Models.CAMPAIGN_SETTINGS> listCampaignSettings)
        {
            if (listCampaignSettings != null && listCampaignSettings.Any()) {
                foreach (var setting in listCampaignSettings) {
                    setting.SettingValue = setting.SettingName.Equals("aweberBodyHtml") ? model.MessageTextHtml : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("aweberSubject") ? model.SubjectMC : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("aweberIsArchived") ? "true" : setting.SettingValue;
                }
            }
        }

        private void GetSendGridSettings(CreateCampaingModel model, ICollection<Models.CAMPAIGN_SETTINGS> listCampaignSettings)
        {
            if (listCampaignSettings != null && listCampaignSettings.Any()) {
                foreach (var setting in listCampaignSettings) {
                    setting.SettingValue = setting.SettingName.Equals("sendGridBodyHtml") ? model.MessageTextHtml : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("sendGridSubject") ? model.Subject : setting.SettingValue;
                }
            }
        }

        private void GetIContactSettings(CreateCampaingModel model, ICollection<Models.CAMPAIGN_SETTINGS> listCampaignSettings)
        {
            if (listCampaignSettings != null && listCampaignSettings.Any()) {
                foreach (var setting in listCampaignSettings) {
                    setting.SettingValue = setting.SettingName.Equals("iContactBodyHtml") ? model.MessageTextHtml : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("iContactSubject") ? model.Subject : setting.SettingValue;
                }
            }
        }

        private void GetActiveCampaignSettings(CreateCampaingModel model, ICollection<Models.CAMPAIGN_SETTINGS> listCampaignSettings)
        {
            if (listCampaignSettings != null && listCampaignSettings.Any()) {
                foreach (var setting in listCampaignSettings) {
                    setting.SettingValue = setting.SettingName.Equals("activeCampaignBodyHtml") ? model.MessageTextHtml : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("activeCampaignSubject") ? model.Subject : setting.SettingValue;
                }
            }
        }

        private void GetGetResponseSettings(CreateCampaingModel model, ICollection<Models.CAMPAIGN_SETTINGS> listCampaignSettings)
        {
            if (listCampaignSettings != null && listCampaignSettings.Any()) {
                foreach (var setting in listCampaignSettings) {
                    setting.SettingValue = setting.SettingName.Equals("getResponseBodyHtml") ? model.MessageTextHtml : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("getResponseSubject") ? model.Subject : setting.SettingValue;
                }
            }
        }

        private void GetPushNotifSettings(CreateCampaingModel model, ICollection<Models.CAMPAIGN_SETTINGS> listCampaignSettings)
        {
            if (listCampaignSettings != null && listCampaignSettings.Any()) {
                foreach (var setting in listCampaignSettings) {
                    setting.SettingValue = setting.SettingName.Equals("pushNotifUrl") ? model.UrlText : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("pushNotifMessageText") ? model.MessageText : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("pushNotifUtmSource") ? model.UTM_Source : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("pushNotifUtmMedium") ? model.UTM_Medium : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("pushNotifUtmCampaign") ? model.UTM_Campaign : setting.SettingValue;
                }
            }
        }

        private void GetPushCrewSettings(CreateCampaingModel model, ICollection<Models.CAMPAIGN_SETTINGS> listCampaignSettings)
        {
            if (listCampaignSettings != null && listCampaignSettings.Any()) {
                foreach (var setting in listCampaignSettings) {
                    setting.SettingValue = setting.SettingName.Equals("pushNotifUrl") ? model.UrlText : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("pushNotifMessageText") ? model.MessageText : setting.SettingValue;
                    //setting.SettingValue = setting.SettingName.Equals("pushNotifUtmSource") ? model.UTM_Source : setting.SettingValue;
                    //setting.SettingValue = setting.SettingName.Equals("pushNotifUtmMedium") ? model.UTM_Medium : setting.SettingValue;
                    //setting.SettingValue = setting.SettingName.Equals("pushNotifUtmCampaign") ? model.UTM_Campaign : setting.SettingValue;
                }
            }
        }

        private void GetPushEngageSettings(CreateCampaingModel model, ICollection<Models.CAMPAIGN_SETTINGS> listCampaignSettings)
        {
            if (listCampaignSettings != null && listCampaignSettings.Any()) {
                foreach (var setting in listCampaignSettings) {
                    setting.SettingValue = setting.SettingName.Equals("pushNotifUrl") ? model.UrlText : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("pushNotifMessageText") ? model.MessageText : setting.SettingValue;
                }
            }
        }

        private void GetOneSignalSettings(CreateCampaingModel model, ICollection<Models.CAMPAIGN_SETTINGS> listCampaignSettings)
        {
            if (listCampaignSettings != null && listCampaignSettings.Any()) {
                foreach (var setting in listCampaignSettings) {
                    setting.SettingValue = setting.SettingName.Equals("pushNotifUrl") ? model.UrlText : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("pushNotifMessageText") ? model.MessageText : setting.SettingValue;
                }
            }
        }

        private void GetMailJetSettings(CreateCampaingModel model, ICollection<Models.CAMPAIGN_SETTINGS> listCampaignSettings)
        {
            if (listCampaignSettings != null && listCampaignSettings.Any()) {
                foreach (var setting in listCampaignSettings) {
                    setting.SettingValue = setting.SettingName.Equals("mailJetBodyHtml") ? model.MessageTextHtml : setting.SettingValue;
                    setting.SettingValue = setting.SettingName.Equals("mailJetSubject") ? model.Subject : setting.SettingValue;
                }
            }
        }

        private static string GetUrlWithoutProtocol(string url)
        {
            try {
                Uri uri = new Uri(url);
                return uri.Host + uri.PathAndQuery + uri.Fragment;
            }
            catch (Exception) {
                return url;
            }

        }

        private Models.Core.FileUpload GetFileUpload()
        {
            Models.Core.FileUpload fileUpload = null;
            for (int i = 0; i < Request.Files.Count; i++) {
                fileUpload = new Models.Core.FileUpload();
                var file = Request.Files[i];
                if (file != null && !String.IsNullOrEmpty(file.FileName)) {
                    fileUpload.Filextension = Helpers.FileHelper.GetFileExtension(file.FileName);

                    if (file != null && file.ContentLength > 0) {
                        using (var binaryReader = new BinaryReader(file.InputStream)) {
                            fileUpload.FileData = binaryReader.ReadBytes(file.ContentLength);
                        }
                    }
                }
            }
            return fileUpload;
        }

        [AuthorizeRoles(Roles.Publisher, Roles.Advertiser)]
        public ActionResult SortCampaigns()
        {
            return View();
        }

        public ActionResult GetTableCampaignsPending(int page = 1, string sort = "RegisterDate", string sortdir = "DESC")
        {
            string idUser = IdentityExtensions.GetUserId(User.Identity);
            return PartialView("_TableCampaignPending", _service.GetTableCampaignPending(GetRoleUser(), idUser, page, sort, sortdir));
        }

        public ActionResult GetTableCampaignsVerify(int page = 1, string sort = "RegisterDate", string sortdir = "DESC")
        {
            string idUser = IdentityExtensions.GetUserId(User.Identity);
            return PartialView("_TableCampaignVerify", _service.GetTableCampaignVerify(GetRoleUser(), idUser, page, sort, sortdir));
        }

        private int GetRoleUser()
        {
            var idUser = IdentityExtensions.GetUserId(User.Identity);
            if (_accessService.ContainRoleUser(idUser, (int)Roles.Advertiser)) {
                return (int)Roles.Advertiser;
            }
            else if (_accessService.ContainRoleUser(idUser, (int)Roles.Publisher)) {
                return (int)Roles.Publisher;
            }
            return 0;
        }

        [HttpPost]
        public ActionResult RedirectSortCampaigns()
        {
            return RedirectToAction("Index", "Home");
        }

        public PartialViewResult ShowDetailCampaign(string idCampaign)
        {
            var products = _service.GetProductsByIdCampaign(new Guid(idCampaign));
            return PartialView("_DetailCampaign", products);
        }

        public string GetAdvertiserWallet(Guid IdCampaign)
        {
            string wallet = string.Empty;
            CampaignManager manager = new CampaignManager();
            wallet = manager.GetAdvertiserWallet(IdCampaign);
            return wallet;
        }

        public ActionResult VerifyCampaign(string idCampaign, bool toValidate)
        {
            Session["IdCampaign"] = idCampaign;
            ViewBag.toValidate = toValidate;
            CreateCampaingModel model = new CreateCampaingModel();
            var campaign = _service.GetCampaignById(idCampaign);
            var product = campaign.PRODUCT;

            model.UserWallet = GetAdvertiserWallet(new Guid(idCampaign));
            model.IdCampaign = new Guid(idCampaign);
            model.Campaign = campaign;
            model.TitleText = campaign.Name;
            model.MessageText = campaign.AdText;
            model.ListCampaingMessageChat = GetCampaignChat(idCampaign);

            if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_SUBSCRIBERS))) {
                GetProviderViewSubscribers();
                FillPushNotifSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_PUSH_CREW))) {
                GetProviderViewPushCrew();
                FillPushNotifSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_MAIL_CHIMP))) {
                GetProviderViewMailChimp();
                FillMailChimpSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_CAMPAIGN_MONITOR))) {
                GetProviderViewCampaignMonitor();
                FillCampaignMonitorSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_AWEBER))) {
                GetProviderViewAweber();
                FillAweberSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_SEND_GRID))) {
                GetProviderSendGrid();
                FillSendGridSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_ACTIVE_CAMPAIGN))) {
                GetProviderActiveCampaign();
                FillActiveCampaignSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_GETRESPONSE))) {
                GetProviderGetResponse();
                FillGetResponseSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_ICONTACT))) {
                GetProviderIContact();
                FillIContactSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_SENDINBLUE))) {
                GetProviderSendinBlue();
                FillIContactSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_PUSH_ENGAGE))) {
                GetProviderPushEngage();
                FillPushNotifSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_ONE_SIGNAL))) {
                GetProviderOneSignal();
                FillPushNotifSettings(model, campaign);
            }
            else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_MAILJET))) {
                GetProviderMailJet();
                FillMailJetSettings(model, campaign);
            }

            var abi = Resources.abi;
            //var str = System.Text.Encoding.Default.GetString(abi);
            var Abistr = new string(abi.Select(Convert.ToChar).ToArray());
            model.Abistr = Abistr;

            ViewBag.ProductPrice = product.Price;
            model.Product = new Models.PRODUCT();
            model.Product.Price = product.Price;
            ViewBag.Site = product.SITE.Name;
            ViewBag.CampaignStatus = campaign.CAT_CAMPAIGN_STATUS_IdStatus.HasValue ? campaign.CAT_CAMPAIGN_STATUS_IdStatus.Value : 0;
            return View(model);
        }

        // Approve campaign
        public JsonResult ValidateCampaign(string idCampaign)
        {
            bool operationResult = false;
            CreateCampaingModel model = new CreateCampaingModel();

            try {
                if (!String.IsNullOrEmpty(idCampaign)) {
                    var campaign = _service.GetCampaignById(idCampaign);
                    var product = campaign.PRODUCT;

                    if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_SUBSCRIBERS))) {
                        FillPushNotifSettings(model, campaign);
                    }
                    else if (product.PARTNER.IdPartner.Equals(new Guid(Utils.Constants.PROVIDER_PUSH_CREW))) {
                        FillPushNotifSettings(model, campaign);
                    }

                    // Creamos el objeto de notificacion
                    Notification notification = new Notification();
                    NotificationManager notificationManager = new NotificationManager();

                    // Enviamos la notificacion para la validacion
                    notification.IdCampaignExternal = campaign.IdCampaign.ToString();
                    notification.IdUser = new Guid(Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity));

                    // Encolamos la notificacion para que el webjob la procese
                    notificationManager.EnqueueCampaignValidator(notification);

                    //Regresamos el mensaje de procesando
                    campaign.CAT_CAMPAIGN_STATUS_IdStatus = (int)KindAds.Comun.Enums.CatCampaignStatusEnum.Verify_Proccess;
                    return Json(new { success = _service.ModifyCampaign(campaign, null, null), message = "Processing campaign" });
                }
            }
            catch (Exception ex) {
                var messageException = telemetria.MakeMessageException(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
                return ResponseError();
            }

            return operationResult ? ResponseSuccess() : ResponseError();
        }


        public string GetIdProductOwner(string IdCampaign)
        {
            string IdOwner = string.Empty;
            CampaignRepository repositorio = new CampaignRepository();
            CampaignEntity campaign = repositorio.FindById(new Guid(IdCampaign));
            Guid productId = campaign.PRODUCT_IdProduct;
            ProductRepository productRepository = new ProductRepository();
            ProductEntity product = productRepository.FindById(productId);
            IdOwner = product.AspNetUsers_Id;
            return IdOwner;
        }

        public PartialViewResult GetCampaignsChat(string message)
        {
            string idCampaign = Session["IdCampaign"].ToString();

            try {
                if (message != null) {
                    //se obtiene los mensajes actuales                   
                    listCampaignChats = _service.GetCampaignChatByIdCampaign(idCampaign);

                    // Se ingresa un nuevo mensaje de chat                   
                    Models.CAMPAIGN_CHAT cm = new Models.CAMPAIGN_CHAT() {
                        AspNetUser_IdCreator = IdentityExtensions.GetUserId(User.Identity),
                        CampaignChatMessage = message,
                        CAMPAIGN_IdCampaign = new Guid(idCampaign)
                    };
                    _service.RegisterCampaignMessage(cm);

                    CampaignRepository repositorio = new CampaignRepository();
                    CampaignEntity campaign = repositorio.FindById(new Guid(idCampaign));
                    string IdProductOwner = GetIdProductOwner(idCampaign);

                    //Envio mensaje al usuario que creo el producto
                    EnqueueChatMessage(message, IdProductOwner);

                    //Envio mensaje al usuario que creo la campaña                    
                    EnqueueChatMessage(message, campaign.AspNetUser_Id);

                    return PartialView("_CampaignChat", listCampaignChats);
                }
                return PartialView("_CampaignChat", null);
            }
            catch (Exception ex) {
                return PartialView("_CampaignChat", null);
            }
        }

        private JsonResult ResponseSuccess()
        {
            return Json(new { success = true });
        }

        private JsonResult ResponseError()
        {
            return Json(new { success = false, error = true });
        }

        private List<string> FillProtocols()
        {
            List<string> listProtocol = new List<string>();
            listProtocol.Add("https://");
            listProtocol.Add("http://");
            ViewData["protocols"] = new SelectList(listProtocol);
            return listProtocol;
        }

        public void ProtocolSelecc(string id)
        {
            Session["ProtocolSelecc"] = id;
        }

        private List<Models.CAMPAIGN_CHAT> GetCampaignChat(string idCampaign)
        {
            return _service.GetCampaignChatByIdCampaign(idCampaign);
        }

        private void GetProviderViewSubscribers()
        {
            TempData["TypeChannel"] = Utils.Constants.PROVIDER_SUBSCRIBERS;
            ViewBag.IsChannelSuscribers = true;
            ViewBag.IsChannelPushCrew = false;
            ViewBag.IsChannelMailChimp = false;
            ViewBag.IsChannelCampaignMonitor = false;
            ViewBag.IsChannelAweber = false;
            ViewBag.IsChannelSendGrid = false;
            ViewBag.IsChannelActiveCampaign = false;
            ViewBag.IsChannelGetResponse = false;
            ViewBag.NeedsUTM = true;
            ViewBag.IsChannelIContact = false;
            ViewBag.IsChannelSendinBlue = false;
            ViewBag.IsChannelPushEngage = false;
            ViewBag.IsChannelOneSignal = false;
            ViewBag.IsChannelMailJet = false;
        }

        private void GetProviderViewPushCrew()
        {
            TempData["TypeChannel"] = Utils.Constants.PROVIDER_PUSH_CREW;
            ViewBag.IsChannelSuscribers = false;
            ViewBag.IsChannelPushCrew = true;
            ViewBag.IsChannelMailChimp = false;
            ViewBag.IsChannelCampaignMonitor = false;
            ViewBag.IsChannelAweber = false;
            ViewBag.IsChannelSendGrid = false;
            ViewBag.IsChannelActiveCampaign = false;
            ViewBag.IsChannelGetResponse = false;
            ViewBag.NeedsUTM = false;
            ViewBag.IsChannelIContact = false;
            ViewBag.IsChannelPushEngage = false;
            ViewBag.IsChannelSendinBlue = false;
            ViewBag.IsChannelOneSignal = false;
            ViewBag.IsChannelMailJet = false;
        }

        private void GetProviderViewMailChimp()
        {
            TempData["TypeChannel"] = Utils.Constants.PROVIDER_MAIL_CHIMP;
            ViewBag.IsChannelSuscribers = false;
            ViewBag.IsChannelPushCrew = false;
            ViewBag.IsChannelMailChimp = true;
            ViewBag.IsChannelCampaignMonitor = false;
            ViewBag.IsChannelAweber = false;
            ViewBag.IsChannelSendGrid = false;
            ViewBag.IsChannelActiveCampaign = false;
            ViewBag.IsChannelGetResponse = false;
            ViewBag.NeedsUTM = false;
            ViewBag.IsChannelIContact = false;
            ViewBag.IsChannelPushEngage = false;
            ViewBag.IsChannelSendinBlue = false;
            ViewBag.IsChannelOneSignal = false;
            ViewBag.IsChannelMailJet = false;
        }

        private void GetProviderViewCampaignMonitor()
        {
            TempData["TypeChannel"] = Utils.Constants.PROVIDER_CAMPAIGN_MONITOR;
            ViewBag.IsChannelSuscribers = false;
            ViewBag.IsChannelPushCrew = false;
            ViewBag.IsChannelMailChimp = false;
            ViewBag.IsChannelCampaignMonitor = true;
            ViewBag.IsChannelAweber = false;
            ViewBag.IsChannelSendGrid = false;
            ViewBag.IsChannelActiveCampaign = false;
            ViewBag.IsChannelGetResponse = false;
            ViewBag.NeedsUTM = false;
            ViewBag.IsChannelIContact = false;
            ViewBag.IsChannelPushEngage = false;
            ViewBag.IsChannelSendinBlue = false;
            ViewBag.IsChannelOneSignal = false;
            ViewBag.IsChannelMailJet = false;
        }

        private void GetProviderViewAweber()
        {
            TempData["TypeChannel"] = Utils.Constants.PROVIDER_AWEBER;
            ViewBag.IsChannelSuscribers = false;
            ViewBag.IsChannelPushCrew = false;
            ViewBag.IsChannelMailChimp = false;
            ViewBag.IsChannelCampaignMonitor = false;
            ViewBag.IsChannelAweber = true;
            ViewBag.IsChannelSendGrid = false;
            ViewBag.IsChannelActiveCampaign = false;
            ViewBag.IsChannelGetResponse = false;
            ViewBag.NeedsUTM = false;
            ViewBag.IsChannelIContact = false;
            ViewBag.IsChannelPushEngage = false;
            ViewBag.IsChannelSendinBlue = false;
            ViewBag.IsChannelOneSignal = false;
            ViewBag.IsChannelMailJet = false;
        }

        private void GetProviderSendGrid()
        {
            TempData["TypeChannel"] = Utils.Constants.PROVIDER_SEND_GRID;
            ViewBag.IsChannelSuscribers = false;
            ViewBag.IsChannelPushCrew = false;
            ViewBag.IsChannelMailChimp = false;
            ViewBag.IsChannelCampaignMonitor = false;
            ViewBag.IsChannelAweber = false;
            ViewBag.IsChannelSendGrid = true;
            ViewBag.IsChannelActiveCampaign = false;
            ViewBag.IsChannelGetResponse = false;
            ViewBag.NeedsUTM = false;
            ViewBag.IsChannelIContact = false;
            ViewBag.IsChannelPushEngage = false;
            ViewBag.IsChannelSendinBlue = false;
            ViewBag.IsChannelOneSignal = false;
            ViewBag.IsChannelMailJet = false;
        }

        private void GetProviderActiveCampaign()
        {
            TempData["TypeChannel"] = Utils.Constants.PROVIDER_ACTIVE_CAMPAIGN;
            ViewBag.IsChannelSuscribers = false;
            ViewBag.IsChannelPushCrew = false;
            ViewBag.IsChannelMailChimp = false;
            ViewBag.IsChannelCampaignMonitor = false;
            ViewBag.IsChannelAweber = false;
            ViewBag.IsChannelSendGrid = false;
            ViewBag.IsChannelActiveCampaign = true;
            ViewBag.IsChannelGetResponse = false;
            ViewBag.NeedsUTM = false;
            ViewBag.IsChannelPushEngage = false;
            ViewBag.IsChannelSendinBlue = false;
            ViewBag.IsChannelOneSignal = false;
            ViewBag.IsChannelMailJet = false;
        }

        private void GetProviderGetResponse()
        {
            TempData["TypeChannel"] = Utils.Constants.PROVIDER_GETRESPONSE;
            ViewBag.IsChannelSuscribers = false;
            ViewBag.IsChannelPushCrew = false;
            ViewBag.IsChannelMailChimp = false;
            ViewBag.IsChannelCampaignMonitor = false;
            ViewBag.IsChannelAweber = false;
            ViewBag.IsChannelSendGrid = false;
            ViewBag.IsChannelActiveCampaign = false;
            ViewBag.IsChannelGetResponse = true;
            ViewBag.NeedsUTM = false;
            ViewBag.IsChannelIContact = false;
            ViewBag.IsChannelPushEngage = false;
            ViewBag.IsChannelSendinBlue = false;
            ViewBag.IsChannelOneSignal = false;
            ViewBag.IsChannelMailJet = false;
        }

        private void GetProviderIContact()
        {
            TempData["TypeChannel"] = Utils.Constants.PROVIDER_ICONTACT;
            ViewBag.IsChannelSuscribers = false;
            ViewBag.IsChannelPushCrew = false;
            ViewBag.IsChannelMailChimp = false;
            ViewBag.IsChannelCampaignMonitor = false;
            ViewBag.IsChannelAweber = false;
            ViewBag.IsChannelSendGrid = false;
            ViewBag.IsChannelActiveCampaign = false;
            ViewBag.NeedsUTM = false;
            ViewBag.IsChannelIContact = true;
            ViewBag.IsChannelGetResponse = false;
            ViewBag.IsChannelSendinBlue = false;
            ViewBag.IsChannelPushEngage = false;
            ViewBag.IsChannelOneSignal = false;
            ViewBag.IsChannelMailJet = false;
        }

        private void GetProviderSendinBlue()
        {
            TempData["TypeChannel"] = Utils.Constants.PROVIDER_SENDINBLUE;
            ViewBag.IsChannelSuscribers = false;
            ViewBag.IsChannelPushCrew = false;
            ViewBag.IsChannelMailChimp = false;
            ViewBag.IsChannelCampaignMonitor = false;
            ViewBag.IsChannelAweber = false;
            ViewBag.IsChannelSendGrid = false;
            ViewBag.IsChannelActiveCampaign = false;
            ViewBag.NeedsUTM = false;
            ViewBag.IsChannelIContact = false;
            ViewBag.IsChannelGetResponse = false;
            ViewBag.IsChannelSendinBlue = true;
            ViewBag.IsChannelPushEngage = false;
            ViewBag.IsChannelOneSignal = false;
            ViewBag.IsChannelMailJet = false;
        }

        private void GetProviderPushEngage()
        {
            TempData["TypeChannel"] = Utils.Constants.PROVIDER_PUSH_ENGAGE;
            ViewBag.IsChannelSuscribers = false;
            ViewBag.IsChannelPushCrew = false;
            ViewBag.IsChannelMailChimp = false;
            ViewBag.IsChannelCampaignMonitor = false;
            ViewBag.IsChannelAweber = false;
            ViewBag.IsChannelSendGrid = false;
            ViewBag.IsChannelActiveCampaign = false;
            ViewBag.NeedsUTM = false;
            ViewBag.IsChannelIContact = false;
            ViewBag.IsChannelGetResponse = false;
            ViewBag.IsChannelSendinBlue = false;
            ViewBag.IsChannelPushEngage = true;
            ViewBag.IsChannelOneSignal = false;
            ViewBag.IsChannelMailJet = false;
        }

        private void GetProviderOneSignal()
        {
            TempData["TypeChannel"] = Utils.Constants.PROVIDER_ONE_SIGNAL;
            ViewBag.IsChannelSuscribers = false;
            ViewBag.IsChannelPushCrew = false;
            ViewBag.IsChannelMailChimp = false;
            ViewBag.IsChannelCampaignMonitor = false;
            ViewBag.IsChannelAweber = false;
            ViewBag.IsChannelSendGrid = false;
            ViewBag.IsChannelActiveCampaign = false;
            ViewBag.NeedsUTM = false;
            ViewBag.IsChannelIContact = false;
            ViewBag.IsChannelGetResponse = false;
            ViewBag.IsChannelSendinBlue = false;
            ViewBag.IsChannelPushEngage = false;
            ViewBag.IsChannelOneSignal = true;
            ViewBag.IsChannelMailJet = false;
        }

        private void GetProviderMailJet()
        {
            TempData["TypeChannel"] = Utils.Constants.PROVIDER_MAILJET;
            ViewBag.IsChannelSuscribers = false;
            ViewBag.IsChannelPushCrew = false;
            ViewBag.IsChannelMailChimp = false;
            ViewBag.IsChannelCampaignMonitor = false;
            ViewBag.IsChannelAweber = false;
            ViewBag.IsChannelSendGrid = false;
            ViewBag.IsChannelActiveCampaign = false;
            ViewBag.NeedsUTM = false;
            ViewBag.IsChannelIContact = false;
            ViewBag.IsChannelGetResponse = false;
            ViewBag.IsChannelSendinBlue = false;
            ViewBag.IsChannelPushEngage = false;
            ViewBag.IsChannelOneSignal = false;
            ViewBag.IsChannelMailJet = true;
        }

    }
}
