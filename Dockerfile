FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./*.sln .
COPY MailCore.Domain/*.csproj MailCore.Domain/
COPY MailCore.Application/*.csproj MailCore.Application/
COPY MailCore.Infrastructure/*.csproj MailCore.Infrastructure/
COPY MailCore.API/*.csproj MailCore.API/
RUN dotnet restore MailCore.API/MailCore.API.csproj

COPY . .
RUN dotnet publish MailCore.API -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "MailCore.API.dll"]
