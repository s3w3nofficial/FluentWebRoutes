FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FluentWebRoutes.Sample/FluentWebRoutes.Sample.csproj", "FluentWebRoutes.Sample/"]
RUN dotnet restore "FluentWebRoutes.Sample/FluentWebRoutes.Sample.csproj"
COPY . .
WORKDIR "/src/FluentWebRoutes.Sample"
RUN dotnet build "FluentWebRoutes.Sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FluentWebRoutes.Sample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FluentWebRoutes.Sample.dll"]
