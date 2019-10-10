using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Gov.Lclb.Cllb.Public
{
    public static class DatabaseTools
    {
        /// <summary>
        /// Logic required to generate a connection string.  If no environment variables exists, defaults to a local sql instance.
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString(IConfiguration Configuration)
        {
            string result = "Server=";

            if (!string.IsNullOrEmpty(Configuration["DATABASE_SERVICE_NAME"]))
            {
                result += Configuration["DATABASE_SERVICE_NAME"];
            }
            else // default to a local connection.
            {
                result += "127.0.0.1";
            }

            result += ";Database=";

            result += GetDatabaseName(Configuration);

            if (!string.IsNullOrEmpty(Configuration["DB_USER"]) && !string.IsNullOrEmpty(Configuration["DB_PASSWORD"]))
            {
                result += ";User Id=" + Configuration["DB_USER"] + ";Password=" + Configuration["DB_PASSWORD"] + ";";
            }

            return result;
        }

        public static string GetSaConnectionString(IConfiguration Configuration)
        {
            string result = "Server=tcp:";

            if (!string.IsNullOrEmpty(Configuration["DATABASE_SERVICE_NAME"]))
            {
                result += Configuration["DATABASE_SERVICE_NAME"];
            }
            else // default to a local connection.
            {
                result += "127.0.0.1";
            }

            //result += ";Database=";

            //result += GetDatabaseName(Configuration);

            result += ";Database=master;User Id=SA";

            if (!string.IsNullOrEmpty(Configuration["DB_ADMIN_PASSWORD"]))
            {
                result += ";Password=" + Configuration["DB_ADMIN_PASSWORD"] + ";";
            }

            return result;
        }

        /// <summary>
        /// Create database if it does not exist - used in OpenShift or other environments that do not automatically create the database.
        /// </summary>
        public static void CreateDatabaseIfNotExists(IConfiguration Configuration)
        {
            // only do this if a sa password was supplied.
            if (!string.IsNullOrEmpty(Configuration["DB_ADMIN_PASSWORD"]))
            {
                string password = Configuration["DB_PASSWORD"].Replace("'", "''");
                string username = Configuration["DB_USER"].Replace("'", "''");

                if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(username))
                {
                    string database = GetDatabaseName(Configuration).Replace("'", "''");

                    string saConnectionString = GetSaConnectionString(Configuration);
                    using (SqlConnection conn = new SqlConnection(saConnectionString))
                    {
                        conn.Open();
                        // fix for OpenShift bug where the pod reports the number of sockets / logical processors in the host computer rather than the amount available.
                        string sql = "EXEC sp_configure 'show advanced options', 1;";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        sql = "RECONFIGURE WITH OVERRIDE;";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        sql = "EXEC sp_configure 'max degree of parallelism', 2;";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        sql = "RECONFIGURE WITH OVERRIDE;";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();


                        // create the login if it does not exist.
                        sql = "IF NOT EXISTS (SELECT name FROM master.sys.server_principals    WHERE name = '" + username + "') BEGIN\n CREATE LOGIN " + username + " WITH PASSWORD = '" + password + "';\nEND";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        sql = "IF  NOT EXISTS(SELECT name FROM sys.databases WHERE name = N'" + database + "')\nBEGIN\nCREATE DATABASE[" + database + "]; ALTER AUTHORIZATION ON DATABASE::[" + database + "] TO " + username + "\nEND";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        sql = "USE " + database + "; IF NOT EXISTS (SELECT su.name as DatabaseUser FROM sys.sysusers su join sys.syslogins sl on sl.sid = su.sid where sl.name = '" + username + "')\nBEGIN\nCREATE USER " + username + " FOR LOGIN " + username + ";END";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Returns the name of the database, as set in the environment.
        /// </summary>
        /// <returns></returns>
        public static string GetDatabaseName(IConfiguration Configuration)
        {
            string result = "";
            if (!string.IsNullOrEmpty(Configuration["DB_DATABASE"]))
            {
                result += Configuration["DB_DATABASE"];
            }
            else // default to a local connection.
            {
                result += "Surveys";
            }

            return result;
        }
    }
}
