# LCSD-8551: One-Stop Service Migration

Replaces all `IDynamicsClient` (AutoRest) usage in `one-stop-service/` with `IDataverseClient` (Dataverse SDK).

## Files Changed

| File | Change |
|---|---|
| `one-stop-service/OneStopUtils.cs` | Full rewrite — DV constructor, bridge helper, async send methods |
| `one-stop-service/ReceiveFromHubService.cs` | Inject `IDataverseClient`; async private handlers |
| `one-stop-service/Controllers/OneStopController.cs` | Hangfire DI form; remove manual `new OneStopUtils(...)` |
| `one-stop-service/Startup.cs` | Factory for `IReceiveFromHubService`; `AddTransient<OneStopUtils>`; Hangfire generic job |
| `cllc-interfaces/Dynamics-Dataverse/Interfaces/IDataverseClient.cs` | Added OneStop message item + licence type methods |
| `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs` | Implemented OneStop message item + licence type methods |
| `cllc-interfaces/Dynamics-Dataverse/Generated/Entities/adoxio_onestopmessageitem.cs` | Hand-crafted entity (pending `pac modelbuilder` regeneration) |
| `generate-entities.ps1` | Added `adoxio_onestopmessageitem` to entity list |

## New IDataverseClient Methods

```csharp
Task<IList<adoxio_onestopmessageitem>> GetPendingOneStopMessagesAsync(CancellationToken ct = default);
Task<IList<adoxio_onestopmessageitem>> GetOneStopMessagesByLicenceIdAsync(string licenceId, CancellationToken ct = default);
Task UpdateOneStopMessageItemAsync(adoxio_onestopmessageitem item, CancellationToken ct = default);
Task<adoxio_licencetype?> GetLicenceTypeByIdAsync(string id, CancellationToken ct = default);
```

## Bridge Helper Pattern

`OneStopUtils.FetchLicenceForOneStop` constructs the AutoRest `MicrosoftDynamicsCRMadoxioLicences` type
from DV-fetched data. This lets the five existing XML builder classes
(`ChangeAddress`, `ChangeName`, `ChangeStatus`, `ProgramAccountRequest`, `ProgramAccountDetailsBroadcast`)
continue to work unchanged, since they all accept `MicrosoftDynamicsCRMadoxioLicences`.

Fields populated by the bridge:
- `AdoxioLicencesid`, `AdoxioLicencenumber`, `AdoxioBusinessprogramaccountreferencenumber`
- `AdoxioOnestopsent`, `AdoxioExpirydate`
- `AdoxioLicenceType.AdoxioLicencetypeid`
- `AdoxioEstablishment.AdoxioName` + address fields
- `AdoxioLicencee.*` (account number, name, email, phone, address)

## Hangfire DI Migration

Old pattern (manual instantiation):
```csharp
BackgroundJob.Enqueue(() => new OneStopUtils(Configuration, _cache).SendChangeAddressRest(null, id, null));
RecurringJob.AddOrUpdate(() => new OneStopUtils(Configuration, cache).CheckForNewLicences(null), interval);
```

New pattern (DI injection via `services.AddTransient<OneStopUtils>()`):
```csharp
BackgroundJob.Enqueue<OneStopUtils>(utils => utils.SendChangeAddressRest(null, id, null));
RecurringJob.AddOrUpdate<OneStopUtils>(utils => utils.CheckForNewLicences(null), interval);
```

## Property Mapping

| AutoRest (`MicrosoftDynamicsCRMadoxioOnestopmessageitem`) | DV SDK (`adoxio_onestopmessageitem`) |
|---|---|
| `AdoxioOnestopmessageitemid` | `Id` (Entity.Id) |
| `AdoxioStatuschangedescription` (int?) | `adoxio_StatusChangeDescription` (`OneStopHubStatusChange?`) |
| `AdoxioDateacknowledgementreceived` | `adoxio_DateAcknowledgementReceived` |
| `AdoxioAcknowledgementstatus` | `adoxio_AcknowledgementStatus` |
| `AdoxioMessagesendstatus` (int?) | `adoxio_MessageSendStatus` (`OneStopMessageStatus?`) |

## Async Boundary in ReceiveFromHubService

`receiveFromHub` is a WCF `[OperationContract]` that must remain synchronous to match
`IReceiveFromHubService`. Private methods `HandleResponseAsync` and `HandleSBNErrorNotificationAsync`
are `async Task<string>` and called via `.GetAwaiter().GetResult()`. This is safe in ASP.NET Core
(no `SynchronizationContext`).
