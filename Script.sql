--generate scripts for the following stored procedures
--[Post_Translate], 
--[Post_Select_Many]
--[Post_Select_Published]

--only on cocozil db
UPDATE [Posts] SET URN = '/en' + URN WHERE LocaleID = 1

--remove -recipe from endpoint
UPDATE Posts SET URN = REPLACE(URN, '-recipe', '') WHERE URN like '%-recipe'

--update cooking segment to recipes segment
UPDATE Posts SET URN = REPLACE(URN, '/cooking/', '/recipes/') WHERE URN like '%/cooking/%'