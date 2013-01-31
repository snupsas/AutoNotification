namespace AutoNotification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.SqlClient;
    using System.Security.Permissions;
    using System.Data;

    public class Notification
    {
        string connectionString = "Data Source=192.168.*.*;Initial Catalog=SocParama;User ID=******;Password=******";
        SqlDependency dependency = null;
        DataTable dataTable = new DataTable();

        public void Initialization()
        {
            SqlDependency.Stop(connectionString);
            SqlDependency.Start(connectionString);  
        }

        public DataTable SomeMethod(Action<object, SqlNotificationEventArgs> handler)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // notificationas bus iskvieciamas visuomet, kai pasikeis bent vienas duomuo, is sio SELECTO grazinamu duomenu set`o (bus atliktas UPDATE, DELETE, INSERT)
                using (SqlCommand command = new SqlCommand("SELECT StudentasID, Vardas, Pavarde, IBAN, AK FROM dbo.tblIseiviaiStudentai", connection))
                {

                    dependency = new SqlDependency();          
                    command.Notification = null;

                    dependency.AddCommandDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(handler);

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    // kad veiktu notificationas butini sie conection`o nustatymai, kitaip eventas bus kvieciamas nuolatos
                    var ON_PARAMETERS = @"SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER, ARITHABORT ON";
                    var OFF_PARAMETERS = @"SET NUMERIC_ROUNDABORT OFF";
                    var cmd_ON_PARAMETERS = new SqlCommand(ON_PARAMETERS, connection);
                    var cmd_OFF_PARAMETERS = new SqlCommand(OFF_PARAMETERS, connection);
                    cmd_ON_PARAMETERS.ExecuteNonQuery();
                    cmd_OFF_PARAMETERS.ExecuteNonQuery();

                    dataTable.Clear();
                    dataTable.Load(command.ExecuteReader(CommandBehavior.CloseConnection));
                    dependency = null;
                    return dataTable;
                }
            }
        }

        public void Termination()
        {
            SqlDependency.Stop(connectionString);
        }

        public bool CanRequestNotifications()
        {
            try
            {
                SqlClientPermission perm = new SqlClientPermission(PermissionState.Unrestricted);
                perm.Demand();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
