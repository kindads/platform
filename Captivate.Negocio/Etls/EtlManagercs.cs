using Captivate.Azure;
using Captivate.Common.Interfaces;
using Captivate.Comun.Interfaces;
using Captivate.Comun.Models;
using Captivate.Comun.Models.AdsMonetization;
using Captivate.Comun.Models.AdsMonetization.GeolocalizationCharts;
using Captivate.Comun.Models.CosmosDocuments;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Negocio.Etls
{   

    public class EtlGeneric<T>  : IEtlManager, ITelemetria  where T : CosmosDbDocument,  new()
    {

        #region properties
        public ITrace telemetria { set; get; }

        public List<T> documents { set; get; }

        public string DatabaseName { set; get; }

        public string CollectionName { set; get; }

        #endregion


        #region Get All Distinct
        public List<string> GetDistincts(List<T> documents, string propertie)
        {
            List<string> distincts = new List<string>();
            string document = string.Empty;
            int i = 0;

            foreach (var d in documents)
            {
                PropertyInfo propertyInfo = d.GetType().GetProperty(propertie);
                string value = propertyInfo.GetValue(d, null).ToString();

                if (document != value)
                {
                    if (i == 0)
                    {
                        i++;
                    }
                    else
                    {
                        distincts.Add(document);
                        i++;
                    }
                    document = value;
                }
            }
            distincts.Add(document);

            //switch (type)
            //{
            //    case "IP":
            //        {
            //            string Ip = string.Empty;
            //            int i = 0;

            //            foreach (var info in documents)
            //            {
            //                if (Ip != info.Ip)
            //                {
            //                    if (i == 0)
            //                    {
            //                        i++;
            //                    }
            //                    else
            //                    {
            //                        distincts.Add(Ip);
            //                        i++;
            //                    }
            //                    Ip = info.Ip;
            //                }
            //            }
            //            distincts.Add(Ip);
            //        }
            //        break;
            //    case "USER":
            //        {
            //            string user = string.Empty;
            //            int i = 0;

            //            foreach (var info in documents)
            //            {
            //                if (user != info.IdUser)
            //                {
            //                    if (i == 0)
            //                    {
            //                        i++;
            //                    }
            //                    else
            //                    {
            //                        distincts.Add(user);
            //                        i++;
            //                    }
            //                    user = info.IdUser;
            //                }
            //            }
            //            distincts.Add(user);
            //        }
            //        break;
            //    case "SITES":
            //        {
            //            string site = string.Empty;
            //            int i = 0;

            //            foreach (var info in documents)
            //            {
            //                if (site != info.IdSite)
            //                {
            //                    if (i == 0)
            //                    {
            //                        i++;
            //                    }
            //                    else
            //                    {
            //                        distincts.Add(site);
            //                        i++;
            //                    }
            //                    site = info.Ip;
            //                }
            //            }
            //            distincts.Add(site);
            //        }
            //        break;
            //    case "YEARS":
            //        {
            //            string year = string.Empty;
            //            int i = 0;

            //            foreach (var info in documents)
            //            {
            //                if (year != info.Year)
            //                {
            //                    if (i == 0)
            //                    {
            //                        i++;
            //                    }
            //                    else
            //                    {
            //                        distincts.Add(year);
            //                        i++;
            //                    }
            //                    year = info.Ip;
            //                }
            //            }
            //            distincts.Add(year);
            //        }
            //        break;
            //    case "MONTHS":
            //        {
            //            string month = string.Empty;
            //            int i = 0;

            //            foreach (var info in documents)
            //            {
            //                if (month != info.Month)
            //                {
            //                    if (i == 0)
            //                    {
            //                        i++;
            //                    }
            //                    else
            //                    {
            //                        distincts.Add(month);
            //                        i++;
            //                    }
            //                    month = info.Ip;
            //                }
            //            }
            //            distincts.Add(month);
            //        }
            //        break;
            //}
            return distincts;
        }


        public List<T> FilterByPropertie(List<T> documents,string propertie, string value)
        {
            List<T> filterDocuments = new List<T>();
            filterDocuments = (from document in documents
                               where propertie == value
                               select document).ToList<T>();

            return filterDocuments;
        }

        public List<string> GetAllDistinctUserIds(List<T> documents)
        {
            List<string> distincs = new List<string>();
            List<T> orderedDocuments = orderByPropertie(documents, nameof(CosmosDbDocument.IdUser));
            distincs = GetDistincts(orderedDocuments, nameof(CosmosDbDocument.IdUser));
            return distincs;
        }

        public List<string> GetAllDistinctSiteIds(List<T> documents)
        {
            List<string> distincs = new List<string>();
            List<T> orderedDocuments = orderByPropertie(documents, nameof(CosmosDbDocument.IdSite));
            distincs = GetDistincts(orderedDocuments, nameof(CosmosDbDocument.IdSite));
            return distincs;
        }

        public List<string> GetAllDistinctIps(List<T> documents)
        {
            List<string> distincs = new List<string>();
            List<T> orderedDocuments = orderByPropertie(documents, nameof(CosmosDbDocument.Ip));
            distincs = GetDistincts(orderedDocuments, nameof(CosmosDbDocument.Ip));
            return distincs;
        }

        public List<string> GetAllDistinctYears(List<T> documents)
        {
            List<string> distincs = new List<string>();
            List<T> orderedDocuments = orderByPropertie(documents, nameof(CosmosDbDocument.Year));
            distincs = GetDistincts(orderedDocuments, nameof(CosmosDbDocument.Year));
            return distincs;
        }

        public List<string> GetAllDistinctMonthsByYear(List<T> documents,string year)
        {
            List<string> distincs = new List<string>();
            List<T> filteredByYear = FilterByPropertie(documents, nameof(CosmosDbDocument.Year), year);

            List<T> orderedDocuments = orderByPropertie(filteredByYear, nameof(CosmosDbDocument.Month));
            distincs = GetDistincts(orderedDocuments, nameof(CosmosDbDocument.Month));
            return distincs;
        }

        public List<string> GetAllDistinctDaysByMonth(List<T> documents, string year,string month)
        {
            List<string> distincs = new List<string>();
            List<T> filteredByYear = FilterByPropertie(documents, nameof(CosmosDbDocument.Year), year);
            List<T> filteredByMonth = FilterByPropertie(filteredByYear, nameof(CosmosDbDocument.Year), month);

            List<T> orderedDocuments = orderByPropertie(filteredByMonth, nameof(CosmosDbDocument.Day));
            distincs = GetDistincts(orderedDocuments, nameof(CosmosDbDocument.Day));
            return distincs;
        }

        public List<string> GetAllDistinctSitesByDay(List<T> documents, string year,string month,string day)
        {
            List<string> distincs = new List<string>();
            List<T> filteredByYear = FilterByPropertie(documents, nameof(CosmosDbDocument.Year), year);
            List<T> filteredByMonth = FilterByPropertie(filteredByYear, nameof(CosmosDbDocument.Year), month);
            List<T> filteredByDay = FilterByPropertie(filteredByMonth, nameof(CosmosDbDocument.Day), day);

            List<T> orderedDocuments = orderByPropertie(filteredByDay, nameof(CosmosDbDocument.IdSite));
            distincs = GetDistincts(orderedDocuments, nameof(CosmosDbDocument.IdSite));
            return distincs;
        }


        public List<string> GetAllDistinctIpsBySite(List<T> documents, string year, string month, string day, string site)
        {
            List<string> distincs = new List<string>();
            List<T> filteredByYear = FilterByPropertie(documents, nameof(CosmosDbDocument.Year), year);
            List<T> filteredByMonth = FilterByPropertie(filteredByYear, nameof(CosmosDbDocument.Year), month);
            List<T> filteredByDay = FilterByPropertie(filteredByMonth, nameof(CosmosDbDocument.Day), day);
            List<T> filteredBySite = FilterByPropertie(filteredByDay, nameof(CosmosDbDocument.IdSite), site);

            List<T> orderedDocuments = orderByPropertie(filteredByDay, nameof(CosmosDbDocument.Ip));
            distincs = GetDistincts(orderedDocuments, nameof(CosmosDbDocument.Ip));
            return distincs;
        }

        public List<T> GetAllDataByHour(string year,string month,string day, string site, string ip, string hour)
        {
            List<T> documents = GetAllData();
            List<T> filteredByYear = FilterByPropertie(documents, nameof(CosmosDbDocument.Year), year);
            List<T> filteredByMonth = FilterByPropertie(filteredByYear, nameof(CosmosDbDocument.Year), month);
            List<T> filteredByDay = FilterByPropertie(filteredByMonth, nameof(CosmosDbDocument.Day), day);
            List<T> filteredBySite = FilterByPropertie(filteredByDay, nameof(CosmosDbDocument.IdSite), site);
            List<T> filteredByHour = FilterByPropertie(filteredBySite, nameof(CosmosDbDocument.IdSite), hour);

            return filteredByHour;
        }

        public List<string> GetAllDistinctTypes()
        {
            List<string> distincs = new List<string>();
            distincs.Add("Default");
            distincs.Add("Sticky");
            distincs.Add("Telemetry");
            return distincs;
        }

        public List<string> GetAllDistinctMetrics()
        {
            List<string> distincs = new List<string>();
            distincs.Add("Impression");
            distincs.Add("Clic");
            return distincs;
        }

        #endregion


        #region Order by

        public List<T> orderByPropertie(List<T> documents, string propertie)
        {
            List<T> ordered = (from info in documents
                                 orderby propertie
                                 select info).ToList();

            return ordered;
        }

        public List<T> OrderByIp(List<T> documents)
        {
            return orderByPropertie(documents, nameof(CosmosDbDocument.Ip) );
        }

        public List<T> OrderBySite(List<T> documents)
        {
            return orderByPropertie(documents, nameof(CosmosDbDocument.IdSite) );
        }

        public List<T> OrderByUser(List<T> documents)
        {
            return orderByPropertie(documents, nameof(CosmosDbDocument.IdUser) );
        }

        public List<T> OrderByYear(List<T> documents)
        {
            return orderByPropertie(documents, nameof(CosmosDbDocument.Year) );
        }

        public List<T> OrderByMonth(List<T> documents)
        {
            return orderByPropertie(documents, nameof(CosmosDbDocument.Month));
        }

        #endregion



        public List<T> GetAll()
        {
            CosmosDBManager cosmosManager = new CosmosDBManager();
            documents = cosmosManager.GetAllDocuments<T>(DatabaseName, CollectionName);
            return documents;
        }

        public Kpis GetDistinctKpis()
        {
            Kpis kpis = new Kpis();

            try
            {
                GetAll();
                kpis.userIds = GetAllDistinctUserIds(documents);
                kpis.siteIds = GetAllDistinctSiteIds(documents);
                kpis.ips = GetAllDistinctIps(documents);
                kpis.types = GetAllDistinctTypes();
                kpis.metrics = GetAllDistinctMetrics();
            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return kpis;
        }

        public bool Execute()
        {
            bool result = false;

            try
            {
                GetAll();
                GetDistinctKpis();

            }
            catch (Exception e)
            {
                var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                telemetria.Critical(messageException);
            }
            return result;
        }


        #region Methods for charts
        public GeoDataChart GetDataForChart(string Year, string Month, string Day, string Site, string Ip, string TypeOfSearch)
        {
            GeoDataChart geoDataChart = new GeoDataChart();
            GetAllData();

            switch (TypeOfSearch)
            {
                // grafica una sola IP todas las horas
                // grafica lineal x:hour, y:count(ip)
                case "S-IP_AH":
                    {
                        GeoDataIpChart dataChart = new GeoDataIpChart();
                        //Todo
                    }
                    break;
                // grafica de todas las IPS todas las horas
                // grafical multilineal x:hour, y: count(ip)
                case "A-IP_AH":
                    {
                        List<GeoDataIpChart> dataChart = new List<GeoDataIpChart>();
                        //Todo

                    }
                    break;
            }

            return geoDataChart;
        }

        #endregion

        #region View Methods

        //Ok
        public List<string> GetYears()
        {
            List<string> years = new List<string>();
            List<T> documents = GetAllData();
            years = GetAllDistinctYears(documents);
            return years;
        }

        public List<string> GetMonths(string Year)
        {
            List<string> months = new List<string>();
            List<T> documents = GetAllData();
            months = GetAllDistinctMonthsByYear(documents,Year);
            return months;
        }

        public List<string> GetDays(string Year,string Month)
        {
            List<string> days = new List<string>();
            List<T> documents = GetAllData();
            days = GetAllDistinctDaysByMonth(documents, Year, Month);
            return days;
        }

        public List<string> GetSites(string Year, string Month, string Day)
        {
            List<string> sites = new List<string>();
            List<T> documents = GetAllData();
            sites = GetAllDistinctSitesByDay(documents,Year,Month,Day);
            return sites;
        }

        public List<string> GetIps(string Year, string Month, string Day, string Site)
        {
            List<string> ips = new List<string>();
            List<T> documents = GetAllData();
            ips = GetAllDistinctIpsBySite(documents,Year,Month,Day,Site);
            return ips;
        }

        public List<T> GetAllData()
        {
            List<T> data = new List<T>();
            data = GetAll();
            return data;
        }

        public List<T> GetDataByHour(string year, string month, string day, string site, string ip, string hour)
        {
            List<T> documents = new List<T>();
            var ips = GetAllDistinctIpsBySite(documents, year, month, day, site);

            foreach(var item in ips)
            {
                documents.AddRange(GetAllDataByHour(year,month,day,site,ip,hour));
            }
            return documents;
        }
        #endregion

        public EtlGeneric(string database, string collection)
        {
            telemetria = new Trace();
            DatabaseName = database;
            CollectionName = collection;
        }
    }
}
