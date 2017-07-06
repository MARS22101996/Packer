FROM microsoft/aspnetcore:1.1.1

WORKDIR /app
COPY bin/Debug/netcoreapp1.0/publish /app
ENV ASPNETCORE_URLS http://*:5003
EXPOSE 5003

ENTRYPOINT ["dotnet","TicketService.WEB.dll"]
