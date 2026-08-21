FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/RF.Server.Core/RF.Server.Core.csproj", "src/RF.Server.Core/"]
COPY ["src/RF.Server.Api/RF.Server.Api.csproj", "src/RF.Server.Api/"]
RUN dotnet restore "src/RF.Server.Api/RF.Server.Api.csproj"
COPY . .
WORKDIR "/src/src/RF.Server.Api"
RUN dotnet publish "RF.Server.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RF.Server.Api.dll"]
