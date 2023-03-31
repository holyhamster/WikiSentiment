
# Wikich.art

Project about collecting, compiling and presenting daily trending topics from different regions of wikipedia.

Powered by Microsoft Azure and Wikipedia API.
<p align="center">

<h2 align="center"> <a href="https://www.wikich.art"> Check out live app!</a> </h2> <br>

<p align="center">
<img src="https://user-images.githubusercontent.com/27297124/229155261-454b13d4-0734-4cfd-9d3e-15e6ee5f48b4.png" /> 
</p>

[x] Backend: Azure Function DbUpdaterTimer (or its webhook implementation, DbUpdaterHttp) querries wiki API for data, compiles and sends it into Azure Storage

[x] Frontend: Angular Webapp hosted on Azure Static Webapps with an API Function to querry Storage for data.
