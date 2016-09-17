Imports System.Web.SessionState
Imports System.Web
Imports Travox.Systems
Imports Travox.Systems.Security

Public Class User
    Public ID As String
    Public Code As String
    Public Name As String

    Public Shared ReadOnly Property Login() As User
        Get
            Dim _get As New User()
            _get.ID = Sessions.Cookie("ID")
            _get.Name = Sessions.Cookie("Name")
            _get.Code = Sessions.Cookie("Code")
            Return _get
        End Get
    End Property

    Public Shared Function DepartmentName() As String
        Dim param As New SQLCollection("@id", DbType.Int64, User.Login.ID)
        Return New DB().GetField("SELECT depart_name FROM department WHERE id = (SELECT TOP 1  department_id FROM staff WHERE id = @id)", param)
    End Function

    Public Shared Sub InitializeEngine(ByRef page As Object, ByVal com_code As String, ByVal user_id As String)
        User.InitializeEngine(page, com_code, user_id, "InitializeEngine")
    End Sub
    Public Shared Sub InitializeEngine(ByRef page As Object, ByVal com_code As String, ByVal user_code As String, ByVal pass_code As String)
        Dim session As HttpSessionState = HttpContext.Current.Session
        Dim database_name As String = DB.InitializeSiteCustomer(com_code, DB.Schema.MBOS)
        Dim base As New DB(database_name)

        Try
            Dim TravoxSession As String = MD5.Encrypt(user_code & ":" & pass_code & (Int((Rnd() * 655355) + 1))).ToString()
            Dim Crypt As New Sessions(page, True)
            Crypt.Write("BASE_ACCESS", database_name, 24)

            If pass_code = "InitializeEngine" Then
                For Each dRow As DataRow In base.GetTable("SELECT code, password FROM staff WHERE id = @id", New SQLCollection("@id", DbType.String, user_code)).Rows
                    user_code = dRow("code")
                    pass_code = dRow("password")
                    Exit For
                Next
            End If

            Dim param As New SQLCollection()
            param.Add("@comcode", DbType.String, com_code)
            param.Add("@usercode", DbType.String, user_code)
            param.Add("@pass", DbType.String, pass_code)
            param.Add("@created", DbType.DateTime, DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"))
            param.Add("@expired", DbType.DateTime, DateTime.Now.AddHours(1).ToString("dd-MM-yyyy HH:mm:ss"))
            param.Add("@session", DbType.String, TravoxSession)
            param.Add("@ip", DbType.String, HttpContext.Current.Request.ServerVariables("remote_addr"))

            base.Execute("UPDATE sys_session.login SET alive='N' WHERE expired_date <= GETDATE()")

            Dim StaffSignIn As String = base.GetField("SELECT COUNT(*) FROM sys_session.login WHERE alive='Y' AND staff_id > 0", param)
            Dim StaffLimit As Integer = CompanyProfile.LimitStaff()

            If String.IsNullOrEmpty(base.GetField("SELECT id FROM /*[GLOBAL]*/site_customer WHERE code=@comcode", param)) Then
                Throw New Exception("Does your company have purchased products from us.")
            ElseIf String.IsNullOrEmpty(base.GetField("SELECT id FROM staff WHERE code=@usercode", param)) Then
                Throw New Exception("The username you entered is incorrect.")
            ElseIf String.IsNullOrEmpty(base.GetField("SELECT id FROM staff WHERE code=@usercode AND password=@pass", param)) Then
                Throw New Exception("The password you entered is incorrect.")
            ElseIf StaffLimit > 0 And MBOS.Int(StaffSignIn) > StaffLimit Then
                Throw New Exception(String.Format("User login is limit {0} account.", StaffLimit))
            Else
                param.Add("@name", DbType.String, base.GetField("SELECT name FROM staff WHERE code=@usercode AND password=@pass", param))
                param.Add("@staff", DbType.String, base.GetField("SELECT id FROM staff WHERE code=@usercode AND password=@pass", param))

                Dim unCrypt As New Sessions(page, False)
                unCrypt.Destroy()
                unCrypt.Write("SessionTravox", TravoxSession)
                unCrypt.Write("COMPANY_ACCESS", com_code, 24)
                unCrypt.Write("COMPANY_NAME", base.GetField("SELECT TOP 1 name FROM company_profile_header WHERE [default] = 'Y'"))
                unCrypt.Write("Name", param("@name").DefaultValue)

                Crypt.Write("ID", param("@staff").DefaultValue, 24)
                Crypt.Write("Code", user_code, 24)

                If Not String.IsNullOrEmpty(base.GetField("SELECT id FROM sys_session.login WHERE staff_id=@staff AND session_id=@session AND alive='Y'", param)) Then
                    base.Execute("UPDATE sys_session.login SET expired_date=@expired WHERE staff_id=@staff AND session_id=@session AND alive='Y'", param)
                Else
                    base.Execute("INSERT INTO sys_session.login(created_date, expired_date, staff_id, username, session_id, ip_address) VALUES(@created, @expired, @staff, @name, @session, @ip)", param)
                End If

                base.Apply()
            End If
        Catch ex As Exception
            base.Rollback()

        End Try
    End Sub

    Public Shared Function SessionExpired(ByVal page As Object) As Boolean
        Try
            Dim base As New DB()
            Dim param As New SQLCollection()
            param.Add("@code", DbType.String, MBOS.CompanyCode)
            param.Add("@user", DbType.String, User.Login.Code)
            param.Add("@expired", DbType.DateTime, DateTime.Today.AddHours(1).ToString("dd-MM-yyyy hh:mm:ss"))
            param.Add("@session", DbType.String, Sessions.Cookie("SessionTravox"))
            param.Add("@ip", DbType.String, HttpContext.Current.Request.ServerVariables("remote_addr"))

            If String.IsNullOrEmpty(base.GetField("SELECT code FROM sys_session.login WHERE username=@code AND session_id=@session AND ipv4_address=@ip", param)) Then
                Throw New Exception("Session timeout. You stopped using more than 1 hour, Please re-login.")
            Else
                base.Execute("UPDATE sys_session.login SET expired_date = @expired WHERE username=@user AND session_id = @session AND ipv4_address = @ip", param)
            End If

        Catch ex As Exception

        End Try
        Return False
    End Function

End Class
