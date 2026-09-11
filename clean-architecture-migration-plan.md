# Clean Architecture migration plan for the customer registry API

## Goals and guardrails

- Keep this as a **behaviour-preserving refactor**: no route changes, no JSON contract changes, no front-end changes.
- Do **not** change sanitisation behaviour as part of this migration. The current API still stores raw input even though `AGENTS.md` states inbound strings should be sanitised, so this plan intentionally preserves current runtime behaviour and defers bringing the implementation into line with that guideline to a separate approved change.
- Keep `src/Contoso.Api` as a **single project** for the first pass. Introduce folders, interfaces and focused classes before considering separate assemblies.
- Add **characterisation tests first** so each slice can merge with the existing behaviour locked down.
- Do **not** introduce MediatR or other new dependencies for this migration. Plain classes are enough for the current size of the app.

## Target layers

### Presentation

Responsible for HTTP only:

- `Program.cs`
- `Controllers/CustomersController.cs`
- Request/response transport models that exist only because of HTTP, such as `Models/CreateCustomerRequest.cs`

This layer should:

- read route/body input
- call an application use case
- translate the use-case result into `Ok`, `Created`, `BadRequest`, `Conflict` or `NotFound`

This layer should **not**:

- call the repository directly
- enforce the duplicate-email rule
- orchestrate validation
- shape customer responses inline

The current API contract is intentionally asymmetric and must stay that way during this refactor unless the team approves a separate contract change:

- `GET /api/customers` returns a projected list item with `displayName` and date-only `created`
- `GET /api/customers/{id}` and `POST /api/customers` return the raw `Customer` shape with `createdUtc`

### Application

Responsible for use-case orchestration:

- listing customers
- fetching a customer by id
- creating a customer
- invoking validation and any approved sanitisation seam
- enforcing cross-entity rules such as duplicate email checks
- mapping domain entities to API-facing response shapes

This is where interfaces for persistence and sanitisation should live so the controller can depend on use cases instead of infrastructure.

### Domain

Responsible for business data and rules that do not need ASP.NET or storage details:

- the `Customer` entity
- any future pure customer rules or invariants that can be enforced without I/O

At the current size of the codebase, keep the domain deliberately small. Do not invent value objects or extra abstractions until a real rule needs them.

### Infrastructure

Responsible for implementation details:

- the in-memory customer repository
- any sanitiser implementation
- DI wiring that connects interfaces to concrete implementations

This lets us keep the current in-memory behaviour while removing the controller-to-repository dependency.

## Specific files to create

Keep the existing project and add folders like these under `src/Contoso.Api`:

- `Application/Customers/Contracts/ICustomerRepository.cs`  
  Abstraction for `GetAll`, `GetById`, `Add` and `EmailExists`.

- `Application/Customers/Contracts/ICustomerSanitizer.cs`  
  Abstraction for inbound string sanitisation so the use case can own that step without knowing implementation details.

- `Application/Customers/ListCustomers/ListCustomersHandler.cs`  
  Returns the current list response shape, including `DisplayName` and formatted `Created`.

- `Application/Customers/GetCustomerById/GetCustomerByIdHandler.cs`  
  Returns a single customer or a not-found result.

- `Application/Customers/CreateCustomer/CreateCustomerHandler.cs`  
  Owns validation, sanitisation, duplicate-email checking, persistence and the created-customer result.

- `Application/Customers/CreateCustomer/CreateCustomerResult.cs`  
  Small result type so the controller can map success, validation failure and duplicate-email conflict without embedding rules.

- `Application/Customers/CustomerListItemResponse.cs`  
  Named response model for the current `GET /api/customers` payload instead of anonymous objects in the controller.

- `Infrastructure/Customers/InMemoryCustomerRepository.cs`  
  Replacement for the current concrete repository dependency, implementing `ICustomerRepository`.

- `Infrastructure/Customers/CustomerSanitizer.cs`  
  Concrete sanitiser implementation behind `ICustomerSanitizer`; it should be a no-op in this migration unless the team approves sanitisation as a separate behaviour change.

- `Domain/Customers/Customer.cs`  
  Final home for the customer entity after the seams are in place.

Not every file has to appear in the first slice. The point is to add only the files needed for the slice being merged.

## Reviewable migration slices

### Slice 1 — lock down current behaviour with tests

**Purpose:** reduce refactor risk before moving logic.

Add integration tests for the behaviour that currently lives implicitly in `CustomersController`:

