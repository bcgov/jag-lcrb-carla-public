using System;

namespace Gov.Lclb.Cllb.OrgbookService
{
  public class Attributes
  {
      public string registration_id { get; set; }
      public string licence_number { get; set; }
      public string establishment_name { get; set; }
      public DateTimeOffset issue_date { get; set; }
      public DateTimeOffset? effective_date { get; set; }
      public DateTimeOffset? expiry_date { get; set; }
      public string civic_address { get; set; }
      public string city { get; set; }
      public string province { get; set; }
      public string postal_code { get; set; }
      public string country { get; set; }
  }
}
