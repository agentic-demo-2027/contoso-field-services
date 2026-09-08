# Contoso Field Services — agent instructions

Customer registry for Contoso field engineers. C# minimal-hosted Web API plus a
TypeScript single-page front end. Agents working in this repository should follow
the conventions below.

## Layout

| Path | Purpose |
|---|---|
| `src/Contoso.Api` | ASP.NET Core Web API (C#, net10.0) |
| `src/web` | TypeScript SPA, no framework, compiled with `tsc` |
| `tests/Contoso.Api.Tests` | xUnit integration tests over the API |

## Working agreements

- **A change is not finished until the tests pass.** Run `dotnet test` from
  `tests/Contoso.Api.Tests` and `npm run build` from `src/web` before reporting done.
- **Keep the stack in step.** A field added to a customer must be reflected in *all* of:
  the TypeScript form, the TypeScript types, the C# request DTO, the validator, the
  entity, the response shaping in the controller, and the tests. A change that updates
  only some of these is incomplete.
- **Validate and sanitise every inbound string.** User-supplied text must be length
  checked and stripped of control characters and markup before it is stored. Never
  concatenate user input into a query or a log message.
- **Do not introduce new dependencies** without saying so explicitly in the pull
  request description, with a one-line reason.
- **Match the existing style.** Comments explain *why*, not *what*. No comment that
  simply restates the line beneath it.

## Commands

```bash
# API
cd src/Contoso.Api && dotnet run

# Tests
cd tests/Contoso.Api.Tests && dotnet test

# Front end
cd src/web && npm run build
```
