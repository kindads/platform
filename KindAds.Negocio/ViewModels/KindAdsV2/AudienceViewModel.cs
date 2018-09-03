using KindAds.Azure;
using KindAds.Common.Interfaces;
using KindAds.Comun.Models;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Negocio.Managersv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Negocio.ViewModels.KindAdsV2
{
    public class AudienceViewModel : ITelemetria
    {
        public AudienceManager manager { set; get; }
        public AudienceDocument audience { set; get; }
        public ITrace telemetria { set; get; }
        public List<AudiencePreferenceDocument> preferences { set; get; }
        public string preferencesStringify { set; get; }
        public string stage { set; get; }

        public AudienceViewModel()
        {
            manager = new AudienceManager();
            audience = new AudienceDocument();
            preferences = new List<AudiencePreferenceDocument>();
            preferencesStringify = string.Empty;
            telemetria = new Trace();
        }

        public List<ProtocolDocument> GetProtocols(string type)
        {
            List<ProtocolDocument> protocols = new List<ProtocolDocument>();
            try
            {
                if (type == "choose")
                {
                    protocols.Add(new ProtocolDocument { Id = "", Name = "Choose protocol ..." });
                    protocols.Add(new ProtocolDocument { Id = "1", Name = "https://" });
                    protocols.Add(new ProtocolDocument { Id = "2", Name = "http://" });
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return protocols;
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

        public List<GenericElement> GetYears()
        {
            //List<string> years = new List<string>();
            //try
            //{
            //    years.Add("Year");
            //    years.AddRange(manager.GetYears());
            //}
            //catch (Exception e)
            //{
            //    var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            //    telemetria.Critical(messageException);
            //}
            //return years;

            List<GenericElement> list = new List<GenericElement>();
            try
            {
                list.Add(new GenericElement { Id = "", Name = "Year" });
                foreach (var item in manager.GetYears())
                {
                    list.Add(new GenericElement { Id = item, Name = item });
                }
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return list;
        }

        public List<GenericElement> GetManyPeople()
        {

            List<GenericElement> list = new List<GenericElement>();
            try
            {
                list.Add(new GenericElement { Id = "", Name = "Choose business size" });
                foreach (var item in manager.GetManyPeople())
                {
                    list.Add(new GenericElement { Id = item, Name = item });
                } 
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }

            return list;
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
