USE [SchoolBus_Data]
GO
/****** Object:  User [schoolbus_Database]    Script Date: 2018/8/24 0:29:01 ******/
CREATE USER [schoolbus_Database] FOR LOGIN [schoolbus_Database] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [schoolbus_Database]
GO
/****** Object:  Table [dbo].[AllUsersTable]    Script Date: 2018/8/24 0:29:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AllUsersTable](
	[objectId] [nchar](10) NOT NULL,
	[Username] [nvarchar](64) NOT NULL,
	[Password] [nvarchar](512) NULL,
	[Sex] [char](1) NOT NULL,
	[isAdmin] [bit] NULL,
	[isClassTeacher] [bit] NOT NULL,
	[isBusTeacher] [bit] NOT NULL,
	[isParent] [bit] NOT NULL,
	[RealName] [nvarchar](32) NOT NULL,
	[HeadImage] [nvarchar](32) NULL,
	[PhoneNumber] [nvarchar](13) NOT NULL,
	[ClassIDs] [text] NULL,
	[ChildIDs] [text] NULL,
	[longitude] [float] NOT NULL,
	[latitude] [float] NOT NULL,
	[precision] [float] NOT NULL,
	[createdAt] [datetime] NOT NULL,
	[updatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_AllUsersTable] PRIMARY KEY CLUSTERED 
(
	[objectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ__AllUsers__536C85E4E71DA4C9] UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Classes]    Script Date: 2018/8/24 0:29:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Classes](
	[objectId] [nchar](10) NOT NULL,
	[ClassDepartment] [text] NULL,
	[ClassGrade] [text] NULL,
	[ClassNumber] [text] NULL,
	[TeacherID] [nchar](10) NULL,
	[createdAt] [datetime] NOT NULL,
	[updatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_Classes] PRIMARY KEY CLUSTERED 
(
	[objectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GeneralData]    Script Date: 2018/8/24 0:29:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GeneralData](
	[objectId] [nchar](10) NOT NULL,
	[Name] [text] NULL,
	[NTitle] [text] NULL,
	[OtherData] [text] NULL,
	[createdAt] [datetime] NOT NULL,
	[updatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_GeneralData] PRIMARY KEY CLUSTERED 
(
	[objectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notifications]    Script Date: 2018/8/24 0:29:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notifications](
	[objectId] [nchar](10) NOT NULL,
	[type] [int] NOT NULL,
	[Sender] [nchar](10) NULL,
	[Receiver] [text] NULL,
	[Content] [text] NULL,
	[Title] [text] NULL,
	[createdAt] [datetime] NOT NULL,
	[updatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED 
(
	[objectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SchoolBuses]    Script Date: 2018/8/24 0:29:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SchoolBuses](
	[objectId] [nchar](10) NOT NULL,
	[BusName] [text] NULL,
	[TeacherObjectID] [nchar](10) NULL,
	[CSChecked] [bit] NULL,
	[LSChecked] [bit] NULL,
	[AHChecked] [bit] NULL,
	[createdAt] [datetime] NOT NULL,
	[updatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_SchoolBuses] PRIMARY KEY CLUSTERED 
(
	[objectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudentsData]    Script Date: 2018/8/24 0:29:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudentsData](
	[objectId] [nchar](10) NOT NULL,
	[StuName] [text] NOT NULL,
	[Sex] [char](1) NOT NULL,
	[ClassID] [nchar](10) NOT NULL,
	[BusID] [nchar](10) NOT NULL,
	[LSChecked] [bit] NOT NULL,
	[CSChecked] [bit] NOT NULL,
	[CHChecked] [bit] NOT NULL,
	[createdAt] [datetime] NOT NULL,
	[updatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_StudentsData] PRIMARY KEY CLUSTERED 
(
	[objectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserQuestions]    Script Date: 2018/8/24 0:29:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserQuestions](
	[objectId] [nchar](10) NOT NULL,
	[Title] [text] NULL,
	[Content] [text] NULL,
	[createdAt] [datetime] NOT NULL,
	[updatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_UserQuestions] PRIMARY KEY CLUSTERED 
(
	[objectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRequest]    Script Date: 2018/8/24 0:29:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRequest](
	[objectId] [nchar](10) NOT NULL,
	[UserID] [nchar](10) NULL,
	[Status] [int] NULL,
	[SolverID] [nchar](10) NULL,
	[ResultReason] [int] NULL,
	[NewContent] [text] NULL,
	[RequestType] [int] NULL,
	[DetailTexts] [text] NULL,
	[createdAt] [datetime] NOT NULL,
	[updatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_UserRequest] PRIMARY KEY CLUSTERED 
(
	[objectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WeeklyIssues]    Script Date: 2018/8/24 0:29:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WeeklyIssues](
	[objectId] [nchar](10) NOT NULL,
	[ReportTeacherID] [nchar](10) NOT NULL,
	[ReportBusID] [nchar](10) NOT NULL,
	[DetailedInformation] [text] NOT NULL,
	[ReportType] [int] NOT NULL,
	[createdAt] [datetime] NOT NULL,
	[updatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_WeeklyIssues] PRIMARY KEY CLUSTERED 
(
	[objectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[AllUsersTable] ADD  CONSTRAINT [DF_AllUsersTable_isAdmin]  DEFAULT ((0)) FOR [isAdmin]
GO

/*ALTER TABLE [dbo].[Classes]  WITH CHECK ADD  CONSTRAINT [FK_Classes_AllUsersTable] FOREIGN KEY([TeacherID])
REFERENCES [dbo].[AllUsersTable] ([objectId])
GO
ALTER TABLE [dbo].[Classes] CHECK CONSTRAINT [FK_Classes_AllUsersTable]
GO*/
/****** Object:  StoredProcedure [dbo].[sp_DeleteAllData]    Script Date: 2018/8/24 0:29:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_DeleteAllData] AS 
EXEC sp_MSForEachTable 'DELETE FROM ?'
GO
CREATE PROCEDURE [dbo].[sp_QueryAllData] AS 
EXEC sp_MSForEachTable 'SELECT * FROM ?'
GO
