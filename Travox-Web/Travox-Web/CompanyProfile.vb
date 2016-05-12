Imports Travox.Systems

Public Class CompanyProfile

    Public Shared Property Name() As String
        Get
            Return Column("name")
        End Get
        Set(ByVal value As String)
            Save("name", value)
        End Set
    End Property
    Public Shared Property Address() As String
        Get
            Return Column("address")
        End Get
        Set(ByVal value As String)
            Save("address", value)
        End Set
    End Property
    Public Shared Property Telephone() As String
        Get
            Return Column("tel")
        End Get
        Set(ByVal value As String)
            Save("tel", value)
        End Set
    End Property
    Public Shared Property Fax() As String
        Get
            Return Column("fax")
        End Get
        Set(ByVal value As String)
            Save("fax", value)
        End Set
    End Property
    Public Shared Property Email() As String
        Get
            Return Column("email")
        End Get
        Set(ByVal value As String)
            Save("email", value)
        End Set
    End Property
    Public Shared Property Website() As String
        Get
            Return Column("website")
        End Get
        Set(ByVal value As String)
            Save("website", value)
        End Set
    End Property
    Public Shared Property TaxID() As String
        Get
            Return Column("taxid")
        End Get
        Set(ByVal value As String)
            Save("taxid", value)
        End Set
    End Property
    Public Shared Property Slogan() As String
        Get
            Return Column("slogan")
        End Get
        Set(ByVal value As String)
            Save("slogan", value)
        End Set
    End Property
    Public Shared Property VoucherVersion() As String
        Get
            Return Column("voucher_version")
        End Get
        Set(ByVal value As String)
            Save("voucher_version", value)
        End Set
    End Property
    Public Shared Property VoucherExpenseTo() As Boolean
        Get
            Return MBOS.Bool(Column("voucher_show_expense_to"))
        End Get
        Set(ByVal value As Boolean)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("voucher_show_expense_to", update)
        End Set
    End Property
    REM :: INVOICE Default Data
    Public Shared Property VAT() As String
        Get
            Return Column("default_vat")
        End Get
        Set(ByVal value As String)
            Save("default_vat", value)
        End Set
    End Property

    REM :: Format No Invoice,Receipt,Tax,Payment
    Public Shared Property FormatITEMP() As String
        Get
            Return Column("format_inv_temp")
        End Get
        Set(ByVal value As String)
            Save("format_inv_temp", value)
        End Set
    End Property
    Public Shared Property FormatINOVAT() As String
        Get
            Return Column("format_inv_novat")
        End Get
        Set(ByVal value As String)
            Save("format_inv_novat", value)
        End Set
    End Property
    Public Shared Property FormatIVAT() As String
        Get
            Return Column("format_inv_vat")
        End Get
        Set(ByVal value As String)
            Save("format_inv_vat", value)
        End Set
    End Property
    Public Shared Property FormatRTEMP() As String
        Get
            Return Column("format_rec_temp")
        End Get
        Set(ByVal value As String)
            Save("format_rec_temp", value)
        End Set
    End Property
    Public Shared Property FormatRVAT() As String
        Get
            Return Column("format_rec_vat")
        End Get
        Set(ByVal value As String)
            Save("format_rec_vat", value)
        End Set
    End Property
    Public Shared Property FormatTAX() As String
        Get
            Return Column("format_tax_vat")
        End Get
        Set(ByVal value As String)
            Save("format_tax_vat", value)
        End Set
    End Property
    Public Shared Property FormatPAYMENT() As String
        Get
            Return Column("format_payment")
        End Get
        Set(ByVal value As String)
            Save("format_payment", value)
        End Set
    End Property
    Public Shared Property FormatTAXAirline() As String
        Get
            Return Column("format_tax_airline")
        End Get
        Set(ByVal value As String)
            Save("format_tax_airline", value)
        End Set
    End Property

    REM :: SMS FAX 
    Public Shared Property SMSSubject() As String
        Get
            Return Column("subject_sms")
        End Get
        Set(ByVal value As String)
            Save("subject_sms", value)
        End Set
    End Property
    Public Shared Property SMSMobile() As String
        Get
            Return Column("mobile_sms")
        End Get
        Set(ByVal value As String)
            Save("mobile_sms", value)
        End Set
    End Property
    Public Shared Property AllowFax() As Boolean
        Get
            Return MBOS.Bool(Column("allow_fax"))
        End Get
        Set(ByVal value As Boolean)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("allow_fax", update)
        End Set
    End Property
    Public Shared Property AllowSMS() As Boolean
        Get
            Return MBOS.Bool(Column("allow_sms"))
        End Get
        Set(ByVal value As Boolean)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("allow_sms", update)
        End Set
    End Property
    Public Shared Property AlertEmail() As String
        Get
            Return Column("alert_email")
        End Get
        Set(ByVal value As String)
            Save("alert_email", value)
        End Set
    End Property
    Public Shared Property BCCEmail() As String
        Get
            Return Column("ns_email")
        End Get
        Set(ByVal value As String)
            Save("ns_email", value)
        End Set
    End Property

    Public Shared Property AllowInvoiceNoVatInTax() As Boolean
        Get
            Return MBOS.Bool(Column("allow_invoice_novat_in_tax"))
        End Get
        Set(ByVal value As Boolean)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("allow_invoice_novat_in_tax", update)
        End Set
    End Property
    Public Shared Property AllowNoVatInTax() As Boolean
        Get
            Return MBOS.Bool(Column("allow_novat_in_tax"))
        End Get
        Set(ByVal value As Boolean)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("allow_novat_in_tax", update)
        End Set
    End Property

    Public Shared Property AutoUpdateDescription()
        Get
            Return MBOS.Bool(Column("auto_update_desc"))
        End Get
        Set(ByVal value)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("auto_update_desc", update)
        End Set
    End Property

    REM :: Lock And Default Invoice,Receipt,Payment Date
    Public Shared Property LockInvoiceDate()
        Get
            Return MBOS.Bool(Column("lock_invoice_date"))
        End Get
        Set(ByVal value)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("lock_invoice_date", update)
        End Set
    End Property
    Public Shared Property LockReceiptDate()
        Get
            Return MBOS.Bool(Column("lock_receipt_date"))
        End Get
        Set(ByVal value)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("lock_receipt_date", update)
        End Set
    End Property
    Public Shared Property LockPaymentDate()
        Get
            Return MBOS.Bool(Column("lock_payment_date"))
        End Get
        Set(ByVal value)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("lock_payment_date", update)
        End Set
    End Property
    Public Shared Property LockTaxDate()
        Get
            Return MBOS.Bool(Column("lock_tax_date"))
        End Get
        Set(ByVal value)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("lock_tax_date", update)
        End Set
    End Property
    Public Shared Property DefaultInvoiceDate() As DefaultDateType
        Get
            Return GetDefaultDateType(Column("default_invoice_date"))
        End Get
        Set(ByVal value As DefaultDateType)
            Save("lock_invoice_date", value.ToString())
        End Set
    End Property
    Public Shared Property DefaultReceiptDate() As DefaultDateType
        Get
            Return GetDefaultDateType(Column("default_receipt_date"))
        End Get
        Set(ByVal value As DefaultDateType)
            Save("default_receipt_date", value.ToString())
        End Set
    End Property
    Public Shared Property DefaultPaymentDate() As DefaultDateType
        Get
            Return GetDefaultDateType(Column("default_payment_date"))
        End Get
        Set(ByVal value As DefaultDateType)
            Save("default_payment_date", value.ToString())
        End Set
    End Property
    Public Shared Property DefaultTaxDate() As DefaultDateType
        Get
            Return GetDefaultDateType(Column("default_tax_date"))
        End Get
        Set(ByVal value As DefaultDateType)
            Save("default_receipt_date", value.ToString())
        End Set
    End Property

    REM :: GDS
    Public Shared Property NoneBSP_AMADEUS() As Boolean
        Get
            Return MBOS.Bool(Column("amadeus_nonbsp"))
        End Get
        Set(ByVal value As Boolean)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("amadeus_nonbsp", update)
        End Set
    End Property
    Public Shared Property ABACUS() As Boolean
        Get
            Return MBOS.Bool(Column("abacus"))
        End Get
        Set(ByVal value As Boolean)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("abacus", update)
        End Set
    End Property
    Public Shared Property AMADEUS() As Boolean
        Get
            Return MBOS.Bool(Column("amadeus"))
        End Get
        Set(ByVal value As Boolean)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("amadeus", update)
        End Set
    End Property
    Public Shared Property GALILEO() As Boolean
        Get
            Return MBOS.Bool(Column("galileo"))
        End Get
        Set(ByVal value As Boolean)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("galileo", update)
        End Set
    End Property

    REM :: TICKET
    Public Shared Property ShowTicketPrice() As Boolean
        Get
            Return MBOS.Bool(Column("show_ticket_price"))
        End Get
        Set(ByVal value As Boolean)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("show_ticket_price", update)
        End Set
    End Property
    Public Shared Property RequireTicket() As Boolean
        Get
            Return MBOS.Bool(Column("require_ticket"))
        End Get
        Set(ByVal value As Boolean)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("require_ticket", update)
        End Set
    End Property

    REM :: BOOKING
    Public Shared Property BookingNODigit() As Integer
        Get
            Return MBOS.Bool(Column("booking_digit"))
        End Get
        Set(ByVal value As Integer)
            Save("booking_digit", value)
        End Set
    End Property
    Public Shared Property MBOSSmall() As Boolean
        Get
            Return MBOS.Bool(Column("mbos_small"))
        End Get
        Set(ByVal value As Boolean)
            Dim update As String = "N"
            If (value) Then update = "Y"
            Save("mbos_small", update)
        End Set
    End Property

    Public Enum DefaultDateType
        None
        CurrentDate
        BillableDate
        TicketHotelDate
    End Enum

    Public Shared Function GetDefaultDateType(ByVal state As String) As DefaultDateType
        Select Case state
            Case "CurrentDate" : Return DefaultDateType.CurrentDate
            Case "BillableDate" : Return DefaultDateType.BillableDate
            Case "TicketHotelDate" : Return DefaultDateType.TicketHotelDate
            Case Else : Return DefaultDateType.None
        End Select
    End Function

    Protected Shared Function Column(ByVal name As String) As String
        Dim base As New DB()
        Return base.GetField(String.Format("SELECT TOP 1 {0} FROM company_profile", name))
    End Function
    Protected Shared Sub Save(ByVal name As String, ByVal value As String)
        Dim base As New DB()
        base.Execute(String.Format("UPDATE company_profile SET update_date=GETDATE(),update_staff={2},{0}={1}", name, value, User.Login.ID))
        Try
            base.Apply()
        Catch
            base.Rollback()
        End Try
    End Sub

    REM :: Config MBOS 
    Public Shared Property LimitBooking() As Integer
        Get
            Return MBOS.Int(Column("limit_booking"))
        End Get
        Set(ByVal value As Integer)
            Save("limit_booking", value)
        End Set
    End Property
    Public Shared Property LimitStaff() As Integer
        Get
            Return MBOS.Int(Column("limit_staff"))
        End Get
        Set(ByVal value As Integer)
            Save("limit_staff", value)
        End Set
    End Property
    Public Shared Property LimitDueDate() As String
        Get
            Dim value As String = Column("CONVERT(VARCHAR,ISNULL(limit_duedate,''),105)")
            Return IIf(value = "01-01-1900", Nothing, value)
        End Get
        Set(ByVal value As String)
            Save("limit_duedate", String.Format("CONVERT(DATE, '{0}', 105)", value))
        End Set
    End Property

End Class