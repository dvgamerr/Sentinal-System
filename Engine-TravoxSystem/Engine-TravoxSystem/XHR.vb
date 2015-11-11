Imports Travox.Systems
Imports Travox.Systems.DataCollection
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions

Public Class XHR

    Public Status As XHRAsync
    Public IP As IPEndPoint
    Public Host As IPHostEntry
    Public Raw As New StringBuilder()

    Private Buffer As [Byte]()
    Private OnSocket As Socket
    Private ReceivedSpeed As Int32 = 0
    Private SpeedTransferred As Int32 = 0
    Private ReceivedLimit As Int32 = 0
    Private ReceivedLength As Int32 = -1
    Private DataLoaded As Boolean = False
    Private Chunked As Int32 = 0
    Private ElapsedTime As New Stopwatch()
    Private ReceivingTime As New Stopwatch()
    Public Cookie As New CookieBuilder()
    Public HTMLHeaders As New StringBuilder()
    Public HTMLBody As New StringBuilder()

    Public ReadOnly Property Transferred As String
        Get
            Return XHR.ToPackageString(IIf(SpeedTransferred = 0, ReceivedSpeed, SpeedTransferred))
        End Get
    End Property


    Public Enum XHRAsync
        Die
        Connected
        Sending
        Sended
        Waiting
        Receiving
        Received
    End Enum

    Public Sub New(Optional limit As Int32 = 0)
        Cookie = New CookieBuilder()
        ElapsedTime.Start()
        ReceivedLimit = IIf(limit = 0, XHR.Megabytes(10), limit)
        Status = XHRAsync.Die
    End Sub
    Protected Overrides Sub Finalize()
        Me.Close()
        Raw = Nothing
        ElapsedTime = Nothing
        ReceivingTime = Nothing
        Cookie = Nothing
        HTMLHeaders = Nothing
        HTMLBody = Nothing
    End Sub

    Private Function Open(server As String, port As Integer) As Socket
        Dim s As Socket = Nothing

        Host = Dns.GetHostEntry(server)
        For Each address As IPAddress In Host.AddressList
            address = IIf(address.ToString() = "::1", IPAddress.Parse("127.0.0.1"), address)
            IP = New IPEndPoint(address, port)

            Dim tempSocket As New Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
            tempSocket.Connect(IP)
            If tempSocket.Connected Then
                s = tempSocket
                Exit For
            End If
        Next
        Return s
    End Function
    Public Sub Close()
        IP = Nothing
        Host = Nothing
        Raw = Nothing
        HTMLHeaders = Nothing
    End Sub

    Public Sub AsyncSend(request As RequestBuilder)
        OnSocket = Me.Open(request.CurrentURL.Host, request.CurrentURL.Port)

        For Each item As String In Cookie.AllKeys
            request.Cookie.Add(item, Cookie(item))
        Next

        If OnSocket Is Nothing Then
            ElapsedTime.Stop()
            Console.WriteLine("{0} << Connected fail", Host.HostName)
        Else
            Console.WriteLine("{0} << Connected successful", Host.HostName)
            HTMLHeaders = New StringBuilder()
            HTMLBody = New StringBuilder()
            Me.Status = XHRAsync.Sending
            Buffer = Encoding.UTF8.GetBytes(request.ToString())

            Console.WriteLine("{0} << Sending ({1})", New String() {Host.HostName, XHR.ToPackageString(Buffer.Length)})
            OnSocket.BeginSend(Buffer, 0, Buffer.Length, SocketFlags.None, AddressOf Me.SendArgsBegin, Nothing)
            Me.WaitRequest()
        End If
    End Sub
    Private Sub AsyncReceive()
        If Me.Status = XHRAsync.Receiving Or Me.Status = XHRAsync.Sended Then
            Buffer = New [Byte](ReceivedLimit) {}
            OnSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, AddressOf Me.ReceiveArgsBegin, Nothing)
        End If
    End Sub

    Private Sub SendArgsBegin(e As IAsyncResult)
        If (OnSocket.EndSend(e) > 0) Then
            Console.WriteLine("{0} >> Sended ({1} ms)", New String() {Host.HostName, ElapsedTime.ElapsedMilliseconds})
            Me.Status = XHRAsync.Sended
            ReceivedSpeed = 0
            ElapsedTime.Reset()
            ElapsedTime.Start()
            ReceivingTime.Start()

            Me.AsyncReceive()
        Else
            Console.WriteLine("{0} >> Die ({1} ms)", New String() {Host.HostName, ElapsedTime.ElapsedMilliseconds})
            Me.Status = XHRAsync.Die
        End If

    End Sub
    Private Sub ReceiveArgsBegin(e As IAsyncResult)
        Dim BytesTransferred As Integer = OnSocket.EndReceive(e)

        If Me.Status = XHRAsync.Sended Then
            Console.WriteLine("{0} >> Wating ({1} ms)", New String() {Host.HostName, ElapsedTime.ElapsedMilliseconds})
            ElapsedTime.Reset()
            ElapsedTime.Start()

            Me.Status = XHRAsync.Receiving
        Else
            Console.WriteLine("{0} >> Receiving ({1})", New String() {Host.HostName, XHR.ToPackageString(BytesTransferred)})
        End If

        If (ReceivingTime.ElapsedMilliseconds / 1000 < 1.0) Then
            ReceivedSpeed += BytesTransferred
        Else
            SpeedTransferred = ReceivedSpeed
            ReceivingTime.Reset()

            ReceivingTime.Start()
            ReceivedSpeed = 0
        End If


        Dim ReceiveAgain As Boolean = True
        If (BytesTransferred <> 0) Then
            Dim RawString As String = Encoding.UTF8.GetString(Buffer, 0, BytesTransferred)
            Raw.Append(RawString)
            HTMLBody.Append(RawString)

            If ReceivedLength = -1 Then
                Dim ExecuteHead As Match = Regex.Match(HTMLBody.ToString(), "\r\n\r\n")
                If (ExecuteHead.Success) Then HTMLHeaders.Append(HTMLBody.ToString().Substring(0, ExecuteHead.Index))

                Dim ContentLength As Match = Regex.Match(HTMLBody.ToString(), "Content-Length: (?<len>\d+)\r\n")
                If ContentLength.Success Then ReceivedLength = Int32.Parse(ContentLength.Groups("len").Value) Else ReceivedLength = -2
                HTMLBody.Remove(0, ExecuteHead.Index + 4)

                For Each getCookie As Match In Regex.Matches(HTMLHeaders.ToString(), "Set-Cookie:.(?<name>.*?)=(?<value>.*?);(?<expires>.*?)[\r|\n|;]")
                    If String.Join("=", Cookie.AllKeys).Contains(getCookie.Groups("name").Value.Trim()) Then
                        Cookie(getCookie.Groups("name").Value.Trim()) = getCookie.Groups("value").Value.Trim()
                    Else
                        Cookie.Add(getCookie.Groups("name").Value.Trim(), getCookie.Groups("value").Value.Trim())
                    End If
                    ' Expires DateTime
                    ' Dim expired As Match = Regex.Match(getCookie.Groups("expires").Value, "expires=([Sun|Mon|Tue|Wed|Thu|Fri|Sat]+,.[\w{2,4}|-]+.[\w{1,2}|:]+.GMT)")
                    ' If expired.Success Then
                Next

            End If

            If ReceivedLength = -2 Then
                Dim FirstChunk As Boolean = False
                Dim TransferEncoding As Match
                If Chunked = 0 Then
                    TransferEncoding = Regex.Match(HTMLHeaders.ToString(), "Transfer-Encoding: (?<type>.*?)\r\n")

                    If TransferEncoding.Success And TransferEncoding.Groups("type").Value = "chunked" Then
                        TransferEncoding = Regex.Match(HTMLBody.ToString(), "(?<hex>.*?)(\r\n)")
                        If (TransferEncoding.Success) Then
                            HTMLBody.Remove(TransferEncoding.Index, TransferEncoding.Length)
                            Chunked = Convert.ToInt32(TransferEncoding.Groups("hex").Value, 16)
                            FirstChunk = True
                        End If
                    End If
                End If

                Dim EndResponse As Match = Regex.Match(HTMLBody.ToString(), "\r\n0\r\n\r\n")
                If (EndResponse.Success) Then
                    HTMLBody.Remove(EndResponse.Index, EndResponse.Length)
                    For Each Chunk As Match In Regex.Matches(HTMLBody.ToString(), "\r\n(.*?)\r\n", RegexOptions.RightToLeft)
                        HTMLBody.Remove(Chunk.Index, Chunk.Length)
                        Chunked += Convert.ToInt32(Chunk.Groups(1).Value, 16)
                    Next

                    ReceiveAgain = False
                End If
            End If
        End If


        If ReceivedLength = HTMLBody.Length Or BytesTransferred = 0 Or Not ReceiveAgain Then
            ReceiveAgain = False
            Console.WriteLine("{0} >> Received ({1} ms)", New String() {Host.HostName, ElapsedTime.ElapsedMilliseconds})
            ElapsedTime.Reset()
            Me.Status = XHRAsync.Received
            Buffer = Nothing
            OnSocket.Shutdown(SocketShutdown.Both)
            OnSocket.Close()
        End If
        If (ReceiveAgain) Then Me.AsyncReceive()
    End Sub

    Public Sub WaitRequest()
        Do
            Threading.Thread.Sleep(10)
        Loop While Me.Status = XHRAsync.Sending Or Me.Status = XHRAsync.Connected
    End Sub

    Public Sub WaitResponse()
        Do
            Threading.Thread.Sleep(10)
        Loop While Me.Status <> XHRAsync.Received And Me.Status <> XHRAsync.Die
    End Sub

    Public Shared Function ToPackageString(bytes As Integer) As [String]
        Dim unit As [String]() = {" b/s", " Kb/s", " Mb/s", " Gb/s"}
        Dim i As [Byte] = 0
        While bytes > 1024
            bytes = bytes / 1024
            i += 1
        End While
        Return Math.Round(bytes, 2) & unit(i)
    End Function

    Public Shared Function Kilobytes(Optional unit As Integer = 1) As Integer
        Return 1024 * unit
    End Function
    Public Shared Function Megabytes(Optional unit As Integer = 1) As Integer
        Return 1024 * XHR.Kilobytes(unit)
    End Function
    Public Shared Function Gigabytes(Optional unit As Integer = 1) As Integer
        Return 1024 * XHR.Megabytes(unit)
    End Function

    Public Shared Function Connect(request As RequestBuilder, Optional ByVal limit_data As Int32 = 0) As String
        Dim conn As XHR = New XHR(limit_data)
        conn.AsyncSend(request)
        conn.WaitResponse()

        Dim result As String = conn.ToString()
        conn.Close()
        Return result
    End Function
    Public Shared Function Connect(request_url As String, Optional ByVal limit_data As Int32 = 0) As String
        Return XHR.Connect(New RequestBuilder(request_url), limit_data)
    End Function

    Public Overloads Function ToString() As String
        Return HTMLBody.ToString()
    End Function

End Class