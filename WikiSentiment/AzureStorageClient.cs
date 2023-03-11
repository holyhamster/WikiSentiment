using Azure;
using Azure.Data.Tables;

namespace WikiSentiment
{
    /// <summary>
    /// API wrapper for azure table storage
    /// </summary>
    public class AzureStorageClient:IDBClient
    {
        TableClient client;

        public AzureStorageClient(TableClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Attempt to get data for given date, returns empty string if none exists
        /// </summary>
        /// <param name="_YYYYMM"></param>
        /// <param name="_DD"></param>
        /// <returns></returns>
        public async Task<string> Load(DateTime date)
        {
            //Format is YYYY-MM as pkey, DD as rkey
            AsyncPageable<DayEntity> result = client.QueryAsync<DayEntity>(
                e => e.PartitionKey == $"{ date.Year:D4}-{date.Month:D2}" && e.RowKey == $"{date.Day:D2}");

            await foreach (var res in result)
            {
                return res.DailyData;
            }

            return "";
        }


        /// <summary>
        /// Uploads string to azure tables
        /// </summary>
        /// <param name="date"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task Upload(DateTime date, string content)
        {
            //Format is YYYY-MM as pkey, DD as rkey
            var dayEntity = new DayEntity() { 
                PartitionKey = $"{date.Year:D4}-{date.Month:D2}", 
                RowKey = $"{date.Day:D2}", 
                DailyData = content };

            await client.UpsertEntityAsync(dayEntity);
        }
    }

    //implementation of azure table entity for daily wiki data
    class DayEntity : ITableEntity
    {
        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public string DailyData { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public DayEntity()
        {

        }
    }
}
