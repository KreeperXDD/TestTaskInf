FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["backend/TestTask.Api/TestTask.Api.csproj", "backend/TestTask.Api/"]
RUN dotnet restore "backend/TestTask.Api/TestTask.Api.csproj"

COPY . .
WORKDIR "/src/backend/TestTask.Api"
RUN dotnet publish "TestTask.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TestTask.Api.dll"]