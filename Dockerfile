# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ./src ./

RUN dotnet restore CustomerApi/CustomerApi.Web.csproj

RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN adduser --disabled-password --home /appuser appuser
USER appuser

COPY --from=build /app/publish ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "CustomerApi.Web.dll"]