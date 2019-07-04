using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public interface IOrgBookClient
    {
        string BaseUrl { get; set; }
        bool ReadResponseAsString { get; set; }

        Task<Address> V2AddressGetAsync(int id);
        Task<Address> V2AddressGetAsync(int id, CancellationToken cancellationToken);
        Task<Response> V2AddressGetAsync(int? page, int? page_size);
        Task<Response> V2AddressGetAsync(int? page, int? page_size, CancellationToken cancellationToken);
        Task<Attribute> V2AttributeGetAsync(int id);
        Task<Attribute> V2AttributeGetAsync(int id, CancellationToken cancellationToken);
        Task<Response2> V2AttributeGetAsync(int? page, int? page_size);
        Task<Response2> V2AttributeGetAsync(int? page, int? page_size, CancellationToken cancellationToken);
        Task<Response4> V2CredentialActiveAsync(int? page, int? page_size);
        Task<Response4> V2CredentialActiveAsync(int? page, int? page_size, CancellationToken cancellationToken);
        Task<CredentialSearch> V2CredentialFormattedAsync(int id);
        Task<CredentialSearch> V2CredentialFormattedAsync(int id, CancellationToken cancellationToken);
        Task<Credential> V2CredentialGetAsync(int id);
        Task<Credential> V2CredentialGetAsync(int id, CancellationToken cancellationToken);
        Task<Response3> V2CredentialGetAsync(int? page, int? page_size);
        Task<Response3> V2CredentialGetAsync(int? page, int? page_size, CancellationToken cancellationToken);
        Task<Response5> V2CredentialHistoricalAsync(int? page, int? page_size);
        Task<Response5> V2CredentialHistoricalAsync(int? page, int? page_size, CancellationToken cancellationToken);
        Task<Credential> V2CredentialLatestAsync(int id);
        Task<Credential> V2CredentialLatestAsync(int id, CancellationToken cancellationToken);
        Task<CredentialType> V2CredentialtypeGetAsync(int id);
        Task<CredentialType> V2CredentialtypeGetAsync(int id, CancellationToken cancellationToken);
        Task<Response6> V2CredentialtypeGetAsync(int? page, int? page_size);
        Task<Response6> V2CredentialtypeGetAsync(int? page, int? page_size, CancellationToken cancellationToken);
        Task<CredentialType> V2CredentialtypeLanguageAsync(int id);
        Task<CredentialType> V2CredentialtypeLanguageAsync(int id, CancellationToken cancellationToken);
        Task<CredentialType> V2CredentialtypeLogoAsync(int id);
        Task<CredentialType> V2CredentialtypeLogoAsync(int id, CancellationToken cancellationToken);
        Task V2FeedbackPostAsync(string from_name, string from_email, string comments);
        Task V2FeedbackPostAsync(string from_name, string from_email, string comments, CancellationToken cancellationToken);
        Task V2FeedbackPostAsync(string from_name, string from_email, string comments, string format);
        Task V2FeedbackPostAsync(string from_name, string from_email, string comments, string format, CancellationToken cancellationToken);
        Task<Issuer> V2IssuerCredentialtypeAsync(int id);
        Task<Issuer> V2IssuerCredentialtypeAsync(int id, CancellationToken cancellationToken);
        Task<Issuer> V2IssuerGetAsync(int id);
        Task<Issuer> V2IssuerGetAsync(int id, CancellationToken cancellationToken);
        Task<Response7> V2IssuerGetAsync(int? page, int? page_size);
        Task<Response7> V2IssuerGetAsync(int? page, int? page_size, CancellationToken cancellationToken);
        Task<Issuer> V2IssuerLogoAsync(int id);
        Task<Issuer> V2IssuerLogoAsync(int id, CancellationToken cancellationToken);
        Task<Name> V2NameGetAsync(int id);
        Task<Name> V2NameGetAsync(int id, CancellationToken cancellationToken);
        Task<Response8> V2NameGetAsync(int? page, int? page_size);
        Task<Response8> V2NameGetAsync(int? page, int? page_size, CancellationToken cancellationToken);
        Task V2QuickloadGetAsync();
        Task V2QuickloadGetAsync(CancellationToken cancellationToken);
        Task V2QuickloadGetAsync(string format);
        Task V2QuickloadGetAsync(string format, CancellationToken cancellationToken);
        Task<Response9> V2SchemaGetAsync(double? id, string name, string version, string origin_did, int? page, int? page_size);
        Task<Response9> V2SchemaGetAsync(double? id, string name, string version, string origin_did, int? page, int? page_size, CancellationToken cancellationToken);
        Task<Schema> V2SchemaGetAsync(int id);
        Task<Schema> V2SchemaGetAsync(int id, CancellationToken cancellationToken);
        Task<CredentialAutocomplete> V2SearchAutocompleteGetAsync(string id);
        Task<CredentialAutocomplete> V2SearchAutocompleteGetAsync(string id, CancellationToken cancellationToken);
        Task<ICollection<CredentialAutocomplete>> V2SearchAutocompleteGetAsync(string ordering, string q, Inactive? inactive, Latest? latest, Revoked? revoked, string category);
        Task<ICollection<CredentialAutocomplete>> V2SearchAutocompleteGetAsync(string ordering, string q, Inactive? inactive, Latest? latest, Revoked? revoked, string category, CancellationToken cancellationToken);
        Task<Response11> V2SearchCredentialFacetsAsync(string ordering, int? page, int? page_size);
        Task<Response11> V2SearchCredentialFacetsAsync(string ordering, int? page, int? page_size, CancellationToken cancellationToken);
        Task<CredentialSearch> V2SearchCredentialGetAsync(string name, Inactive5? inactive, Latest5? latest, Revoked5? revoked, string category, string credential_type_id, string issuer_id, string topic_id, string id);
        Task<CredentialSearch> V2SearchCredentialGetAsync(string name, Inactive5? inactive, Latest5? latest, Revoked5? revoked, string category, string credential_type_id, string issuer_id, string topic_id, string id, CancellationToken cancellationToken);
        Task<Response10> V2SearchCredentialGetAsync(string ordering, int? page, int? page_size, string name, Inactive2? inactive, Latest2? latest, Revoked2? revoked, string category, string credential_type_id, string issuer_id, string topic_id);
        Task<Response10> V2SearchCredentialGetAsync(string ordering, int? page, int? page_size, string name, Inactive2? inactive, Latest2? latest, Revoked2? revoked, string category, string credential_type_id, string issuer_id, string topic_id, CancellationToken cancellationToken);
        Task<Response13> V2SearchCredentialTopicFacetsAsync(string ordering, int? page, int? page_size);
        Task<Response13> V2SearchCredentialTopicFacetsAsync(string ordering, int? page, int? page_size, CancellationToken cancellationToken);
        Task<CredentialTopicSearch> V2SearchCredentialTopicGetAsync(string name, Inactive4? inactive, Latest4? latest, Revoked4? revoked, string category, string credential_type_id, string issuer_id, string topic_id, string id);
        Task<CredentialTopicSearch> V2SearchCredentialTopicGetAsync(string name, Inactive4? inactive, Latest4? latest, Revoked4? revoked, string category, string credential_type_id, string issuer_id, string topic_id, string id, CancellationToken cancellationToken);
        Task<Response12> V2SearchCredentialTopicGetAsync(string ordering, int? page, int? page_size, string name, Inactive3? inactive, Latest3? latest, Revoked3? revoked, string category, string credential_type_id, string issuer_id, string topic_id);
        Task<Response12> V2SearchCredentialTopicGetAsync(string ordering, int? page, int? page_size, string name, Inactive3? inactive, Latest3? latest, Revoked3? revoked, string category, string credential_type_id, string issuer_id, string topic_id, CancellationToken cancellationToken);
        Task<Topic> V2TopicCredentialActiveAsync(int id);
        Task<Topic> V2TopicCredentialActiveAsync(int id, CancellationToken cancellationToken);
        Task<Topic> V2TopicCredentialAsync(int id);
        Task<Topic> V2TopicCredentialAsync(int id, CancellationToken cancellationToken);
        Task<Topic> V2TopicCredentialHistoricalAsync(int id);
        Task<Topic> V2TopicCredentialHistoricalAsync(int id, CancellationToken cancellationToken);
        Task<Topic> V2TopicCredentialsetAsync(int id);
        Task<Topic> V2TopicCredentialsetAsync(int id, CancellationToken cancellationToken);
        Task<Topic> V2TopicFormattedAsync(int id);
        Task<Topic> V2TopicFormattedAsync(int id, CancellationToken cancellationToken);
        Task<Topic> V2TopicGetAsync(int id);
        Task<Topic> V2TopicGetAsync(int id, CancellationToken cancellationToken);
        Task<Response14> V2TopicGetAsync(int? page, int? page_size);
        Task<Response14> V2TopicGetAsync(int? page, int? page_size, CancellationToken cancellationToken);
        Task<Topic> V2TopicIdentAsync(int? page, int? page_size, string source_id, string type);
        Task<Topic> V2TopicIdentAsync(int? page, int? page_size, string source_id, string type, CancellationToken cancellationToken);
        Task<Response16> V2TopicIdentFormattedAsync(int? page, int? page_size, string source_id, string type);
        Task<Response16> V2TopicIdentFormattedAsync(int? page, int? page_size, string source_id, string type, CancellationToken cancellationToken);
        Task<Topic> V2TopicRelatedFromAsync(int id);
        Task<Topic> V2TopicRelatedFromAsync(int id, CancellationToken cancellationToken);
        Task<Topic> V2TopicRelatedToAsync(int id);
        Task<Topic> V2TopicRelatedToAsync(int id, CancellationToken cancellationToken);
    }
}