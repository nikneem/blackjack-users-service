#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PollStar.Users.Api/PollStar.Users.Api.csproj", "PollStar.Users.Api/"]
RUN dotnet restore "PollStar.Users.Api/PollStar.Users.Api.csproj"
COPY . .
WORKDIR "/src/PollStar.Users.Api"
RUN dotnet build "PollStar.Users.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PollStar.Users.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PollStar.Users.Api.dll"]