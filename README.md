
Backend
- .NET 10
- Dapper
- SQL Server
- Swagger

Frontend
- Next.js 16
- TypeScript
- Tailwind CSS
- App Router
- React Hooks

Pré-requisitos
- .NET 10 SDK
- Node.js 18+
- SQL Server (Express, Developer ou Standard)

Configuração e Execução
- Clonar o repositório 
git clone https://github.com/diegodvp/sales-system
cd SalesSolution

- Executar script
sqlcmd -S SEU_SERVIDOR -E -i "src\SalesAPI\Data\Scripts\CreateDatabase.sql"

- Configurar a connection string
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=SalesDB;User Id=SEU_USUARIO;Password=SUA_SENHA;TrustServerCertificate=True;"
  }
}

- Executar backend
dotnet restore
dotnet build
dotnet run --project src/SalesAPI

- Executar frontend
cd src/salesfrontend
npm install
npm run dev

- API disponível em: https://localhost:5001
- Aplicação disponível em: http://localhost:3000