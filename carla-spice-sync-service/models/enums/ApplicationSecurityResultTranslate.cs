namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public static class ApplicationSecurityScreeningResultTranslate
    {
        const int SECURITY_STATUS_NOT_SENT = 845280000;
        const int SECURITY_STATUS_SENT = 845280001;
        const int SECURITY_STATUS_CONSENT_NOT_VALIDATED = 845280002;
        const int SECURITY_STATUS_WITHDRAWN = 845280003;
        const int SECURITY_STATUS_PASSED = 845280004;
        const int SECURITY_STATUS_FAILED = 845280005;

        public static int? GetTranslatedSecurityStatus(string result)
        {
            int? status = null;
            if (!string.IsNullOrEmpty(result))
            {
                switch (result.ToUpper())
                {
                    case "PASS":
                        status = SECURITY_STATUS_PASSED;
                        break;
                    case "FAIL":
                        status = SECURITY_STATUS_FAILED;
                        break;
                    case "WITHDRAWN":
                        status = SECURITY_STATUS_WITHDRAWN;
                        break;
                    case "REQUEST SENT":
                        status = SECURITY_STATUS_SENT;
                        break;
                    case "REQUEST NOT SENT":
                        status = SECURITY_STATUS_NOT_SENT;
                        break;
                    case "CONSENT NOT VALIDATED":
                        status = SECURITY_STATUS_CONSENT_NOT_VALIDATED;
                        break;
                }
            }
            return status;
        }
    }
}
