USE [Drawings]
GO
/****** Object:  Table [dbo].[Drawings]    Script Date: 10/05/2012 13:35:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Drawings](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CATEGORY] [int] NOT NULL,
	[STATUS] [int] NOT NULL,
	[CONSULTANT] [nvarchar](256) NULL,
	[FILENAME] [nvarchar](512) NULL,
	[SCALE] [nvarchar](10) NULL,
	[LEGACY_DRAWING] [nvarchar](256) NULL,
	[COMMENTS] [nvarchar](256) NULL,
	[NUMBER] [nvarchar](40) NOT NULL,
	[PROJECT_TITLE] [nvarchar](256) NULL,
	[TITLE_LINE_1] [nvarchar](256) NULL,
	[ELECTRONIC] [bit] NULL,
	[TITLE_LINE_2] [nvarchar](256) NULL,
	[TITLE_LINE_3] [nvarchar](256) NULL,
	[TITLE_LINE_4] [nvarchar](256) NULL,
	[SHEET] [nvarchar](10) NOT NULL,
	[SHEET_SIZE] [nvarchar](10) NULL,
	[SHEET_REVISION] [nvarchar](10) NULL,
	[DRAWN_BY] [nvarchar](20) NULL,
	[DRAWN_DATE] [datetime] NULL,
	[CHECKED_BY] [nvarchar](20) NULL,
	[CHECKED_DATE] [datetime] NULL,
	[APPRV_BY] [nvarchar](20) NULL,
	[APPRV_DATE] [datetime] NULL,
	[REV_A] [nvarchar](10) NULL,
	[REV_B] [nvarchar](10) NULL,
	[REV_C] [nvarchar](10) NULL,
	[REV_D] [nvarchar](10) NULL,
	[REV_E] [nvarchar](10) NULL,
	[REV_F] [nvarchar](10) NULL,
	[REV_G] [nvarchar](10) NULL,
	[DRAWN_BY_A] [nvarchar](20) NULL,
	[DRAWN_BY_B] [nvarchar](20) NULL,
	[DRAWN_BY_C] [nvarchar](20) NULL,
	[DRAWN_BY_D] [nvarchar](20) NULL,
	[DRAWN_BY_E] [nvarchar](20) NULL,
	[DRAWN_BY_F] [nvarchar](20) NULL,
	[DRAWN_BY_G] [nvarchar](20) NULL,
	[DATE_A] [datetime] NULL,
	[DATE_B] [datetime] NULL,
	[DATE_C] [datetime] NULL,
	[DATE_D] [datetime] NULL,
	[DATE_E] [datetime] NULL,
	[DATE_F] [datetime] NULL,
	[DATE_G] [datetime] NULL,
	[DESCRIPTION_A] [nvarchar](256) NULL,
	[DESCRIPTION_B] [nvarchar](256) NULL,
	[DESCRIPTION_C] [nvarchar](256) NULL,
	[DESCRIPTION_D] [nvarchar](256) NULL,
	[DESCRIPTION_E] [nvarchar](256) NULL,
	[DESCRIPTION_F] [nvarchar](256) NULL,
	[DESCRIPTION_G] [nvarchar](256) NULL,
	[APPRV_BY_A] [nvarchar](20) NULL,
	[APPRV_BY_B] [nvarchar](20) NULL,
	[APPRV_BY_C] [nvarchar](20) NULL,
	[APPRV_BY_D] [nvarchar](20) NULL,
	[APPRV_BY_E] [nvarchar](20) NULL,
	[APPRV_BY_F] [nvarchar](20) NULL,
	[APPRV_BY_G] [nvarchar](20) NULL,
 CONSTRAINT [PK_Drawings] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
