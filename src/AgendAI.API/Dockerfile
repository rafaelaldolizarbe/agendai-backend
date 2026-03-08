# ─── Build ───────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia os csproj primeiro (cache de dependências)
COPY AgendAI.slnx .
COPY src/AgendAI.API/AgendAI.API.csproj src/AgendAI.API/
COPY src/AgendAI.Worker/AgendAI.Worker.csproj src/AgendAI.Worker/
COPY src/AgendAI.Core/AgendAI.Core.csproj src/AgendAI.Core/
COPY src/AgendAI.Infrastructure/AgendAI.Infrastructure.csproj src/AgendAI.Infrastructure/

# Restore separado para aproveitar cache do Docker
RUN dotnet restore

# Copia o restante do código
COPY . .

# Publica a API
RUN dotnet publish src/AgendAI.API/AgendAI.API.csproj \
    -c Release -o /app/publish 

# ─── Runtime ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Usuário não root por segurança
RUN useradd -m appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "AgendAI.API.dll"]