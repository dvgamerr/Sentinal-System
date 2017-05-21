Imports System.Security.Cryptography
Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Text

Public Class MBOS
    Private Const KeyEncryption As String = "Mbos3S@E1e!M38vY"
    Private Const PassPhrase As String = "MBOS!8502#FAR33"
    Private Const SaltCryptography As String = "Travox.WebSecurity2013"

    Public Shared Function FileStore(ByVal pathname As String, ByVal data As String) As String
        Dim target As String = "D:\mid-backOffice_Backup\#Store_Data\" & pathname
        Dim pathfile As String = IO.Path.GetDirectoryName(target)
        If (IO.File.Exists(target)) Then IO.File.Delete(target)
        If (Not IO.Directory.Exists(pathfile)) Then IO.Directory.CreateDirectory(pathfile)
        File.WriteAllText(target, data)
        Return target
    End Function

    Public Shared Function Timestamp() As Long
        Return CLng(DateTime.UtcNow.Subtract(New DateTime(1970, 1, 1)).TotalSeconds)
    End Function
    Public Shared Function ParseDate(ByVal from_format As String, ByVal to_format As String, ByVal date_time As String) As String
        Dim result As String = ""
        Dim exact As DateTime
        If DateTime.TryParseExact(date_time, from_format, CultureInfo.CurrentCulture.DateTimeFormat, DateTimeStyles.NoCurrentDateDefault, exact) Then
            result = exact.ToString(to_format)
        Else
            If Date.TryParse(date_time, CultureInfo.CurrentCulture.DateTimeFormat, DateTimeStyles.NoCurrentDateDefault, exact) Then
                result = exact.ToString(to_format)
            Else
                Throw New Exception("Error ParseDate")
            End If
        End If
        Return result
    End Function

    Public Shared Function Null(ByVal value As Object) As Boolean
        If (value Is Nothing) Then
            Return True
        ElseIf String.IsNullOrEmpty(value) Then
            Return True
        Else
            Return False
        End If
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

    Public Shared Function Convert(Of T)(ByVal value As String) As T
        Dim type As T = Nothing
        Dim index = Nothing
        If (TypeOf type Is Boolean) Then
            If (String.IsNullOrEmpty(value) OrElse (value.ToLower = "n" Or value.ToLower = "false" Or value.ToLower = "no" Or value.ToLower = "0" Or value = "N/A" Or value.ToLower = "inactive")) Then
                index = False
            ElseIf (value.ToLower = "y" Or value.ToLower = "true" Or value.ToLower = "yes" Or value.ToLower = "1" Or value.ToLower = "active") Then
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
        End If
        Return index
    End Function


End Class