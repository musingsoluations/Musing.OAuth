Copilot Instructions — Web API (Clean Architecture)

Purpose: provide clear, actionable instructions for GitHub Copilot (or any AI assistant) when authoring, refactoring, or reviewing code for the Web API project. This project is a .NET 10 Web API using Clean Architecture, PostgreSQL (Postgres) with EF Core, Wolverine for internal messaging, and Riok.Mapperly for mapping.

⸻

Quick facts / tech stack
	•	Platform: .NET 10 (ASP.NET Core)
	•	Architecture: Clean Architecture (Presentation → Application → Domain → Infrastructure)
	•	Database: PostgreSQL (use Npgsql provider)
	•	ORM: EF Core (latest stable compatible with .NET 10)
	•	Messaging / Mediation: Wolverine (used instead of MediatR)
	•	Mapper: Riok.Mapperly (source-generator based mapper; not AutoMapper)
	•	DI / Hosting: Built-in ASP.NET Core DI + Wolverine extensions
	•	Code analysis: Roslyn analyzers, StyleCop or EditorConfig rules, nullable references enabled
	•	Security: OAuth2/OIDC (IdentityServer or cloud provider), TLS, principle of least privilege, secrets in Key Vault/Secret Manager
	•	Observability: OpenTelemetry, Serilog (structured logging), Prometheus/Grafana optionally

⸻

High-level guidance for Copilot

When you generate code, follow these constraints and preferences:
	1.	Respect Clean Architecture: Producers should place code into Presentation (API controllers, minimal API endpoints), Application (use cases, commands/queries, DTOs), Domain (entities, value objects, domain services), and Infrastructure (EF Core, repositories, external integrations). Do not leak infrastructure types into Application or Domain.
	2.	Prefer explicit, small APIs: Keep controller endpoints focused. Prefer small request/response DTOs rather than exposing EF entities.
	3.	Async-first: Use async/await for all I/O operations (EF Core, HttpClient, file I/O). Return Task<T> or ValueTask<T> where it makes sense; avoid blocking calls. Always accept CancellationToken from controller actions and pass it through to downstream calls.
	4.	Use Wolverine for in-process messaging: Implement command/notification handlers as Wolverine handlers. Keep messages immutable and serializable.
	5.	Use Riok.Mapperly: Generate mapping code with Mapperly. Prefer DTO projection using ProjectTo... equivalent patterns (i.e., use Select with compiled expressions or Mapperly expression mapping) to avoid loading whole entities when possible.
	6.	EF Core best practices: Clean, parameterised queries; avoid client evaluation; use AsNoTracking() on read-only queries; prefer projection to DTOs; use explicit Include only when necessary; use compiled queries for hot paths; limit result sizes; prefer pagination; use DbContext per-unit-of-work (Scoped lifetime).
	7.	Security-first mindset: Validate inputs, sanitize strings used in SQL (but rely on parameterized queries), enable HTTPS, secure CORS with specific origins, use policies & role-based authorization, log access events (PII redaction), protect secrets using Key Vault/Secret Manager.
	8.	Testing & CI: Generate unit tests for Application and Domain logic (Xunit/NUnit). Integration tests for EF Core use either Testcontainers or in-memory Postgres. Add contract-style tests for messages and handlers.

⸻

Project layout (folders / projects)

Recommended solution layout (each is a separate project):
	•	src/Api — ASP.NET Core Web API project (Presentation)
	•	Minimal API or Controllers
	•	DTOs for requests/responses
	•	FluentValidation request validators (optional)
	•	OpenAPI/Swagger configuration
	•	API versioning and health checks
	•	src/Application — Application layer
	•	Use cases (Commands/Queries) expressed as Wolverine messages/handlers
	•	Contracts (DTOs, interfaces)
	•	Application services (business orchestration)
	•	Mapping interfaces (Mapperly generated types referenced here)
	•	Unit tests target this project
	•	src/Domain — Domain layer
	•	Entities, value objects, domain events, domain exceptions
	•	Domain interfaces (persistence abstractions) — keep free of EF Core
	•	src/Infrastructure — Infrastructure layer
	•	EF Core DbContext, Migrations
	•	Repository implementations (if used), store implementations
	•	Wolverine configuration for durability/endpoints (if using outbox/transports)
	•	External integrations (SMTP, 3rd party APIs)
	•	tests/ — Test projects
	•	Application.Tests — unit tests
	•	Integration.Tests — integration tests with Postgres

