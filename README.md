# Dating App Backend
Simple API Rest that tries to emulate how dating apps process and deliver the swipes and matches made with DotNet Core 6 and uses Vertical Slice Architecture with Minimal Api's.

## Architecture Overview
![alt text](https://github.com/juan-canseco/dating-app-backend/blob/main/img/architecture-overview.png)

## How this works?
When you send a request to create a swipe the request handler will check if user swipes exist in cache in case if not exists the handler store the swipe in SQL database, then put the recently created swipe in cache and then publish an event with the created swipe info in a message broker in order to be processed asynchronously by another services.



### Libraries used
- Carter
- FluentValidation 
- MediatR
- EFCore
- Dapper
- StackExchangeRedis
- MassTransit 
