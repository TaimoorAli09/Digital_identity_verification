# STAGE 1: BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["backend.csproj", "./"]
RUN dotnet restore "backend.csproj"

COPY . .

RUN dotnet publish "backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: RUNTIME
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

COPY keys /app/keys

ENV ASPNETCORE_URLS=http://+:5299
EXPOSE 5299

ENTRYPOINT ["dotnet", "backend.dll"]