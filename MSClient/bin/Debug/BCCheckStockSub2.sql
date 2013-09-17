
CREATE TABLE [dbo].[BCCheckStockSub2](
	[ROWORDER] [int] IDENTITY(0,1) NOT NULL,
	[ItemCode] [varchar](255) NOT NULL,
	[SerialNo] [varchar] (255) NOT NULL,
	[Qty] [money] NOT NULL,
	[CheckinDate] [datetime] NOT NULL,
	[CheckedDate] [datetime] NULL,
	[CheckinTime] [datetime] NOT NULL,
	[CheckedTime] [datetime] NULL,
	[CheckDocNo] [varchar] (255) NOT NULL,
	[ISCANCEL] [int] NOT NULL
	)