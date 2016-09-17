using System.Data.SqlClient;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;


public class MSSQL
{
    //    private const int _DB_TIMEOUT = 30;
    //    private string _DB_NAME = "travoxmos";
    //    private string _DB_USER = "travoxmos";
    //    private string _DB_PASS = "systrav";

    //    private string _DB_SERVER = "db3.ns.co.th";
    //    private enum QueryCase
    //    {
    //        INSERT,
    //        UPDATE,
    //        DELETE,
    //        SELECT
    //    }

    //    protected SqlTransaction transection;
    //    protected SqlConnection connection;

    //    public bool Connected
    //    {
    //        get { return !(connection.State == ConnectionState.Closed); }
    //    }
    //    string ConnectionString
    //    {
    //        get {

    //            return string.Format("", _DB_NAME, _DB_USER, _DB_PASS, _DB_SERVER);
    //        }
    //    }

    //    public MSSQL(string database = null, string servername = "db3.ns.co.th", string username = "travoxmos", string password = "systrav")
    //    {
    //        if (MBOS.Null(database))
    //        {
    //            _DB_NAME = MBOS.CompanyBase();
    //        }
    //        else
    //        {
    //            _DB_NAME = database;
    //        }
    //        _DB_SERVER = servername;
    //        _DB_USER = username;
    //        _DB_PASS = password;
    //    }

    //    protected override void MSSQL()
    //    {
    //        transection = null;
    //        connection = null;
    //        if (connection.State == ConnectionState.Open)
    //            connection.Close();

    //        base.Finalize();
    //    }

    //    public string Execute(string query, ParameterCollection param = null)
    //    {
    //        string AfterInsertID = "";
    //        if (param == null)
    //            param = new ParameterCollection();
    //        if ((connection == null || connection.State == ConnectionState.Closed))
    //        {
    //            connection = new SqlConnection(this.ConnectionString());
    //            connection.Open();
    //            transection = connection.BeginTransaction();
    //        }
    //        if ((this.Connected))
    //        {
    //            SqlCommand command = BuildCommands(query, connection, param);
    //            command.Transaction = transection;
    //            AfterInsertID = command.ExecuteScalar();
    //        }
    //        return AfterInsertID;
    //    }

    //    public bool Apply()
    //    {
    //        bool result = false;
    //        try
    //        {
    //            transection.Commit();
    //            connection.Close();
    //            result = true;
    //        }
    //        catch (Exception ex)
    //        {
    //            transection.Rollback();
    //            connection.Close();
    //            throw new Exception(ex.Message());
    //        }
    //        return result;
    //    }

    //    public void Rollback()
    //    {
    //        try
    //        {
    //            transection.Rollback();
    //            connection.Close();
    //        }
    //        catch
    //        {
    //        }
    //    }

    //    public string GetField(string query, ParameterCollection param = null)
    //    {
    //        if (param == null)
    //            param = new ParameterCollection();
    //        DataTable dtQuery = GetTable(query, param);
    //        string result = "";
    //        if ((dtQuery.Columns.Count >= 1 & dtQuery.Rows.Count >= 1))
    //        {
    //            result = dtQuery.Rows(0)(0).ToString();
    //        }
    //        return result;
    //    }

    //    public DataTable GetTable(string query, ParameterCollection param = null)
    //    {
    //        if (param == null)
    //            param = new ParameterCollection();
    //        return GetTable("MBOS_TableQuery", query, param);
    //    }

    //    public DataTable GetTable(string table_name, string query, ParameterCollection param = null)
    //    {
    //        DataTable result = new DataTable();
    //        SqlTransaction trans;
    //        SqlConnection conn = new SqlConnection(this.ConnectionString());

    //        if (param == null)
    //            param = new ParameterCollection();

    //        conn.Open();
    //        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted);
    //        SqlCommand mbosCommand = BuildCommands(query, conn, param);
    //        mbosCommand.Transaction = trans;
    //        SqlDataAdapter adapter = new SqlDataAdapter(mbosCommand);
    //        try
    //        {
    //            adapter.Fill(result);
    //            trans.Commit();
    //        }
    //        catch (Exception ex)
    //        {
    //            trans.Rollback();
    //            throw new Exception(ex.Message(), new Exception("DB.GetTable()"));
    //        }
    //        conn.Close();

    //        return result;
    //    }

    //    //Public Function StoredParamOne(ByVal store_name As String, ByVal schema_name As Schema, ByVal param As ParameterCollection) As String
    //    //    param = StoredParam(store_name, schema_name, param)
    //    //    Dim result As String = StoreOutput
    //    //    StoreOutput = Nothing
    //    //    If (Not String.IsNullOrEmpty(result)) Then result = param.Item(result).DefaultValue.ToString()
    //    //    Return result
    //    //End Function
    //    public NameValueCollection StoredProcedure(string store_name, ParameterCollection param = null)
    //    {
    //        NameValueCollection result = new NameValueCollection();
    //        SqlConnection conn = new SqlConnection(this.ConnectionString());
    //        if (param == null)
    //            param = new ParameterCollection();

    //        conn.Open();
    //        SqlCommand mbosCommand = BuildCommands(store_name, conn, param);
    //        mbosCommand.CommandType = CommandType.StoredProcedure;
    //        mbosCommand.Connection = conn;
    //        mbosCommand.ExecuteNonQuery();
    //        SqlDataAdapter adapter = new SqlDataAdapter(mbosCommand);
    //        mbosCommand.ExecuteNonQuery();
    //        conn.Close();
    //        foreach (Parameter para in param)
    //        {
    //            result.Add(para.Name, para.DefaultValue);
    //        }
    //        return result;
    //    }

