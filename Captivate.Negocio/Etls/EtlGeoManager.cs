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
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Negocio.Etls
{
    //public class EtlGeoManager : IEtlManager, ITelemetria
    //{

    //    #region properties
    //    public ITrace telemetria { set; get; }

    //    public List<GeoInfo> geoInfo { set; get; }

    //    public string DatabaseName { set; get; }

    //    public string CollectionName { set; get; }

    //    public EtlGeneric<GeoInfo> Core { set; get; }

    //    #endregion


    //    #region Get All Distinct
      

    //    public List<string> GetAllDistinctUserIds(List<GeoInfo> documents)
    //    {
    //        List<string> distincs = new List<string>();
    //        List<GeoInfo> orderByIp = Core.OrderByUser(documents);
    //        distincs = Core.GetAllDistinctUserIds(documents);
    //        return distincs;
    //    }

    //    public List<string> GetAllDistinctSiteIds(List<GeoInfo> data)
    //    {           
    //        List<string> distincs  = Core.GetAllDistinctSiteIds(geoInfo);
    //        return distincs;
    //    }

    //    public List<string> GetAllDistinctIps(List<GeoInfo> documents)
    //    {
    //        List<string> distincs = new List<string>();
    //        List<GeoInfo> orderByIp = Core.OrderByIp(documents);
    //        distincs = Core.GetAllDistinctIps(documents);
    //        return distincs;
    //    }

    //    public List<string> GetAllDistinctTypes()
    //    {
    //        List<string> distincs = new List<string>();
    //        distincs = Core.GetAllDistinctTypes();
    //        return distincs;
    //    }

    //    public List<string> GetAllDistinctMetrics()
    //    {
    //        List<string> distincs = new List<string>();
    //        distincs = Core.GetAllDistinctMetrics();
    //        return distincs;
    //    }

    //    #endregion


    //    #region Order by
    //    public List<GeoInfo> OrderByIp(List<GeoInfo> documents)
    //    {
    //        List<GeoInfo> orderByIp = Core.OrderByIp(documents);
    //        return orderByIp;
    //    }

    //    public List<GeoInfo> OrderBySite(List<GeoInfo> documents)
    //    {
    //        List<GeoInfo> orderByIp = Core.OrderBySite(documents);
    //        return orderByIp;
    //    }

    //    public List<GeoInfo> OrderByUser(List<GeoInfo> documents)
    //    {
    //        List<GeoInfo> orderByIp = Core.OrderByUser(documents);
    //        return orderByIp;
    //    }

    //    #endregion



    //    public List<GeoInfo> GetAll()
    //    {   
    //        geoInfo = Core.GetAll();
    //        return geoInfo;
    //    }

    //    public bool GetDistinctKpis()
    //    {
    //        bool result = false;
    //        try
    //        {
    //            Kpis kpis = Core.GetDistinctKpis();
    //            result = true;
    //        }
    //        catch (Exception e)
    //        {
    //            var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
    //            telemetria.Critical(messageException);
    //        }
    //        return result;
    //    }

    //    public bool Execute()
    //    {
    //        bool result = false;

    //        try
    //        {
    //            GetAll();
    //            if(GetDistinctKpis())
    //            {
    //                //Seccion de cortes para graficas, llamar a los metodos de graficas
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            var messageException = telemetria.MakeMessageException(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
    //            telemetria.Critical(messageException);
    //        }
    //        return result;
    //    }


    //    #region Methods for charts
    //    public GeoDataChart GetDataForChart(string Year,string Month,string Day,string Site,string Ip, string TypeOfSearch)
    //    {
    //        GeoDataChart geoDataChart = new GeoDataChart();
    //        GetAllData();

    //        switch(TypeOfSearch)
    //        {
    //            // grafica una sola IP todas las horas
    //            // grafica lineal x:hour, y:count(ip)
    //            case "S-IP_AH":
    //                {
    //                    GeoDataIpChart dataChart = new GeoDataIpChart();
    //                    //Todo
    //                }
    //                break;
    //            // grafica de todas las IPS todas las horas
    //            // grafical multilineal x:hour, y: count(ip)
    //            case "A-IP_AH":
    //                {
    //                    List<GeoDataIpChart> dataChart = new List<GeoDataIpChart>();
    //                    //Todo

    //                }break;
    //        }

    //        return geoDataChart;
    //    }

    //    #endregion

    //    #region View Methods
    //    public List<string> GetYears()
    //    {
    //        List<string> years = new List<string>();
    //        years = Core.GetYears();
    //        return years;
    //    }

    //    public List<string> GetMonths(string year)
    //    {
    //        List<string> months = new List<string>();
    //        months = Core.GetMonths(year);
    //        return months;
    //    }

    //    public List<string> GetDays(string year,string month)
    //    {
    //        List<string> days = new List<string>();
    //        days = Core.GetDays(year, month);
    //        return days;
    //    }

    //    public List<string> GetSites(string year, string month,string day)
    //    {
    //        List<string> sites = new List<string>();
    //        sites = Core.GetSites(year, month, day);
    //        return sites;
    //    }

    //    public List<string> GetIps(string year,string month,string day,string site)
    //    {
    //        List<string> ips = new List<string>();
    //        ips = Core.GetIps(year, month, day, site);
    //        return ips;
    //    }

    //    public List<GeoInfo> GetAllData()
    //    {
    //        List<GeoInfo> data = new List<GeoInfo>();
    //        data = Core.GetAll();
    //        return data;
    //    }

    //    public List<GeoInfo> GetDataByHour(string year,string month,string day,string site, string ip,string hour)
    //    {
    //        List<GeoInfo> data = new List<GeoInfo>();
    //        data = Core.GetDataByHour(year, month, day, site, ip, hour);
    //        return data;
    //    }
    //    #endregion

    //    public EtlGeoManager()
    //    {
    //        telemetria = new Trace();
    //        DatabaseName = ConfigurationManager.AppSettings["CosmosDatabaseName"];
    //        CollectionName = ConfigurationManager.AppSettings["CosmosGeoCollectionName"];

    //        Core = new EtlGeneric<GeoInfo>(DatabaseName,CollectionName);
    //    }
    //}

    public class EtlGeoLocationManager: EtlGeneric<GeoInfo>
    {
        public EtlGeoLocationManager() :
            base(ConfigurationManager.AppSettings["CosmosDatabaseName"],
                ConfigurationManager.AppSettings["CosmosGeoCollectionName"])
        { }
    }
}
