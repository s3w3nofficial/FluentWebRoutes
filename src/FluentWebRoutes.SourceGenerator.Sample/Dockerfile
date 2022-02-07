FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/FluentWebRoutes.SourceGenerator.Sample/FluentWebRoutes.SourceGenerator.Sample.csproj", "FluentWebRoutes.SourceGenerator.Sample/"]
RUN dotnet restore "src/FluentWebRoutes.SourceGenerator.Sample/FluentWebRoutes.SourceGenerator.Sample.csproj"
COPY . .
WORKDIR "/src/FluentWebRoutes.SourceGenerator.Sample"
RUN dotnet build "FluentWebRoutes.SourceGenerator.Sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FluentWebRoutes.SourceGenerator.Sample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FluentWebRoutes.SourceGenerator.Sample.dll"]
