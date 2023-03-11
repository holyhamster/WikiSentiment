# Wiki Sentiment

Collects regional data from wikipedia API (titles, views, language links) and uploads them into a database.

Two ways of calling database builder
/UpdaterHttp/ is a HTTP-triggered Azure Function (http://url:port/api/WikiDBUpdaterHttp?date=2020-01-01&days=1&discard)
/UpdaterTimer/ is a Timer Azure Function (called daily)

/WikiSentiment/:
DataBaseBuilder accepts selected time, API-necessary data (list of languages and keywords to ignore list) and IDataBaseClient interface to upload the results.
AzureStorageClient is an implementation of IDataBaseClient for Azure Storage
DailyCollection, LanguageCollection and Article are different levels of aggregating data using API calls from WikiAPIRequests
