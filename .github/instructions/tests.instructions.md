---
applyTo: "tests/**/*.cs"
---

# Review rules for tests

1. **Assert the behaviour, not the implementation.** A test that only checks a status code
   when the endpoint returns data should assert the data too.
2. **Cover the failure paths.** A new validation rule needs a test that violates it.
3. **No shared mutable state between tests.** The API uses an in-memory store, so tests must
   generate unique values rather than relying on an empty store.
