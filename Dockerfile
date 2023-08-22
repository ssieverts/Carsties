#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 7002
EXPOSE 7001

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AuctionService/AuctionService.csproj", "AuctionService/"]
RUN dotnet restore "AuctionService/AuctionService.csproj"
COPY . .
WORKDIR "/src/AuctionService"
RUN dotnet build "AuctionService.csproj" -c Release -o /app/build
COPY ["SearchService/SearchService.csproj", "SearchService/"]
RUN dotnet restore "SearchService/SearchService.csproj"
COPY . .
WORKDIR "/src/SearchService"
RUN dotnet build "SearchService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuctionService.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN dotnet publish "SearchService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuctionService.dll"]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SearchService.dll"]
