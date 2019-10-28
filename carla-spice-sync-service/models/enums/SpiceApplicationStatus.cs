namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public enum SpiceApplicationStatus
    {
        Cleared = 525840010,
        NotCleared = 525840011,
        FitAndProper = 525840021,
        NotFitAndProper = 525840023,
        Withdrawn = 525840012
    }

    public enum LCRBApplicationSecurityStatus
    {
        NotSent = 845280000,
        Sent = 845280001,
        Sending = 845280007,
        Incomplete = 845280002,
        Withdrawn = 845280003,
        Passed = 845280004,
        Failed = 845280005
    }

    public enum LCRBWorkerSecurityStatus
    {
        Pass = 845280000,
        Fail = 845280001,
        Withdrawn = 845280003
    }

    public enum LCRBWorkerSecurityStatusCode
    {
        Active = 1,
        Rejected = 850280005,
        Withdrawn = 850280004
    }

    public class TranslateStatus
    {
        public static LCRBApplicationSecurityStatus? BusinessResultSpiceToLCRB(SpiceApplicationStatus status)
        {
            switch (status)
            {
                case SpiceApplicationStatus.FitAndProper:
                    return LCRBApplicationSecurityStatus.Passed;
                case SpiceApplicationStatus.NotFitAndProper:
                    return LCRBApplicationSecurityStatus.Failed;
                case SpiceApplicationStatus.Withdrawn:
                    return LCRBApplicationSecurityStatus.Withdrawn;
                default:
                    return null;
            }
        }

        public static LCRBWorkerSecurityStatus? WorkerResultSpiceToLCRB(SpiceApplicationStatus status)
        {
            switch (status)
            {
                case SpiceApplicationStatus.Cleared:
                    return LCRBWorkerSecurityStatus.Pass;
                case SpiceApplicationStatus.NotCleared:
                    return LCRBWorkerSecurityStatus.Fail;
                case SpiceApplicationStatus.Withdrawn:
                    return LCRBWorkerSecurityStatus.Withdrawn;
                default:
                    return null;
            }
        }

        public static LCRBWorkerSecurityStatusCode? WorkerResultSpiceToLCRBStatusCode(SpiceApplicationStatus status)
        {
            switch (status)
            {
                case SpiceApplicationStatus.Cleared:
                    return LCRBWorkerSecurityStatusCode.Active;
                case SpiceApplicationStatus.NotCleared:
                    return LCRBWorkerSecurityStatusCode.Rejected;
                case SpiceApplicationStatus.Withdrawn:
                    return LCRBWorkerSecurityStatusCode.Withdrawn;
                default:
                    return null;
            }
        }
    }
}
