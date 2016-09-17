Imports System.Web
Imports System.Collections.Specialized
Imports Travox.Systems
Imports Travox.Systems.Security

Public Class Sessions
    Private ReadCookie As HttpRequest
    Private WriteCookie As HttpResponse
    Private EncodeCookie As Boolean

    Public Sub New(ByVal current As Object)
        Me.New(current, True)
    End Sub
    Public Sub New(ByVal current As Object, ByVal encode As Boolean)
        ReadCookie = current.Page.Request
        WriteCookie = current.Page.Response
        EncodeCookie = encode
    End Sub
    Public Sub Write(ByVal name As String, ByVal value As String, Optional hour As Decimal = 0)
        Try
            If (Not MBOS.Debug And EncodeCookie) Then
                name = MD5.Encrypt(name).ToUpper()
                value = SHA512.Encrypt(value)
            End If
            Dim isCookie = New HttpCookie(name)
            isCookie.Value = value
            If (hour > 0) Then isCookie.Expires = DateTime.Now.AddMinutes(hour * 60)
            WriteCookie.Cookies.Add(isCookie)
        Catch ex As Exception
            Throw New Exception("Cookie is not allow.")
        End Try
    End Sub
    Public Function Read(ByVal name As String) As String
        If (Not MBOS.Debug And EncodeCookie) Then name = MD5.Encrypt(name).ToUpper()
        Dim result As String = "|" & String.Join("|", ReadCookie.Cookies.AllKeys) & "|"
        If result.Contains("|" & name & "|") Then
            If (Not MBOS.Debug And EncodeCookie) Then
                result = SHA512.Decrypt(ReadCookie.Cookies.Item(name).Value)
            Else
                result = ReadCookie.Cookies.Item(name).Value
            End If
        Else
            result = "N/A"
        End If
        Return IIf(result = "N/A", Nothing, result)
    End Function
    Public Function Destroy(ByVal name As String) As Boolean
        Dim iFound As Boolean = True
        If (Not MBOS.Debug And EncodeCookie) Then name = MD5.Encrypt(name).ToUpper()
        If WriteCookie.Cookies.Item(name) Is Nothing Then
            iFound = False
        Else
            WriteCookie.Cookies(name).Expires = DateTime.Now.AddDays(-1)
            WriteCookie.Cookies.Remove(name)
        End If
        Return iFound
    End Function
    Public Sub Destroy()
        For Each name As String In ReadCookie.Cookies.AllKeys
            If (name <> "ASP.NET_SessionId") Then WriteCookie.Cookies(name).Expires = DateTime.Now.AddDays(-1)
        Next
    End Sub
    Public Function Contains(ByVal name As String) As Boolean
        Dim found As Boolean = False
        Dim ReadCookies As HttpCookieCollection = HttpContext.Current.Request.Cookies()
        Dim result As String = "|" & String.Join("|", ReadCookies.AllKeys) & "|"
        If result.Contains("|" & MD5.Encrypt(name).ToUpper() & "|") Or result.Contains("|" & name & "|") Then found = True
        Return found
    End Function

    Public Shared Function Cookie(ByVal name As String) As String
        Dim ReadCookies As HttpCookieCollection = HttpContext.Current.Request.Cookies()
        Dim result As String = "|" & String.Join("|", ReadCookies.AllKeys) & "|"
        Dim md5Encrypt As String = MD5.Encrypt(name).ToUpper()
        If result.Contains("|" & md5Encrypt & "|") Then
            result = SHA512.Decrypt(ReadCookies.Item(md5Encrypt).Value)
        ElseIf result.Contains("|" & name & "|") Then
            result = ReadCookies.Item(name).Value
        Else
            result = "N/A"
        End If
        Return IIf(result = "N/A", Nothing, result)
    End Function

    Public Shared Function Session(ByVal name As String) As String
        Dim ReadSessions As NameObjectCollectionBase.KeysCollection = HttpContext.Current.Session.Keys
        Dim result As String = "N/A"
        For Each key As String In ReadSessions
            If (key = name) Then
                result = HttpContext.Current.Session(key)
                Exit For
            End If
        Next
        Return IIf(result = "N/A", Nothing, result)
    End Function


End Class

