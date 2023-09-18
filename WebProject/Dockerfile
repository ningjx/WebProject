FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS publish
COPY . /app/publish/

FROM base AS final
COPY --from=publish /app/publish .
#VOLUME /home/Web/logs:/app/publish/WebProjects/WebProjects/logs
ENTRYPOINT ["dotnet", "WebProject.dll"]