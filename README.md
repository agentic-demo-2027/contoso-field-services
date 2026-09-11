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

## How Copilot is configured here

Two different mechanisms, and the difference matters.

### Automatic — applied to every pull request, nobody has to ask

| File | Scope | Read by |
|---|---|---|
| `AGENTS.md` | whole repo | cloud agent, code review |
| `.github/copilot-instructions.md` | whole repo | chat, cloud agent, code review |
| `.github/instructions/csharp-api.instructions.md` | `src/Contoso.Api/**/*.cs` | cloud agent, code review |
| `.github/instructions/frontend.instructions.md` | `src/web/**/*.ts` | cloud agent, code review |
| `.github/instructions/tests.instructions.md` | `tests/**/*.cs` | cloud agent, code review |

Path-specific files use `applyTo` frontmatter, so the C# rules are only considered when C#
files change. A repository ruleset requests the review automatically on every pull request
into `main`.

### On demand — chosen by name when you want a specialist

Custom agents live in `.github/agents/`. They are **not** triggered automatically: you pick
one from the agent dropdown on GitHub, in an IDE, or in the CLI, and hand it a task.

| Agent | Good for |
|---|---|
| `api-reviewer` | A focused audit of the C# API outside the normal review flow |
| `frontend-reviewer` | A focused audit of the SPA |
| `docs-checker` | Checking the docs still match after a change |

The filename is the agent's identifier. Moving one of these files to `/agents/` in the
organisation's `.github` repository would make it available across every repository in the
organisation, without changing its contents.
