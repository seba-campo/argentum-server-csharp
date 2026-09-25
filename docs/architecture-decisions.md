# Architecture Decision Records (ADRs)

Este documento registra las decisiones arquitectónicas clave tomadas a lo largo del desarrollo del servidor en C#.

---

## ADR 001: Elección de C# y .NET 9 como Lenguaje y Plataforma

* **Fecha:** Septiembre 2026
* **Estado:** Aceptado

### Contexto
El servidor original de Argentum Online (AO20) está desarrollado íntegramente en Visual Basic 6 (VB6), una tecnología deprecada sin soporte oficial de Microsoft, atada a 32-bit y dependencias COM obsoletas (`DX8VB.dll`, `msado15.dll`, OCXs). Se busca migrar a una tecnología moderna, mantenible y eficiente.

### Decisión
Elegir **C# (.NET 9)** como lenguaje y runtime para la reimplementación del servidor.

### Consecuencias
* **Positivas:**
  * Soporte multiplataforma nativo (desarrollo y ejecución en Windows, Linux y contenedores Docker).
  * Excelente rendimiento en procesamiento de paquetes de red y serialización binaria de baja latencia (`Span<T>`, `Memory<T>`, `System.IO.Pipelines`).
  * Curva de adopción fluida para desarrolladores con experiencia en TypeScript/JavaScript y backend web.
  * Ecosistema moderno de pruebas unitarias (`xUnit`), logging (`Microsoft.Extensions.Logging`, Serilog) y persistencia.
* **Desafíos:**
  * Debe replicarse con absoluta precisión el protocolo de paquetes binarios para no romper compatibilidad con el cliente original de AO.

---

## ADR 002: Modularización en Capas (Clean Architecture)

* **Fecha:** Septiembre 2026
* **Estado:** Aceptado

### Contexto
El servidor VB6 tiene un alto acoplamiento: módulos globales `.bas` acceden y mutan directamente el estado global (`UserList`, `MapData`, `Npclist`), mezclando lógica de red, persistencia en base de datos y reglas de combate en un mismo archivo.

### Decisión
Dividir el servidor en 5 proyectos con responsabilidades estrictamente delimitadas:
1. `Argentum.Core`: Modelos puros del dominio (entidades, enumeraciones, interfaces). Cero dependencias externas.
2. `Argentum.Network`: Serialización, deserialización y buffers de paquetes binarios. Depende de `Core`.
3. `Argentum.Gameplay`: Lógica de juego, cálculos de combate, movimiento, hechizos e inventario. Depende de `Core`.
4. `Argentum.Storage`: Capa de persistencia (SQLite / PostgreSQL / Dapper). Depende de `Core`.
5. `Argentum.Server`: Orquestador principal, punto de entrada (`Program.cs`), Game Loop y servidor TCP. Depende de todas las anteriores.
6. `Argentum.Tests`: Pruebas unitarias independientes para certificar la fidelidad de las fórmulas y protocolos.

### Consecuencias
* Permite probar la lógica de combate e inventario sin necesidad de sockets ni conexiones de red.
* Permite reemplazar el motor de base de datos o la biblioteca de red sin tocar la lógica del juego.

---

## ADR 003: Estado en Memoria Autoritativo y Persistencia Asíncrona (Write-Behind)

* **Fecha:** Septiembre 2026
* **Estado:** Aceptado

### Contexto
En un MMO en tiempo real con bucles de simulación (ticks) de 25-50ms, las consultas directas y sincrónicas a la base de datos introducen latencia inaceptable y congelamientos de juego (*lag spikes*). Al mismo tiempo, retrasar todo el guardado a intervalos periódicos puede provocar duplicación de ítems (*duping*) y pérdidas de progreso (*rollbacks*) si el proceso se detiene abruptamente.

### Decisión
1. **Autoridad en RAM:** El estado vivo del juego (coordenadas, vida actual, maná, inventarios) reside 100% en memoria en el servidor para responder en tiempo real a los clientes.
2. **Operaciones Económicas Críticas:** Acciones de alto valor (intercambio entre usuarios, compras en subasta, depósitos de banco) se validan en memoria y despachan un evento transaccional asíncrono e inmediato a una cola de persistencia (`Channel<T>`), asegurando que se escriba en disco sin bloquear el Game Loop.
3. **Estado Volátil:** Posiciones de navegación, hambre, sed y estadísticas temporales se persisten en intervalos periódicos o al desloguear.
