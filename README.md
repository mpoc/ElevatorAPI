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
Note: the elevator object in the response represents the current state of the elevator, not the state at the point of the action.

## Run

### Docker

    docker build -t elevator-api .
    touch database.db
    docker run -p 5000:80 -v ${PWD}/database.db:/app/database.db --name elevator-api elevator-api

### Locally

    dotnet publish -c Release
    dotnet ./bin/Release/net5.0/ElevatorAPI.dll

## Instructions

To experiment with the project, you could:

1. Use the `POST ​/api​/buildings` endpoint to create a building with elevators.
1. Choose an elevator and use its id to call the elevator form a floor to a floor using the endpoint `GET ​/api​/elevators​/{id}​/call`.
1. Give the id of the elevator to the `GET ​/api​/logs` endpoint to see a history of its actions.
