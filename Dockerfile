FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY NuGet.config ./
COPY AspireBudgetTelegramBot/*.csproj ./AspireBudgetTelegramBot/
RUN dotnet restore

# Copy everything else and build
COPY . ./
WORKDIR /app/AspireBudgetTelegramBot
RUN dotnet publish -c Release -o ../out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "AspireBudgetTelegramBot.dll"]