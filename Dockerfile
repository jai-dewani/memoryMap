FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

COPY . ./ 
RUN dotnet restore

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /App
COPY --from=build-env /App/out ./
CMD ["dotnet", "codecrafters-redis.dll", "--port", "8080"]