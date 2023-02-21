FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS publish
COPY /bin/Release/net6.0/ /app/publish/

FROM base AS final
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebProject.dll"]