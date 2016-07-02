SELECT isnull(ad_cost,0) as ad_cost, isnull(ad_price,0) as ad_price, isnull(pax_adult,0) as pax_adult 
, isnull(ch_cost,0) as ch_cost, isnull(ch_price,0) as ch_price, isnull(pax_child,0) as pax_child 
, booking_info_id, product_name, product_short_desc 
FROM nippon_b2c.travox.view_additional_service WITH(NOLOCK) 
WHERE booking_info_id = @booking_info_id
