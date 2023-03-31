using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;

namespace WikiSentiment.UpdateDB
{
    public static class HttpGetDbData
    {
        const string TablesName = "datatables"; //name of the table in Azure Storage
        const string TableConnection = "TableStorageConnection";    //name of the connection string in settings.json/variables

        //API called by the angular app, need to have a &month variable in the header
        [FunctionName("HttpGetDbData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Microsoft.Azure.WebJobs.Table(TablesName), StorageAccount(TableConnection)] TableClient tableClient,
            ILogger log)
        {
            string YYYYMM = req.Query["month"];

            if (YYYYMM?.Length != 7 ||
                !int.TryParse(YYYYMM.Split('-')[0], out int year) ||
                !int.TryParse(YYYYMM.Split('-')[1], out int month))
                return new BadRequestObjectResult($"bad parameter: {YYYYMM}");

            //send request to the table api
            Dictionary<string,string> request = await LoadMonth(tableClient, YYYYMM);
            var returnString = System.Text.Json.JsonSerializer.Serialize(request, new JsonSerializerOptions()
            {
                WriteIndented = false
            });
            return new OkObjectResult(returnString);
        }

        public static async Task<Dictionary<string, string>> LoadMonth(TableClient client, string YYYYMM)
        {
           Azure.AsyncPageable<TableEntity> request = client.QueryAsync<TableEntity>(e => e.PartitionKey == YYYYMM);

            var dict = new Dictionary<string, string>();
            await foreach (var response in request)
                dict[$"{response.PartitionKey}-{response.RowKey}"] = response.GetString("DailyData");

            return dict;
        }
    }
}
