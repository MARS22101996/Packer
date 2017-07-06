FROM microsoft/aspnetcore:1.1.1

WORKDIR /app
COPY bin/Debug/netcoreapp1.0/publish /app
ENV ASPNETCORE_URLS http://*:5002
EXPOSE 5002

ENTRYPOINT ["dotnet","TeamService.WEB.dll"]
