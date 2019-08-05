using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.XtraReports.UI;

namespace DXCosmosExample.Models {
    public class HomeModel {
        public IEnumerable<string> Reports { get; set; }
    }

    public class ReportModel {
        public XtraReport Report { get; set; }
        public string ReportUrl { get; set; }
    }
}
