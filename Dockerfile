FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY backend/FinCore.Api/FinCore.Api.csproj backend/FinCore.Api/
RUN dotnet restore backend/FinCore.Api/FinCore.Api.csproj

COPY backend/FinCore.Api backend/FinCore.Api
RUN dotnet publish backend/FinCore.Api/FinCore.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FinCore.Api.dll"]
