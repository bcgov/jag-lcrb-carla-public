# Dataverse Integration Docs

Documentation for the Dataverse SDK migration replacing the AutoRest-generated `DynamicsClient` with `Microsoft.PowerPlatform.Dataverse.Client.ServiceClient`.

## Documents

| File | Covers |
|---|---|
| [setup-and-authentication.md](setup-and-authentication.md) | Project skeleton, NuGet packages, solution setup, `DataverseClient` auth wrapper, health check, env vars |
| [entity-generation.md](entity-generation.md) | `pac modelbuilder` setup, entity generation script, full entity list, missing entities, re-generation guide |
| [application-operations.md](application-operations.md) | LCSD-8536: Application CRUD, WithChildren parallel loading (Licence, Establishment, LegalEntity), ApplicationExtension, AnnualVolume |
| [licence-operations.md](licence-operations.md) | LCSD-8537: Licence CRUD, WithChildren parallel loading (ServiceArea, HourOfSale, OffSiteStorage, TermsConditions), child entity CRUD |
| [worker-operations.md](worker-operations.md) | LCSD-8538: Worker CRUD, WithChildren parallel loading (PersonalHistorySummary, PreviousAddress), child entity CRUD |
