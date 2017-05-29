CREATE TABLE [DbVersion] (
    [Id] 					INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    [DbVersion]     		[INT] 			NOT NULL,    
	[UpdateDate]     		[datetime]		NOT NULL
);
GO

CREATE TABLE [MainOptions] (
    [Id] 					INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    [CompanyName]     		[text] 			NOT NULL,    
	[Permissions]     		[nvarchar](256)	NULL
);
GO

CREATE TABLE [User](
	[Id] 				INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	[Logon] 			[nvarchar](256) UNIQUE NOT NULL,	
	[Firstname] 		[nvarchar](100) NOT NULL,
	[Lastname] 			[nvarchar](100) NOT NULL,	
	[CreationDate] 		[datetime] NOT NULL,		
	[Address] 			[text] NULL,
	[PostalCode] 		[text] NULL,
	[City] 				[nvarchar](30) NULL,
	[Country]			[nvarchar](30) NOT NULL Default 'PT',	
	[Password] 			[nvarchar](256) NOT NULL,
	[Theme] 			[nvarchar](256) NOT NULL default 'dark',
	[Accentcolor] 		[int] NOT NULL default -16741888,
	[Language] 			[nvarchar] (30) NOT NULL default 'PT-pt',	
	[PIN] 				[nvarchar] (10) NOT NULL default '1234',
	[Email] 			[nvarchar](256) NOT NULL default 'mail@server.com' 
);
GO

CREATE TABLE [Customer](
	[Id]           			INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	[Name]         			[text] NOT NULL,
	[CreationDate] 			[datetime] NOT NULL,
	[BirthDate] 			[datetime] NOT NULL,
	[TaxIdNumber]  			[nvarchar](50) UNIQUE NOT NULL,
	[MobilePhone]  			[nvarchar](30) NULL,
	[Telephone]    			[nvarchar](30) NULL,
	[Fax]          			[nvarchar](30) NULL,
	[EMail]        			[nvarchar](255) NULL,
	[URL]          			[text] NULL,
	[Address]      			[text] NULL,
	[PostalCode]   			[text] NULL,
	[City]         			[nvarchar](30) NULL,
	[Country]      			[nvarchar](30) NULL,	
	[IBAN]         			[nvarchar](50) NULL,
	[Contact]      			[nvarchar](255) NULL,
	[DrivingLicense]		[text] NULL,
	[DrivingLicenseType]	[text] NULL,
	[DrivingLicenseDate]	[datetime] NOT NULL,
	[Comments]         		[text] NULL,
	[NextAppointment] 		[datetime] NULL,
	[IntegrationRef]		[nvarchar](100) UNIQUE NULL,
	[IntegrationDate]		[datetime] NULL
);
GO

CREATE TABLE [Doctor](
	[Id]           			INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	[Name]         			[text] NOT NULL,
	[CreationDate] 			[datetime] NOT NULL,
	[TaxIdNumber]  			[nvarchar](50) UNIQUE NOT NULL,
	[MobilePhone]  			[nvarchar](30) NULL,
	[Telephone]    			[nvarchar](30) NULL,
	[Fax]          			[nvarchar](30) NULL,
	[EMail]        			[nvarchar](255) NULL,
	[URL]          			[text] NULL,
	[Address]      			[text] NULL,
	[PostalCode]   			[text] NULL,
	[City]         			[nvarchar](30) NULL,
	[Country]      			[nvarchar](30) NULL,	
	[IBAN]         			[nvarchar](50) NULL,
	[Comments]         		[text] NULL,
	[Contact]      			[nvarchar](255) NULL
);
GO

CREATE TABLE [AppointmentType](
	[Id]           			INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	[Name]         			[text] NOT NULL,
	[CreationDate] 			[datetime] NOT NULL	
);
GO


CREATE TABLE [Appointment](
	[Id]           			INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	[Id_AppointmentType]	[bigint] NOT NULL,
	[Id_Doctor] 			[bigint] NOT NULL,
	[Id_Customer]			[bigint] NOT NULL,
	[Id_User]				[bigint] NOT NULL,	
	[Number]     			[integer] NOT NULL default 0,	
	[CreationDate] 			[datetime] NOT NULL,
	[AppointmentDate] 		[datetime] NOT NULL,	
	[Description]  			[text] NOT NULL default (''),		
	[ClientDiscount]		[decimal](4,2) NOT NULL Default 0,	
	[FinanceDiscount]		[decimal](12,4) NOT NULL Default 0,	
	[PaymentType]			[nvarchar](100) NOT NULL Default 'PP',	
	[Coin]					[nvarchar](32) NOT NULL Default 'EUR',	
	[Exchange]				[decimal](12,4) NOT NULL Default 1,	
	[TotalProducts]			[decimal](12,4) NOT NULL Default 0,	
	[VATValue]				[decimal](12,4) NOT NULL Default 0,	
	[ComercialDiscountVal]	[decimal](12,4) NOT NULL Default 0,	
	[Report] 				[text] NOT NULL default ('NoReport.rpt'),
	[Comments]  			[text] NOT NULL default (''),
 FOREIGN KEY ([Id_AppointmentType]) 	REFERENCES [AppointmentType] ([Id]) ON UPDATE  NO ACTION ON DELETE  NO ACTION,
 FOREIGN KEY ([Id_Doctor]) 				REFERENCES [Doctor] ([Id]) ON UPDATE  NO ACTION ON DELETE  NO ACTION,
 FOREIGN KEY ([Id_Customer]) 			REFERENCES [Customer] ([Id]) ON UPDATE  NO ACTION ON DELETE  NO ACTION,
 FOREIGN KEY ([Id_User]) 				REFERENCES [User] ([Id]) ON UPDATE  NO ACTION ON DELETE  NO ACTION
);
GO
