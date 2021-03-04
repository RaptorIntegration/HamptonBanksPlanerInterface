CREATE TABLE [dbo].[BoardsSavedDetails] (
   [NumberOfBoards] [smallint] NOT NULL
      CONSTRAINT [DF_BoardsSavedDetails_NumberOfBoards] DEFAULT ((0)),
   [Folder] [varchar](100) NOT NULL
      CONSTRAINT [DF_BoardsSavedDetails_Folder] DEFAULT ('')
)


GO
