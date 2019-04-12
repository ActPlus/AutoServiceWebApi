# AutoServiceWebApi
ASP.Net Core 2.1 Rest CRUD

Created by: Kirill Kuznecov
Uses C# with Asp.net framework
implements Rest Api

Instalation steps:

1.Define Path to External Api ip in WebService/appsettings.json/"CustomerValidation":"applicationUrl"
It can be set To the same adress as WebService is running at, because It implements Debug CustomerTestController, which simulates Customer Api.
It simple check if Customer Id In POST REQUEST with argument {id} is same as 259806db-436e-450e-b57d-b5b1b3b955ba. Which is only valid CustomerId.
Otherwise it will return NotFound() response with 404 error code.

Customer Validation can be turned off, and connection to external Api will not be required.
It can be done by changing WebService/appsettings/"CustomerValidation":"enabled" to false.

2.Open in Visual Studio and build with docker module




//Those steps doesnt work
/*
2.Open a command prompt and navigate to your project folder, into second WebService folder.
3.Use the following commands to build and run your Docker image:

$ docker build -t webservice .
$ docker run -d -p 5000:80 --name WebService webservice
*/