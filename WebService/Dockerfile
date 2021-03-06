#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
ARG ServiceHost

WORKDIR /app
EXPOSE 5000:80


FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["WebService/WebService.csproj", "WebService/"]
RUN dotnet restore "WebService/WebService.csproj"
COPY . .
WORKDIR "/src/WebService"
RUN dotnet build "WebService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WebService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebService.dll"]