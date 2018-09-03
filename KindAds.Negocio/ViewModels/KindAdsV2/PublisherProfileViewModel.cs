using KindAds.Azure;
using KindAds.Common.Interfaces;
using KindAds.Common.Utils;
using KindAds.Comun.Models.CosmosDocuments.KindAdsV2;
using KindAds.Negocio.Managersv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Comun.Models.ViewModel.KindAdsV2
{
    public class PublisherProfileViewModel : ITelemetria
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid Country { get; set; }
        public Guid ExperienceLevel { get; set; }
        public List<PublisherPreferenceDocument> preferences { set; get; }
        public List<FileUpload> Files { get; set; }

        public PublisherProfileDocument profile { set; get; }

        public string preferencesStringify { set; get; }

        public PublisherProfileManager manager { set; get; }
        public ITrace telemetria { set; get; }

        public PublisherProfileViewModel()
        {
            preferencesStringify = string.Empty;
            profile = new PublisherProfileDocument();
            manager = new PublisherProfileManager();
            telemetria = new Trace();
        }

        public List<IndustryDocument> GetIndustries()
        {
            List<IndustryDocument> industries = new List<IndustryDocument>();
            try
            {
                industries = manager.GetCatIndustry();
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return industries;
        }
    }
}
