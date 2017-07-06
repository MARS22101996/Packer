FROM microsoft/aspnetcore:1.1.1

WORKDIR /app
COPY bin/Debug/netcoreapp1.0/publish /app
ENV ASPNETCORE_URLS http://*:5005
EXPOSE 5005

ENTRYPOINT ["dotnet","ApiGateway.dll"]
