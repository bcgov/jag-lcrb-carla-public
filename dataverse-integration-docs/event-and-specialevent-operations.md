# Event and Special Event Operations (LCSD-8539)

Implements `adoxio_specialevent` and `adoxio_event` methods in `DataverseClient`, including related SepDrinkSalesForecast, EventSchedule, and EventLocation child entities.

## Special Event CRUD (adoxio_specialevent)

| Method | Description |
|---|---|
| `GetSpecialEventByIdAsync(id)` | Retrieve single special event by GUID; returns `null` on not-found |
| `GetSpecialEventByIdWithChildrenAsync(id)` | Special event + SepDrinkSalesForecast records |
| `GetSpecialEventByLicenceNumberAsync(licenceNumber)` | Query by `adoxio_specialeventpermitnumber`; returns first match |
| `CreateSpecialEventAsync(specialEvent)` | Creates record; returns new `Guid` |
| `UpdateSpecialEventAsync(specialEvent)` | Updates existing record |

### WithChildren relationship keys

| Relationship | Entity | FK on child |
|---|---|---|
| `adoxio_specialevent_adoxio_sepdrinksalesforecast_SpecialEvent` | `adoxio_sepdrinksalesforecast` | `adoxio_specialevent` |

Child records are queried by `adoxio_specialevent = specialEvent.Id` and attached to `specialEvent.RelatedEntities`.

### Permit number lookup

`GetSpecialEventByLicenceNumberAsync` filters on `adoxio_specialeventpermitnumber`, which is the SEP permit number field on `adoxio_specialevent`. This field is distinct from the `adoxio_licencenumber` field on `adoxio_licences`.

## Event CRUD (adoxio_event)

| Method | Description |
|---|---|
| `GetEventByIdAsync(id)` | Retrieve single event by GUID; returns `null` on not-found |
| `GetEventByIdWithChildrenAsync(id)` | Event + EventSchedules + EventLocations loaded in parallel |
| `GetEventSchedulesByEventIdAsync(eventId)` | List all schedules for an event |
| `GetEventLocationsByEventIdAsync(eventId)` | List all locations for an event |

### WithChildren relationship keys

| Relationship | Entity | FK on child |
|---|---|---|
| `adoxio_event_schedules` | `adoxio_eventschedule` | `adoxio_eventid` |
| `adoxio_event_eventlocations` | `adoxio_eventlocation` | `adoxio_eventid` |

Both child collections are queried in parallel via `Task.WhenAll` and attached to `ev.RelatedEntities`.

## Files modified

- `cllc-interfaces/Dynamics-Dataverse/DataverseClient.cs` — Special Event and Event implementations
