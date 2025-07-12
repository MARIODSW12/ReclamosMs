FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
WORKDIR /src/Reclamos.API

RUN dotnet restore Reclamos.API.sln

RUN dotnet publish Reclamos.API.sln -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /out .

EXPOSE 80
ENTRYPOINT ["dotnet", "Reclamos.API.dll"]
