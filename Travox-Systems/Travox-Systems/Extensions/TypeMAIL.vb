Imports System.IO
Imports System.Net.Mail
Imports System.Net.Mime
Imports System.Threading
Imports System.ComponentModel
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Collections.Specialized

Namespace Extensions
    Public Class TypeMAIL
        Private Mail As New MailMessage
        Private ContentMail As New StringBuilder
        Private emailExp As New Regex("[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}")
        Public ErrorMessage As String = ""
        Public From As String = ""

        Public Enum SendTo
            [FROM]
            [TO]
            [CC]
            [BCC]
        End Enum

        Public Sub New(ByVal email_from As String, Optional ByVal subject As String = Nothing, Optional ByVal ns_email As Boolean = False)
            Try
                If (Not String.IsNullOrEmpty(email_from) And Not String.IsNullOrEmpty(subject)) Then
                    If (Not emailExp.IsMatch(email_from)) Then Throw New Exception("Invalid email from.")
                    Mail.From = New MailAddress(email_from)
                    From = email_from
                    Mail.IsBodyHtml = True
                    Mail.Subject = "MBOS :: " & subject
                    Mail.SubjectEncoding = Global.System.Text.Encoding.UTF8
                End If
                If (ns_email) Then
                    For Each email As Match In emailExp.Matches(New DB().GetField("SELECT TOP 1 ns_email FROM company_profile"))
                        Mail.Bcc.Add(New MailAddress(email.Value))
                    Next
                End If
            Catch ex As Exception
                ErrorMessage = ex.Message()
            End Try
        End Sub

        Public Sub Subject(ByVal subject As String)
            Mail.Subject = "MBOS :: " & subject
        End Sub

        Public Sub Add(ByVal emails As String, Optional ByVal type As SendTo = SendTo.TO)
            If Not String.IsNullOrEmpty(emails) Then
                Try
                    For Each email As Match In emailExp.Matches(emails)
                        Select Case type
                            Case SendTo.FROM : Mail.From = New MailAddress(email.Value.Trim())
                            Case SendTo.TO : Mail.To.Add(New MailAddress(email.Value.Trim()))
                            Case SendTo.CC : Mail.CC.Add(New MailAddress(email.Value.Trim()))
                            Case SendTo.BCC : Mail.Bcc.Add(New MailAddress(email.Value.Trim()))
                        End Select
                    Next
                Catch ex As Exception
                    ErrorMessage = ex.Message()
                End Try
            End If
        End Sub

        Public Sub Attach(ByVal filename_fullpath As String)
            If (File.Exists(filename_fullpath)) Then
                Mail.Attachments.Add(New Attachment(filename_fullpath))
            Else
                ContentMail.Insert(0, "<p class=""error""><b>Not Found File :</b>" & Path.GetFileName(filename_fullpath) & "</p>")
            End If
        End Sub

        Public Sub Content(ByVal contents As String)
            Mail.IsBodyHtml = False
            ContentMail = New StringBuilder()
            ContentMail.Append(contents)
        End Sub

        Public Sub Body(ByVal contents As String)
            Mail.IsBodyHtml = True
            ContentMail.Append(contents)
        End Sub
        Public Sub BodyHTML(ByVal template_name As String)
            Mail.IsBodyHtml = True
            Dim filename As String = MBOS.WebPath & "Resources\TemplateMail\" & MBOS.CompanyCode & "\" & template_name & ".html"
            If (Not File.Exists(filename)) Then filename = filename.Replace("\" & MBOS.CompanyCode & "\", "\MOS\")
            If (File.Exists(filename)) Then
                filename = File.ReadAllText(filename).Replace("  ", " ")
                ContentMail.Append(filename)
            End If
        End Sub

        Public Sub DataSource(ByVal param As NameValueCollection)
            For Each item As String In param.AllKeys
                Dim KeyItem As String = String.Format("{{{0}}}", item)
                If ContentMail.ToString.Contains(KeyItem) Then
                    If String.IsNullOrEmpty(param(item)) Then param(item) = "&nbsp;"
                    ContentMail.Replace(KeyItem, param(item))
                End If
            Next
        End Sub
        Public Sub DataSource(ByVal data As DataTable)
            If (data.Rows.Count > 0) Then
                For Each row As DataRow In data.Rows
                    For Each column As DataColumn In data.Columns
                        If (ContentMail.ToString.Contains("{" & column.ColumnName & "}")) Then
                            If (String.IsNullOrEmpty(row(column).ToString())) Then row(column) = "&nbsp;"
                            ContentMail.Replace("{" & column.ColumnName & "}", row(column))
                        End If
                    Next
                    Exit For
                Next
            End If
        End Sub
        Public Sub DataSourceRow(ByVal data As DataTable)
            Dim BodyRow As New StringBuilder()
            Dim tag_begin As String = "{" & data.TableName & "}"
            Dim tag_end As String = "{/" & data.TableName & "}"
            If (ContentMail.ToString.Contains(tag_begin) And ContentMail.ToString.Contains(tag_end) And data.Rows.Count > 0) Then
                Dim index_begin As Integer = ContentMail.ToString.IndexOf(tag_begin)
                Dim index_end As Integer = ContentMail.ToString.IndexOf(tag_end)
                Dim rowHTML As String = ContentMail.ToString.Substring((index_begin), (index_end + tag_begin.Length) - index_begin + 1)
                ContentMail.Remove(index_begin, rowHTML.Length)
                rowHTML = rowHTML.Replace(tag_begin, "").Replace(tag_end, "")
                For Each row As DataRow In data.Rows
                    Dim dataRow As String = rowHTML.Clone()
                    For Each column As DataColumn In data.Columns
                        If (rowHTML.Contains("{" & column.ColumnName & "}")) Then
                            If (String.IsNullOrEmpty(row(column).ToString())) Then row(column) = "&nbsp;"
                            dataRow = dataRow.Replace("{" & column.ColumnName & "}", row(column)).Trim()
                        End If
                    Next
                    BodyRow.Append(dataRow)
                Next
                ContentMail.Insert(index_begin, BodyRow.ToString())
            End If
        End Sub

        Public Function Sending() As Boolean
            Dim isSuccess As Boolean = True
            Try
                If (ErrorMessage <> Nothing) Then Throw New Exception(ErrorMessage)
                If (Mail.To.Count = 0) Then Throw New Exception("Can not find the destination email.")

                Dim client As New SmtpClient()
                Mail.Body = IIf(Mail.IsBodyHtml, Me.HTMLPattern().ToString(), ContentMail.ToString())
                'If MBOS.CompanyCode = "CCT" Then
                'client.Credentials = New Net.NetworkCredential("info@travelconnecxion.com", "Chaamcct9!")
                'client.Host = "mail.travelconnecxion.com"
                'client.Port = 587
                'Else
                client.Host = New DB("travox_global").GetField("SELECT TOP 1 ip_smtp_server FROM initial")
                'End If
                client.Send(Mail)
            Catch ex As SmtpException
                ErrorMessage = "Send :: " & ex.Message()
                isSuccess = False
                'Extensions.GlobalTravox.LogErrorMessage(ErrorMessage & vbCrLf & ex.StackTrace() & vbCrLf & ex.Source())
            End Try
            Mail.Dispose()
            Return isSuccess
        End Function

        Public Function GetContent() As StringBuilder
            Return IIf(Mail.IsBodyHtml, Me.HTMLPattern(), ContentMail)
        End Function

        Private Function HTMLPattern() As StringBuilder
            Dim pattern As New StringBuilder()
            pattern.Append("<!DOCTYPE html>")
            pattern.Append("<html><head><title>MBOSEngine E-Mail</title>")
            pattern.Append("<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8""></head><body>")
            pattern.Append("<style type='text/css'>html,body{padding:0px;margin:0px;background:#FFF;}html,body,table{font-size:12px;font-family:Tahoma;}</style>")
            pattern.Append(ContentMail.ToString())
            pattern.Append("</body></html>")
            Return pattern
        End Function
    End Class
End Namespace
