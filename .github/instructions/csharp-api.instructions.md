---
applyTo: "src/Contoso.Api/**/*.cs"
---

# Review rules for the C# API

These apply automatically to every C# file under `src/Contoso.Api`.

Report only issues that would matter to a reviewer on this team. For each one, name the
file and line, say what is wrong in one sentence, and show the corrected code.

Check, in priority order:

1. **Unsanitised input.** Any inbound string written to storage or logs without length
   validation and removal of control characters or markup. Flag string concatenation of
   user input into queries or log messages — use structured logging with parameters.
2. **Missing validation.** A new field on a request DTO with no matching rule in
   `CustomerValidator`.
3. **Incomplete cross-layer change.** A field added to the DTO but missing from the entity,
   the validator, or the controller's response shaping.
4. **Business logic in the controller.** New rules added to `CustomersController` rather
   than a dedicated service. Say where it should live instead.
5. **Response contract changes.** Any change to the shape or naming of fields returned by an
   existing endpoint is a breaking change for consumers. Flag it explicitly, even when the
   change looks like an improvement.
6. **Untested behaviour.** New endpoints or validation rules with no corresponding test.

Do not comment on formatting, naming style, or preference-level choices.
