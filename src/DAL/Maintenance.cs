using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBsoft.Appointment.DAL
{
    public static class Maintenance
    {        
        public static string SQLiteQueryDir { get { return AppDomain.CurrentDomain.BaseDirectory + @"Query"; } }

       
        public static class SQLite
        {
            public static void CheckServer(string dbFile)
            {
                try
                {
                    string connString = string.Format("Data Source = {0}; Version = 3; UseUTF16Encoding = True;", dbFile);
                    System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(connString);
                    conn.Open();
                    conn.Close();
                }
                catch { throw; }
            }
                       
            public static bool CheckDb(string companyCode, string sqliteDir)
            {
                string dbFile = System.IO.Path.Combine(sqliteDir, companyCode) + ".sqlite";
                System.IO.FileInfo f = new System.IO.FileInfo(dbFile);
                if (f.Exists)
                    return true;
                else
                    return false;


            }
            public static void CreateDb(string companyCode, string sqliteDir)
            {
                string dbFile = System.IO.Path.Combine(sqliteDir, companyCode) + ".sqlite";
                System.IO.FileInfo f = new System.IO.FileInfo(dbFile);
                string connString = string.Format("Data Source = {0}; Version = 3; UseUTF16Encoding = True;", dbFile);

                // Check if admin database exists
                try
                {
                    using (System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(connString))
                    {
                        conn.Open();
                        conn.Close();
                    }
                }
                catch { throw; }

            }
            public static void UpdateDb(string companyCode, string sqliteDir)
            {
                string dbFile = System.IO.Path.Combine(sqliteDir, companyCode) + ".sqlite";
                System.IO.FileInfo f = new System.IO.FileInfo(dbFile);
                string connString = string.Format("Data Source = {0}; Version = 3; UseUTF16Encoding = True;", dbFile);

                int CurrentDbVersion = 0;
                int AvailableUpdateVersion = 0;

                using (System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection(connString))
                {
                    conn.Open();
                    System.Data.SQLite.SQLiteTransaction trans = conn.BeginTransaction();
                    // Get Current Database Version
                    try
                    {
                        using (System.Data.SQLite.SQLiteCommand cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = trans;
                            cmd.CommandText = "SELECT [DbVersion] FROM [DbVersion] ORDER BY [Id] DESC";
                            try
                            {
                                object dbv = cmd.ExecuteScalar();
                                CurrentDbVersion = (int)dbv;
                            }
                            catch { CurrentDbVersion = 0; }


                            // Get Update Files
                            List<DbVersionFile> UpdateFiles = new List<DbVersionFile>();
                            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(SQLiteQueryDir);
                            foreach (var item in di.GetFiles("Appointment*.sql", System.IO.SearchOption.TopDirectoryOnly))
                            {
                                string sversion = item.Name.Replace("Appointment", "").Replace(".sql", "");
                                int version = int.Parse(sversion);
                                UpdateFiles.Add(new DbVersionFile() { DbVersion = version, QueryFile = item });
                            }
                            UpdateFiles.Sort();
                            AvailableUpdateVersion = UpdateFiles.Last().DbVersion;

                            List<DbVersionFile> FilesToExecute = new List<DbVersionFile>();
                            foreach (var item in UpdateFiles)
                            {
                                if (item.DbVersion > CurrentDbVersion)
                                    FilesToExecute.Add(item);
                            }

                            foreach (var sqlFile in FilesToExecute)
                            {
                                ExecuteSqlFile(conn, trans, sqlFile);
                                try
                                {
                                    cmd.CommandText = String.Format("INSERT INTO [DbVersion] ([DbVersion],[UpdateDate]) VALUES ({0},'{1}')",
                                        sqlFile.DbVersion,
                                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                    int res = cmd.ExecuteNonQuery();
                                    if (res != 1)
                                        throw new ApplicationException("Db Version Update failed");
                                }
                                catch
                                {
                                    throw;
                                }

                            }
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }


            private static void ExecuteSqlFile(System.Data.Common.DbConnection conn, System.Data.Common.DbTransaction trans, DbVersionFile sqlFile)
            {

                StringBuilder query = new StringBuilder();
                List<string> QuerysToExecute = new List<string>();

                int TotalLines = 0;
                int currentLine = 0;
                double unitPercent = 0d;

                OnProgressChanged(new ProgressEventArgs(0, 100, string.Format("Processing {0}", sqlFile.QueryFile.Name)));

                using (System.IO.StreamReader rdr = new System.IO.StreamReader(sqlFile.QueryFile.FullName, Encoding.Default))
                {
                    while (rdr.Peek() >= 0)
                    {
                        string line = rdr.ReadLine();
                        TotalLines++;
                    }
                    rdr.Close();
                }

                unitPercent = 1d * 100d / (double)TotalLines;


                bool AddQuery = true;
                using (System.IO.StreamReader rdr = new System.IO.StreamReader(sqlFile.QueryFile.FullName, Encoding.Default))
                {
                    while (rdr.Peek() >= 0)
                    {
                        currentLine++;
                        double percentvalue = (double)currentLine * unitPercent;
                        string line = rdr.ReadLine();

                        OnProgressChanged(new ProgressEventArgs((int)percentvalue, 100, string.Format("Parsing line {0}", currentLine)));
                        if (line.ToUpper() == "GO")
                        {
                            if (AddQuery)
                                QuerysToExecute.Add(query.ToString());

                            AddQuery = true;
                            query.Clear();
                        }
                        else
                        {
                            if (line == "" || line.StartsWith("/*"))
                            {
                                // Empty line or comment. Do nothing.
                            }
                            else
                            {
                                query.AppendLine(line);
                            }
                        }
                    }
                    rdr.Close();
                }

                System.Data.Common.DbCommand cmd;
                if (conn is System.Data.SQLite.SQLiteConnection)
                {
                    cmd = new System.Data.SQLite.SQLiteCommand();
                    cmd.Connection = conn as System.Data.SQLite.SQLiteConnection;
                    cmd.Transaction = trans as System.Data.SQLite.SQLiteTransaction;

                }
                else
                {
                    cmd = null;
                }

                TotalLines = QuerysToExecute.Count;
                unitPercent = 1d * 100d / (double)TotalLines; ;
                currentLine = 0;
                foreach (string item in QuerysToExecute)
                {
                    currentLine++;
                    double percentvalue = (double)currentLine * unitPercent;
                    OnProgressChanged(new ProgressEventArgs((int)percentvalue, 100, string.Format("Executing line {0} of {1}\n\r{2}", currentLine, TotalLines, item)));
                    cmd.CommandText = item;
                    int res;
                    try { res = cmd.ExecuteNonQuery(); }
                    catch (Exception ex01)
                    {
                        OnProgressChanged(new ProgressEventArgs(0, 100, "Error: " + ex01.Message));
                        throw;
                    }
                    System.Threading.Thread.Sleep(5);
                }
            }
        }




        private static void OnProgressChanged(ProgressEventArgs e)
        {
            ProgressChanged?.Invoke("DAL.Maintenance", e);
        }
        private static void OnUpdateFinished(EventArgs e)
        {
            UpdateFinished?.Invoke("DAL.Maintenance", e);
        }


        public static event ProgressEventHandler ProgressChanged;
        public static event EventHandler UpdateFinished;

        public delegate void ProgressEventHandler(object sender, ProgressEventArgs e);

        #region Nested Classes
        public class DbVersionFile : IComparable<DbVersionFile>
        {
            public int DbVersion { get; set; }
            public System.IO.FileInfo QueryFile { get; set; }

            public int CompareTo(DbVersionFile other)
            {
                return DbVersion.CompareTo(other.DbVersion);
            }
        }

        public class ProgressEventArgs : System.EventArgs
        {
            double currentValue;
            double maxValue;
            object status;

            public ProgressEventArgs(double CurrentValue, double MaxValue, object Status)
            {
                currentValue = (ulong)CurrentValue;
                maxValue = (ulong)MaxValue;
                status = Status;
            }

            public double CurrentValue { get { return currentValue; } set { currentValue = value; } }
            public double MaxValue { get { return maxValue; } set { maxValue = value; } }
            public object Status { get { return status; } set { status = value; } }
        }
        #endregion
    }
}
