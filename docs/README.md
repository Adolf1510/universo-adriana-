# BoticaPOS (.NET 8 WPF)

## Qué se creó
Solución profesional con arquitectura por capas para botica/farmacia:
- `BoticaPOS.UI`: WPF + MVVM + DI + manejo global de errores + logging.
- `BoticaPOS.Application`: casos de uso (ventas/comprobantes), DTOs y validaciones FluentValidation.
- `BoticaPOS.Domain`: entidades y reglas FEFO, numeración y restauración de stock.
- `BoticaPOS.Infrastructure`: SQL Server (Dapper), repositorios async, unit-of-work transaccional, cache y sender stub.
- `BoticaPOS.Tests`: unit tests de reglas núcleo + integración opcional.

## Ubicación de archivos clave
- Solución: `/BoticaPOS.sln`
- Script SQL completo: `/database/BoticaPOS.sql`
- Startup WPF + Serilog + DI: `/BoticaPOS.UI/App.xaml.cs`
- Regla FEFO: `/BoticaPOS.Domain/Services/FefoAllocator.cs`
- Venta transaccional: `/BoticaPOS.Application/Services/SalesService.cs`
- Reversa/anulación transaccional: `/BoticaPOS.Infrastructure/Repositories/VentaRepository.cs`

## Prerrequisitos
1. Visual Studio 2022 (17.8+)
2. .NET 8 SDK
3. SQL Server Express (instancia `\.\SQLEXPRESS`)
4. SSMS

## Configuración DB
1. Abrir SSMS y conectarse a `\.\SQLEXPRESS`.
2. Ejecutar `database/BoticaPOS.sql` completo.
3. Validar creación de tablas e índices.

## Configuración app
1. Revisar `BoticaPOS.UI/appsettings.json`.
2. `ConnectionStrings:BoticaPOS` por defecto:
   `Server=.\SQLEXPRESS;Database=BoticaPOS;Trusted_Connection=True;TrustServerCertificate=True;`

## Build y ejecución
1. Abrir `BoticaPOS.sln`.
2. Restaurar paquetes NuGet.
3. Seleccionar `BoticaPOS.UI` como startup project.
4. Ejecutar (F5).

## Ejecución de pruebas
- Unit tests:
  ```bash
  dotnet test BoticaPOS.Tests/BoticaPOS.Tests.csproj
  ```
- Integración opcional (por defecto saltable):
  ```bash
  RUN_INTEGRATION_TESTS=1 dotnet test BoticaPOS.Tests/BoticaPOS.Tests.csproj --filter SaleTransactionIntegrationTests
  ```

## Troubleshooting
- **No conecta a SQL**: validar nombre de instancia (`.\SQLEXPRESS`) y permisos de Windows.
- **Error de connection string**: actualizar `appsettings.json`.
- **Usuario admin**: seed crea `admin` con `PasswordHash=NULL` y `RequirePasswordChange=1`; implementar cambio de clave en primer ingreso.
- **Envió electrónico**: módulo es `FakeInvoiceSender` (stub offline-first), falla simulada configurable por `%`.

## Confiabilidad implementada
- Confirmación de venta atómica bajo `UnitOfWork`.
- Anulación de venta atómica con restauración de stock + auditoría.
- Sender electrónico desacoplado para no romper flujo POS.
- Acceso DB async y logging estructurado a archivos rolling.
