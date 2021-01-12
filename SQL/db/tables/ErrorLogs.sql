CREATE TABLE [dbo].[ErrorLogs] (
   [Id] [int] NOT NULL
      IDENTITY (1,1),
   [MachineName] [nvarchar](50) NOT NULL,
   [Logged] [datetime] NOT NULL,
   [Level] [nvarchar](50) NOT NULL,
   [Message] [nvarchar](max) NOT NULL,
   [Exception] [nvarchar](max) NULL

   ,CONSTRAINT [PK_dbo.ErrorLogs] PRIMARY KEY CLUSTERED ([Id])
)


GO
