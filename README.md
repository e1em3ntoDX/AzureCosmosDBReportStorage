# ASP.NET Core Reporting – Store Reports within a Database Using Azure Cosmos DB

## Prerequisites 

- Azure subscription ([create one for free](https://azure.microsoft.com/free/) or [try Azure Cosmos DB for free](https://azure.microsoft.com/try/cosmosdb/) without an Azure subscription)
- [.NET Core 2.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/2.1) or later
- [Microsoft.Azure.Cosmos](https://www.nuget.org/packages/Microsoft.Azure.Cosmos/) NuGet Package.
- [DevExpress Reporting](https://www.devexpress.com/subscriptions/reporting) must be installed on your machine ([download free 30-day trial](https://www.devexpress.com/Products/Try/))

The report storage implementation demonstrated in this sample supports both the SQL and MongoDB API. Use the [CosmosClient](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos.cosmosclient?view=azure-dotnet) class located in the [Microsoft.Azure.Cosmos](https://www.nuget.org/packages/Microsoft.Azure.Cosmos/) NuGet Package, if using the SQL API (for Azure Cosmos DB Storage access).
To start using this sample, open the ```appSettings.json``` file and provide your connection details:

```json
"CosmosSettings": {
    "DatabaseId": "TestDb",
    "ContainerId": "Reports",
    "SqlConnection": "<sql connection>",
    "MongoConnection": "<mongo connection>"
  }
  ```