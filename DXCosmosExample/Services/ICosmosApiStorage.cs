using System.Collections.Generic;
using System.Threading.Tasks;
using DXCosmosExample.Models;

namespace DXCosmosExample.Services {
    public interface ICosmosApiStorage {
        Task UpdateReportItem(ReportItem reportItem);
        Task<string> CreateReportItem(ReportItem reportItem);
        Task<ReportItem> GetReportItem(string id);
        Task<Dictionary<string, string>> GetReports();
    }
}
