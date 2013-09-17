CREATE TABLE [dbo].[BCCheckStock](
	[ROWORDER] [int] IDENTITY(0,1) NOT NULL,
	[CusCode] [varchar](255) NOT NULL,
	[CheckDate] [datetime] NOT NULL,
	[CheckTime] [datetime] NOT NULL,
	[CheckDocNo] [varchar] (255) NOT NULL,
	[ISCANCEL] [int] NOT NULL
	)