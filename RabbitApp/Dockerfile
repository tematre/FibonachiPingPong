FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["RabbitApp/RabbitApp.csproj", "RabbitApp/"]
RUN dotnet restore "RabbitApp/RabbitApp.csproj"
COPY . .
WORKDIR "/src/RabbitApp"
RUN dotnet build "RabbitApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RabbitApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RabbitApp.dll"]
