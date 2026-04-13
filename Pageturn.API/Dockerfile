FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Pageturn.API/Pageturn.API.csproj", "Pageturn.API/"]
COPY ["Pageturn.Core/Pageturn.Core.csproj", "Pageturn.Core/"]
COPY ["Pageturn.Infrastructure/Pageturn.Infrastructure.csproj", "Pageturn.Infrastructure/"]

RUN dotnet restore "Pageturn.API/Pageturn.API.csproj"

COPY . .
WORKDIR "/src/Pageturn.API"
RUN dotnet build "Pageturn.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pageturn.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Pageturn.API.dll"]
