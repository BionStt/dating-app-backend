# Dating App Backend
Simple API Rest that tries to emulate how dating apps process and deliver the swipes and matches made with DotNet Core 6 and uses [Vertical Slice Architecture](https://code-maze.com/vertical-slice-architecture-aspnet-core/) with [Minimal Api's](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0).

## Architecture Overview
![alt text](https://github.com/juan-canseco/dating-app-backend/blob/main/img/architecture-overview.png)

## How this works?
When you send a request to create a swipe, the request handler will check if user swipes exist in cache in case if not exists the handler store the swipe in SQL database, then put the recently created swipe in cache and then publish an event with the created swipe info in a message broker in order to be processed asynchronously by another services.

## How to run
In order to execute this project, you will need to open the terminal and locate the project root folder, once the above is done you will need to execute the following commands:
```bash
// First | Build the project images
docker-compose build
// Second | Up the project 
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
// Third | Shutdown the project
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```
In case you don’t have docker installed in your pc, you will need to install RabbitMq, Redis, MSSQL; Once installed in the appsettings.Development.json of the Matching.Api project you will need to change the following properties:
```json
{
  "ConnectionStrings": {
    "MatchingApplication": "YOUR SQL SERVER DATABASE CONNECTION STRING"
  },
  "EventBusSettings": {
    "HostAddress": "YOUR RABBITMQ HOST ADDRESS"
  },
  "CacheSettings": {
    "ConnectionString": "YOUR REDIS HOST ADDRESS"
  }
}
```


### Libraries used
- [Carter](https://github.com/CarterCommunity/Carter)
- [FluentValidation](https://docs.fluentvalidation.net/en/latest/) 
- [MediatR](https://github.com/jbogard/MediatR)
- [EF Core](https://learn.microsoft.com/en-us/ef/core/)
- [Dapper](https://github.com/DapperLib/Dapper)
- [StackExchangeRedis](https://stackexchange.github.io/StackExchange.Redis/)
- [MassTransit](https://masstransit.io/documentation/configuration) 
- [Testcontainers](https://dotnet.testcontainers.org/)

### Credits
Inspired by
- [Minimal API Vertical Slice Architecture](https://github.com/isaacOjeda/MinimalApiArchitecture) by Issac Ojeda
- [aspnetrun/run-aspnetcore-microservices](https://github.com/aspnetrun/run-aspnetcore-microservices) by Mehmet Ozkaya
- [Design Dating Platform - Tinder](https://techtakshila.com/system-design-interview/chapter-5/) by Tech Takshila
- [CQRS Validation Pipeline with MediatR and FluentValidation](https://code-maze.com/cqrs-mediatr-fluentvalidation/) by CodeMaze
- [ASP.NET Core Integration Tests with Test Containers & Postgres](https://www.azureblue.io/asp-net-core-integration-tests-with-test-containers-and-postgres/) by Matthias Güntert
- [Integration Test In Asp.Net Core 6 Using SqlServer Image and TestContainers](https://hamidmosalla.com/2022/09/10/integration-test-in-asp-net-core-6-using-sqlserver-image-and-testcontainers/) by Hamid Mosalla

