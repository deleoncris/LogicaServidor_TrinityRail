FROM mcr.microsoft.com/dotnet/sdk:10.0 AS publish
WORKDIR /src

COPY ["LogicaServidor.csproj", "./"]
RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

RUN apt-get update && apt-get install -y \
    iproute2 \
    nmap \
    gawk \
    ipcalc-ng \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "LogicaServidor.dll"]