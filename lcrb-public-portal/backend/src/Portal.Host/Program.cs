using Portal.Host.HealthChecks;
using Portal.Integrations.Dataverse;
using Portal.Integrations.Documents;
using Portal.Integrations.Geocoding;
using Portal.Integrations.IdentityDirectory;
using Portal.Integrations.Notifications;
using Portal.Integrations.OneStop;
using Portal.Integrations.Payments;
using Portal.Integrations.Pdf;
using Portal.Integrations.Registries;
using Portal.Accounts;
using Portal.Applications;
using Portal.Assistant;
using Portal.Identity;
using Portal.LegalEntity;
using Portal.Licences;
using Portal.Partners;
using Portal.ReferenceData;
using Portal.SpecialEvents;
using Portal.Staff;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAccountsModule()
    .AddApplicationsModule()
    .AddAssistantModule()
    .AddIdentityModule()
    .AddLegalEntityModule()
    .AddLicencesModule()
    .AddPartnersModule()
    .AddReferenceDataModule()
    .AddSpecialEventsModule()
    .AddStaffModule()
    .AddDataverseIntegration()
    .AddDocumentsIntegration()
    .AddGeocodingIntegration()
    .AddIdentityDirectoryIntegration()
    .AddNotificationsIntegration()
    .AddOneStopIntegration()
    .AddPaymentsIntegration()
    .AddPdfIntegration()
    .AddRegistriesIntegration()
    .AddPortalHealthChecks();

var app = builder.Build();

app.MapPortalHealthChecks();
app.MapAccountsEndpoints();
app.MapApplicationsEndpoints();
app.MapAssistantEndpoints();
app.MapIdentityEndpoints();
app.MapLegalEntityEndpoints();
app.MapLicencesEndpoints();
app.MapPartnersEndpoints();
app.MapReferenceDataEndpoints();
app.MapSpecialEventsEndpoints();
app.MapStaffEndpoints();

app.Run();

public partial class Program;
