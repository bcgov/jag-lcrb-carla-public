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

## Phase 2: Dead code removal from cllc-public-app

With all controllers migrated to `IDataverseClient`, the old `MicrosoftDynamicsCRM*` overloads in `cllc-public-app` became unreachable dead code and were deleted.

### Files cleaned in Models.Extensions/

| File | What was removed |
|---|---|
| `Application.cs` (1724→762 lines) | Old `CopyValues(MicrosoftDynamicsCRMadoxioApplication, ...)`, `CopyValuesForCovidApplication`, `CopyValuesForChangeOfLocation`, `GetCachedLicenceType`, `GetCachedApplicationPicklists(IDynamicsClient)`, `PopulateLicenceType(IDynamicsClient)`, `ToViewModel(IDynamicsClient)`, `ToSummaryViewModel(MicrosoftDynamicsCRM)` |
| `License.cs` (548→373 lines) | Old `GetEndorsements`, `GetHoursOfServiceList`, `GetAreaCapacitySync`, `GetOffsiteStorage`, `GetServiceAreas` (all IDynamicsClient params), old `ToViewModel(MicrosoftDynamicsCRMadoxioLicences, IDynamicsClient)`. `GetHourService(int,int?,int?)` kept — shared by new DV `GetHoursOfServiceListAsync` |
| `LicenceEvent.cs` (611→322 lines) | Old `ToViewModel(MicrosoftDynamicsCRMadoxioEvent, IDynamicsClient)`, old `CopyValues(MicrosoftDynamicsCRMadoxioEvent, ...)`. `DetermineEventClass` kept — pure logic, no AutoRest deps |
| `MonthlyReport.cs` (174→85 lines) | Old `ToViewModel(MicrosoftDynamicsCRMadoxioCannabismonthlyreport, IDynamicsClient, bool)` |
| `Contact.cs` (634→203 lines) | Old `ToViewModel(MicrosoftDynamicsCRMcontact)`, `CopyHeaderValues`, `CopyValues`, `CopyContactUserSettings`, `CopyValuesNoEmailPhone` (all MicrosoftDynamicsCRM overloads), old `ToModel(this Contact)` returning `MicrosoftDynamicsCRMcontact` |
| `SpecialEvent.cs` (450→156 lines) | Old `ToViewModel(MicrosoftDynamicsCRM, IDynamicsClient)`, `ToSummaryViewModel(MicrosoftDynamicsCRM)`, `CopyValues(MicrosoftDynamicsCRM)` |
| `Adoxio_LegalEntity.cs` (246→99 lines) | Old `CopyValues(MicrosoftDynamicsCRMadoxioLegalentity)`, `ToViewModel(MicrosoftDynamicsCRMadoxioLegalentity)` |
| `Adoxio_Establishment.cs` (278→95 lines) | Old `CopyValues(MicrosoftDynamicsCRM*)`, `ToViewModel(MicrosoftDynamicsCRM*)`, old `ToModel(ViewModels.Establishment)` returning `MicrosoftDynamicsCRMadoxioEstablishment` |
| `Worker.cs` (236→110 lines) | Old `ToViewModel(MicrosoftDynamicsCRMadoxioWorker)`, `CopyValues(MicrosoftDynamicsCRM)`, `CopyValuesNoEmailPhone(MicrosoftDynamicsCRM)` |
| `ApplicationExtension.cs` (61→28 lines) | Old `CopyValues(MicrosoftDynamicsCRMadoxioApplicationextension, ...)`, old `ToViewModel(MicrosoftDynamicsCRMadoxioApplicationextension)` |
| `AdoxioTiedHouseConnections.cs` | Emptied — both methods dead; DV version lives in `AdoxioTiedHouseConnectionDataverse.cs` |
| `Alias.cs`, `LicenceEventLocation.cs`, `LicenceEventSchedule.cs`, `PreviousAddress.cs`, `SepDrinksSalesForecast.cs`, `SepEventDates.cs`, `SepEventLocation.cs`, `SepServiceArea.cs` | Old MicrosoftDynamicsCRM overloads removed; DV methods retained |
| `CapacityArea.cs`, `OffsiteStorage.cs`, `PolicyDocument.cs` | Old overloads removed from end of file |
| `Invoice.cs`, `LicenseeChangeLog.cs` | Interleaved old/DV layout flattened; only DV methods kept |
| `ApplicationType.cs`, `ApplicationTypeContent.cs`, `IndigenousNation.cs`, `LicenseType.cs`, `SepCity.cs`, `SepDrinkTypes.cs` | Old blocks removed; DV methods retained |
| `PoliceJurisdiction.cs`, `TiedHouseAssociation.cs`, `User.cs`, `AdoxioApplicationTermsConditionsLimitations.cs` | Emptied — only had old methods, no DV equivalents needed |

### Other files cleaned

| File | What was removed |
|---|---|
| `Contexts/DynamicsExtensions.cs` | `GetInvoiceById(Guid/string)`, `GetApplicationById(Guid)`, `GetApplicationByIdWithChildren(Guid)`, `GetInventoryReportsForMonthlyReport(string)`, `GetApplicationTypeByName/ById(string)`, `GetSystemformViewModel(IDynamicsClient, ...)` (~200 lines), `IsMostlyLiquor(List<MicrosoftDynamicsCRM*>)` |
| `Utils/StatusUtility.cs` (318→65 lines) | `GetTranslatedApplicationStatus(MicrosoftDynamicsCRM)`, `GetTranslatedApplicationStatusV2(MicrosoftDynamicsCRM)`, `GetLicenceStatus(MicrosoftDynamicsCRM, IList<...>)` |
| `ViewModels/SecurityScreeningStatusItem.cs` | `public MicrosoftDynamicsCRMcontact Contact { get; set; }` property (dead, never assigned by any controller) |

### Post-cleanup state

After cleanup, `MicrosoftDynamicsCRM*` references in `cllc-public-app` are reduced to:
- `Startup.cs` — DI registration (intentionally kept until legacy client is fully decommissioned, LCSD-8564)
- One comment in `AdoxioTiedHouseConnectionDataverse.cs`