⸻

Naming & code conventions
	•	Enable nullable references everywhere. Treat warnings as errors in CI.
	•	Use PascalCase for types and public members; camelCase for local variables and parameters.
	•	Keep methods short (max ~50–80 lines); prefer extraction to private methods or domain services.
	•	Use I prefix for interfaces (e.g., IUserRepository). Keep implementation names descriptive (e.g., PostgresUserRepository).
	•	Avoid async void except for top-level event handlers. Prefer Task/Task<T>.

EditorConfig / style rules: include a project .editorconfig and enable analyzers such as Microsoft.CodeAnalysis.FxCopAnalyzers, Roslynator, and dotnet_style_* rules.

⸻

EF Core — practical rules and examples

DbContext & lifetime
	•	Register DbContext as services.AddDbContext<AppDbContext>(options => ...) with Scoped lifetime.
	•	Keep DbContext short-lived and not shared across threads. Don’t store it in singletons.

Migrations & schema
	•	Keep migrations in Infrastructure project. Use named migrations and review generated SQL.
	•	Use explicit HasColumnType when precision matters (money/decimal, timestamps).

Querying
	•	Always prefer projection to DTOs: context.Users.Where(...).Select(u => new UserDto { ... }).
	•	Use AsNoTracking() for read-only queries.
	•	Avoid Include unless you need the related entities. When including many collections, consider .AsSplitQuery() to avoid cartesian explosion.
	•	For frequently-run queries, consider EF.CompileQuery(...) to reduce expression compilation overhead.

Saving & concurrency
	•	Use optimistic concurrency tokens (rowversion / xmin / timestamp) for critical concurrent updates.
	•	Use SaveChangesAsync(cancellationToken) and handle DbUpdateConcurrencyException gracefully.

Performance
	•	Avoid N+1 by using projection or carefully planned Include.
	•	Use Take/Skip for pagination. Consider keyset pagination for large datasets.
	•	Use connection pooling (default in Npgsql) and tune maximum pool size in connection string if needed.

⸻

Wolverine (instead of MediatR)
	•	Prefer Wolverine handlers for commands/events in Application layer. Keep messages immutable (records are great).
	•	Configure Wolverine in Program.cs/Startup using builder.Services.AddWolverine().
	•	Use Wolverine durable inbox/outbox if you need guaranteed delivery; prefer transactional outbox patterns when publishing events as part of a DB transaction.
	•	Keep message contracts small; version messages carefully.
	•	Write integration tests for message handlers using Wolverine’s test harness.

⸻

Riok.Mapperly guidance
	•	Define mapping interfaces in Application layer (e.g., IUserMapper), and implement them using Mapperly attributes or partial classes.
	•	Prefer expression-based mapping or projection helpers for EF Core queries. Mapperly can generate Expression<Func<T, TResult>> mappings which are suitable for projection.
	•	Keep mapping logic simple — push complex mapping logic into domain services if it contains business rules.
	•	Unit test mappings to ensure fields map correctly.

Example Mapperly pattern:

[Mapper]
public partial class UserMapper
{
    public partial UserDto Map(User entity);
    public static partial Expression<Func<User, UserDto>> Projection();
}


⸻