- `GET /api/customers` returns `DisplayName` and date-only `Created`
- `GET /api/customers/{id}` returns the current not-found payload
- `GET /api/customers/{id}` success preserves the raw `Customer` payload, including `createdUtc`
- `POST /api/customers` rejects duplicate emails with `409 Conflict`
- `POST /api/customers` keeps the duplicate-email check case-insensitive
- `POST /api/customers` returns current validation errors for bad payloads
- `POST /api/customers` success preserves the raw `Customer` payload, including `createdUtc`

**Expected production changes:** none or near-none.

**Why first:** the existing tests only prove happy-path create and one validation error, which is too little protection for a controller refactor.

### Slice 2 — extract the read path first

**Purpose:** create the layering seam on the lowest-risk endpoints.

Work:

- create `CustomerListItemResponse`
- create `ListCustomersHandler`
- create `GetCustomerByIdHandler`
- update `CustomersController` so `GetAll` and `GetById` delegate to handlers

Keep the controller route attributes and HTTP responses exactly as they are today.

**Why second:** the read endpoints do not mutate state and have fewer branches, so they are the safest place to prove the approach.

### Slice 3 — extract the create use case

**Purpose:** move the most tangled logic out of the controller.

Work:

- create `ICustomerRepository`
- create `CreateCustomerHandler`
- create `CreateCustomerResult`
- move validation orchestration, duplicate-email enforcement, the sanitisation seam and entity creation into the handler
- keep `CustomersController.Create` as a thin HTTP adapter

Keep `CustomerValidator` initially if that reduces churn; if needed, call it from the handler before deciding whether to turn it into an injectable service.

If response shaping has already moved out of `CustomersController` by this point, update the `AGENTS.md` "Keep the stack in step" bullet that currently says customer changes must include "the response shaping in the controller", so the repository guidance matches the new location of that mapping code.

**Why third:** once this slice lands, the controller stops owning the most complex business flow.

### Slice 4 — move infrastructure behind interfaces

**Purpose:** finish the dependency direction.

Work:

- replace direct use of `CustomerRepository` with `ICustomerRepository`
- create `InMemoryCustomerRepository`
- register the interface in `Program.cs`

This keeps the same in-memory behaviour but means application code no longer depends on the concrete storage class.

### Slice 5 — move the domain model to its final home

**Purpose:** clean up structure after the seams are proven.

Work:

- move `Customer` to `Domain/Customers/Customer.cs`
- update namespaces/usings
- leave external API contracts where they are if they are still HTTP-specific

This slice should be mostly mechanical because the earlier slices already separated responsibilities.

## What should remain untouched

- `src/web/**`  
  No SPA changes are required for this migration because the goal is to preserve the current HTTP contract, including the existing list-response/type mismatch. Fixing that API/SPA drift should be handled as a separate contract-cleanup issue rather than mixed into the architectural refactor.

- `.github/workflows/ci.yml`  
  The existing `dotnet test` and `npm run build` checks are already the right guardrails for a behaviour-preserving refactor.

- `Program.cs` hosting model  
  Apart from adding DI registrations as slices require them, the minimal-host setup should stay as-is. Replacing the host style would add churn without helping the architecture goal.

- `/health` endpoint  
  It is unrelated to the customer refactor.

- Existing API routes and payload shapes  
  Keeping them stable lets the team review architecture changes without also reviewing behaviour changes.

## Risks

- **Insufficient behavioural coverage before refactoring.**  
  Mitigation: do Slice 1 first and make sure the new tests cover the current success, error and conflict paths.

- **Accidentally changing API contracts while introducing named response models.**  
  Mitigation: assert exact response fields in integration tests before and after the read-path extraction.

- **Over-engineering the solution.**  
  Mitigation: keep one project, use plain handlers, and avoid adding frameworks or separate assemblies in this first migration.

- **Blurring application and domain responsibilities.**  
  Mitigation: keep I/O-driven rules such as duplicate-email checks in the application layer, and keep the domain layer small until richer pure business rules appear.

## Highest-risk slice

**Slice 3 carries the most risk.**

That is the slice where validation, duplicate-email handling, sanitisation, entity creation, persistence and the `201 Created` flow all move at once. It has the most branches, it affects write behaviour, and it is the easiest place to accidentally change status codes or payloads.

That is why the safest order is:

1. add characterisation tests
2. prove the pattern on reads
3. extract create
4. swap infrastructure dependencies
5. do the final structural tidy-up
