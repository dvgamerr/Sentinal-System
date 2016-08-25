Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports Travox.Systems.DataCollection
Imports System.Web.UI.WebControls
Imports System.Collections.Specialized


Public Class DB
    Private Const _DB_TIMEOUT As Integer = 30
    Private _DB_NAME As String = "travoxmos"
    Private _DB_USER As String = "travoxmos"
    Private _DB_PASS As String = "systrav"
    Private _DB_SERVER As String = "db3.ns.co.th"

    Public Enum Schema
        [GLOBAL]
        [MBOS]
        [HOTEL]
        [GROUPTOUR]
        [TRANSFER]
    End Enum
    Private Enum QueryCase
        [INSERT]
        [UPDATE]
        [DELETE]
        [SELECT]
    End Enum


    Private StoreOutput As String
    Protected transection As SqlTransaction
    Protected connection As SqlConnection

    Public ReadOnly Property Connected() As Boolean
        Get
            Return Not (connection Is Nothing OrElse connection.State = ConnectionState.Closed)
        End Get
    End Property
    ReadOnly Property ConnectionString() As String
        Get
            Return String.Format(My.Resources.ConnectionMBOS, _DB_NAME, _DB_USER, _DB_PASS, _DB_SERVER)
        End Get
    End Property

    Public Sub New(Optional ByVal database As String = Nothing, Optional ByVal servername As String = "db3.ns.co.th", Optional ByVal username As String = "travoxmos", Optional ByVal password As String = "systrav")
        If MBOS.Null(database) Then
            _DB_NAME = MBOS.CompanyBase()
        Else
            _DB_NAME = database
        End If
        _DB_SERVER = servername
        _DB_USER = username
        _DB_PASS = password
    End Sub

    Protected Overrides Sub Finalize()
        transection = Nothing
        connection = Nothing
        If (Not connection Is Nothing) Then connection.Close()
        MyBase.Finalize()
    End Sub

    Public Function Execute(query As String, Optional param As ParameterCollection = Nothing) As String
        Dim AfterInsertID As String = ""
        If param Is Nothing Then param = New ParameterCollection()
        If (connection Is Nothing OrElse connection.State = ConnectionState.Closed) Then
            connection = New SqlConnection(Me.ConnectionString())
            Do While connection Is Nothing OrElse connection.State = ConnectionState.Closed
                Try
                    connection.Open()
                Catch
                    Console.WriteLine("Id {0}-{1} Waiting...", Threading.Thread.CurrentThread.ManagedThreadId, _DB_NAME)
                    Threading.Thread.Sleep(5000)
                End Try
            Loop
            transection = connection.BeginTransaction()
        End If
        If (Me.Connected) Then
            Dim command As SqlCommand = BuildCommands(query, connection, param)
            command.Transaction = transection
            AfterInsertID = command.ExecuteScalar()
        End If
        Return AfterInsertID
    End Function

    Public Function Apply() As Boolean
        Dim result As Boolean = False
        Try
            transection.Commit()
            connection.Close()
            result = True
        Catch ex As Exception
            transection.Rollback()
            connection.Close()
            Throw New Exception(ex.Message())
        End Try
        Return result
    End Function

    Public Sub Rollback()
        Try
            transection.Rollback()
            connection.Close()
        Catch
        End Try
    End Sub

    Public Function GetField(query As String, Optional param As ParameterCollection = Nothing) As String
        If param Is Nothing Then param = New ParameterCollection()
        Dim dtQuery As DataTable = GetTable(query, param)
        Dim result As String = ""
        If (dtQuery.Columns.Count >= 1 And dtQuery.Rows.Count >= 1) Then
            result = dtQuery.Rows(0)(0).ToString()
        End If
        Return result
    End Function

    Public Function GetTable(query As String, Optional param As ParameterCollection = Nothing) As DataTable
        If param Is Nothing Then param = New ParameterCollection()
        Return GetTable("MBOS_TableQuery", query, param)
    End Function

    Public Function GetTable(table_name As String, query As String, Optional param As ParameterCollection = Nothing) As DataTable
        Dim result As New DataTable
        Dim trans As SqlTransaction, conn As New SqlConnection(Me.ConnectionString())

        If param Is Nothing Then param = New ParameterCollection()
        Do While conn Is Nothing OrElse conn.State = ConnectionState.Closed
            Try
                conn.Open()
            Catch
                Console.WriteLine("Id {0}-{1} Waiting...", Threading.Thread.CurrentThread.ManagedThreadId, _DB_NAME)
                Threading.Thread.Sleep(5000)
            End Try
        Loop

        trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted)
        Dim mbosCommand As SqlCommand = BuildCommands(query, conn, param)
        mbosCommand.Transaction = trans
        Dim adapter As New SqlDataAdapter(mbosCommand)
        Try
            adapter.Fill(result)
            trans.Commit()
        Catch ex As Exception
            trans.Rollback()
            Throw New Exception(ex.Message(), New Exception("DB.GetTable()"))
        End Try
        conn.Close()

        Return result
    End Function

    'Public Function StoredParamOne(ByVal store_name As String, ByVal schema_name As Schema, ByVal param As ParameterCollection) As String
    '    param = StoredParam(store_name, schema_name, param)
    '    Dim result As String = StoreOutput
    '    StoreOutput = Nothing
    '    If (Not String.IsNullOrEmpty(result)) Then result = param.Item(result).DefaultValue.ToString()
    '    Return result
    'End Function
    Public Function StoredProcedure(ByVal store_name As String, Optional ByVal param As ParameterCollection = Nothing) As NameValueCollection
        Dim result As New NameValueCollection()
        Dim conn As New SqlConnection(Me.ConnectionString())
        If param Is Nothing Then param = New ParameterCollection()

        Do While conn Is Nothing OrElse conn.State = ConnectionState.Closed
            Try
                conn.Open()
            Catch
                Console.WriteLine("Id {0}-{1} Waiting...", Threading.Thread.CurrentThread.ManagedThreadId, _DB_NAME)
                Threading.Thread.Sleep(5000)
            End Try
        Loop

        Dim mbosCommand As SqlCommand = BuildCommands(store_name, conn, param)
        mbosCommand.CommandType = CommandType.StoredProcedure
        mbosCommand.Connection = conn
        mbosCommand.ExecuteNonQuery()
        Dim adapter As New SqlDataAdapter(mbosCommand)
        mbosCommand.ExecuteNonQuery()
        conn.Close()
        For Each para As Parameter In param
            result.Add(para.Name, para.DefaultValue)
        Next
        Return result
    End Function

    Public Shared Function InitializeSiteCustomer(code As String, db_column As Schema) As String
        Dim result As String = Nothing
        Dim base As New DB("travox_global")
        Dim dtSiteCustomer As DataTable = base.GetTable(My.Resources.DB_SITECUSTOMER, New SQLCollection("@com_code", DbType.String, code))
        Select Case db_column
            Case Schema.MBOS : result = "database_name"
            Case Else : result = "company_name"
        End Select
        If (dtSiteCustomer.Rows.Count > 0) Then result = dtSiteCustomer.Rows(0)(result).ToString() Else result = Nothing
        Return result
    End Function

    Private Function BuildCommands(ByVal query As String, ByVal mbosConn As SqlConnection, ByVal param As ParameterCollection) As SqlCommand
        Dim qCase As QueryCase = QueryCase.SELECT
        Try
            If (query.Contains("SQL::")) Then query = MBOS.FileRead("!SQLStore\" & query.Replace("SQL::", "") & ".sql")
            If (query.ToLower().Contains("insert into") And query.ToLower().Contains("values")) Then
                qCase = QueryCase.INSERT
                query &= " SELECT @@IDENTITY"
            ElseIf (query.ToLower().Contains("update") And query.ToLower().Contains("set")) Then
                qCase = QueryCase.UPDATE
            ElseIf (query.ToLower().Contains("delete") And query.ToLower().Contains("from")) Then
                qCase = QueryCase.DELETE
            End If
        Catch
            Throw New Exception("SQL Query file", New Exception("SQL query file path is not exists."))
        End Try

        Dim mbosCommand As New SqlCommand(query, mbosConn)
        mbosCommand.CommandTimeout = _DB_TIMEOUT
        For Each para As Parameter In param
            If (para.Name.Contains("@")) Then
                If (para.Direction <> ParameterDirection.Input) Then
                    mbosCommand.Parameters.Add(para.Name, SqlDbType.VarChar, 8000)
                Else
                    Dim paramSize As Integer = 0
                    If (para.DefaultValue IsNot Nothing) Then paramSize = para.DefaultValue.Length
                    mbosCommand.Parameters.Add(para.Name, para.DbType, paramSize)
                    mbosCommand.Parameters.Item(para.Name).Size = paramSize
                    mbosCommand.Parameters.Item(para.Name).DbType = para.DbType
                    mbosCommand.Parameters.Item(para.Name).Value = DBNull.Value
                End If
                mbosCommand.Parameters.Item(para.Name).Direction = para.Direction
                Select Case qCase
                    Case QueryCase.SELECT
                        If para.DbType = DbType.Date Or para.DbType = DbType.DateTime Or para.DbType = DbType.DateTime2 Then
                            mbosCommand.Parameters.Item(para.Name).DbType = DbType.String
                            mbosCommand.Parameters.Item(para.Name).Value = MBOS.Convert(Of DateTime)(para.DefaultValue).ToString("yyyy-MM-dd HH:mm:ss")
                        ElseIf ((para.DbType = DbType.String And para.DefaultValue IsNot Nothing) Or Not String.IsNullOrEmpty(para.DefaultValue)) Then
                            mbosCommand.Parameters.Item(para.Name).Value = para.DefaultValue
                        End If
                    Case Else
                        Select Case para.DbType
                            Case DbType.String
                                If (para.DefaultValue IsNot Nothing) Then
                                    mbosCommand.Parameters.Item(para.Name).Value = para.DefaultValue
                                End If
                            Case DbType.Decimal
                                If (para.DefaultValue IsNot Nothing) Then
                                    mbosCommand.Parameters.Item(para.Name).Value = MBOS.Dec(para.DefaultValue)
                                End If
                            Case DbType.Date
                                mbosCommand.Parameters.Item(para.Name).Size = 10
                                mbosCommand.Parameters.Item(para.Name).DbType = DbType.String
                                If (Not String.IsNullOrEmpty(para.DefaultValue)) Then
                                    mbosCommand.Parameters.Item(para.Name).Value = MBOS.Convert(Of DateTime)(para.DefaultValue).ToString("yyyy-MM-dd")
                                End If
                                query = query.Replace(para.Name, "CONVERT(VARCHAR," & para.Name & ", 105)")
                            Case DbType.DateTime, DbType.DateTime2
                                mbosCommand.Parameters.Item(para.Name).Size = 18
                                mbosCommand.Parameters.Item(para.Name).DbType = DbType.String
                                If (Not String.IsNullOrEmpty(para.DefaultValue)) Then
                                    mbosCommand.Parameters.Item(para.Name).Value = MBOS.Convert(Of DateTime)(para.DefaultValue).ToString("yyyy-MM-dd HH:mm:ss")
                                End If
                                query = query.Replace(para.Name, "CONVERT(DATETIME," & para.Name & ", 120)")
                            Case Else
                                If (Not String.IsNullOrEmpty(para.DefaultValue)) Then
                                    mbosCommand.Parameters.Item(para.Name).Value = para.DefaultValue
                                End If
                        End Select
                End Select
            ElseIf (query.Contains(para.Name)) Then
                query = query.Replace("/*" & para.Name & "*/", " " & para.DefaultValue & " ")
            End If
        Next
        mbosCommand.CommandText = query
        Return mbosCommand
    End Function

End Class