SELECT [ref_id], [airline_code], [airline_name], [gds] 
, isnull(convert(varchar(16),[client_deadline],121),'') as [client_deadline] 
, isnull(convert(varchar(16),[airline_deadline],121),'') as [airline_deadline]  
, isnull(convert(varchar(16),[ticket_date],121),'') as [ticket_date] 
, [operate_by_code], [pnr], [ad_cost], [ch_cost] 
, [ad_price], [ch_price], [ad_tax], [ch_tax], [currency], credit_card_success 
, [service_charge], [invoice_name], [invoice_address], [invoice_district], [invoice_province] 
, [invoice_postcode], [invoice_country], [invoice_tel], [invoice_fax], [invoice_email] 
, [payment_type], [agent_name], [agent_address], isnull([insurance_price],0) as insurance_price, [insurance_code] 
FROM view_header WITH(NOLOCK) 
WHERE code = @code /*TICKET_DATE*/