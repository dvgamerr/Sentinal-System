SELECT [booking_info_detail_id], booking_info_id, [origin_code], [origin_name] 
, [destination_code], [destination_name], [class], [sub_class] 
, isnull(convert(varchar(16),[departure_date],121),'') as [departure_date] 
, isnull(convert(varchar(16),[arrival_date],121),'') as [arrival_date] 
, [flight_no], [airline_code], [airline_name] 
FROM view_route WITH(NOLOCK) 
WHERE booking_info_id = @booking_info_id