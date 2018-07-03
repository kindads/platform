using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.Comun.Models
{
   

    public class StickyHtmlEntity : TableEntity
    {
        public StickyHtmlEntity(string guid, string rowKey)
        {
            this.PartitionKey = guid;
            this.RowKey = rowKey;
            this.Timestamp = DateTime.Now;
            this.ETag = "*";
            this.Html = string.Empty;
        }

        public StickyHtmlEntity() { }

        public string Html { get; set; }
    }

    public class DefaultHtmlEntity : TableEntity
    {
        public DefaultHtmlEntity(string guid, string rowKey)
        {
            this.PartitionKey = guid;
            this.RowKey = rowKey;
            this.Timestamp = DateTime.Now;
            this.ETag = "*";
            this.Html = string.Empty;
        }

        public DefaultHtmlEntity() { }

        public string Html { get; set; }
    }
}
