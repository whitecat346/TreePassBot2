# 基础镜像：.NET运行时
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Node.js构建环境
FROM node:20-alpine AS node-build
WORKDIR /client

# 复制前端项目文件
COPY ["backmanager.client/package.json", "backmanager.client/package-lock.json*", "./"]
# 安装前端依赖，使用缓存
RUN npm ci --no-audit --no-fund

# .NET构建环境
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 复制后端项目文件
COPY ["BackManager/BackManager.Server/BackManager.Server.csproj", "BackManager/BackManager.Server/"]
COPY ["backmanager.client/backmanager.client.esproj", "backmanager.client/"]

# 恢复.NET依赖
RUN dotnet restore "./BackManager/BackManager.Server/BackManager.Server.csproj"

# 复制前端构建产物
COPY --from=node-build /client/node_modules /src/backmanager.client/node_modules

# 复制所有项目文件
COPY . .

# 构建后端项目
WORKDIR "/src/BackManager/BackManager.Server"
RUN dotnet build "./BackManager.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build --verbosity minimal

# 发布项目
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BackManager.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --self-contained false --verbosity minimal

# 最终生产镜像
FROM base AS final
WORKDIR /app

# 复制发布产物
COPY --from=publish /app/publish .

# 设置环境变量
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true

# 启动应用
ENTRYPOINT ["dotnet", "BackManager.Server.dll"]
