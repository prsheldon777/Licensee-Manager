USE [LicenseeManager]
GO
/****** Object:  Table [dbo].[Licensees]    Script Date: 11/11/2025 2:13:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Licensees](
	[LicenseeId] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NULL,
	[LicenseNumber] [nvarchar](50) NOT NULL,
	[LicenseTypeID] [int] NULL,
	[OfficeId] [int] NULL,
	[Status] [int] NOT NULL,
	[IssueDate] [datetime] NULL,
	[ExpirationDate] [datetime] NULL,
	[CreatedAt] [datetime] NULL,
	[UpdatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[LicenseeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LicenseeStatusAudit]    Script Date: 11/11/2025 2:13:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LicenseeStatusAudit](
	[AuditId] [int] IDENTITY(1,1) NOT NULL,
	[LicenseeId] [int] NULL,
	[OldStatus] [int] NULL,
	[NewStatus] [int] NULL,
	[ChangedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[AuditId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LicenseType]    Script Date: 11/11/2025 2:13:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LicenseType](
	[LicenseTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[LicenseTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Offices]    Script Date: 11/11/2025 2:13:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Offices](
	[OfficeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[City] [nvarchar](50) NULL,
	[State] [nvarchar](50) NULL,
	[Active] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OfficeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Licensees] ON 
GO
INSERT [dbo].[Licensees] ([LicenseeId], [FirstName], [LastName], [Email], [LicenseNumber], [LicenseTypeID], [OfficeId], [Status], [IssueDate], [ExpirationDate], [CreatedAt], [UpdatedAt]) VALUES (1, N'Patrick', N'Sheldon', N'psheldon@email.com', N'PS101', 1, 1, 2, CAST(N'2025-11-07T00:00:00.000' AS DateTime), CAST(N'2029-10-18T00:00:00.000' AS DateTime), CAST(N'2025-11-06T00:00:00.000' AS DateTime), CAST(N'2025-11-10T13:16:18.487' AS DateTime))
GO
INSERT [dbo].[Licensees] ([LicenseeId], [FirstName], [LastName], [Email], [LicenseNumber], [LicenseTypeID], [OfficeId], [Status], [IssueDate], [ExpirationDate], [CreatedAt], [UpdatedAt]) VALUES (2, N'Tony', N'Montana', N'tmontana@email.com', N'135AG', 1, 3, 1, CAST(N'2025-11-06T00:00:00.000' AS DateTime), CAST(N'2025-11-27T00:00:00.000' AS DateTime), CAST(N'2025-11-06T00:00:00.000' AS DateTime), CAST(N'2025-11-10T13:16:14.043' AS DateTime))
GO
INSERT [dbo].[Licensees] ([LicenseeId], [FirstName], [LastName], [Email], [LicenseNumber], [LicenseTypeID], [OfficeId], [Status], [IssueDate], [ExpirationDate], [CreatedAt], [UpdatedAt]) VALUES (3, N'John', N'Smith', N'jsmith@fake.email', N'P2501', 1, 1, 3, CAST(N'2025-11-06T00:00:00.000' AS DateTime), CAST(N'2025-11-05T00:00:00.000' AS DateTime), CAST(N'2025-11-06T00:00:00.000' AS DateTime), CAST(N'2025-11-10T13:14:03.813' AS DateTime))
GO
INSERT [dbo].[Licensees] ([LicenseeId], [FirstName], [LastName], [Email], [LicenseNumber], [LicenseTypeID], [OfficeId], [Status], [IssueDate], [ExpirationDate], [CreatedAt], [UpdatedAt]) VALUES (5, N'Chuck', N'Green', N'cgreen@fake.email', N'p3501', 1, 2, 1, CAST(N'2025-11-06T00:00:00.000' AS DateTime), CAST(N'2028-10-03T00:00:00.000' AS DateTime), CAST(N'2025-11-06T00:00:00.000' AS DateTime), CAST(N'2025-11-10T13:14:15.753' AS DateTime))
GO
INSERT [dbo].[Licensees] ([LicenseeId], [FirstName], [LastName], [Email], [LicenseNumber], [LicenseTypeID], [OfficeId], [Status], [IssueDate], [ExpirationDate], [CreatedAt], [UpdatedAt]) VALUES (1003, N'Kerry', N'Winters', N'kwinters@fake.email', N'p4531', 1, 2, 1, CAST(N'2025-11-06T00:00:00.000' AS DateTime), CAST(N'2026-01-29T00:00:00.000' AS DateTime), CAST(N'2025-11-06T00:00:00.000' AS DateTime), CAST(N'2025-11-10T13:14:29.563' AS DateTime))
GO
INSERT [dbo].[Licensees] ([LicenseeId], [FirstName], [LastName], [Email], [LicenseNumber], [LicenseTypeID], [OfficeId], [Status], [IssueDate], [ExpirationDate], [CreatedAt], [UpdatedAt]) VALUES (2003, N'Emily', N'Walters', N'ewalters@fake.email', N'p5', 3, 2, 2, CAST(N'2025-11-07T00:00:00.000' AS DateTime), CAST(N'2025-11-19T00:00:00.000' AS DateTime), CAST(N'2025-11-07T10:27:10.237' AS DateTime), CAST(N'2025-11-10T13:15:09.520' AS DateTime))
GO
INSERT [dbo].[Licensees] ([LicenseeId], [FirstName], [LastName], [Email], [LicenseNumber], [LicenseTypeID], [OfficeId], [Status], [IssueDate], [ExpirationDate], [CreatedAt], [UpdatedAt]) VALUES (2004, N'Andy', N'Anderson', N'aanderson@fake.email', N'p8153', 1, 1, 2, CAST(N'2025-11-08T00:00:00.000' AS DateTime), CAST(N'2028-10-24T00:00:00.000' AS DateTime), CAST(N'2025-11-08T15:29:38.353' AS DateTime), CAST(N'2025-11-10T13:16:34.270' AS DateTime))
GO
INSERT [dbo].[Licensees] ([LicenseeId], [FirstName], [LastName], [Email], [LicenseNumber], [LicenseTypeID], [OfficeId], [Status], [IssueDate], [ExpirationDate], [CreatedAt], [UpdatedAt]) VALUES (2005, N'Walter', N'White', N'walterwhite@email.com', N'31589', 1, 2, 2, CAST(N'2025-11-08T00:00:00.000' AS DateTime), CAST(N'2028-10-08T00:00:00.000' AS DateTime), CAST(N'2025-11-08T15:31:44.070' AS DateTime), CAST(N'2025-11-10T13:16:29.353' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Licensees] OFF
GO
SET IDENTITY_INSERT [dbo].[LicenseeStatusAudit] ON 
GO
INSERT [dbo].[LicenseeStatusAudit] ([AuditId], [LicenseeId], [OldStatus], [NewStatus], [ChangedAt]) VALUES (19, 2, 2, 1, CAST(N'2025-11-08T01:38:44.687' AS DateTime))
GO
INSERT [dbo].[LicenseeStatusAudit] ([AuditId], [LicenseeId], [OldStatus], [NewStatus], [ChangedAt]) VALUES (20, 5, 1, 2, CAST(N'2025-11-08T01:46:09.037' AS DateTime))
GO
INSERT [dbo].[LicenseeStatusAudit] ([AuditId], [LicenseeId], [OldStatus], [NewStatus], [ChangedAt]) VALUES (21, 5, 2, 3, CAST(N'2025-11-08T01:46:12.630' AS DateTime))
GO
INSERT [dbo].[LicenseeStatusAudit] ([AuditId], [LicenseeId], [OldStatus], [NewStatus], [ChangedAt]) VALUES (22, 2003, 2, 3, CAST(N'2025-11-08T01:51:49.743' AS DateTime))
GO
INSERT [dbo].[LicenseeStatusAudit] ([AuditId], [LicenseeId], [OldStatus], [NewStatus], [ChangedAt]) VALUES (23, 5, 3, 2, CAST(N'2025-11-08T01:59:01.390' AS DateTime))
GO
INSERT [dbo].[LicenseeStatusAudit] ([AuditId], [LicenseeId], [OldStatus], [NewStatus], [ChangedAt]) VALUES (24, 5, 2, 3, CAST(N'2025-11-08T01:59:15.417' AS DateTime))
GO
INSERT [dbo].[LicenseeStatusAudit] ([AuditId], [LicenseeId], [OldStatus], [NewStatus], [ChangedAt]) VALUES (25, 2003, 3, 2, CAST(N'2025-11-08T02:01:49.693' AS DateTime))
GO
INSERT [dbo].[LicenseeStatusAudit] ([AuditId], [LicenseeId], [OldStatus], [NewStatus], [ChangedAt]) VALUES (26, 5, 3, 1, CAST(N'2025-11-08T02:02:23.040' AS DateTime))
GO
INSERT [dbo].[LicenseeStatusAudit] ([AuditId], [LicenseeId], [OldStatus], [NewStatus], [ChangedAt]) VALUES (1017, 3, 2, 3, CAST(N'2025-11-08T15:20:20.917' AS DateTime))
GO
INSERT [dbo].[LicenseeStatusAudit] ([AuditId], [LicenseeId], [OldStatus], [NewStatus], [ChangedAt]) VALUES (1018, 3, 3, 2, CAST(N'2025-11-08T15:25:27.077' AS DateTime))
GO
INSERT [dbo].[LicenseeStatusAudit] ([AuditId], [LicenseeId], [OldStatus], [NewStatus], [ChangedAt]) VALUES (1019, 3, 2, 3, CAST(N'2025-11-08T15:25:53.373' AS DateTime))
GO
INSERT [dbo].[LicenseeStatusAudit] ([AuditId], [LicenseeId], [OldStatus], [NewStatus], [ChangedAt]) VALUES (1020, 2004, 1, 2, CAST(N'2025-11-08T15:31:12.760' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[LicenseeStatusAudit] OFF
GO
SET IDENTITY_INSERT [dbo].[LicenseType] ON 
GO
INSERT [dbo].[LicenseType] ([LicenseTypeId], [Name]) VALUES (1, N'Permanent')
GO
INSERT [dbo].[LicenseType] ([LicenseTypeId], [Name]) VALUES (2, N'Temporary')
GO
INSERT [dbo].[LicenseType] ([LicenseTypeId], [Name]) VALUES (3, N'Half Time')
GO
SET IDENTITY_INSERT [dbo].[LicenseType] OFF
GO
SET IDENTITY_INSERT [dbo].[Offices] ON 
GO
INSERT [dbo].[Offices] ([OfficeId], [Name], [City], [State], [Active]) VALUES (1, N'First Office', N'Raleigh', N'NC', 1)
GO
INSERT [dbo].[Offices] ([OfficeId], [Name], [City], [State], [Active]) VALUES (2, N'Burlington Office', N'Burlington', N'NC', 1)
GO
INSERT [dbo].[Offices] ([OfficeId], [Name], [City], [State], [Active]) VALUES (3, N'New York Office', N'New York', N'NY', 1)
GO
INSERT [dbo].[Offices] ([OfficeId], [Name], [City], [State], [Active]) VALUES (1002, N'Ohio Office', N'Sandusky', N'OH', 0)
GO
INSERT [dbo].[Offices] ([OfficeId], [Name], [City], [State], [Active]) VALUES (2002, N'Los Angeles Office', N'Los Angeles ', N'CA', 1)
GO
SET IDENTITY_INSERT [dbo].[Offices] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__tmp_ms_x__E8890166718944AB]    Script Date: 11/11/2025 2:13:43 PM ******/
ALTER TABLE [dbo].[Licensees] ADD UNIQUE NONCLUSTERED 
(
	[LicenseNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Licensees] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Offices] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[Licensees]  WITH CHECK ADD  CONSTRAINT [FK_Licensees_LicenseType] FOREIGN KEY([LicenseTypeID])
