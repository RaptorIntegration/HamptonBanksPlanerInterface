CREATE TABLE [dbo].[Audit] (
   [id] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [UserName] [varchar](50) NULL,
   [KeyName] [varchar](50) NULL,
   [TableName] [varchar](50) NULL,
   [TableKey] [varchar](50) NULL,
   [Col] [varchar](50) NULL,
   [Val] [varchar](50) NULL,
   [Prev] [varchar](50) NULL,
   [Deleted] [bit] NULL,
   [Added] [bit] NULL

   ,CONSTRAINT [PK_Audit] PRIMARY KEY CLUSTERED ([id])
)


GO
