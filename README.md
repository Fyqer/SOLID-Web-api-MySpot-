# MySpot - SOLID Web Api 
API which helps to manage a parking slots, additionaly project contains a exmaple .rest file with example queries, unit test and default logger which write logs into logs.file . Project was created by following a devmentors "SOLID Web API" course.
After launch, application will create new rows in postgress databse. 

## Arhitecture
Main puropose of this project is to follow best standards and patterns of objective programming: 
- Project is made with Clean Architecture principle ( splitted into Domain / Infrastructure / API / Application layers
- CQRS ( Request into database are are splitted into  Command, Queries and Handlers )
- SOLID standards


## Main Tech Stack and used libararies
- NET 7
- Postgres Databse
- Docker ( docker_compose.yaml )

- Xunit, Serilog, REST



