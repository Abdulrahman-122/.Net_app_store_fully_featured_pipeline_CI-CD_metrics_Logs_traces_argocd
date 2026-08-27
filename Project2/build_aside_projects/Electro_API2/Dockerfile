FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ElectroAPI.csproj ./

RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false   
# this will remove any using any .exe executable files which are related to microsoft instead it will use universal .dll files that are used with docker 

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

EXPOSE 8080     
#this app will be exposed on port 8080
COPY --from=build /app/publish	.
ENTRYPOINT ["dotnet","ElectroAPI.dll"]

