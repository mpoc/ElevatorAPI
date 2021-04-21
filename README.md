# Elevator API

Simple web API to simulate buildings with elevators.

## Endpoints

An easy way to interact with the API is through the Swagger documentation page, which allows to make requests.
It can be seen after running the project at http://localhost:5000/swagger/index.html

### Buildings

1. `GET ​/api​/buildings` - Get all buildings.
1. `POST ​/api​/buildings` - Create a building with a certain number of elevators.
1. `GET ​/api​/buildings​/{id}` - Get a building by id.
1. `DELETE ​/api​/buildings​/{id}` - Delete a building.
1. `GET ​/api​/buildings​/{id}​/elevators` - Get elevators for a building.

### Elevators

1. `GET ​/api​/elevators​/{id}` - Get an elevator by id.
1. `DELETE ​/api​/elevators​/{id}` - Delete an elevator.
1. `GET ​/api​/elevators​/{id}​/call` - Call an elevator from a floor to a floor.

### Logs

1. `GET ​/api​/logs` Get all logs OR get all logs for a specific elevator id (if `elevatorId` is specified).

## Run

### Docker

    docker build -t elevator-api .
    touch database.db
    docker run -p 5000:80 -v ${PWD}/database.db:/app/database.db --name elevator-api elevator-api

### Locally

    dotnet publish -c Release
    dotnet ./bin/Release/net5.0/ElevatorAPI.dll
