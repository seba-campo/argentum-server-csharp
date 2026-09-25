# Argentum Online Server (C# / .NET 9)

Reimplementación moderna, modular y de alto rendimiento del servidor de **Argentum Online (AO20)** en C# (.NET 9), manteniendo compatibilidad con el protocolo de red y la esencia del juego original.

## Arquitectura de la Solución

El proyecto está organizado bajo los estándares de Clean Architecture y la convención de Microsoft:

```text
argentum-server-csharp/
├── docs/                                 # Base de conocimiento, ADRs y especificaciones
│   ├── README.md
│   └── architecture-decisions.md         # Registro de decisiones de arquitectura (ADRs)
│
├── src/
│   ├── Argentum.Core/                    # Dominio puro: modelos (User, Map, Item), enums y contratos
│   ├── Argentum.Network/                 # Protocolo binario: NetReader, NetWriter, PacketIds, paquetes
│   ├── Argentum.Gameplay/                # Reglas: combate, hechizos, inventario, fórmulas
│   ├── Argentum.Storage/                 # Persistencia: SQLite (Database.db) / Repositorios
│   └── Argentum.Server/                  # Host ejecutable: Game Loop (Tick Engine) y Listener TCP
│
└── tests/
    └── Argentum.Tests/                   # Pruebas unitarias automatizadas (xUnit)
```

## Requisitos de Desarrollo

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (o superior)
- Visual Studio 2022 (con carga de trabajo *.NET desktop* o *ASP.NET*) o VS Code con la extensión *C# Dev Kit*

## Comandos Principales

```bash
# Compilar toda la solución
dotnet build

# Ejecutar las pruebas unitarias
dotnet test

# Iniciar el servidor
dotnet run --project src/Argentum.Server
```
