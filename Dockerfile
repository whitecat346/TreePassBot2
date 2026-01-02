############################
# 1. 前端构建（Vue）
############################
FROM node:20-alpine AS frontend-build
WORKDIR /src

# 注意：路径变成了 BackManager/backmanager.client/...
COPY BackManager/backmanager.client/package*.json ./BackManager/backmanager.client/

# 进入前端目录安装依赖
WORKDIR /src/BackManager/backmanager.client
RUN npm ci --no-audit --no-fund

# 复制前端源码（注意上下文路径）
COPY BackManager/backmanager.client/ .
RUN npm run build


############################
# 2. 后端构建（ASP.NET Core）
############################
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-build
WORKDIR /src

# ---------------------------------------------------------
# 关键步骤：复制所有涉及到的 csproj 文件以利用层缓存
# ---------------------------------------------------------
COPY ["TreePassBot2.slnx", "./"]
COPY ["BackManager/BackManager.Server/BackManager.Server.csproj", "BackManager/BackManager.Server/"]
COPY ["TreePassBot2.Data/TreePassBot2.Data.csproj", "TreePassBot2.Data/"]
COPY ["TreePassBot2.Core/TreePassBot2.Core.csproj", "TreePassBot2.Core/"]
COPY ["TreePassBot2.Infrastructure/TreePassBot2.Infrastructure.csproj", "TreePassBot2.Infrastructure/"]
COPY ["TreePassBot2.PluginSdk/TreePassBot2.PluginSdk.csproj", "TreePassBot2.PluginSdk/"]
COPY ["TreePassBot2.ServiceDefaults/TreePassBot2.ServiceDefaults.csproj", "TreePassBot2.ServiceDefaults/"]
COPY ["TreePassBot2.BotEngine/TreePassBot2.BotEngine.csproj", "TreePassBot2.BotEngine/"]
# 如果还有其他依赖，请在这里继续添加 COPY

# 还原依赖（此时 Docker 已经拥有了所有必要的 csproj）
RUN dotnet restore "BackManager/BackManager.Server/BackManager.Server.csproj"

# ---------------------------------------------------------
# 复制剩余的所有源代码
# ---------------------------------------------------------
COPY . .

# 将前端构建产物复制到后端的 wwwroot 目录
# 注意：源路径和目标路径都发生了变化
COPY --from=frontend-build /src/BackManager/backmanager.client/dist ./BackManager/BackManager.Server/wwwroot

# 发布
WORKDIR /src/BackManager/BackManager.Server
RUN dotnet publish "BackManager.Server.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    -p:UseAppHost=false \
    -p:DockerBuild=true


############################
# 3. 运行时镜像
############################
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app

COPY --from=backend-build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8081

EXPOSE 8081

ENTRYPOINT ["dotnet", "BackManager.Server.dll"]
