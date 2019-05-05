FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src

COPY src/*/*/*.csproj /src/csproj-files/
COPY tests/*/*/*.csproj /src/csproj-files/

ARG RUN=pwd
RUN ${RUN}

COPY . .
WORKDIR /src/src/presentation/Shop.Web/Shop.Web
RUN dotnet publish -c Release -o /app

FROM build as unittest
WORKDIR /src/tests/core/Services.Tests/Services.Tests

FROM build as functionaltest
WORKDIR /src/tests/presentation/Web.Tests/Web.Tests

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Shop.Web.dll"]