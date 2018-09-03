using KindAds.Azure;
using KindAds.Common.Interfaces;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Negocio.Managersv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KindAds.Comun.Models.ViewModel.KindAdsV2
{
    public class AdvertiserProfileViewModel : ITelemetria
    {
        public AdvertiserProfileManager manager { set; get; }
        public AdvertiserProfileDocument profile { set; get; }
        public List<AdvertiserPreferenceDocument> preferences { set; get; }

        public string preferencesStringify { set; get; }

        public ITrace telemetria { set; get; }

        public AdvertiserProfileViewModel()
        {
            manager = new AdvertiserProfileManager();
            profile = new AdvertiserProfileDocument();
            preferences = new List<AdvertiserPreferenceDocument>();
            preferencesStringify = string.Empty;
            telemetria = new Trace();
        }


        public List<IndustryDocument> GetIndustries(string type)
        {
            List<IndustryDocument> industries = new List<IndustryDocument>();
            try
            {
                if (type == "choose")
                {
                    industries.Add(new IndustryDocument { Id = "", Name = "Choose category ..." });
                    industries.AddRange(manager.GetIndustries());
                }
                else
                {
                    industries.AddRange(manager.GetIndustries());
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return industries;
        }

        public List<SelectListItem> GetYears()
        {
            List<SelectListItem> years = new List<SelectListItem>();
            try
            {
                years.Add(new SelectListItem() {
                    Text= "Choose Year",Value=""
                });
                years.AddRange(manager.GetYears().Select(y => new SelectListItem {
                    Text = y,
                    Value = y
                }));
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return years;
        }

        public List<SelectListItem> GetManyPeople()
        {
            List<SelectListItem> many = new List<SelectListItem>();
            try
            {
                many.Add(new SelectListItem
                {
                    Text= "Choose business size",
                    Value=""
                });
                many.AddRange(manager.GetManyPeople().Select(m => new SelectListItem {
                    Text = m,
                    Value=m
                }));
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return many;
        }

        public List<BusinessExpertiseDocument> GetBusinessExpertise()
        {
            List<BusinessExpertiseDocument> expertices = new List<BusinessExpertiseDocument>();
            try
            {
                expertices.Add(new BusinessExpertiseDocument { Id = "", Name = "Choose experience level" });
                expertices.AddRange(manager.GetBusinessExpertise());
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return expertices;
        }

        public List<CountryDocument> GetCountries()
        {
            List<CountryDocument> countries = new List<CountryDocument>();
            try
            {
                countries.Add(new CountryDocument { Id = "", Name = "Choose country" });
                countries.AddRange(manager.GetCountries());
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return countries;
        }
    }
}
