USE [Parse]
GO

/****** Object:  StoredProcedure [dbo].[PersonInsert]    Script Date: 8/6/2024 5:28:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[PersonInsert] 
@name nvarchar(200),
@language nvarchar(100),
@id nvarchar(100),
@bio nvarchar(max),
@version float,
@firstname nvarchar(200),
@lastname nvarchar(200)
As
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

INSERT INTO [dbo].[Person]
           ([name]
           ,[language]
           ,[id]
           ,[bio]
           ,[version]
		   ,[firstname]
		   ,[lastname])
     VALUES
	 (
	 @name,
	 @language,
	 @id,
	 @bio,
	 @version,
	 @firstname,
	 @lastname
	 )

END
GO

