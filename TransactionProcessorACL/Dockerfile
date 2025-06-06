FROM stuartferguson/txnprocbase:latest AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
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

# Sort out certificate stuff here
RUN openssl x509 -inform DER -in /src/TransactionProcessorACL/aspnetapp-root-cert.cer -out /src/TransactionProcessorACL/aspnetapp-root-cert.crt
RUN cp /src/TransactionProcessorACL/aspnetapp-root-cert.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates

FROM build AS publish
RUN dotnet publish "TransactionProcessorACL.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransactionProcessorACL.dll"]