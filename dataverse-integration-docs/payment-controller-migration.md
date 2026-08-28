# PaymentController Migration (LCSD-8557)

Migrates `cllc-public-app/Controllers/PaymentController.cs` from `IDynamicsClient` (AutoRest) to `IDataverseClient` (Dataverse SDK).

## What changed

### Constructor

`IDynamicsClient` removed. `IDataverseClient` only. The old AutoRest static overloads (`GetLiquorPaymentStatus(MicrosoftDynamicsCRMadoxioApplication, IDynamicsClient, ...)`, `GetCannabisPaymentStatus(MicrosoftDynamicsCRMadoxioApplication, IDynamicsClient, ...)`, `UpdateRelatedLeReviewStatus(string, string, IDynamicsClient)`) and `_dynamicsClient` field were removed once `ApplicationsController` was confirmed fully migrated. Zero IDynamicsClient references remain.

```csharp
public PaymentController(..., IDataverseClient dataverse, ...)
```

### Invoice entity mapping

| AutoRest (`MicrosoftDynamicsCRMinvoice`) | DV SDK (`Invoice`) |
|---|---|
| `Statecode` (`int?`, `Adoxio_invoicestates` enum) | `StateCode` (`invoice_statecode?`) |
| `Statuscode` (`int?`, `Adoxio_invoicestatuses` enum) | `StatusCode` (`invoice_statuscode?`) |
| `Totalamount` (`decimal?`) | `TotalAmount?.Value` (`Money → decimal`) |
| `Invoicenumber` | `InvoiceNumber` |
| `AdoxioTransactionid` | `adoxio_TransactionID` |
| `AdoxioReturnedtransactionid` | `adoxio_returnedtransactionid` |
| `Invoiceid` | `Id` (`Guid`) |

Enum value integers are identical between AutoRest and DV SDK:

| Meaning | AutoRest | DV SDK |
|---|---|---|
| Invoice active/new | `Adoxio_invoicestates.New = 0` | `invoice_statecode.Active = 0` |
| Invoice paid | `Adoxio_invoicestates.Paid = 2` | `invoice_statecode.Paid = 2` |
| Invoice cancelled | `Adoxio_invoicestates.Cancelled = 3` | `invoice_statecode.Canceled = 3` |
| Status new | `Adoxio_invoicestatuses.New = 1` | `invoice_statuscode.New = 1` |
| Status paid | `Adoxio_invoicestatuses.Paid = 100001` | `invoice_statuscode.Complete = 100001` |
| Status cancelled | `Adoxio_invoicestatuses.Cancelled = 100003` | `invoice_statuscode.Canceled = 100003` |

### Application entity mapping

| AutoRest | DV SDK |
|---|---|
| `application._adoxioInvoiceValue` (string GUID) | `application.adoxio_Invoice?.Id.ToString()` |
| `application._adoxioLicencefeeInvoiceValue` | `application.adoxio_LicenceFeeInvoice?.Id.ToString()` |
| `application._adoxioSecondaryapplicationinvoiceValue` | `application.adoxio_SecondaryApplicationInvoice?.Id.ToString()` |
| `application.AdoxioApplicationid` | `application.Id.ToString()` |
| `application._adoxioApplicantValue` | `application.adoxio_Applicant?.Id.ToString()` |
| `AdoxioPrimaryapplicationinvoicepaid = 1` | `adoxio_PrimaryApplicationInvoicePaid = adoxio_generalyesno.Yes` |
| `AdoxioSecondaryapplicationinvoicepaid = 1` | `adoxio_SecondaryApplicationInvoicePaid = adoxio_generalyesno.Yes` |
| `AdoxioLicencefeeinvoicepaid = true` | `adoxio_LicenceFeeInvoicePaid = true` |
| `AdoxioPaymentrecieved = true` | `adoxio_PaymentRecieved = true` (note spelling preserved) |
| `AdoxioPaymentmethod = 3` | `adoxio_PaymentMethod = adoxio_application_adoxio_paymentmethod.CreditCard` |
| `AdoxioAppchecklistpaymentreceived = 1` | `adoxio_AppChecklistPaymentReceived = adoxio_generalyesno.Yes` |
| `AdoxioInvoicetrigger = 1` / `0` | `adoxio_InvoiceTrigger = adoxio_generalyesno.Yes` / `.No` |
| `AdoxioLicencefeeinvoicetrigger = 1` / `0` | `adoxio_LicenceFeeInvoiceTrigger = adoxio_generalyesno.Yes` / `.No` |

