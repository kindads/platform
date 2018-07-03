using System;
using Captivate.Comun.Interfaces;
using Captivate.Negocio.Etls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Captivate.UnitTest
{
    [TestClass]
    public class EtlUnitTest
    {
        [TestMethod]
        public void GeoTest()
        {
            IEtlManager manager = new EtlGeoLocationManager();
            manager.Execute();
        }
    }
}
