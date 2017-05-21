Imports System.Collections.Specialized
Imports System.Text
Imports System.Text.RegularExpressions

Namespace DataCollection
    Public Class RequestBuilder
        Const UserAgent As [String] = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/44.0.2403.125 Safari/537.36"

        Private ContentLength As Int32 = 0
        Private GetVariable As NameValueCollection
        Private PostVariable As NameValueCollection
        Public RawBody As StringBuilder
        Public Cookie As CookieBuilder

        Public Headers As NameValueCollection

        Public Method As By = By.[GET]
        Public Referer As [String]
        ' http:'forum.tirkx.com/main/index.php
        Public Origin As [String]
        ' http:'forum.tirkx.com
        Public Connection As [String] = "keep-alive"
        Public CacheControl As [String] = "max-age=0"
        Public ContentType As [String]
        Public Accept As [String]

        Public uri As Uri
        Public Enum By
            [POST]
            [GET]
        End Enum

        Public Sub New(ByVal URL As String, Optional ByVal HTTPS As Boolean = False)
            Cookie = New CookieBuilder()
            RawBody = New StringBuilder()
            Headers = New NameValueCollection()
            GetVariable = New NameValueCollection()
            PostVariable = New NameValueCollection()
            uri = New Uri(URL)
        End Sub

        Public Sub [GET](ByVal name As String, ByVal value As String)
            GetVariable.Add(name, value)
        End Sub

        Public Sub AddBody(ByVal name As String, ByVal value As String)
            PostVariable.Add(name, value)
        End Sub

        Public Sub AppendRaw(ByVal raw As String)
            RawBody.Append(raw)
        End Sub

        Public Sub Clear()
            PostVariable.Clear()
            GetVariable.Clear()
        End Sub

        Public Sub SetCookie(ByVal name As String, ByVal value As String)
            If String.Join("=", Cookie.AllKeys).Contains(name) Then Cookie(name) = value Else Cookie.Add(name, value)
        End Sub

        Public Overrides Function ToString() As String
            Dim _post As [String] = POSTString()
            ContentLength = _post.Length
            Dim _sb As New StringBuilder()
            _sb.AppendLine(Method.ToString() + " " + uri.PathAndQuery + " HTTP/1.1")
            _sb.AppendLine("Host: " & uri.Host & IIf(uri.Port = 80 Or uri.Port = 443, "", ":" & uri.Port))
            _sb.AppendLine("Cache-Control: " + CacheControl)

            If Not [String].IsNullOrEmpty(ContentType) Then
                _sb.AppendLine("Content-Type: " + ContentType)
            ElseIf RawBody.Length > 0 Then

            ElseIf PostVariable.Count > 0 Then
                _sb.AppendLine("Content-Type: application/x-www-form-urlencoded; charset=UTF-8")
                _sb.AppendLine("X-Requested-With: XMLHttpRequest")
            End If

            For Each key As String In Headers.AllKeys
                _sb.AppendFormat("{0}: {1}", key, Headers(key))
                _sb.AppendLine()
            Next

            If (Headers.Count = 0) Then
                If Not [String].IsNullOrEmpty(UserAgent) Then _sb.AppendLine("User-Agent: " + UserAgent)
                If Not [String].IsNullOrEmpty(Referer) Then _sb.AppendLine("Referer: " + Referer)
                If Not [String].IsNullOrEmpty(Origin) Then _sb.AppendLine("Origin: " + Origin)
            End If

            If ContentLength > 0 Then _sb.AppendLine("Content-Length: " + ContentLength.ToString())
            If Cookie.Count > 0 Then _sb.AppendLine("Cookie: " + CookieString())


            _sb.AppendLine("Connection: " + Connection)
            If PostVariable.Count > 0 Then
                _sb.AppendLine("")
                _sb.Append(_post)
                _sb.AppendLine("")
            ElseIf (RawBody.Length > 0) Then
                _sb.AppendLine("Content-Length: " + RawBody.Length.ToString())
                _sb.AppendLine("")
                _sb.Append(RawBody)
                _sb.AppendLine("")
            End If
            Return _sb.ToString()
        End Function

        Public Function POSTString() As [String]
            Dim _s As [String] = ""

            If PostVariable.Count > 0 Then
                For Each key As String In PostVariable.AllKeys
                    _s += key & "=" & Uri.EscapeUriString(PostVariable(key)) & "&"
                Next
                If _s.Length > 0 Then _s = _s.Substring(0, _s.Length - 1)
            ElseIf (RawBody.Length > 0) Then
                _s = RawBody.ToString()
            End If
            Return _s
        End Function

        Public Function CookieString() As [String]
            Dim _s As [String] = ""
            For Each key As String In Cookie.AllKeys
                _s += key & "=" & Cookie(key) & "; "
            Next
            If _s.Length > 0 Then _s = _s.Substring(0, _s.Length - 2)
            Return _s
        End Function

    End Class
End Namespace
