############################
# 1. 前端构建（Vue）
############################
FROM node:20-alpine AS frontend-build
WORKDIR /client

# 先只复制 package.json 以利用缓存
COPY backmanager.client/package*.json ./
RUN npm ci --no-audit --no-fund

# 复制前端所有源码并构建
COPY backmanager.client/ .
RUN npm run build


############################
# 2. 后端构建（ASP.NET Core）
############################
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-build
WORKDIR /src

# 复制 csproj 并还原依赖
COPY BackManager.Server/*.csproj ./BackManager.Server/
RUN dotnet restore BackManager.Server/BackManager.Server.csproj

# 复制后端所有源码
COPY BackManager.Server/ ./BackManager.Server/

# 将前端构建产物复制到后端的 wwwroot 目录
# 注意：这里确保目标路径存在
COPY --from=frontend-build /client/dist ./BackManager.Server/wwwroot

# 发布项目
# 修改点：移除了 --self-contained，因为运行时镜像里已经有环境了
# 修改点：使用 -p:UseAppHost=false 生成纯 DLL，适配容器启动
RUN dotnet publish BackManager.Server/BackManager.Server.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    -p:UseAppHost=false


############################
# 3. 运行时镜像
############################
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine
WORKDIR /app

# 从构建阶段复制发布的文件
COPY --from=backend-build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
# 建议监听 80 端口，或者保持你原来的 8081
ENV ASPNETCORE_URLS=http://+:8081

EXPOSE 8081

# 因为上面使用了 UseAppHost=false，这里直接运行 dll
ENTRYPOINT ["dotnet", "BackManager.Server.dll"]
