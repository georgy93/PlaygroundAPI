# MyApi

A playground API

## Framework
 .NET 5
<br/>

## Project Structure

### Application Core

The Application Core is formed by the **Domain** and **Application** layers, where the Application references the Domain.

  The Application Core takes its name from its position at the core of this diagram. 
It has no dependencies on other application layers. The application entities and interfaces are at the very center.

  The Application Core holds the business model, which includes entities, services, and interfaces.
  These interfaces include abstractions for operations that will be performed using Infrastructure, such as data access, file system access, network calls, etc.
  Sometimes services or interfaces defined at this layer will need to work with non-entity types that have no dependencies on UI or Infrastructure.
  These can be defined as simple Data Transfer Objects (DTOs).


Application Core Types
• Entities (business model classes that are persisted (POCOs)) and Aggregates
• Interfaces (contracts)
• Services (use cases)
• DTOs (has only data)
• Specifications (validation)
• Exceptions

|                   |  DTO | Value Object  | POCO / Entity   |
| -------------     | ---- | ------------- | ----            |
| Contains Data     | yes  |  yes          | yes             |
| Contains Logic    | no   | yes           | yes/no          |
| Has own Identity  | no   |  no           | yes/no          |

#### Domain
This will contain all entities, enums, exceptions, interfaces, types and logic specific to the domain layer.
#### Application
This layer contains all application logic and coordinates business actions. It also defines interfaces that are implemented by outside layers.
For example, if the application need to access a notification service, a new interface would be added to application and an implementation would be created within infrastructure.

 The Application layer is dependent on the domain layer and optionally also on a Utils project in our solution (general helpers and extensions, that we have created to encapsulate boilerplate code).
It can also include Nugget package references (for example Refit library, Newtonsoft Json, etc.)

The Application layer should be created following the CQRS principles and contain our MediatR handlers.

1. CQS - Command & Query separation principle:
	- A command is a method that mutates state (should be void in general)	
	- A Query   is a method that returns a result and does not modify a state

2. CQRS extends the CQS - now talking in terms of classes
    - queries start with "Get"
    - commands are named like "DoSomething" (no crud - naming, such as create, update, delete)
    - events are named like "SomethingHappened" (past tense, for example "RecordAdded" or "RecordAddedEvent")

*We can perform Database READ queries from the Application layer (for example using fast micro-ORM such as Dapper in our MediatR query handlers, but when we modify state
we should do it through service/repository interfaces. These interfaces should be implemented in the **Infrastructure** layer.*
<br/>

### Infrastructure

  The Infrastructure project typically includes data access implementations. In a typical ASP.NET Core web application, these implementations include the Entity Framework (EF) DbContext, 
  any EF Core Migration types that have been defined, and data access implementation classes. The most common way to abstract data access implementation code is through
the use of the Repository design pattern.

  In addition to data access implementations, the Infrastructure project should contain implementations of services that must interact with other infrastructure concerns. 
  These services should implement interfaces defined in the Application Core, and so Infrastructure should have a reference to the Application Core project. 
  If we find that we have services defined in Infrastructure that do not depend on any infrastructure-related types, see if we can move them into our Application Core project. 
  Generally, if we can move services into Application Core (without adding dependencies to this project), we should do so.

Infrastructure Types
• EF Core types (DbContext, Migrations)
• Data access implementation types (Repositories)
• Infrastructure-specific services (FileLogger, SmtpNotifier, etc.)


Infrastructure logic
- InMemory Data Cache
- EF Core DbContext, Migrations
-  Database interaction
- Service implementations (from Application core)
- Other WEB API Clients
- services with network calls and etc.
- Pub/Sub with Message Brokers (RabbitMQ / Kafka / etc.)
<br/>

### WebUI / Host

  This layer should have no buisness or infrastructure knowledge. It should reference the Application and Ifrastructure layers only in the Startup.cs file
  in order to configure their Dependency Injection registrations.

  The controllers should be simple. They should only send commands/queries through MediatR. These commands/queries will then be handled by their respective handlers in the Application layer.
  Mapping requests/responses is also allowed in a controller's action.

  Use DTOs in the end-points as request models and map them to Command/Query if we are not developing the client side. Otherwise no backward compatibility. 
  However, if we own and develop the clint side its okay to use directly the command/query as request model.

## Getting Started
The project is developed under Kubernetes environment. In order to run a local Kubernetes cluster on our machine we need to install [Minikube](https://minikube.sigs.k8s.io/docs/start/), [Kubefwd](https://kubefwd.com/) and have a hypervisor enabled, such as Hyper-V.
These instructions will get us a copy of the project up and running on our local machine for development and testing purposes.

### Prerequisites
+ [Enable Hyper-V](https://docs.microsoft.com/en-us/virtualization/hyper-v-on-windows/quick-start/enable-hyper-v) on our machine
+ Install [Minikube](https://minikube.sigs.k8s.io/docs/start/) by first installing  [Chocolatey](https://chocolatey.org/)
 1. Open a power shell command window as an admin
 2. Copy and paste the following command and then execute it
```
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
```
 3. After Chocolatey is installed open new power shell window, copy and paste the next command ```choco install minikube``` and execute it.

+ Services running in K8S (and Minikube respectively) are not visible to the outside world. In order to expose them for our development purposes, or just running the tests locally,
we need to install [Kubefwd](https://kubefwd.com/).
Installing kubefwd requires executing the following 3 commands in power shell window:
```
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
Invoke-Expression (New-Object System.Net.WebClient).DownloadString('https://get.scoop.sh')
scoop install kubefwd
```

+ At last we need to configure Environment Variables for ```kubectl``` and ```kubefwd```, so that when we execute power shell commands they are recognized.
To do so,  open Environment Variables menu and in the System Variables section add the followings:

Variable  | Value
------------- | -------------
kubectl  | 'path to kubectl folder on your machine'
kubefwd  | 'path to kubefwd folder on your machine'

Example:

Variable  | Value
------------- | -------------
kubectl  | C:\minikube
kubefwd  | C:\kubefwd

### Starting the local environment
Now that Minikube is installed we can proceed with starting the environment.
Open a new power shell windows as an admin and navigate to
...\project_folder and execute the following command
```
.\startEnv.ps1
```
This will execute a script, which basically schedules the deployments of our infrastructure dependencies in Minikube.
Optionally, when we see the message "Done! kubectl is now configured to use "minikube" by default" we can start a new powershell window and execute this command  ```minikube dashboard``` . 
It will start a window in our browser with kubernetes dashboard where we can check the progress of the deployments.
After everything becomes green the environment is ready.

Another optional command is ```minikube addons enable metrics-server```, which when executed, will show Memory/CPU usage for the available Pods in the Minikube dashboard.

more usefull commands:  
```kubectl top pods```                                    # Show metrics for all pods  
```kubectl top pod POD_NAME --containers```               # Show metrics for a given pod and its containers  
```kubectl top pod POD_NAME --sort-by=cpu```              # Show metrics for a given pod and sort it by 'cpu' or 'memory'  
```kubectl describe pods```                               # show info for pods  
```kubectl top nodes```                                   # Show metrics for all nodes  
```kubectl describe nodes```                              # show info for nodes  
