
# Wikich.art

Project about collecting, compiling and presenting daily trending topics from different regions of wikipedia.

Powered by Microsoft Azure and Wikipedia API.
<p align="center">

<h2 align="center"> <a href="https://www.wikich.art"> Check out live app!</a> </h2> <br>

<p align="center">
<img src="https://user-images.githubusercontent.com/27297124/229155261-454b13d4-0734-4cfd-9d3e-15e6ee5f48b4.png" /> 
</p>

## Backend: 

- <a href=https://github.com/holyhamster/WikichArt/tree/master/DBUpdaterTimer>DbUpdaterTimer</a> is a scheduled Azure serverless Function (or its webhook implementation - <a href=https://github.com/holyhamster/WikichArt/tree/master/DbUpdaterHttp>DbUpdaterHttp</a>) that calls DataBaseBuilder. 
- <a href=https://github.com/holyhamster/WikichArt/blob/master/WikiLibrary/DataBaseBuilder.cs>DataBaseBuiler</a> creates <a href=https://github.com/holyhamster/WikichArt/blob/master/WikiLibrary/WikiData/DailyCollection.cs>DailyCollections</a> and uploads them to <a href=https://github.com/holyhamster/WikichArt/blob/master/WikiLibrary/IDataBaseClient.cs>IDataBaseClient</a> (default implementation is <a href=https://github.com/holyhamster/WikichArt/blob/master/WikiLibrary/AzureStorageClient.cs>AzureStorageClient</a>, a Table in Azure Storage)
- <a href=https://github.com/holyhamster/WikichArt/blob/master/WikiLibrary/WikiData/DailyCollection.cs>DailyCollection</a> sends requests from <a href=https://github.com/holyhamster/WikichArt/blob/master/WikiLibrary/WikiAPIRequests.cs>WikiAPIRequests</a> to create serializable regional collections with article names (plus english names if exist), viewcount, as well as top articles across all locales.

## Frontend:

- <a href=https://github.com/holyhamster/WikichArt/tree/master/frontapp/src>Angular web app</a> built with bootstrap and hosted on Azure Static Webapps.
- The app requests data from <a href=https://github.com/holyhamster/WikichArt/tree/master/frontapp/api>API</a>, another Azure Function with connection to Storage Account
