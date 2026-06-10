# LCSD-8541: Port 46 ModelExtension files to new entity classes

All 46 files from `Dynamics-Autorest/ModelExtensions/` have been ported to `Dynamics-Dataverse/Extensions/` and the originals deleted.

## What changed

| Old pattern | New pattern |
|---|---|
| `partial class MicrosoftDynamicsCRM*` adding `@odata.bind` JSON properties | **Omitted** — OData bind strings are an AutoRest/REST artifact; the Dataverse SDK sets relationships via `EntityReference` objects directly |
| `partial class MicrosoftDynamicsCRM*` adding non-SDK properties (e.g. `PhsLink`, `CasLink`) | Partial class addition on the generated SDK entity in `ContactExtensions.cs` |
| Standalone enum types (`SecurityStatusPicklist`, `OneStopMessageStatus`, etc.) | Moved to `Gov.Lclb.Cllb.Interfaces` namespace in the Dataverse project |
| Metadata helper classes (`MicrosoftDynamicsCRMlabel`, `MicrosoftDynamicsCRMoption`, etc.) | Renamed to `DynamicsLabel`, `DynamicsOption`, etc. in the same namespace |
| OData collection wrappers (`MicrosoftDynamicsCRMadoxioApplicationCollection`, etc.) | Empty extension stubs — SDK returns `EntityCollection<T>`, not OData envelopes |

## File mapping (46 files)

