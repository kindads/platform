using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Captivate.Azure;
using Captivate.Comun.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Captivate.UnitTest
{
    [TestClass]
    public class PopulateCosmosDbUnitTest
    {


        private List<GeoInfo> GetGeoLocationData(int length)
        {
            List<GeoInfo> data = new List<GeoInfo>();
            for(int i=0;i<=(length-1);i++)
            {
                data.Add(GetSingleGeoLocationData());
            }
            return data;
        }


        private string GetRandomIdSite()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            string IdSite = string.Empty;           
            int option = rnd.Next(1, 3);

            if (option == 1)
            {
                IdSite = "B6DE5AB4-7DBF-427E-8F4D-38B9219B7C4C";
            }
            else
            {
                IdSite = "757DD20E-B548-4ABA-A618-BB577221DC4D";
            }

            return IdSite;
        }


        public double RandomHour()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            int hour = rnd.Next(1, 25); 
            return hour;
        }

        private void GetAddress(ref GeoInfo info)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            int option = rnd.Next(1, 4); 

            switch (option)
            {
                case 1:
                    {
                        //Mexico                       
                        info.Address = "Periférico Blvrd Manuel Ávila Camacho 184 Reforma Soc 11650 Ciudad de México, CDMX ";
                        info.latitude = "19.4350349";
                        info.longitude = "-99.2146001";
                        info.Ip = "110.5.3.2";
                        info.PrintTime = DateTime.Now.AddHours(RandomHour()).ToString();
                    }
                    break;
                case 2:
                    {
                        //Japon
                        info.Address = "Universidad Waseda , Tokio, Japon";
                        info.latitude = "36.0000000";
                        info.longitude = "138.0000000";
                        info.Ip = "110.15.13.12";
                        info.PrintTime = DateTime.Now.AddHours(RandomHour()).ToString();
                    }
                    break;
                case 3:
                    {
                        //USA
                        info.Address = "Bellevue, WA 98004, EE. UU";
                        info.latitude = "47.6060024";
                        info.longitude = "-122.2313516";
                        info.Ip = "10.50.100.100";
                        info.PrintTime = DateTime.Now.AddHours(RandomHour()).ToString();
                    }
                    break;
            }
        }


        private GeoInfo GetSingleGeoLocationData()
        {          
            GeoInfo info = new GeoInfo();
            info.IdUser = "44f1e76b-24ae-4889-937b-5e1c45575ae8".ToUpper();
            info.IdSite = GetRandomIdSite();
            GetAddress(ref info);
            return info;
        }

        [TestMethod]
        public void PopulateGeoLocation()
        {
            CosmosDBManager cosmosManager = new CosmosDBManager();
            string DatabaseName = "kindadsTelemetry-dev";
            string CollectionName = "geo-telemetry-dev";


            List<GeoInfo> data = GetGeoLocationData(500);

            Parallel.ForEach(data, (info) => {
                bool resultCreateCollection = cosmosManager.CreateCollectionIfNotExist(DatabaseName, CollectionName);
                bool resultAddDocument = cosmosManager.CreateDocumentIfNotExists<GeoInfo>(DatabaseName, CollectionName, info);
            });

            Assert.AreEqual(true, true);
        }
    }
}
