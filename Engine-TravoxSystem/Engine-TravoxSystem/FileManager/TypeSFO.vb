Imports System.Text
Imports System.IO

Namespace FileManager

    Public Structure TableEntrie
        Public Key As [String] ' SFO Key 
        Public Value As [String] ' SFO Value 
        Public OffsetKey As UInt32 ' Offset of the param_key from start of key_table 
        Public OffsetData As UInt32 ' Offset of the param_data from start of data_table 
        Public ParamFormat As UInt32 ' Type of data of param_data in the data_table 
        Public ParamLength As UInt32 ' Used Bytes by param_data in the data_table 
        Public ParamMaxLength As UInt32 ' Total bytes reserved for param_data in the data_table 
    End Structure

    Public Class TypeSFO
        Private _Magic As [String] ' Always PSF 
        Private _Version As [Double] ' Usually 1.1 
        Private _KeyTableStart_ As UInt32 ' Start position of key_table 
        Private _DataTableStart_ As UInt32 ' Start position of data_table 
        Private _IndexTableEntries_ As UInt32 ' Number of entries in index_table

        Private _Entries As TableEntrie() = {}

        Public ReadOnly Property ToArray() As TableEntrie()
            Get
                Return _Entries
            End Get
        End Property
        Public ReadOnly Property Magic() As [String]
            Get
                Return _Magic
            End Get
        End Property
        Public ReadOnly Property Version() As [Double]
            Get
                Return _Version
            End Get
        End Property
        Public Function Param(key As [String]) As [String]
            Dim _Data As [String] = "0"
            For Each item As TableEntrie In _Entries
                If item.Key.ToLower() = key.ToLower().Trim() Then _Data = item.Value
            Next
            Return _Data
        End Function
        Public Sub Param(key As [String], value As UInt32)
            If (String.IsNullOrEmpty(value)) Then value = 0
            Me.AddEntrie(key, value.ToString(), &H404UI)
        End Sub
        Public Sub Param(key As [String], value As [String])
            If (String.IsNullOrEmpty(value)) Then value = " "
            Me.AddEntrie(key, value.ToString(), &H204UI)
        End Sub

        ' Constructor
        Public Sub New()
        End Sub
        Public Sub Load(RawData As [Byte]())
            Try
                _Magic = Me.GetMagic(RawData)
                _Version = Me.GetVersion(RawData)
                _KeyTableStart_ = BitConverter.ToUInt32(RawData, &H8UI)
                _DataTableStart_ = BitConverter.ToUInt32(RawData, &HCUI)
                _IndexTableEntries_ = BitConverter.ToUInt32(RawData, &H10UI)

                _Entries = New TableEntrie(_IndexTableEntries_ - 1) {}
                Dim iEtries As Int32 = 0
                While iEtries < _IndexTableEntries_
                    _Entries(iEtries).OffsetKey = BitConverter.ToUInt16(RawData, &H14UI + (&H10UI * iEtries))
                    _Entries(iEtries).ParamFormat = BitConverter.ToUInt16(RawData, &H16UI + (&H10UI * iEtries))
                    _Entries(iEtries).ParamLength = BitConverter.ToUInt32(RawData, &H18UI + (&H10UI * iEtries))
                    _Entries(iEtries).ParamMaxLength = BitConverter.ToUInt32(RawData, &H1CUI + (&H10UI * iEtries))
                    _Entries(iEtries).OffsetData = BitConverter.ToUInt32(RawData, &H20UI + (&H10UI * iEtries))

                    _Entries(iEtries).Key = Me.ConvertToKey(RawData, CType(_Entries(iEtries).OffsetKey + _KeyTableStart_, Int32))
                    If _Entries(iEtries).ParamFormat = &H204UI Then
                        _Entries(iEtries).Value = Me.ConvertToString(RawData, CType(_Entries(iEtries).OffsetData + _DataTableStart_, Int32), CType(_Entries(iEtries).ParamLength, Int32))
                    ElseIf _Entries(iEtries).ParamFormat = &H404UI Then
                        _Entries(iEtries).Value = Me.ConvertToInt32(RawData, CType(_Entries(iEtries).OffsetData + _DataTableStart_, Int32)).ToString()
                    End If

                    ' Console.WriteLine(_Entries[iEtries].Key + " ::: " + _Entries[iEtries].Value);
                    iEtries += 1
                    'Console.WriteLine("Key: " + _KeyTableStart_ + " | Data: " + _DataTableStart_ + " | Length: " + _IndexTableEntries_);
                End While
            Catch ex As Exception
                Console.WriteLine("-----------------------------------------------------")
                Console.WriteLine("SfoFIle.Load(" & RawData.Length & " Bytes) Method")
                Console.WriteLine("EX:: " & ex.Message)
                Console.WriteLine("-----------------------------------------------------")
            End Try
        End Sub

        Public Sub Save()

        End Sub
        Public Sub SaveAs(path As [String])
            _IndexTableEntries_ = CType(_Entries.Length, UInt32)
            _KeyTableStart_ = (CType(_Entries.Length, UInt32) * &H10UI) + &H14UI
            Dim LengthKeyEntrie As UInt32 = 0
            Dim LengthDataEntrie As UInt32 = 0
            Dim iEtries As Int32 = 0
            While iEtries < _IndexTableEntries_
                ' New Offset
                _Entries(iEtries).OffsetKey = LengthKeyEntrie
                _Entries(iEtries).OffsetData = LengthDataEntrie

                ' Calculator Langth
                Dim CountBytes As UInt32 = 0
                Dim ReadBytes As [Byte]() = New [Byte](_Entries(iEtries).Value.Length * 3 - 1) {}
                Encoding.UTF8.GetBytes(_Entries(iEtries).Value, 0, _Entries(iEtries).Value.Length, ReadBytes, 0)
                For Each wChar As [Byte] In ReadBytes
                    If wChar > &H0UI Then
                        CountBytes += 1
                    End If
                Next

                ' New Langth
                _Entries(iEtries).ParamLength = CountBytes
                If _Entries(iEtries).ParamMaxLength < CountBytes Then
                    _Entries(iEtries).ParamMaxLength = CountBytes + 1
                End If

                ' Calculator Offset
                LengthKeyEntrie += (CType(_Entries(iEtries).Key.Length, UInt32) + 1)
                LengthDataEntrie += _Entries(iEtries).ParamMaxLength
                iEtries += 1
            End While
            _DataTableStart_ = LengthKeyEntrie + _KeyTableStart_

            If File.Exists(path) Then
                File.Delete(path)
            End If
            Dim ParamWrite As New FileStream(path, FileMode.Create, FileAccess.ReadWrite)
            ParamWrite.Position = &H1UI

            ' Stream Write Header SFO
            ParamWrite.Write(New [Byte]() {&H50UI, &H53UI, &H46UI, &HFFUI, &HFFUI}, 0, 5)
            Me.WriteToStream(ParamWrite, _KeyTableStart_, &H8UI)
            Me.WriteToStream(ParamWrite, _DataTableStart_, &HCUI)
            Me.WriteToStream(ParamWrite, _IndexTableEntries_, &H10UI)

            iEtries = 0
            While iEtries < _IndexTableEntries_
                Me.WriteToStream(ParamWrite, _Entries(iEtries).OffsetKey, &H14UI + (&H10UI * iEtries))
                Me.WriteToStream(ParamWrite, _Entries(iEtries).ParamFormat, &H16UI + (&H10UI * iEtries))
                Me.WriteToStream(ParamWrite, _Entries(iEtries).ParamLength, &H18UI + (&H10UI * iEtries))
                Me.WriteToStream(ParamWrite, _Entries(iEtries).ParamMaxLength, &H1CUI + (&H10UI * iEtries))
                Me.WriteToStream(ParamWrite, _Entries(iEtries).OffsetData, &H20UI + (&H10UI * iEtries))

                Me.WriteToStream(ParamWrite, _Entries(iEtries).Key, (_KeyTableStart_ + _Entries(iEtries).OffsetKey))
                If _Entries(iEtries).ParamFormat = &H404UI Then
                    Me.WriteToStream(ParamWrite, UInt32.Parse(_Entries(iEtries).Value), (_DataTableStart_ + _Entries(iEtries).OffsetData))
                ElseIf _Entries(iEtries).ParamFormat = &H204UI Then
                    Me.WriteToStream(ParamWrite, _Entries(iEtries).Value, (_DataTableStart_ + _Entries(iEtries).OffsetData))
                End If
                Me.WriteToStream(ParamWrite, &H0UI, (_DataTableStart_ + _Entries(iEtries).OffsetData + _Entries(iEtries).ParamMaxLength))

                iEtries += 1
            End While
            ParamWrite.Close()
        End Sub

        Private Sub AddEntrie(key As [String], value As [String], format As UInt32)
            Try
                If value.Length > 0 Then
                    Dim FoundEntrie As [Boolean] = False
                    For iEntrie As Int32 = 0 To _Entries.Length - 1
                        If _Entries(iEntrie).Key.ToLower() = key.ToLower().Trim() Then
                            _Entries(iEntrie).Key = key.Trim()
                            _Entries(iEntrie).Value = value.Trim()
                            _Entries(iEntrie).ParamFormat = format
                            FoundEntrie = True
                            Exit For
                        End If
                    Next
                    If Not FoundEntrie Then
                        _IndexTableEntries_ += 1
                        Dim tmpData As TableEntrie() = _Entries
                        _Entries = New TableEntrie(_IndexTableEntries_ - 1) {}
                        Dim iEtries As Int32 = 0
                        While iEtries < tmpData.Length
                            _Entries(iEtries) = tmpData(iEtries)
                            iEtries += 1
                        End While

                        _Entries(iEtries) = New TableEntrie() With { _
                            .Key = key, _
                            .Value = value, _
                            .OffsetKey = &H0UI, _
                            .OffsetData = &H0UI, _
                            .ParamFormat = format
                        }
                    End If
                Else
                    Console.WriteLine("SfoFile.AddEntrie(" & key & ", " & value & ") Method")
                End If
            Catch ex As Exception
                Console.WriteLine("-----------------------------------------------------")
                Console.WriteLine("SfoFile.AddEntrie(" & key & ", " & value & ") Method")
                Console.WriteLine(ex.Message)
                Console.WriteLine("-----------------------------------------------------")
            End Try
        End Sub
        Private Sub WriteToStream(stream As FileStream, value As [String], position As Long)
            Dim ValBytes As [Byte]() = New [Byte](value.Length * 3 - 1) {}
            Encoding.UTF8.GetBytes(value, 0, value.Length, ValBytes, 0)

            stream.Position = position
            For Each wChar As [Byte] In ValBytes
                If wChar > &H0UI Then
                    stream.WriteByte(wChar)
                End If
            Next
        End Sub
        Private Sub WriteToStream(stream As FileStream, value As UInt32, position As Long)
            Dim ValBytes As [Byte]() = BitConverter.GetBytes(value)
            stream.Position = position
            stream.Write(ValBytes, 0, ValBytes.Length)
        End Sub

        Private Function GetMagic(data As [Byte]()) As [String]
            Return Encoding.UTF8.GetString(data, 1, 3)
        End Function
        Private Function GetVersion(data As [Byte]()) As [Double]
            Return data(5) + (data(4) / 100.0)
        End Function

        Private Function ConvertToString(data As [Byte](), index As Int32, length As Int32) As [String]
            Dim iLoop As Int32 = 0
            Dim result As [String] = ""
            While iLoop < length
                Dim i As Int32 = index + iLoop
                If data(i) > &H0UI AndAlso data(i) < &HC0UI Then
                    result += Encoding.UTF8.GetChars(data, (i), 1)(0).ToString()
                ElseIf data(i) >= &HC0UI AndAlso data(i) <= &HCFUI Then
                    result += Encoding.UTF8.GetString(data, (i), 2)
                    iLoop += 1
                ElseIf data(i) >= &HE0UI AndAlso data(i) <= &HEFUI Then
                    result += Encoding.UTF8.GetString(data, (i), 3)
                    iLoop += 2
                End If
                iLoop += 1
            End While
            Return result
        End Function
        Private Function ConvertToInt32(data As [Byte](), index As Int32) As Int32
            Return BitConverter.ToInt32(data, index)
        End Function
        Private Function ConvertToKey(data As [Byte](), index As Int32) As [String]
            Dim iLoop As Int32 = 0
            Dim result As [String] = ""
            While data(index + iLoop) > &H0UI
                result += Encoding.UTF8.GetChars(data, (index + iLoop), 1)(0).ToString()
                iLoop += 1
            End While
            Return result
        End Function

    End Class
End Namespace