# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG PROJ_DIR
ARG PROJ_FILE
WORKDIR /src

COPY . .

# 1. CLEANUP
RUN find . -name "AssemblyInfo.cs" -delete
RUN find . -type d -name "bin" -exec rm -rf {} +
RUN find . -type d -name "obj" -exec rm -rf {} +

# 2. FORCE PROJECT SETTINGS
RUN sed -i '/<PropertyGroup>/a <OutputType>Exe</OutputType><GenerateProgramFile>false</GenerateProgramFile>' "${PROJ_DIR}/${PROJ_FILE}"

# 3. PACKAGES
RUN dotnet add "${PROJ_DIR}/${PROJ_FILE}" package RabbitMQ.Client --version 6.8.1
RUN dotnet add "${PROJ_DIR}/${PROJ_FILE}" package Newtonsoft.Json
RUN dotnet add "${PROJ_DIR}/${PROJ_FILE}" package Swashbuckle.AspNetCore

# 4. BUILD
RUN dotnet restore "${PROJ_DIR}/${PROJ_FILE}"
RUN dotnet publish "${PROJ_DIR}/${PROJ_FILE}" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# 5. AUTOMATIC ENTRYPOINT
# This finds the only DLL that isn't a system library and runs it
ENTRYPOINT ["sh", "-c", "dotnet $(ls *.dll | grep -ivE 'Microsoft|Newtonsoft|RabbitMQ|System|Swashbuckle' | head -n 1)"]