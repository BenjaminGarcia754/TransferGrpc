FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR ServidorGrpc

EXPOSE 9090
EXPOSE 9091
EXPOSE 80

#COPY Project Files

COPY ./*csproj ./
RUN dotnet restore

#COPY everything else and build
COPY . .
RUN dotnet publish -c Release -o out

#Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /ServidorGrpc
COPY --from=build /ServidorGrpc/out .
ENTRYPOINT ["dotnet", "TransferGrpc.dll"]