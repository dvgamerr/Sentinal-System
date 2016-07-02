using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Travox.Sentinel.Engine;
using Travox.Systems;
using Travox.Systems.DataCollection;
using Travox.Systems.Extensions;

namespace Travox.Sentinel.Crawler
{
    public class Secretary : Controller
    {

        public Secretary()
        {
            //base.DBName = "nissin_mos";
            //base.OnceTime = true;
            base.SetTimeout = new TimeSpan(23, 50, 00);
            //base.SetIntervel = new TimeSpan(0, 0, 5);
        }

        public override void Start()
        {
            base.Start();
        }
        
        public override void Update()
        {
            base.Update();
            const String CrawlerPath = @"D:\ip1\travox.com\www\crawler\documents\";
            const String ViewerPath = @"C:\ip1\travox.com\www\mos\mos_demo\report_viewer\";

            SecretaryEvent Period = SecretaryEvent.Unknow;

            SQLCollection param = new SQLCollection("@id", DbType.Int32, base.State.CompanyID);
            List<HandlerItem> EventEmail = new List<HandlerItem>();

            foreach (DataRow Row in new DB("travox_system").GetTable(base.GetResource("secretary_report.sql"), param).Rows)
            {
                ParameterDate SystemDate;

                switch(Row["period"].ToString())
                {
                    case "Daily": Period = SecretaryEvent.Daily; break;
                    case "Weekly": Period = SecretaryEvent.Weekly; break;
                    case "Monthly": Period = SecretaryEvent.Monthly; break;
                }

                SystemDate.From = DateTime.Now.Date;
                SystemDate.To = DateTime.Now.Date;
                DateTime DateEndMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
                String PeriodDate = "";
                String OutputEmailType = Row["output_email"].ToString().Trim();
                String OutputPrinter = Row["output_printer"].ToString().Trim();

                if (Period == SecretaryEvent.Monthly && DateTime.Now.Date == DateEndMonth)
                {
                    PeriodDate = SystemDate.From.ToString("yyyy");
                    SystemDate.From  = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    SystemDate.To  = DateEndMonth;
                }
                else if (Period == SecretaryEvent.Weekly && DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                {
                    PeriodDate = SystemDate.From.ToString("MM-yyyy");
                    SystemDate.From = SystemDate.From.AddDays(-7);
                }
                else if (Period == SecretaryEvent.Daily)
                {
                    PeriodDate = SystemDate.From.ToString("dd-MM-yyyy");
                }
                else
                {
                    Period = SecretaryEvent.Unknow;
                }

                if ((!MBOS.Null(OutputEmailType) || MBOS.Bool(OutputPrinter)) && Period != SecretaryEvent.Unknow)
                {
                    RequestBuilder ReportViewer = new RequestBuilder((!App.DebugMode) ? "mos.travox.com/mos_v2/report_viewer/default.aspx" : "localhost:8026/Default.aspx");
                    ReportViewer.By = RequestBuilder.Method.POST;
                    ReportViewer.SetCookie("ASP.NET_SessionId", "xxxxxxxxxxxxxxxxxxxxxxxx");
                    ReportViewer.SetCookie("COMPANY_ACCESS", base.State.CompanyCode);
                    ReportViewer.SetCookie("ID", "-4");
                    ReportViewer.SetCookie("Code", "TX");
                    ReportViewer.SetCookie("Name", "Travox Sentinel");

                    //ReportViewer.POST("ItemType", JSON.Serialize<ItemType>(new ItemType { ExportType = OutputEmailType }));
                    //ReportViewer.POST("Report", JSON.Serialize<ItemReport>(new ItemReport { Name = Row["report_key"].ToString(), Filename = Row["report_key"].ToString() + ".rpt" }));
                    ReportViewer.POST("period_begin", SystemDate.From.ToString("dd-MM-yyyy"));
                    ReportViewer.POST("period_end", SystemDate.To.ToString("dd-MM-yyyy"));

                    param.Add("@s_id", DbType.Int32, Row["secretary_id"].ToString());
                    param.Add("@name", DbType.String, Row["report_name"].ToString());
                    param.Add("@email", DbType.String, Row["email"].ToString());
                    param.Add("@code", DbType.String, base.State.CompanyCode);
                    param.Add("@file", DbType.String, null);

                    String[] ArgsString = { Row["period"].ToString(), PeriodDate, Row["report_name"].ToString() };
                    HandlerItem Item = new HandlerItem();
                    Item.Subject = String.Format("Secretary({0} {1}) Report of \"{2}\"", ArgsString);
                    Item.OnEmail = !MBOS.Null(OutputEmailType);
                    Item.OnPrinter = MBOS.Bool(OutputPrinter);
                    Item.Mail = Row["email"].ToString();
                    Item.Ajax = ReportViewer;
                    EventEmail.Add(Item);
                }
            }

            foreach (HandlerItem Item in EventEmail)
            {
                //CallbackException data = JSON.Deserialize<CallbackException>(XHR.Connect(Item.Ajax));
                CallbackException data = new CallbackException();
                String getItems = Regex.Match(data.getItems, @"[\\|/]Temp[\\|/](?<path>.*)").Groups["path"].Value;
                String AttachFile = null;
                if (!data.onError)
                {
                    String FolderName = Path.GetDirectoryName(CrawlerPath + getItems) + "\\";
                    String FileName = Path.GetFileName(getItems);
                    if (!App.DebugMode)
                    {
                        if (Directory.Exists(FolderName)) Directory.CreateDirectory(FolderName);
                        File.Copy(ViewerPath + data.getItems, FolderName + FileName);
                        File.Delete(ViewerPath + data.getItems);
                        AttachFile = FolderName + FileName;
                    }
                    else
                    {
                        AttachFile = @"D:\Travox Mid-Back\TravoxViewer\TravoxViewer\Temp\" + getItems;
                    }
                }

                if (Item.OnEmail)
                {
                    DB db = new DB("travox_system");
                    NameValueCollection MailParam = new NameValueCollection();
                    TypeMAIL ReportSecretary = new TypeMAIL("info@ns.co.th", Item.Subject, false);
                    ReportSecretary.Add(Item.Mail);
                    ReportSecretary.Body(base.GetResource("MailTemplate.html"));

                    String SQL = db.Execute("INSERT INTO crawler.secretary_email (secretary_id, company_code, report_name, email) VALUES (@s_id, @code, @name, @email)", param);

                    MailParam.Add("message_email", data.exMessage);
                    MailParam.Add("print_date", DateTime.Now.Date.ToString("dd-MM-yyyy"));
                    if (!data.onError && data.exTitle != "ManualException")
                    {
                        ReportSecretary.Attach(AttachFile);
                        param["@file"].DefaultValue = getItems;
                    }
                    else
                    {
                        param["@file"].DefaultValue = data.exMessage;
                    }
                    param.Add("@e_id", DbType.Int32, SQL);
                    db.Execute("UPDATE crawler.secretary_email SET filename=@file WHERE email_id=@e_id", param);

                    ReportSecretary.DataSource(MailParam);
                    if (!ReportSecretary.Sending())
                    {
                        param["@file"].DefaultValue = ReportSecretary.ErrorMessage;
                        db.Execute("UPDATE crawler.secretary_email SET filename=@file WHERE email_id=@e_id", param);
                    }
                    db.Apply();
                }

                if (Item.OnPrinter)
                {

                }
            }
            param = null;
            EventEmail = null;
            Console.WriteLine("{0} Updated", base.State.DatabaseName);

        }

        public override void Stop()
        {
            base.Stop();
        }

        private enum SecretaryEvent
        {
            Unknow,   
            Daily,
            Weekly,
            Monthly
        }

        public struct ItemType
        {
            public String ExportType;
        }

        public struct ItemReport
        {
            public String Name;
            public String Filename;
        }
        private struct ParameterDate
        {
            public DateTime From;
            public DateTime To;
        }

        public struct HandlerItem
        {
            public Boolean OnEmail;
            public Boolean OnPrinter;
            public RequestBuilder Ajax;
            public String Subject;
            public String AttachFile;
            public String Mail;
        }

    //    Public Class CommandStore
    //Public Shared Function Select_Customer_List() As String
    //    Dim sql As String = "SELECT DISTINCT s.database_name,s.id FROM travox_global.travoxmos.secretary sec "
    //    sql += "LEFT JOIN travox_global.travoxmos.site_customer s ON sec.site_customer_id = s.id"
    //    Return sql
    //End Function

    //Public Shared Function Print_Secretary_Log() As String
    //    Dim strConn As String = "select l.system_date, "
    //    strConn += "convert(varchar(10),l.system_date,105) +' ' + CONVERT(char(5),(l.system_date),108) as date_time "
    //    strConn += " ,s.code as cmpcode,l.report_name,upper(sec.period) as period, l.output_destination "
    //    strConn += " from travox_global.travoxmos.secretary_print_log  l "
    //    strConn += " left join travox_global.travoxmos.secretary sec on (l.secretary_id = sec.id) "
    //    strConn += " left join travox_global.travoxmos.site_customer s on (sec.site_customer_id = s.id) ORDER BY l.system_date DESC"
    //    Return strConn
    //End Function

    //Public Shared Function Select_Secretary_List(ByVal id As String) As String
    //    Return "SELECT * FROM travox_global.travoxmos.secretary WHERE status = 'ACTIVE' AND site_customer_id ='" & id & "' "
    //End Function

    //Public Shared Function Company_Profile() As String
    //    Dim sql As String = "select top 1 upper(name) name, isnull(address,' - ') + char(13)"
    //    sql += " + 'Tel : ' + isnull(tel,' - ') + '  Fax : '+ isnull(fax,' - ') + char(13)"
    //    sql += " +  'Website : ' + isnull(website,' - ') + ', e-mail : ' + isnull(email,' - ') address"
    //    sql += ", ''  tel, ''  website from company_profile"
    //    Return sql
    //End Function

    //Public Shared Function Select_Staff_Name(ByVal id As String) As String
    //    Return "SELECT code, code + ' - ' + name name FROM staff where id='" & id & "' "
    //End Function

    //Public Shared Function Select_Secretary_Parameter(ByVal id As String) As String
    //    Return "SELECT * FROM travox_global.travoxmos.secretary_parameter WHERE secretary_id='" & id & "' "
    //End Function

    //Public Shared Function Insert_Secretary_PrintLog(ByVal data As EMailData, ByVal type As String) As String
    //    Dim strCon As String = "insert into travox_global.travoxmos.secretary_print_log "
    //    strCon += "(system_date, secretary_id, report_name, output_type, output_destination) "
    //    strCon += "values (GETDATE()," & data.SecretaryID & ",'" & data.ReportName & "','" & type & "','" & data.EmailTo & "')"
    //    Return strCon
    //End Function

    //Public Shared Function Select_Secretary_PrintLog(ByVal data As EMailData) As String
    //    Dim strConn As String = "select convert(varchar(10),l.system_date,105) +' ' + CONVERT(char(5),(l.system_date),108) as date_time "
    //    strConn += " ,s.code as cmpcode,l.report_name,upper(sec.period) as period, l.output_destination "
    //    strConn += " from travox_global.travoxmos.secretary_print_log  l "
    //    strConn += " left join travox_global.travoxmos.secretary sec on (l.secretary_id = sec.id) "
    //    strConn += " left join travox_global.travoxmos.site_customer s on (sec.site_customer_id = s.id) "
    //    Return strConn
    //End Function








    //_StateTextAlert = "Starting..."
// Dim dtSecretary As DataTable = Query.SQL(drCustomer("database_name"), CommandStore.Select_Secretary_List(drCustomer("id")))
    //                Dim rptLoop As Integer = 0
    //                _StateMaxinium += (dtSecretary.Rows.Count() * 2)
    //                _ThreadReport.ReportProgress(rptLoop)

    //                For Each drSecretary As DataRow In dtSecretary.Rows
    //                    Dim genDataReport As New ReportData
    //                    If (drSecretary("output_email").ToString() = "Y") Then genDataReport.SendEmail = True
    //                    If (drSecretary("output_printer").ToString() = "Y") Then genDataReport.SendPrinter = True
    //                    Dim SendDayly As Boolean = drSecretary("period") = "Daily"
    //                    Dim SendWeekly As Boolean = Date.Now.DayOfWeek = 0 And drSecretary("period") = "Weekly"
    //                    Dim SendMonthly As Boolean = DateCurrent = DateEndMonth And drSecretary("period") = "Monthly"

    //                    If ((SendDayly Or SendWeekly Or SendMonthly) And (genDataReport.SendEmail Or genDataReport.SendPrinter)) Then
    //                        _StateTextAlert = "Data Loading..."
    //                        _ThreadReport.ReportProgress(rptLoop)

    //                        Dim StartDate As DateTime = DateTime.Now.AddMonths(-1).AddDays(-(DateTime.Now.Day - 1))
    //                        Dim EndDate As DateTime = StartDate.AddDays(DateTime.DaysInMonth(StartDate.Year, StartDate.Month) - 1)
    //                        Dim dayEnd As String = EndDate.ToString("dd")
    //                        Dim dayCurrent As String = DateTime.Now.ToString("dd")

    //                         Check Day for Send Email
    //                        1.	Daily : ตั้งเวลาส่งทุกวัน เวลา 23:50 นาที
    //                        2.	Weekly : ตั้งเวลาส่งทุกวันอาทิตย์ เวลา 23:50 นาที
    //                        3.	Monthly : ตั้งเวลาส่งทุกวันสิ้นเดือน เวลา 23:50 นาที

    //                        genDataReport.DBName = drCustomer("database_name").ToString()
    //                        genDataReport.LoginID = drSecretary("system_staff").ToString.Trim()
    //                        genDataReport.Param.Period = drSecretary("period").ToString()
    //                        genDataReport.Param.FileType = "PDF"
    //                        genDataReport.Param.FileName = drSecretary("report_name").ToString.Trim()

    //                        If drSecretary("period") = "Daily" Then
    //                            genDataReport.Param.DateFrom = DateCurrent
    //                            genDataReport.Param.DateTo = DateCurrent
    //                        ElseIf drSecretary("period") = "Weekly" Then
    //                            genDataReport.Param.DateFrom = DateBeginWeekly
    //                            genDataReport.Param.DateTo = DateCurrent
    //                        ElseIf drSecretary("period") = "Monthly" Then
    //                            genDataReport.Param.DateFrom = DateBeginMonth
    //                            genDataReport.Param.DateTo = DateEndMonth
    //                        End If

    //                         GET Parameter Fields
    //                        For Each drParameter As DataRow In Query.SQL(genDataReport.DBName, CommandStore.Select_Secretary_Parameter(drSecretary("id").ToString())).Rows
    //                            Select Case drParameter.Item("parameter_name").ToString().Trim()
    //                                Case "Currency"
    //                                    genDataReport.Param.Currency = drParameter.Item("parameter_value").ToString()
    //                            End Select
    //                        Next
    //                        rptLoop += 1

    //                         Gen Data To Send Email And Printer
    //                        _StateTextAlert = "Report Loading..."
    //                        _ThreadReport.ReportProgress(rptLoop)
    //                        Dim reportFilePath As String = Me.DataReport_Load(genDataReport)
    //                        If (reportFilePath <> NoReportList) Then
    //                            genDataReport.Mail.EmailFrom = Query.SQLOneField(genDataReport.DBName, "SELECT system_mail_from FROM travox_global.travoxmos.initial")
    //                            genDataReport.Mail.EmailTo = drSecretary("email").ToString()
    //                            genDataReport.Mail.EmailBcc = ""
    //                            genDataReport.Mail.EmailSubject = genDataReport.Param.Period & " Report of " & drSecretary("report_nickname").ToString()
    //                            genDataReport.Mail.EmailSubject += " Print on : " & DateTime.Now.ToString("dd-MM-yyyy HH:mm")
    //                            genDataReport.Mail.FilePath = reportFilePath
    //                            genDataReport.Mail.ReportName = drSecretary("report_name").ToString.Trim()
    //                            genDataReport.Mail.SecretaryID = drSecretary("id").ToString()

    //                            _DataMails.Add(genDataReport)
    //                        Else
    //                            Console.WriteLine(NoReportList)
    //                        End If
    //                        rptLoop += 1
    //                    End If
    //                Next

    //            _StateTextAlert = "Email Sending..."
    //            _ThreadReport.ReportProgress(_StateMaxinium)
    //            Dim log As New Logfile("EMAIL Sended")
    //            For Each data As ReportData In _DataMails
    //                If (data.SendEmail) Then
    //                    Thread.Sleep(1000)

    //                    log.LogMessage("ReportName: (" & data.Mail.SecretaryID & ") " & data.Mail.ReportName)
    //                    Dim email As New TypeMAIL(data.Mail.EmailFrom, data.Mail.EmailSubject)
    //                    email.Add(data.Mail.EmailTo)
    //                    If (String.IsNullOrEmpty(data.Param.FileName)) Then
    //                        log.LogMessage("File Not Found. in (" & data.Param.DateFrom & " to " & data.Param.DateTo & ")")
    //                        email.Body("File Not Found. in (" & data.Param.DateFrom & " to " & data.Param.DateTo & ")")
    //                    Else
    //                        log.LogMessage("File : " & IO.Path.GetFileName(data.Mail.FilePath))
    //                        email.Attach(data.Mail.FilePath)
    //                    End If

    //                    If (Not email.Sending()) Then
    //                        log.LogMessage("SENDING Fail (" & email.ErrorMessage & ")")
    //                    Else
    //                        log.LogMessage("SENDING Successful.")
    //                        Query.SQL(data.DBName, CommandStore.Insert_Secretary_PrintLog(data.Mail, "EMAIL"))
    //                    End If
    //                End If
    //                If (data.SendPrinter) Then
    //                    log.LogMessage("Printer: (" & data.Mail.SecretaryID & ") " & data.Mail.ReportName)
    //                    #####################
    //                     Printer Send Event
    //                    ####################
    //                    Query.SQL(data.DBName, CommandStore.Insert_Secretary_PrintLog(data.Mail, "PRINTER"))
    //                End If
    //            Next
    //            log.LogMessage("---")
    //            log.Write()
    //        Catch ex As Exception
    //            Dim log As New Logfile("Exception")
    //            log.LogMessage("Exception:: Thread_DoWork()")
    //            log.LogMessage(ex.Message)
    //            log.LogMessage(ex.Source)
    //            log.LogMessage(ex.StackTrace)
    //            log.Write()
    //            If _ThreadReport.WorkerSupportsCancellation = True Then _ThreadReport.CancelAsync()
    //        End Try
    }
}
