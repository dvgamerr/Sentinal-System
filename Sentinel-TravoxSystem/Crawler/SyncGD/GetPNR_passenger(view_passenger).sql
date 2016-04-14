SELECT [passenger_info_id], booking_info_id, ticket_no, [initial_name], [first_name] 
, [last_name], [passenger_type], [ticket_price], [ticket_tax], [ticket_cost] 
, [ticket_sell],isnull(convert(varchar(16),[birthday],121),'') as [birthday], [passport_no] 
FROM view_passenger WITH(NOLOCK) 
WHERE booking_info_id = @booking_info_id