    //    public static string InitializeSiteCustomer(string code, Schema db_column)
    //    {
    //        string result = null;
    //        DB @base = new DB("travox_global");
    //        DataTable dtSiteCustomer = @base.GetTable(My.Resources.DB_SITECUSTOMER, new SQLCollection("@com_code", DbType.String, code));
    //        switch (db_column)
    //        {
    //            case Schema.MBOS:
    //                result = "database_name";
    //            default:
    //                result = "company_name";
    //        }
    //        if ((dtSiteCustomer.Rows.Count > 0))
    //            result = dtSiteCustomer.Rows(0)(result).ToString();
    //        else
    //            result = null;
    //        return result;
    //    }

    //    private SqlCommand BuildCommands(string query, SqlConnection mbosConn, ParameterCollection param)
    //    {
    //        QueryCase qCase = QueryCase.SELECT;
    //        try
    //        {
    //            if ((query.Contains("SQL::")))
    //                query = MBOS.FileRead("!SQLStore\\" + query.Replace("SQL::", "") + ".sql");
    //            if ((query.ToLower().Contains("insert into") & query.ToLower().Contains("values")))
    //            {
    //                qCase = QueryCase.INSERT;
    //                query += " SELECT @@IDENTITY";
    //            }
    //            else if ((query.ToLower().Contains("update") & query.ToLower().Contains("set")))
    //            {
    //                qCase = QueryCase.UPDATE;
    //            }
    //            else if ((query.ToLower().Contains("delete") & query.ToLower().Contains("from")))
    //            {
    //                qCase = QueryCase.DELETE;
    //            }
    //        }
    //        catch
    //        {
    //            throw new Exception("SQL Query file", new Exception("SQL query file path is not exists."));
    //        }

    //        SqlCommand mbosCommand = new SqlCommand(query, mbosConn);
    //        mbosCommand.CommandTimeout = _DB_TIMEOUT;
    //        foreach (Parameter para in param)
    //        {
    //            if ((para.Name.Contains("@")))
    //            {
    //                if ((para.Direction != ParameterDirection.Input))
    //                {
    //                    mbosCommand.Parameters.Add(para.Name, SqlDbType.VarChar, 8000);
    //                }
    //                else
    //                {
    //                    int paramSize = 0;
    //                    if ((para.DefaultValue != null))
    //                        paramSize = para.DefaultValue.Length;
    //                    mbosCommand.Parameters.Add(para.Name, para.DbType, paramSize);
    //                    mbosCommand.Parameters.Item(para.Name).Size = paramSize;
    //                    mbosCommand.Parameters.Item(para.Name).DbType = para.DbType;
    //                    mbosCommand.Parameters.Item(para.Name).Value = DBNull.Value;
    //                }
    //                mbosCommand.Parameters.Item(para.Name).Direction = para.Direction;
    //                switch (qCase)
    //                {
    //                    case QueryCase.SELECT:
    //                        if (para.DbType == DbType.Date | para.DbType == DbType.DateTime | para.DbType == DbType.DateTime2)
    //                        {
    //                            mbosCommand.Parameters.Item(para.Name).DbType = DbType.String;
    //                            mbosCommand.Parameters.Item(para.Name).Value = MBOS.Convert<DateTime>(para.DefaultValue).ToString("yyyy-MM-dd HH:mm:ss");
    //                        }
    //                        else if (((para.DbType == DbType.String & para.DefaultValue != null) | !string.IsNullOrEmpty(para.DefaultValue)))
    //                        {
    //                            mbosCommand.Parameters.Item(para.Name).Value = para.DefaultValue;
    //                        }
    //                    default:
    //                        switch (para.DbType)
    //                        {
    //                            case DbType.String:
    //                                if ((para.DefaultValue != null))
    //                                {
    //                                    mbosCommand.Parameters.Item(para.Name).Value = para.DefaultValue;
    //                                }
    //                            case DbType.Decimal:
    //                                if ((para.DefaultValue != null))
    //                                {
    //                                    mbosCommand.Parameters.Item(para.Name).Value = MBOS.Dec(para.DefaultValue);
    //                                }
    //                            case DbType.Date:
    //                                mbosCommand.Parameters.Item(para.Name).Size = 10;
    //                                mbosCommand.Parameters.Item(para.Name).DbType = DbType.String;
    //                                if ((!string.IsNullOrEmpty(para.DefaultValue)))
    //                                {
    //                                    mbosCommand.Parameters.Item(para.Name).Value = MBOS.Convert<DateTime>(para.DefaultValue).ToString("yyyy-MM-dd");
    //                                }
    //                                query = query.Replace(para.Name, "CONVERT(VARCHAR," + para.Name + ", 105)");
    //                            case DbType.DateTime:
    //                            case DbType.DateTime2:
    //                                mbosCommand.Parameters.Item(para.Name).Size = 18;
    //                                mbosCommand.Parameters.Item(para.Name).DbType = DbType.String;
    //                                if ((!string.IsNullOrEmpty(para.DefaultValue)))
    //                                {
    //                                    mbosCommand.Parameters.Item(para.Name).Value = MBOS.Convert<DateTime>(para.DefaultValue).ToString("yyyy-MM-dd HH:mm:ss");
    //                                }
    //                                query = query.Replace(para.Name, "CONVERT(DATETIME," + para.Name + ", 120)");
    //                            default:
    //                                if ((!string.IsNullOrEmpty(para.DefaultValue)))
    //                                {
    //                                    mbosCommand.Parameters.Item(para.Name).Value = para.DefaultValue;
    //                                }
    //                        }
    //                }
    //            }
    //            else if ((query.Contains(para.Name)))
    //            {
    //                query = query.Replace("/*" + para.Name + "*/", " " + para.DefaultValue + " ");
    //            }
    //        }
    //        mbosCommand.CommandText = query;
    //        return mbosCommand;
    //    }

}