### Static method overloads for backward compatibility

`GetCannabisPaymentStatus` and `GetLiquorPaymentStatus` are called by `ApplicationsController` with AutoRest types. The original AutoRest-typed static overloads are **preserved unchanged**. New DV-typed overloads are added alongside them:

```csharp
// Old — called from ApplicationsController (kept)
public static async Task<PaymentResult> GetCannabisPaymentStatus(
    MicrosoftDynamicsCRMadoxioApplication application, IDynamicsClient dynamicsClient, IBCEPService bcep)

// New — called from PaymentController instance methods
public static async Task<PaymentResult> GetCannabisPaymentStatus(
    adoxio_application_dv application, IDataverseClient dataverse, IBCEPService bcep)
```

Same pattern for `GetLiquorPaymentStatus` and `UpdateRelatedLeReviewStatus`.

### GetPaymentTypeAsync extension method

Added `GetPaymentTypeAsync(this adoxio_application, IDataverseClient)` to `DynamicsExtensions.cs`. Replaces the synchronous AutoRest `GetPaymentType(this MicrosoftDynamicsCRMadoxioApplication, IDynamicsClient)` call used in instance methods.

Handles the "Licensee Changes" application type by loading the account's licences and checking whether the majority have a Liquor category licence type.

### GetDynamicsApplication / GetDynamicsWorker

Both private helpers now return DV types and call the async Dataverse methods:

- `GetDynamicsApplication` → `_dataverse.GetApplicationByIdWithChildrenAsync(id)`
- `GetDynamicsWorker` → `_dataverse.GetWorkerByIdAsync(workerId)`

### GetSpecialEventData

Renamed to `GetSpecialEventDataAsync`, returns `Task<adoxio_specialevent?>`, calls `_dataverse.GetSpecialEventByIdAsync(eventId)`.

### Invoice not expanded on loaded entities

`GetApplicationByIdWithChildrenAsync` and `GetWorkerByIdAsync` return entity references for invoices (ID only), not expanded invoice entities. All status checks load the invoice separately:

```csharp
var existingInvoice = await _dataverse.GetInvoiceByIdAsync(existingInvoiceId);
if (existingInvoice?.StatusCode == invoice_statuscode.Complete) ...
```

### UpdateRelatedLeReviewStatus DV overload

The DV overload loads the PCL application via `GetApplicationByIdAsync`, checks both invoice paid fields, then sets `statuscode` on the LE Review application:

```csharp
patch.statuscode = (adoxio_application_statuscode)AdoxioApplicationStatusCodes.UnderReview;
await dataverse.UpdateApplicationAsync(patch);
```

Application extension is fetched via `GetApplicationExtensionByApplicationIdAsync` rather than relying on the expanded nav property.

## IDataverseClient methods used (all pre-existing)

- `GetApplicationByIdWithChildrenAsync`
- `GetApplicationByIdAsync`
- `UpdateApplicationAsync`
- `GetInvoiceByIdAsync`
- `UpdateInvoiceAsync`
- `GetWorkerByIdAsync`
- `UpdateWorkerAsync`
- `GetSpecialEventByIdAsync`
- `UpdateSpecialEventAsync`
- `GetApplicationTypeByIdAsync`
- `GetApplicationExtensionByApplicationIdAsync`
- `GetLicenceByIdAsync`
- `GetLicencesByAccountIdAsync`
- `GetLicenceTypeByIdAsync`

No new IDataverseClient methods were required.
