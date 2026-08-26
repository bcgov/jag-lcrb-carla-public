# LCSD-8540: Account, Contact, and Supporting Entity Operations

Implements the remaining `NotImplementedException` stubs in `DataverseClient`, completing the full `IDataverseClient` surface.

## Entities Covered

| Entity | Logical Name | Operations |
|---|---|---|
| Account | `account` | GetById, GetByIdWithChildren, GetByName, GetAll, Create, Update |
| Contact | `contact` | GetById, Create, Update |
| Establishment | `adoxio_establishment` | GetById, GetByAccountId, Update |
| LegalEntity | `adoxio_legalentity` | GetById, GetByAccountId |
| TiedHouseConnection | `adoxio_tiedhouseconnection` | GetByAccountId, Create, Delete |
| Annotation | `annotation` | GetByObjectId, GetById, Create, Update, Delete |
| SharePointDocumentLocation | `sharepointdocumentlocation` | GetByObjectId, Create, Update |
| Pagination | — | `RetrievePagedAsync<T>` |

## Account WithChildren

`GetAccountByIdWithChildrenAsync` fires three parallel queries keyed to the account ID and populates `RelatedEntities`:

| Child | Filter attribute | Relationship key |
|---|---|---|
| `adoxio_establishment` | `adoxio_licencee` | `adoxio_account_adoxio_establishment_Licencee` |
| `adoxio_legalentity` | `adoxio_account` | `adoxio_account_adoxio_legalentity_Account` |
| `adoxio_tiedhouseconnection` | `adoxio_accountid` | `adoxio_account_adoxio_tiedhouseconnection_Licensee` |

## GetAccountsAsync filter

The optional `filter` parameter is applied as a `LIKE` condition on the `name` attribute. Pass `null` to retrieve all accounts (use with care on large orgs).

## Annotation parent lookup

`GetAnnotationsByObjectIdAsync` filters by the `objectid` attribute (the polymorphic parent reference on `annotation`). Callers pass the parent record's GUID as a string.

## SharePoint document location parent lookup

`GetSharePointDocLocByObjectIdAsync` filters by `regardingobjectid` and returns the first match (`TopCount = 1`), consistent with the existing usage pattern.

## Pagination

`RetrievePagedAsync<T>` sets `query.PageInfo` on the caller-supplied `QueryExpression` and returns a `(Results, NextPagingCookie)` tuple. Pass `null` for the first page; pass the returned cookie for subsequent pages. `MoreRecords = false` returns `null` as the cookie to signal end of data.

```csharp
var (page1, cookie) = await client.RetrievePagedAsync<adoxio_licences>(query, pageSize: 500);
while (cookie != null)
{
    var (page, next) = await client.RetrievePagedAsync<adoxio_licences>(query, pageSize: 500, pagingCookie: cookie);
    // process page
    cookie = next;
}
```
