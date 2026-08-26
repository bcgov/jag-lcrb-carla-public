# Worker Operations (LCSD-8538)

Implements `adoxio_worker` methods in `DataverseClient`, including related PersonalHistorySummary and PreviousAddress child entities.

## Worker CRUD

| Method | Description |
|---|---|
| `GetWorkerByIdAsync(id)` | Retrieve single worker by GUID; returns `null` on not-found |
| `GetWorkerByIdWithChildrenAsync(id)` | Worker + PersonalHistorySummary + PreviousAddress loaded in parallel |
| `CreateWorkerAsync(worker)` | Creates worker record; returns new `Guid` |
| `UpdateWorkerAsync(worker)` | Updates existing worker record |

### WithChildren relationship keys

| Relationship | Entity |
|---|---|
| `adoxio_worker_adoxio_personalhistorysummary` | `adoxio_personalhistorysummary` |
| `adoxio_previousaddress_worker` | `adoxio_previousaddress` |

Both child collections are queried in parallel via `Task.WhenAll` and attached to `worker.RelatedEntities`.

## Personal History Summary (adoxio_personalhistorysummary)

| Method | Description |
|---|---|
| `GetPersonalHistorySummariesByWorkerIdAsync(workerId)` | List all summaries for a worker |
| `CreatePersonalHistorySummaryAsync(summary)` | Create; returns new `Guid` |
| `UpdatePersonalHistorySummaryAsync(summary)` | Update existing record |

## Previous Address (adoxio_previousaddress)

| Method | Description |
|---|---|
| `GetPreviousAddressesByWorkerIdAsync(workerId)` | List all addresses for a worker |
| `CreatePreviousAddressAsync(address)` | Create; returns new `Guid` |
| `UpdatePreviousAddressAsync(address)` | Update existing record |
| `DeletePreviousAddressAsync(id)` | Delete by GUID string; no-ops on invalid GUID |

## Files modified

- `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs` — Worker, PersonalHistorySummary, PreviousAddress implementations
- `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs` — Added 7 new method signatures
