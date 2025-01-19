FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ArbeidskravCloud/ArbeidskravCloud.csproj", "ArbeidskravCloud/"]
RUN dotnet restore "ArbeidskravCloud/ArbeidskravCloud.csproj"
COPY . .
WORKDIR "/src/ArbeidskravCloud"
RUN dotnet build "ArbeidskravCloud.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ArbeidskravCloud.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArbeidskravCloud.dll"]