| Old file | New file | Notes |
|---|---|---|
| `MicrosoftDynamicsCRMaccount.cs` | `AccountExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioAlias.cs` | `AdoxioAliasExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioAnnualvolume.cs` | `AdoxioAnnualvolumeExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioApplicationCollection.cs` | `AdoxioApplicationCollectionExtensions.cs` | OData envelope — SDK returns EntityCollection |
| `MicrosoftDynamicsCRMadoxioCannabisinventoryreport.cs` | `AdoxioCannabisinventoryreportExtensions.cs` | Was empty |
| `MicrosoftDynamicsCRMadoxioCannabismonthlyreport.cs` | `AdoxioCannabismonthlyreportExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioEstablishment.cs` | `AdoxioEstablishmentExtensions.cs` | Was empty |
| `MicrosoftDynamicsCRMadoxioEvent.cs` | `AdoxioEventExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioEventlocation.cs` | `AdoxioEventlocationExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioEventschedule.cs` | `AdoxioEventscheduleExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioHoursofsale.cs` | `AdoxioHoursofserviceExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioLdborder.cs` | `AdoxioLdborderExtensions.cs` | OData binds + date format dropped |
| `MicrosoftDynamicsCRMadoxioLegalentity.cs` | `AdoxioLegalentityExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioLicences.cs` | `AdoxioLicencesExtensions.cs` | OData binds + string indexer dropped (SDK Entity already has attribute indexer) |
| `MicrosoftDynamicsCRMadoxioLicenseechangelog.cs` | `AdoxioLicenseechangelogExtensions.cs` | OData binds dropped; entity not generated |
| `MicrosoftDynamicsCRMadoxioLicensesesCollection.cs` | `AdoxioLicencesCollectionExtensions.cs` | OData envelope |
| `MicrosoftDynamicsCRMadoxioLogin.cs` | `AdoxioLoginExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioOffsitestorage.cs` | `AdoxioOffsitestorageExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioOnestopmessageitem.cs` | `AdoxioOnestopmessageitemExtensions.cs` | Was empty; entity not generated |
| `MicrosoftDynamicsCRMadoxioPreviousaddress.cs` | `AdoxioPreviousaddressExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioSepdrinksalesforecast.cs` | `AdoxioSepdrinksalesforecastExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioServicearea.cs` | `AdoxioServiceareaExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioSpddatarow.cs` | `AdoxioSpddatarowExtensions.cs` | String indexer dropped; entity not generated |
| `MicrosoftDynamicsCRMadoxioSpecialevent.cs` | `AdoxioSpecialeventExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioSpecialeventCollection.cs` | `AdoxioSpecialeventCollectionExtensions.cs` | OData envelope |
| `MicrosoftDynamicsCRMadoxioSpecialeventlicencedarea.cs` | `AdoxioSpecialeventlicencedareaExtensions.cs` | OData binds dropped; entity not generated |
| `MicrosoftDynamicsCRMadoxioSpecialeventlocation.cs` | `AdoxioSpecialeventlocationExtensions.cs` | OData binds dropped; entity not generated |
| `MicrosoftDynamicsCRMadoxioSpecialeventschedule.cs` | `AdoxioSpecialeventscheduleExtensions.cs` | OData binds dropped; entity not generated |
| `MicrosoftDynamicsCRMadoxioSpecialeventtandc.cs` | `AdoxioSpecialeventtandcExtensions.cs` | OData binds dropped; entity not generated |
| `MicrosoftDynamicsCRMadoxioWorker.cs` | `AdoxioWorkerExtensions.cs` | `SecurityStatusPicklist` enum preserved |
| `MicrosoftDynamicsCRMcontact.cs` | `ContactExtensions.cs` | `PhsLink`, `CasLink` preserved as partial class additions; `DateFormatConverter` dropped (not needed in SDK) |
| `MicrosoftDynamicsCRMinvoice.cs` | `InvoiceExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMlabel.cs` | `DynamicsLabel.cs` | Renamed; moved to `Gov.Lclb.Cllb.Interfaces` |
| `MicrosoftDynamicsCRMlist.cs` | `ListExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMlocalizedLabel.cs` | `DynamicsLocalizedLabel.cs` | Renamed; references updated |
| `MicrosoftDynamicsCRMoption.cs` | `DynamicsOption.cs` | Renamed; references updated |
| `MicrosoftDynamicsCRMoptionSet.cs` | `DynamicsOptionSet.cs` | Renamed; references updated |
| `MicrosoftDynamicsCRMpicklistAttributeMetadata.cs` | `DynamicsPicklistAttributeMetadata.cs` | Renamed; references updated |
| `MicrosoftDynamicsCRMpicklistAttributeMetadataCollection.cs` | `DynamicsPicklistAttributeMetadataCollection.cs` | Renamed; references updated |
| `OneStopMessageStatus.cs` | `OneStopMessageStatus.cs` | Moved to `Gov.Lclb.Cllb.Interfaces` |
| `StatsResultModel.cs` | `StatsResultModel.cs` | `CommRegions`, `StatsResultModel`, `StatsResultResponse` moved |
| `MicrosoftDynamicsCRMadoxioApplication.cs` | `AdoxioApplicationExtensions.cs` | OData binds dropped |
| `OneStopHubStatusChangeType.cs` | `OneStopHubStatusChangeType.cs` | Moved to `Gov.Lclb.Cllb.Interfaces` |
| `MicrosoftDynamicsCRMadoxioTiedhouseconnection.cs` | `AdoxioTiedhouseconnectionExtensions.cs` | OData binds dropped |
| `MicrosoftDynamicsCRMadoxioTiedhouseconnectionCollection.cs` | `AdoxioTiedhouseconnectionCollectionExtensions.cs` | OData envelope |
| `MicrosoftDynamicsCRMsharepointdocumentlocation.cs` | `SharePointDocumentLocationExtensions.cs` | OData binds dropped |

## Why OData bind properties were dropped

AutoRest generated a REST client that used OData `@odata.bind` strings like `"adoxio_Applicant@odata.bind": "/accounts(guid)"` to wire up navigation properties when creating/updating records via HTTP PATCH/POST. The Dataverse SDK replaces this entirely: lookups are set as `EntityReference` fields on the entity object before calling `ServiceClient.Create()` or `Update()`. No bind strings are needed.

## Renamed metadata classes

| Old name | New name |
|---|---|
| `MicrosoftDynamicsCRMlabel` | `DynamicsLabel` |
| `MicrosoftDynamicsCRMlocalizedLabel` | `DynamicsLocalizedLabel` |
| `MicrosoftDynamicsCRMoption` | `DynamicsOption` |
| `MicrosoftDynamicsCRMoptionSet` | `DynamicsOptionSet` |
| `MicrosoftDynamicsCRMpicklistAttributeMetadata` | `DynamicsPicklistAttributeMetadata` |
| `MicrosoftDynamicsCRMpicklistAttributeMetadataCollection` | `DynamicsPicklistAttributeMetadataCollection` |

Any callers referencing the old `MicrosoftDynamicsCRM*` class names for these metadata types need to be updated to use the new `Dynamics*` names.
