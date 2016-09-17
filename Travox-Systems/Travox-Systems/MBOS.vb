Imports System.IO
Imports System.Web
Imports System.Text.RegularExpressions

Public Class MBOS

    Public Shared ReadOnly Property Debug() As Boolean
        Get
            Dim _debug As Boolean = False
            If (HttpContext.Current.Request.Url.Host.Contains("localhost")) Then _debug = True
            Return _debug
        End Get
    End Property

    Private Shared _CompanyCode As String
    Public Shared Property CompanyCode() As String
        Get
            Dim code As String = _CompanyCode
            If String.IsNullOrEmpty(_CompanyCode) Then
                If Not String.IsNullOrEmpty(Response.Cookie("COMPANY_ACCESS")) Then
                    code = Response.Cookie("COMPANY_ACCESS")
                Else
                    Throw New Exception("Session Denied", New Exception("You stop using the system for too long. Please re-login."))
                End If
            End If
            Return code.ToUpper()
        End Get
        Set(ByVal value As String)
            _CompanyCode = value
        End Set
    End Property

    Private Shared _CompanyBase As String
    Public Shared Property CompanyBase() As String
        Get
            Dim base As String = _CompanyBase
            If String.IsNullOrEmpty(_CompanyBase) Then
                If Not String.IsNullOrEmpty(Response.Cookie("BASE_ACCESS")) Then
                    base = Response.Cookie("BASE_ACCESS")
                Else
                    Throw New Exception("Session Denied", New Exception("You stop using the system for too long. Please re-login."))
                End If
            End If
            Return base.ToLower()
        End Get
        Set(ByVal value As String)
            _CompanyBase = value
        End Set
    End Property

    Public Shared ReadOnly Property WebPath() As String
        Get
            Return HttpContext.Current.Server.MapPath("~\")
        End Get
    End Property


    Public Shared Function FileRead(ByVal name_withextension As String) As String
        Dim path As String = MBOS.WebPath & name_withextension
        If (IO.File.Exists(path)) Then
            path = File.ReadAllText(path)
        ElseIf (IO.File.Exists(name_withextension)) Then
            path = File.ReadAllText(name_withextension)
        Else : path = Nothing
        End If
        Return path
    End Function

    Public Shared Function Timestamp() As Long
        Return CLng(DateTime.UtcNow.Subtract(New DateTime(1970, 1, 1)).TotalSeconds)
    End Function
    Public Shared Function ParseDate(ByVal from_format As String, ByVal to_format As String, ByVal date_time As String) As String
        Dim result As String = ""
        Dim exact As DateTime = DateTime.ParseExact(date_time, from_format, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat)
        result = exact.ToString(to_format)
        Return result
    End Function

    Public Shared Function Int(ByVal value As String) As Integer
        Return MBOS.Convert(Of Integer)(value)
    End Function
    Public Shared Function Dec(ByVal value As String) As Decimal
        Return MBOS.Convert(Of Decimal)(value)
    End Function
    Public Shared Function DT(ByVal value As String) As DateTime
        Return MBOS.Convert(Of DateTime)(value)
    End Function
    Public Shared Function Bool(ByVal value As String) As Boolean
        Return MBOS.Convert(Of Boolean)(value)
    End Function
    Public Shared Function ToYN(ByVal value As String) As String
        Return IIf(MBOS.Convert(Of Boolean)(value), "Y", "N")
    End Function
    Public Shared Function Null(ByVal obj As Object) As Boolean
        Return String.IsNullOrEmpty(obj) Or IsDBNull(obj)
    End Function

    Public Shared Function Convert(Of T)(ByVal value As String) As T
        Dim type As T = Nothing
        Dim index = Nothing
        If (TypeOf type Is Boolean) Then
            If (String.IsNullOrEmpty(value) OrElse (value.ToLower = "n" Or value.ToLower = "false" Or value.ToLower = "no" Or value.ToLower = "0")) Then
                index = False
            ElseIf (value.ToLower = "y" Or value.ToLower = "true" Or value.ToLower = "yes" Or value.ToLower = "1") Then
                index = True
            End If
        ElseIf (TypeOf type Is Decimal) Then
            If (String.IsNullOrEmpty(value) Or Not IsNumeric(value)) Then value = Decimal.Parse("0")
            index = Decimal.Parse(Regex.Replace(value, ",", ""))
        ElseIf (TypeOf type Is Double) Then
            If (String.IsNullOrEmpty(value) Or Not IsNumeric(value)) Then value = Double.Parse("0")
            index = Double.Parse(Regex.Replace(value, ",", ""))
        ElseIf (TypeOf type Is Integer OrElse TypeOf type Is Int32 OrElse TypeOf type Is Int64) Then
            If (String.IsNullOrEmpty(value) Or Not IsNumeric(value)) Then value = Integer.Parse("0")
            If (value.Contains(".")) Then value = Math.Round(Decimal.Parse(value), 0).ToString()
            index = Integer.Parse(value)
        ElseIf (TypeOf type Is DateTime) Or (TypeOf type Is Date) Then
            Try
                Dim FormatDateTime As String = "dd-MM-yyyy HH:mm:ss"
                If (FormatDateTime.Length <> value.Trim.Length) Then
                    Select Case value.Trim.Length
                        Case 10 : value = value.Trim & " 00:00:00"
                        Case 16 : value = value.Trim & ":00"
                    End Select
                End If
                index = DateTime.ParseExact(value, FormatDateTime, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat)
            Catch
                Throw New Exception("Datetime not format ""DD-MM-YYYY"" OR ""DD-MM-YYYY HH:MM:SS"".")
            End Try
        ElseIf (TypeOf type Is Money.VATType) Then
            If (value.ToUpper = "INCLUDE" Or value.ToUpper = "INC") Then
                index = Money.VATType.INCLUDE
            ElseIf (value.ToUpper = "EXCLUDE" Or value.ToUpper = "EXC") Then
                index = Money.VATType.EXCLUDE
            End If
        End If
        Return index
    End Function


End Class