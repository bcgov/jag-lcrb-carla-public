# data-access/api

Generated API client. Not edited by hand.

There is no OpenAPI spec for the new portal backend yet, so this folder is empty. Until then, the repositories in `../repositories/` use mock implementations over the prototype's seed data.

When the spec is available:

1. Generate the client here with `pnpm generate:client` (`tools/generate-client.mjs`).
2. Add a real implementation for each repository next to its mock, using the generated client.
3. Add mappers in `../mappers/` wherever a contract shape differs from the domain model.
4. Switch the provider in `app.config.ts` from the mock to the real implementation.
