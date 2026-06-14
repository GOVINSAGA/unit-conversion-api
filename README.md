# Unit Conversion API

A RESTful API built with **ASP.NET Core (.NET 10)** for converting numerical values between different units of measurement.

## Supported Conversion Categories

| Category | Units |
|---|---|
| **Length** | meter, kilometer, centimeter, millimeter, mile, yard, foot, inch |
| **Temperature** | celsius, fahrenheit, kelvin |
| **Weight/Mass** | kilogram, gram, milligram, pound, ounce, ton |
| **Volume** | liter, milliliter, gallon, quart, pint, cup, fluid_ounce |
| **Speed** | meters_per_second, kilometers_per_hour, miles_per_hour, knot |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or later)

### Run Locally

```bash
# Clone the repository
git clone <repo-url>
cd <repo-name>

### Run the API (Backend)

```bash
# Run the API
dotnet run --project src/UnitConversionApi

# The API will be available at https://localhost:5001 (or http://localhost:5038)
# Scalar API docs UI is served at the root
```

### Run the UI (Frontend)

The project includes a modern Angular frontend. To run it, you need [Node.js](https://nodejs.org/) installed.

```bash
# Open a new terminal and navigate to the frontend directory
cd frontend

# Install dependencies
npm install

# Start the Angular development server
npm start

# The UI will be available at http://localhost:4200
```

### Run Tests

```bash
dotnet test
```

### Run with Docker

```bash
# Build and run with Docker Compose
docker compose up --build

# API available at http://localhost:8080
```

---

## API Endpoints

### Convert a Value

```
GET /api/convert?value={value}&from={fromUnit}&to={toUnit}
```

**Example:**
```bash
curl "https://localhost:5001/api/convert?value=100&from=meter&to=foot"
```

**Response (200 OK):**
```json
{
  "value": 100,
  "fromUnit": "meter",
  "toUnit": "foot",
  "result": 328.08399,
  "category": "length"
}
```

**Error Response (400 Bad Request):**
```json
{
  "title": "Invalid Conversion Request",
  "detail": "Incompatible units: 'meter' (length) cannot be converted to 'celsius' (temperature). Both units must belong to the same measurement category.",
  "status": 400
}
```

### List All Supported Units

```
GET /api/units
```

**Response:**
```json
[
  {
    "category": "length",
    "units": ["meter", "kilometer", "centimeter", "millimeter", "mile", "yard", "foot", "inch"]
  },
  {
    "category": "temperature",
    "units": ["celsius", "fahrenheit", "kelvin"]
  }
]
```

### List Units by Category

```
GET /api/units/{category}
```

**Example:**
```bash
curl "https://localhost:5001/api/units/temperature"
```

---

## Architecture & Design Decisions

### Project Structure

```
├── src/
│   └── UnitConversionApi/          # Main API project
│       ├── Controllers/            # API endpoints
│       ├── Models/                 # Request/response DTOs
│       ├── Services/               # Business logic
│       │   └── Converters/         # Individual conversion strategies
│       └── Middleware/             # Cross-cutting concerns
├── tests/
│   └── UnitConversionApi.Tests/    # Unit + integration tests
├── Dockerfile                      # Multi-stage Docker build
├── docker-compose.yml
└── .editorconfig                   # Code style enforcement
```

### Key Design Decisions

1. **Strategy Pattern for Converters**
   Each conversion category is implemented as a separate class conforming to the `IUnitConverter` interface. This makes the system easily extensible — adding a new category requires only creating a new class, with zero changes to existing code.

2. **Factor-Based vs Formula-Based Conversion**
   - **Length, Weight, Volume, Speed** use a `FactorBasedConverter` base class. Each unit has a conversion factor relative to a base unit (e.g., meter, kilogram). This approach scales cleanly to any number of units.
   - **Temperature** uses direct formula conversion (Celsius ↔ Fahrenheit is non-linear: `F = C × 9/5 + 32`), so it implements `IUnitConverter` directly with formula dispatch.

3. **GET Endpoint for Conversions**
   The conversion operation is a pure, idempotent query with no side effects. `GET` is the semantically correct HTTP verb, and it enables easy testing via browser, bookmarks, and caching.

4. **RFC 7807 ProblemDetails for Errors**
   All error responses follow the [RFC 7807](https://www.rfc-editor.org/rfc/rfc7807) standard, providing structured, machine-readable error information.

5. **Native OpenAPI + Scalar UI**
   Uses .NET 10's built-in `AddOpenApi()` instead of Swashbuckle (which is deprecated), paired with [Scalar](https://scalar.com/) for a modern, interactive API documentation UI.

6. **Dependency Injection**
   All converters are registered via DI as `IUnitConverter` singletons. The `UnitConverterRegistry` aggregates them automatically — no manual wiring needed when adding new converters.

### Trade-offs

| Decision | Trade-off |
|---|---|
| **Hardcoded conversion data** | Simple for now; a production system would use a database or configuration file for dynamic unit management. |
| **Single-process, in-memory** | No external dependencies needed; for "hundreds of units" at scale, consider caching or a lookup table in a database. |
| **`double` precision** | Sufficient for most real-world conversions but not suitable for financial/scientific precision (would need `decimal`). |

### Extensibility: Adding a New Conversion Category

1. Create a new class in `Services/Converters/` implementing `IUnitConverter` (or extending `FactorBasedConverter`):

```csharp
public class AreaConverter : FactorBasedConverter
{
    public AreaConverter() : base(new Dictionary<string, double>
    {
        ["square_meter"] = 1.0,
        ["square_kilometer"] = 1_000_000.0,
        ["acre"] = 4_046.8564224,
        ["hectare"] = 10_000.0
    }) { }

    public override string Category => "area";
}
```

2. Register it in `Program.cs`:

```csharp
builder.Services.AddSingleton<IUnitConverter, AreaConverter>();
```

That's it — the registry and API endpoints automatically discover and serve the new category.

---

## Testing

The solution includes **69 tests** across three layers:

- **Unit Tests** — Individual converter accuracy (known conversions, edge cases, bidirectional consistency)
- **Service Tests** — Registry dispatch logic, error handling
- **Integration Tests** — Full HTTP pipeline via `WebApplicationFactory` (happy paths, validation errors, 404s)

```bash
dotnet test --verbosity normal
```
