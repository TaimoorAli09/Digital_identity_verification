# STAGE 1: BUILD
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["backend.csproj", "./"]
RUN dotnet restore "backend.csproj"

COPY . .
RUN dotnet build "backend.csproj" -c Release -o /app/build


# STAGE 2: PUBLISH
FROM build AS publish
RUN dotnet publish "backend.csproj" -c Release -o /app/publish /p:UseAppHost=false


# STAGE 3: RUNTIME
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

# ✅ IMPORTANT: COPY KEYS HERE (runtime stage)
COPY keys /app/keys

ENV ASPNETCORE_URLS=http://+:5299
EXPOSE 5299

ENTRYPOINT ["dotnet", "backend.dll"]