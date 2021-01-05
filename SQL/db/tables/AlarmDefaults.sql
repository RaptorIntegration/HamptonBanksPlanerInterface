CREATE TABLE [dbo].[AlarmDefaults] (
   [AlarmID] [int] NOT NULL,
   [Active] [bit] NULL,
   [Category] [varchar](50) NULL,
   [Prefix] [varchar](50) NULL,
   [InfomasterPrefix] [varchar](50) NULL,
   [ColumnName] [varchar](50) NULL

   ,CONSTRAINT [PK_AlarmDefaults] PRIMARY KEY CLUSTERED ([AlarmID])
)


GO
