# Getting Started with Create React App
AEBackend is a backend solution developed using ASP.NET Core that manages users, ships, and ports. It provides RESTful APIs to handle user and ship management, and calculates the closest port to a given ship along with the estimated arrival time based on velocity and geolocation.

## Features
* User Management
	* Retrieve User (All users and find by Id)
	* Create new user
	* Update Ship assigned / unassigned to particular user

* Ship Management
	* Retrieve Ship (All ships and find by Id)
	* Create new ship
	* Update ship velocity
	* Retrieve ship asigned to user and unassigned ship
	* Find the closest port to a ship and calculate the estimated arrival time

* Port Management
	* No API for Port (Data will be seeded with initial data when running apps)

## Technologies
* Language: C#
* Framework: ASP NET Core Web API
* Database: SQL Server (running docker)
* Testing: xUnit

# System Design
* Implementing repository pattern (to abstract data access logic), with unit of work (grouping operation that done by repository)
* Implementing service: it contains business logic and interacts with repositories
* Presentation or Controller layer: Focus on handling HTTP request and response

# Instalation
1. Clone repository
git clone https://github.com/yourusername/AEBackendSolution.git
cd AEBackendSolution

2. Restore Dependencies
dotnet restore

3. Migration
* using terminal
dotnet ef migrations add InitialCreate
dotnet ef database update
* using Nuget Package Manager (Visual Studio)
Add-Migration InitialCreate
Update-Database

4. Run Application (click on running icon in visual studio or if using terminal, we can type:)
dotnet run

# API Endpoint

## User Management
* Find user based on specifict primary key / id
```
GET /api/User/{id}
```

* Retrieve all users
```
GET /api/User
```

* Create or register new user
```
POST /api/User
```
Request Body
  ```javascript
      {
  	  "name": "string",
  	  "role": "string"
  	}
  ```

* Assign user to specific ship
```
POST /api/User/{userId}/assign/{shipId}  (No request body)
```

* Unassign user to specific ship
```
POST /api/User/{userId}/unassign/{shipId} (No request body)
```


## Ship Management
* Find ship by primary key or id
```
GET /api/Ship/{id}
```

* Retrieve all ships
```
GET /api/Ship
```

* Create or register new ship
```
POST /api/Ship
```
  Request Body
  ```javascript
      {
  	  "name": "string",
  	  "latitude": 0,
  	  "longitude": 0,
  	  "velocity": 0
  	}
  ```

* Update velocity of specific ship
  ```
  PACTH /api/Ship/{id}/Velocity
  ```
Request body
  ```
  25.0
  ```

* Retrieve all mapped ships of a particular user
```
GET /api/Ship/assigned/{userId}
```

* Retrieve all ships that are not mapped to a user
```
GET /api/Ship/unassigned
```

* Find closest port to a ship
```
GET /api/Ship/closest-port?shipId=
```

# Testing
If you are using terminal, go to solution directory and exec: 
```
dotnet test
```

# Seeding Data
We add data seeder in startup/program.cs. So, once the application is running, the PortSeeder will be executed. (Note: if the data have exists then system won't be created again)

# Error Handling
* Creating global exception middleware
* Createing custom exception

# Best Practices
1. Follow most of S.O.L.I.D principles for code organization
2. Use async/await for asynchronous operations.
3. Keep controller clean due to separate data access and business logic layer
4. Validate inputs and handle exceptions gracefully.
5. Create separate unit test for controller and service. Testing controller will focus on correct status codes, data, and handle requests properly. Whereas service focus on the core business logic of application
6. Implement generic repository (GetById, GetAll, Get(Custom predicate, order, selection and etc), insert, update, delete)
7. Add global filter (IsDeleted) in most of entities

# Documentation
For detailed API usage and examples, refer to the Swagger documentation generated by ASP.NET Core, which is available at /swagger endpoint when the application is running.

# Contact
For any questions or feedback, please contact khoirudi16@gmail.com or mk.muhammadkhoirudin@gmail.com
