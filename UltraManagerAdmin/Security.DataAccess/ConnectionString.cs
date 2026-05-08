namespace Security.DataAccess
{
   static class ConnectionString
    {
        public static string Get()
        {
            return Properties.Settings.Default.AVS_SECURITYConnectionString1.ToString().Trim();
        }
    }
}
