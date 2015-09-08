SELECT secretary_id, period, output_email, output_printer, email, report_name, report_key
FROM crawler.secretary s INNER JOIN document.report r ON r.report_id = s.report_id
WHERE s.status = 'ACTIVE' AND site_customer_id = @id