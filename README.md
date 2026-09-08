# Contoso Field Services

Customer registry used by Contoso field engineers. Deliberately small: a C# Web API and a
TypeScript single-page front end, with enough structure to be realistic and little enough
to read in one sitting.

> Contoso is a fictitious company. This repository exists to demonstrate GitHub Copilot
> agentic workflows and contains no real customer data.

## Stack

- **API** — ASP.NET Core (C#, net10.0), in-memory store
- **Front end** — TypeScript SPA, no framework, compiled with `tsc`
- **Tests** — xUnit integration tests over the API

## Running it

```bash
# API on http://localhost:5266
cd src/Contoso.Api && dotnet run

# Front end (open index.html after building)
cd src/web && npm install && npm run build

# Tests
cd tests/Contoso.Api.Tests && dotnet test
```

## API

| Method | Route | Purpose |
|---|---|---|
| `GET` | `/health` | Liveness check |
| `GET` | `/api/customers` | List customers |
| `GET` | `/api/customers/{id}` | Fetch one customer |
| `POST` | `/api/customers` | Create a customer |

A customer currently has a first name, an email address and a region.

## Agents

`AGENTS.md` holds the working agreements every agent follows. Repository-level review
agents live in `.github/agents/` and can be asked for by name in a pull request:

| Agent | Reviews |
|---|---|
| `api-reviewer` | C# validation, sanitisation, layering |
| `frontend-reviewer` | Type drift, injection risk, API contract match |
| `docs-checker` | Documentation accuracy after a change |
