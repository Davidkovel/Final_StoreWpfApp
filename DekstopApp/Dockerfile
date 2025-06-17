FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build

WORKDIR /app

COPY ShopWpfApp .

RUN dotnet restore

RUN msbuild /t:Build /p:Configuration=Release ShopWpfApp.sln




# Docker compose file:
    #services:
    #  mssql:
    #    image: mcr.microsoft.com/mssql/server:2022-latest
    #    environment:
    #      - ACCEPT_EULA=Y
    #      - SA_PASSWORD=123456I!@
    #    ports:
    #        - "1433:1433"
    #    volumes:
    #      - ./data:/var/opt/mssql/data
    #      - ./log:/var/opt/mssql/log
    #      - ./secrets:/var/opt/mssql/secrets
    #  
    #  redis:
    #    image: redis:latest
    #    ports:
    #        - "6379:6379"
    #    volumes:
    #      - ./storage/redis/data:/data
    #    restart: always
    #  
    #  storewpfapp:
    #    build:
    #      context: ./DekstopApp
    #      dockerfile: Dockerfile
    #    depends_on:
    #      - mssql
    #      - redis
    #    ports:
    #        - "8080:80"
    #    environment:
    #      - DB_CONNECTION_STRING=Server=mssql;Database=Shop;User Id=sa;Password=123456I!@;
    #      - REDIS_CONNECTION_STRING=redis://Redis:6379
        