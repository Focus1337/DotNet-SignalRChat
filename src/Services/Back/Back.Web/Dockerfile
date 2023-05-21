FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Back.Web/Back.Web.csproj", "Services/Back.Web/"]
COPY ["Services/Back.Infrastructure/Back.Infrastructure.csproj", "Services/Back.Infrastructure/"]
COPY ["Services/Back.Application/Back.Application.csproj", "Services/Back.Application/"]
RUN dotnet restore "Services/Back.Web/Back.Web.csproj"
COPY . .
WORKDIR "/src/Services/Back.Web"
RUN dotnet build "Back.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Back.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Back.Web.dll"]
