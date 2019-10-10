namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class VoteQuestion
    {
        public string id { get; set; }
        public string title { get; set; }
        public string question { get; set; }
        public string slug { get; set; }
        public ViewModels.VoteOption[] options { get; set; }
    }
}
