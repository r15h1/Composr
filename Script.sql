--generate scripts for the following stored procedures
--[Post_Translate], 
--[Post_Select_Many]
--[Post_Select_Published]

--only on cocozil db
UPDATE [Posts] SET URN = '/en' + URN WHERE LocaleID = 1