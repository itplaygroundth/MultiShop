CREATE TABLE [dbo].[BCCheckStockSub1](
	[ROWORDER] [int] IDENTITY(0,1) NOT NULL,
	[ItemCode] [varchar](255) NOT NULL,
	[ItemName] [varchar](255) NOT NULL,
	[UnitCode] [varchar](255) NOT NULL,
	[StockQty] [money] NOT NULL,
	[CheckDate] [datetime] NOT NULL,
	[CheckTime] [datetime] NOT NULL,
	[CheckDocNo] [varchar] (255) NOT NULL,
	[ISCANCEL] [int] NOT NULL
	)