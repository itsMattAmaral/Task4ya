FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8000
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY Task4ya.Api/Task4ya.Api.csproj Task4ya.Api/
COPY Task4ya.Application/Task4ya.Application.csproj Task4ya.Application/
COPY Task4ya.Domain/Task4ya.Domain.csproj Task4ya.Domain/
COPY Task4ya.Infrastructure/Task4ya.Infrastructure.csproj Task4ya.Infrastructure/
RUN dotnet restore Task4ya.Api/Task4ya.Api.csproj

# Copy all source code and build
COPY . .
RUN dotnet build Task4ya.Api/Task4ya.Api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Task4ya.Api/Task4ya.Api.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY https/aspnetapp.pfx /https/aspnetapp.pfx

ENV ASPNETCORE_URLS=http://+:8000;https://+:8443
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Task4ya.Api.dll"]
