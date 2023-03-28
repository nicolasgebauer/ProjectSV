Create Database BienesRaicesDB
GO

USE [BienesRaicesDB]
GO

CREATE TABLE [dbo].[Person](
	[Rut] VARCHAR(12) PRIMARY KEY,
)
GO

CREATE TABLE [dbo].[Multyproperty] (
    [Comunne] VARCHAR(50),
    [Block] VARCHAR(50),
    [Site] VARCHAR(50),
	[Rut] VARCHAR(12),
	[Percentage] FLOAT ,
    [Page] VARCHAR(50),
    [InscriptionNumber] VARCHAR(50),
    [InscriptionYear] INT ,
    [InscriptionDate] DATE ,
	[StartCurrencyYear] INT ,
	[EndCurrencyYear] INT,
    PRIMARY KEY ([Comunne],[Block],[Site],[Rut])
)
GO

CREATE TABLE [dbo].[Inscription] (
	[AtentionNumber] INT IDENTITY(1,1) ,
	[CNE] VARCHAR(29),
	[Comunne] VARCHAR(50),
    [Block] VARCHAR(50),
    [Site] VARCHAR(50),
	[Page] VARCHAR(50),
	[InscriptionNumber] VARCHAR(50),
    [InscriptionDate] DATE ,
	CONSTRAINT [PK_Inscription] PRIMARY KEY CLUSTERED(
	[AtentionNumber] ASC)
)
GO

CREATE TABLE [dbo].[Alienator](
	[AtentionNumber] INT ,
	[Rut] VARCHAR(12),
	[Percentage] FLOAT ,
	PRIMARY KEY ([AtentionNumber],[Rut]),
	FOREIGN KEY ([Rut]) REFERENCES [dbo].[Person]([Rut]),
	FOREIGN KEY ([AtentionNumber]) REFERENCES [dbo].[Inscription]([AtentionNumber])
)
GO

CREATE TABLE [dbo].[Acquirer](
	[AtentionNumber] INT ,
	[Rut] VARCHAR(12),
	[Percentage] FLOAT ,
	PRIMARY KEY ([AtentionNumber],[Rut]),
	FOREIGN KEY ([Rut]) REFERENCES [dbo].[Person]([Rut]),
	FOREIGN KEY ([AtentionNumber]) REFERENCES [dbo].[Inscription]([AtentionNumber])
)
GO

USE [BienesRaicesDB]
GO


INSERT [dbo].[Person] ([Rut]) VALUES (N'10915348-6')
GO
