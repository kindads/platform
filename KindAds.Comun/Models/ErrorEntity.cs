using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KindAds.Common.Models
{
    public class ErrorEntity : TableEntity
    {
        public ErrorEntity(string guid, string rowKey)
        {
            this.PartitionKey = guid;
            this.RowKey = rowKey;
            this.Timestamp = DateTime.Now;
            this.ETag = "*";
            this.Excepcion = string.Empty;
        }

        public ErrorEntity() { }

        public string Excepcion { get; set; }
    }
}
