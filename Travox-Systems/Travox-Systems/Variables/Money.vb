Public Class Money
    Enum VATType
        INCLUDE
        EXCLUDE
    End Enum

    Private _Money As Decimal = 0
    Private _CurrencyRate As Decimal = 1
    Public Sub New()
    End Sub
    Public Sub New(ByVal value As String)
        If (Not String.IsNullOrEmpty(value)) Then _Money = CDec(value.Replace(",", ""))
    End Sub
    Public Sub New(ByVal value As String, ByVal rate As String)
        If (Not String.IsNullOrEmpty(value)) Then _Money = CDec(value.Replace(",", ""))
        If (Not String.IsNullOrEmpty(rate)) Then _CurrencyRate = CDec(rate.Replace(",", ""))
    End Sub
    Public Shared Operator -(ByVal v1 As Money) As Decimal
        Return -(v1._Money * v1._CurrencyRate)
    End Operator
    Public Shared Operator +(ByVal v1 As Money, ByVal v2 As Money) As Decimal
        Return (v1._Money * v1._CurrencyRate) + (v2._Money * v2._CurrencyRate)
    End Operator
    Public Shared Operator -(ByVal v1 As Money, ByVal v2 As Money) As Decimal
        Return (v1._Money * v1._CurrencyRate) - (v2._Money * v2._CurrencyRate)
    End Operator
    Public Shared Operator *(ByVal v1 As Money, ByVal v2 As Money) As Decimal
        Return (v1._Money * v1._CurrencyRate) * (v2._Money * v2._CurrencyRate)
    End Operator
    Public Shared Operator /(ByVal v1 As Money, ByVal v2 As Money) As Decimal
        Return (v1._Money * v1._CurrencyRate) / (v2._Money * v2._CurrencyRate)
    End Operator
    Public Function ToDeciaml() As String
        Return Math.Round(_Money * _CurrencyRate, 2)
    End Function
    Public Overrides Function ToString() As String
        Return Math.Round(_Money * _CurrencyRate, 2).ToString("##,##0.00")
    End Function
    Public Function VAT(ByVal type As VATType, ByVal percent As String) As Money
        Dim _vat As Decimal = MBOS.Convert(Of Decimal)(percent)
        Select Case type
            Case VATType.INCLUDE : _vat = (_Money * _vat) / (100 + _vat)
            Case VATType.EXCLUDE : _vat = (_Money * _vat)
        End Select
        Return New Money(_vat, _CurrencyRate)
    End Function
    Public Function ChangeRate(ByVal new_rate As String) As Money
        Return New Money(_Money * _CurrencyRate / MBOS.Convert(Of Decimal)(new_rate))
    End Function
End Class