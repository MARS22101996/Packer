FROM microsoft/aspnetcore:1.1.1

WORKDIR /app
COPY bin/Debug/netcoreapp1.0/publish /app
ENV ASPNETCORE_URLS http://*:5004
EXPOSE 5004

ENTRYPOINT ["dotnet","StatisticService.WEB.dll"]