Security checklist (must follow)
	•	Authentication & Authorization
	•	Use OAuth2/OIDC; centralize auth/roles in API gateway or Identity provider.
	•	Use policy-based authorization (policies over role-strings where possible).
	•	Transport & secrets
	•	Enforce TLS everywhere. HSTS headers.
	•	Store secrets in Azure Key Vault, AWS Secrets Manager, or environment variables for local dev via Secret Manager.
	•	Avoid storing credentials in source control.
	•	Input validation & sanitization
	•	Use FluentValidation or DataAnnotations for model validation.
	•	Never build SQL or dynamic EF queries by concatenating strings.
	•	Data handling
	•	Mask or avoid logging PII; use structured logging with dedicated fields and avoid logging full objects.
	•	Encrypt sensitive data at rest if required (Postgres encryption features or application-level encryption).
	•	Network & DB privileges
	•	Use least privilege for DB accounts; separate read-only and read-write roles where useful.
	•	Use firewall rules / VPC for DB access.
	•	OWASP precautions
	•	Validate inputs, set secure headers (CSP, X-Frame-Options, X-Content-Type-Options), rate limit endpoints, and protect against CSRF for cookie-based flows.
	•	Dependency hygiene
	•	Keep NuGet packages updated, review transitive dependencies, run third-party vulnerability scanning in CI.

⸻

Async programming best practices
	•	Prefer async all the way down for I/O bound work. Do not use Task.Run to offload CPU-bound work except in rare scenarios.
	•	Accept and pass CancellationToken everywhere that may be cancelled (controllers, repository methods, network calls).
	•	Prefer IAsyncEnumerable<T> for streaming large result sets. Use await foreach for consumption.
	•	Use ValueTask<T> only for extremely hot methods where allocation matters and the method may complete synchronously often.
	•	Beware of ConfigureAwait(false) — not necessary in ASP.NET Core server-side code, but acceptable in libraries intended for wider consumption.
	•	Avoid Task.Result / .GetAwaiter().GetResult() in server code.

⸻

Observability & reliability
	•	Add structured logging via Serilog (or logger of choice). Enrich logs with RequestId, UserId (when available), CorrelationId.
	•	Add OpenTelemetry tracing and metrics to capture request spans, DB calls, and Wolverine messages.
	•	Implement health checks (/healthz) including database connectivity and dependent services.
	•	Add rate-limiting, circuit breaker and retry patterns for external calls (Polly).

⸻

Tests, CI & PR hygiene
	•	Unit tests for Application and Domain logic; mock EF Core using EF Core InMemory or better: use an actual Postgres with Testcontainers for integration tests.
	•	Add contract/integration tests for Wolverine message flows.
	•	Enforce code coverage gates conservatively.
	•	CI tasks:
	•	dotnet restore
	•	dotnet build –no-restore -warnaserror
	•	dotnet test (with test result publishing)
	•	run static analyzers and security scans

⸻

Useful snippets & templates
	•	Controller action pattern: accept CancellationToken, map request DTO to message/command, send to Wolverine and return appropriate HTTP status.
	•	Repository pattern: prefer thin repositories when domain logic exists in Domain layer. Don’t leak EF specifics into Domain.
	•	Transaction + Outbox: use EF Core transaction and Wolverine transactional outbox support for atomic publish of messages.

⸻

Code review checklist for Copilot suggestions

When Copilot proposes code, verify:
	1.	Placement: Is the code in the correct Clean Architecture layer?
	2.	Async: Are all I/O calls asynchronous and does the method accept CancellationToken?
	3.	EF Core: Are queries projected and not pulling unnecessary data? No client-eval warnings?
	4.	Security: Are any secrets hard-coded? Is input validated and sanitized?
	5.	Tests: Is there a corresponding unit test or an actionable TODO to add one?
	6.	Logging/Observability: Are relevant logs/traces created for important flows?

⸻

Additional preferences & team rules
	•	Keep public DTOs minimal; version APIs explicitly when breaking changes are required.
	•	Use OpenAPI/Swagger with example responses for important endpoints.
	•	Prefer composition over inheritance for domain models.
	•	Avoid using dynamic or reflection-heavy code unless absolutely necessary.
	•	Always include CancellationToken in async public APIs that may be cancelled by caller.

⸻

If unsure, default to: safety, clarity, testability, and performance in that order.