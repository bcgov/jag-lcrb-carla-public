namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public class CompletedWorkerScreening
    {
        public string RecordIdentifier { get; set; }
        public SpiceApplicationStatus Result { get; set; }
        public Worker Worker { get; set; }
    }
}
