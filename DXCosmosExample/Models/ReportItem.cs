using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace DXCosmosExample.Models {
    public class ReportItem {
        public byte[] ReportLayout { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }

    public class ReportItemMongo {
        public static ReportItemMongo CreateFromReportItem(ReportItem item) {
            return new ReportItemMongo() {
                ReportLayout = item.ReportLayout,
                ReportName = item.Id
            };
        }

        [JsonProperty(PropertyName = "_id")]
        public BsonObjectId id { get; set; }
        [JsonProperty(PropertyName = "ReportLayout")]
        public byte[] ReportLayout { get; set; }
        [JsonProperty(PropertyName = "reportName")]
        public string ReportName { get; set; }

    }
}
