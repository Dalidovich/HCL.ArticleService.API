#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HCL.ArticleService.API/HCL.ArticleService.API.csproj", "HCL.ArticleService.API/"]
COPY ["HCL.ArticleService.API.BLL/HCL.ArticleService.API.BLL.csproj", "HCL.ArticleService.API.BLL/"]
COPY ["HCL.ArticleService.API.DAL/HCL.ArticleService.API.DAL.csproj", "HCL.ArticleService.API.DAL/"]
COPY ["HCL.ArticleService.API.Domain/HCL.ArticleService.API.Domain.csproj", "HCL.ArticleService.API.Domain/"]
RUN dotnet restore "HCL.ArticleService.API/HCL.ArticleService.API.csproj"
COPY . .
WORKDIR "/src/HCL.ArticleService.API"
RUN dotnet build "HCL.ArticleService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HCL.ArticleService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HCL.ArticleService.API.dll"]