FROM mcr.microsoft.com/dotnet/sdk:7.0
WORKDIR /app

# Copy everything
COPY . .

EXPOSE 5000

ENTRYPOINT ["dotnet", "run"]