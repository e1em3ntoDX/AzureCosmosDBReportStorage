using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DXCosmosExample.Models;

namespace DXCosmosExample.Services {
    public class CosmosSqlApiStorage : ICosmosApiStorage {
        readonly CosmosClient client;
        string databaseId;
        string containerId;
        Container reportsContainer {
            get {
                return client.GetContainer(databaseId, containerId);
            }
        }

        public static void Register(IServiceCollection serviceProvider, IConfiguration cosmosConfig) {
            var storage = new CosmosSqlApiStorage(cosmosConfig["SqlConnection"], cosmosConfig["DatabaseId"], cosmosConfig["ContainerId"]);
            serviceProvider.AddSingleton<ICosmosApiStorage>(storage);
            storage.Initialize().Wait();
        }

        public CosmosSqlApiStorage(string connection, string databaseId, string containerId) {
            client = new CosmosClient(connection);
            this.databaseId = databaseId;
            this.containerId = containerId;
        }
        public async Task Initialize() {
            Database db = await client.CreateDatabaseIfNotExistsAsync(databaseId);
            Container container = await db.CreateContainerIfNotExistsAsync(containerId, "/id");
            var iterator = container.GetItemQueryIterator<ReportItem>(
                new QueryDefinition("select * from " + containerId),
                requestOptions: new QueryRequestOptions() {
                    MaxItemCount = 1
                });
            bool hasAdminReport = false;
            while(iterator.HasMoreResults && !hasAdminReport) {
                var item = (await iterator.ReadNextAsync()).FirstOrDefault();
                hasAdminReport = item != null && item.Id == CosmosReportStorageWebExtension.AdminReportName;
            }
            if(!hasAdminReport)
                await container.CreateItemAsync(CosmosReportStorageWebExtension.CreateAdminReport());
        }

        public async Task<ReportItem> GetReportItem(string id) {
            return await reportsContainer.ReadItemAsync<ReportItem>(id, new PartitionKey(id));
        }

        public async Task UpdateReportItem(ReportItem reportItem) {
            await reportsContainer.ReplaceItemAsync(reportItem, reportItem.Id, new PartitionKey(reportItem.Id));
        }

        public async Task<string> CreateReportItem(ReportItem reportItem) {
            await reportsContainer.CreateItemAsync(reportItem, new PartitionKey(reportItem.Id));
            return reportItem.Id;
        }

        public async Task<Dictionary<string, string>> GetReports() {
            Dictionary<string, string> reports = new Dictionary<string, string>();
            var items = reportsContainer.GetItemQueryIterator<ReportItem>(
                new QueryDefinition("select * from " + containerId),
                requestOptions: new QueryRequestOptions() {
                    MaxItemCount = 1
                });
            while(items.HasMoreResults) {
                var item = (await items.ReadNextAsync()).FirstOrDefault();
                if(item != null) {
                    reports.Add(item.Id, item.Id);
                }
            }
            return reports;
        }
    }
}
