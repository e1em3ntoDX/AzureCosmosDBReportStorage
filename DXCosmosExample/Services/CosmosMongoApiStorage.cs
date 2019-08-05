using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using DXCosmosExample.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DXCosmosExample.Services {
    public class CosmosMongoApiStorage : ICosmosApiStorage {
        readonly MongoClient mongoClient;
        string databaseId;
        string containerId;
        IMongoCollection<ReportItemMongo> mongoCollection {
            get {
                return mongoClient.GetDatabase(databaseId).GetCollection<ReportItemMongo>(containerId);
            }
        }

        public static void Register(IServiceCollection serviceProvider, IConfiguration cosmosConfig) {
            var storage = new CosmosMongoApiStorage(cosmosConfig["MongoConnection"], cosmosConfig["DatabaseId"], cosmosConfig["ContainerId"]);
            serviceProvider.AddSingleton<ICosmosApiStorage>(storage);
            var adminReport = storage.mongoCollection.Find(x => x.ReportName == CosmosReportStorageWebExtension.AdminReportName).FirstOrDefault();
            if(adminReport == null) {
                storage.mongoCollection.InsertOne(ReportItemMongo.CreateFromReportItem(CosmosReportStorageWebExtension.CreateAdminReport()));
            }
        }

        public CosmosMongoApiStorage(string connection, string databaseId, string containerId) {
            this.databaseId = databaseId;
            this.containerId = containerId;
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connection));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            mongoClient = new MongoClient(settings);
        }
        
        public async Task<ReportItem> GetReportItem(string id) {
            var item = (await mongoCollection.FindAsync(x => x.ReportName == id)).FirstOrDefault();
            return new ReportItem() {
                Id = item.ReportName,
                ReportLayout = item.ReportLayout
            };
        }

        public async Task UpdateReportItem(ReportItem reportItem) {
            UpdateDefinition<ReportItemMongo> updateDefinition = Builders<ReportItemMongo>.Update.Set(x => x.ReportLayout, reportItem.ReportLayout);
            await mongoCollection.UpdateOneAsync(x => x.ReportName == reportItem.Id, updateDefinition);
        }

        public async Task<string> CreateReportItem(ReportItem reportItem) {
            await mongoCollection.InsertOneAsync(ReportItemMongo.CreateFromReportItem(reportItem));
            return reportItem.Id;
        }

        public async Task<Dictionary<string, string>> GetReports() {
            return mongoCollection.AsQueryable().ToDictionary(x => x.ReportName, x => x.ReportName);
        }
    }
}
