Imports System
Imports System.Text
Imports System.Net
Imports System.Web
Imports System.IO
Imports System.Xml
Imports Travox.Systems.Extensions
Imports System.Text.RegularExpressions

Namespace Extensions
    Public Class TypeSMS
        Const SMSDelivery As String = "https:'sms-delivery.com/api.php?userid={0}&password={1}&sender={2}&recipient={3}&message={4}"
        Private Username As String
        Private Password As String
        Private _error As Byte = 0

        Public Sub New()
            Dim database As New DB()
            Dim dtInital As New DataTable '= database.GetTable(My.Resources.SELECT_InitialSMS)
            If dtInital.Rows.Count > 0 Then
                Username = dtInital.Rows(0)("sms_user_id").ToString
                Password = dtInital.Rows(0)("sms_password").ToString
            Else
                Username = ""
                Password = ""
            End If
        End Sub

        Public Shared Function Convert(ByVal number As String) As String
            Return Regex.Split(number, ",")(0).Trim().Replace("-", "")
        End Function

        Public Shared Function Convert(ByVal country As String, ByVal number As String) As String
            number = Convert(number)
            If (number.Substring(0, 1) = "0") Then number = country & number.Substring(1)
            Return number
        End Function

        Public Function Send(ByVal number As String, ByVal message As String) As Boolean
            Dim result As Boolean = False
            Try
                ' , Username, Password, CompanyProfile.SMSSubject, number, message
                'Dim _reqyest As CallbackObject = SMSSender(Username, Password, number, "NS.CO.TH", message)
                ' (results.IndexOf("yes") <> -1) Then result = True Else _error = Byte.Parse(results)
            Catch
                _error = 20
            End Try
            Return result
        End Function

        ''' <summary>
        ''' Global FUNCTION CREATED By Jack (Code Error can Rewrite)
        ''' </summary>
        ''' <param name="_User">nippon</param>
        ''' <param name="_Pass">sysits</param>
        ''' <param name="_PhoneNumber">08xxxxxxxx, 09yyyyyyyyyy</param>
        ''' <param name="_Sender">COMPANY CODE</param>
        ''' <param name="_Msg">MESSAGE TEXT</param>
        ''' <returns>String IndexOf (Success) Or (Error)</returns>
        ''' <remarks></remarks>
        Function SMSSender(ByVal _User As String, ByVal _Pass As String, ByVal _PhoneNumber As String, ByVal _Sender As String, ByVal _Msg As String) As String

            'Dim _Force As String = "premium"
            'Dim _Result As String = "Success"
            'Dim _Fail As Integer = 0
            'Dim _RemarkError As String = ""

            'Dim SMSParam As CallbackObject
            'SMSParam.param = True
            'SMSParam.exMessage = ""
            'Try

            '    If _User.Length = 0 Or _Pass.Length = 0 Or _PhoneNumber.Length = 0 Or _Msg.Length = 0 Then
            '        SMSParam.isSuccess = False
            '        SMSParam.exMessage = "Error: Invalid detail.(username,password,phonenumber,message)"
            '        _Result = "Error: Invalid detail.(username,password,phonenumber,message)"
            '    Else

            '        Dim Scheduled As String = ""
            '        System.Net.ServicePointManager.Expect100Continue = False
            '        Dim request As WebRequest = WebRequest.Create("http:'www.thaibulksms.com/sms_api.php")
            '        request.Method = "POST"
            '        Dim postData As String = "username=" & System.Uri.EscapeUriString(_User) & _
            '                "&password=" & System.Uri.EscapeUriString(_Pass) & _
            '                "&msisdn=" & System.Uri.EscapeUriString(_PhoneNumber) & _
            '                "&message=" & System.Uri.EscapeUriString(_Msg) & _
            '                "&sender=" & System.Uri.EscapeUriString(_Sender) & _
            '                "&ScheduledDelivery=" & System.Uri.EscapeUriString(Scheduled.ToString()) & _
            '                "&force=" & System.Uri.EscapeUriString(_Force)
            '        Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
            '        request.ContentType = "application/x-www-form-urlencoded"
            '        request.ContentLength = byteArray.Length
            '        Dim dataStream As Stream = request.GetRequestStream()
            '        dataStream.Write(byteArray, 0, byteArray.Length)
            '        dataStream.Close()
            '        Dim response As WebResponse = request.GetResponse()
            '        Console.WriteLine(CType(response, HttpWebResponse).StatusDescription)
            '        dataStream = response.GetResponseStream()
            '        Dim reader As New StreamReader(dataStream)
            '        Dim responseFromServer As String = reader.ReadToEnd()
            '        Console.WriteLine(responseFromServer)
            '        reader.Close()
            '        dataStream.Close()
            '        response.Close()

            '        Dim returnData As String = responseFromServer
            '        Dim xml As New XmlDocument()
            '        xml.LoadXml(returnData)
            '        Dim xnList = xml.SelectNodes("/SMS")
            '        Dim count_node As Integer = xnList.Count
            '        If count_node > 0 Then
            '            For Each xn As XmlNode In xnList
            '                Dim xnSubList = xml.SelectNodes("/SMS/QUEUE")
            '                Dim countSubNode As Integer = xnSubList.Count
            '                If countSubNode > 0 Then
            '                    For Each xnSub As XmlNode In xnSubList
            '                        If xnSub("Status").InnerText.ToString() = "1" Then
            '                            Dim msisdnReturn As String = xnSub("Msisdn").InnerText
            '                            Dim useCredit As String = xnSub("UsedCredit").InnerText
            '                            Dim creditRemain As String = xnSub("RemainCredit").InnerText
            '                        Else
            '                            Dim sub_status_detail As String = xnSub("Detail").InnerText
            '                            _Fail += 1
            '                            _RemarkError += _Fail & "." & sub_status_detail & "|"
            '                        End If
            '                    Next
            '                Else
            '                    Dim status_detail As String = ""
            '                    If xn("Status").InnerText = "0" Then
            '                        status_detail = xn("Detail").InnerText
            '                    Else
            '                        status_detail = "Can not read data(Not XML Format)"
            '                    End If
            '                    SMSParam.isSuccess = False
            '                    SMSParam.exMessage = "Error: " & status_detail
            '                    _Result = "Error: " & status_detail
            '                End If
            '            Next
            '        Else
            '            SMSParam.isSuccess = False
            '            SMSParam.exMessage = "Error: Can not read data(Not XML Format)"
            '            _Result = "Error: Can not read data(Not XML Format)"
            '        End If

            '    End If

            '    If _RemarkError <> "" Then
            '        _Result += ": " & _RemarkError
            '        SMSParam.exMessage = ": " & _RemarkError
            '    End If

            '    Return SMSParam

            'Catch ex As Exception
            '    SMSParam.isSuccess = False
            '    SMSParam.exMessage = "Error: Coding error."
            '    Return SMSParam
            'End Try
            Return "" 'SMSParam
        End Function

        Public Function ErrorMessage() As String
            Dim msg As String = ""
            Select Case _error
                Case 20 : msg = "Exception Function TypeSMS.Send"
                Case 21 : msg = "SMS Sending Failed"
                Case 22 : msg = "Invalid username/password combination"
                Case 23 : msg = "Credit exhausted"
                Case 24 : msg = "Gateway unavailable"
                Case 25 : msg = "Invalid schedule date format"
                Case 26 : msg = "Unable to schedule"
                Case 27 : msg = "Username is empty"
                Case 28 : msg = "Password is empty"
                Case 29 : msg = "Recipient is empty"
                Case 30 : msg = "Message is empty"
                Case 31 : msg = "Sender is empty"
                Case 32 : msg = "One or more required fields are empty"
                Case Else : msg = "Unknow Error CODE : " & _error
            End Select
            Return msg
        End Function
    End Class
End Namespace