REFERENCES [dbo].[LicenseType] ([LicenseTypeId])
ON UPDATE CASCADE
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Licensees] CHECK CONSTRAINT [FK_Licensees_LicenseType]
GO
ALTER TABLE [dbo].[Licensees]  WITH CHECK ADD  CONSTRAINT [FK_Licensees_Offices] FOREIGN KEY([OfficeId])
REFERENCES [dbo].[Offices] ([OfficeId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Licensees] CHECK CONSTRAINT [FK_Licensees_Offices]
GO
ALTER TABLE [dbo].[LicenseeStatusAudit]  WITH CHECK ADD  CONSTRAINT [FK_LicenseeStatusAudit_Licensees] FOREIGN KEY([LicenseeId])
REFERENCES [dbo].[Licensees] ([LicenseeId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[LicenseeStatusAudit] CHECK CONSTRAINT [FK_LicenseeStatusAudit_Licensees]
GO
/****** Object:  StoredProcedure [dbo].[GetExpiredLicensees]    Script Date: 11/11/2025 2:13:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetExpiredLicensees]
	@mode int = 0,
	@DaysAhead int
AS
BEGIN
	if @mode = 0 -- Default mode: Get licensees with expired licenses
	BEGIN
		SELECT *
		FROM Licensees
		WHERE ExpirationDate < GETDATE()
		AND Status != 1 -- Active or Expired licensees only
	END
	ELSE IF @mode = 1 -- Upcoming expirations within specified days
	BEGIN
		SELECT *
		FROM Licensees
		WHERE ExpirationDate BETWEEN GETDATE() AND DATEADD(DAY, @DaysAhead, GETDATE())
		AND Status = 2 -- Active licensees only
	END
END

SELECT * FROM Licensees WHERE ExpirationDate < GETDATE()
GO
/****** Object:  StoredProcedure [dbo].[SetExpired]    Script Date: 11/11/2025 2:13:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SetExpired]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Today DATE = CAST(GETDATE() AS DATE);

    UPDATE l
    SET 
        l.Status = 3,
        l.UpdatedAt = GETDATE()
    FROM dbo.Licensees l
    WHERE 
        l.Status = 2
        AND l.ExpirationDate IS NOT NULL
        AND CAST(l.ExpirationDate AS DATE) < @Today;
END;
RETURN 0
GO
