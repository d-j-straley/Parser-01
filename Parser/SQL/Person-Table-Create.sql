USE [Parse]
GO

/****** Object:  Table [dbo].[Person]    Script Date: 8/6/2024 5:28:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Person](
	[RecordID] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](200) NULL,
	[language] [nvarchar](100) NULL,
	[id] [nvarchar](100) NULL,
	[bio] [nvarchar](max) NULL,
	[version] [float] NULL,
	[firstname] [nvarchar](200) NULL,
	[lastname] [nvarchar](200) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

