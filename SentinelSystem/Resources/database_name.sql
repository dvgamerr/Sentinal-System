SELECT id, code, database_name, [description] FROM site_customer 
WHERE [status] = 'ACTIVE' AND ISNULL(database_name,'') <> '' AND sentinel = 'Y'