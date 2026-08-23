# Build de teste/dev via docker-compose — não é a imagem de produção do TCC (isso é trabalho
# futuro, ver ARCHITECTURE.md §8: sem pipeline de CI/CD nem deploy configurados ainda).
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia só os .csproj primeiro para cachear o restore entre builds.
COPY src/FaeterjAcademico.Domain/FaeterjAcademico.Domain.csproj src/FaeterjAcademico.Domain/
COPY src/FaeterjAcademico.Application/FaeterjAcademico.Application.csproj src/FaeterjAcademico.Application/
COPY src/FaeterjAcademico.Infrastructure/FaeterjAcademico.Infrastructure.csproj src/FaeterjAcademico.Infrastructure/
COPY src/FaeterjAcademico.Api/FaeterjAcademico.Api.csproj src/FaeterjAcademico.Api/
RUN dotnet restore src/FaeterjAcademico.Api/FaeterjAcademico.Api.csproj

COPY src/ src/
RUN dotnet publish src/FaeterjAcademico.Api/FaeterjAcademico.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "FaeterjAcademico.Api.dll"]
