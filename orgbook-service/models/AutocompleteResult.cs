using System;

namespace Gov.Lclb.Cllb.OrgbookService
{

    public class Name {
        public int id { get; set; }
        public string text { get; set; }
        public string language { get; set; }
        public int credential_id { get; set; }
        public string type { get; set; }
    }

    public class AutocompleteResult
    {
        public int id  { get; set; }
        public bool inactive { get; set; }
        public Name[] names { get; set; }
    }
}
