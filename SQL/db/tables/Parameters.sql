CREATE TABLE [dbo].[Parameters] (
   [Recipeid] [int] NOT NULL,
   [ID] [int] NOT NULL,
   [ItemName] [varchar](100) NULL,
   [ItemValue] [float] NULL,
   [min] [float] NULL,
   [max] [float] NULL

   ,CONSTRAINT [PK_Parameters] PRIMARY KEY CLUSTERED ([Recipeid], [ID])
)


GO
