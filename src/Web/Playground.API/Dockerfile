#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Web/Playground.API/Playground.API.csproj", "Web/Playground.API/"]
COPY ["Infrastructure/Playground.Infrastructure/Playground.Infrastructure.csproj", "Infrastructure/Playground.Infrastructure/"]
COPY ["Infrastructure/Playground.Messaging.Kafka/Playground.Messaging.Kafka.csproj", "Infrastructure/Playground.Messaging.Kafka/"]
COPY ["ApplicationCore/Playground.Application/Playground.Application.csproj", "ApplicationCore/Playground.Application/"]
COPY ["ApplicationCore/Playground.Utils/Playground.Utils.csproj", "ApplicationCore/Playground.Utils/"]
COPY ["ApplicationCore/Playground.Domain/Playground.Domain.csproj", "ApplicationCore/Playground.Domain/"]
COPY ["Infrastructure/Playground.Messaging.RabbitMq/Playground.Messaging.RabbitMq.csproj", "Infrastructure/Playground.Messaging.RabbitMq/"]
COPY ["Infrastructure/Playground.Persistence/Playground.Persistence.csproj", "Infrastructure/Playground.Persistence/"]
RUN dotnet restore "Web/Playground.API/Playground.API.csproj"
COPY . .
WORKDIR "/src/Web/Playground.API"
RUN dotnet build "Playground.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Playground.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Playground.API.dll"]
