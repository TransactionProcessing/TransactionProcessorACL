FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TransactionProcessorACL/NuGet.Config", "."]
COPY ["TransactionProcessorACL/TransactionProcessorACL.csproj", "TransactionProcessorACL/"]
COPY ["TransactionProcessorACL.DataTransferObjects/TransactionProcessorACL.DataTransferObjects.csproj", "TransactionProcessorACL.DataTransferObjects/"]
COPY ["TransactionProcessorACL.BusinessLogic/TransactionProcessorACL.BusinessLogic.csproj", "TransactionProcessorACL.BusinessLogic/"]
COPY ["TransactionProcessorACL.Models/TransactionProcessorACL.Models.csproj", "TransactionProcessorACL.Models/"]
RUN dotnet restore "TransactionProcessorACL/TransactionProcessorACL.csproj"
COPY . .
WORKDIR "/src/TransactionProcessorACL"
RUN dotnet build "TransactionProcessorACL.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TransactionProcessorACL.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransactionProcessorACL.dll"]