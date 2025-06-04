USE [ARB]
GO

DELETE [dbo].[StudentSectionErrors]
DROP TABLE [dbo].[StudentSectionErrors]

GO

DELETE [dbo].[StudentSectionErrorsArchive]
DROP TABLE [dbo].[StudentSectionErrorsArchive]

GO

CREATE TABLE [dbo].[StudentSectionErrors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdCourseID] [int] NULL,
	[CourseCode] [nvarchar](100) NULL,
	[SectionCode] [nvarchar](100) NULL,
	[AdClassSchedID] [int] NULL,
	[OldSectionCode] [nvarchar](100) NULL,
	[OldSectionId] [int] NULL,
	[NewSectionId] [int] NULL,
	[StartDate] [datetime] NULL,
	[ProgramStartMonthAndDay] [nvarchar](10) NULL,
	[ProgramStartDay] [int] NULL,
	[ProgramStartMonth] [int] NULL,
	[ProgramLetterInitial] [nvarchar](10) NULL,
	[SyStudentID] [int] NULL,
	[Si_PseudoRegistratonTrackingID] [int] NULL,
	[ForcastedNumberOfStudents] [int] NULL,
	[CVueMaxStudents] [int] NULL,
	[TargetStudentCount] [int] NULL,
	[AdProgramVersionID] [int] NULL,
	[LastAdClassSchedIDTaken] [int] NULL,
	[ScheduleGroupName] [nvarchar](100) NULL,
	[AdEnrollID] [int] NULL,
	[AdEnrollSchedID] [int] NULL,
	[GroupNumber] [uniqueidentifier] NULL,
	[GroupTypeKey] [int] NULL,
	[CounselorLocationCode] [int] NULL,
	[GroupStatusKey] [int] NULL,
	[GroupTargetStudentCount] [int] NULL,
	[GroupMinimumStudentCount] [int] NULL,
	[HasInstructor] [bit] NULL,
	[InstructorAssignedStatusID] [int] NULL,
	[AdTeacherID] [int] NULL,
	[PrimaryInstructor] [nvarchar](100) NULL,
	[FacultyType] [int] NULL,
	[FacultyTypeName] [nvarchar](100) NULL,
	[Term] [nvarchar](100) NULL,
	[Campus] [nvarchar](100) NULL,
	[NewGroupCategory] [int] NULL,
	[IsTheCourseTaughtByFullTimeFaculty] [bit] NULL,
	[SysLmsVendorID] [int] NULL,
	[ProcessID] [uniqueidentifier] NULL,
	[JobID] [int] NULL,
	[StatusID] [int] NULL,
	[OneDayBeforeRunningLive] [bit] NULL,
	[SectionGuid] [uniqueidentifier] NULL,
	[EndDate] [datetime] NULL,
	[LmsExtractStatus] [char](1) NULL,
	[StudentNumber] [nvarchar](100) NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[ErrorMessage] [nvarchar](MAX) NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedOn] [datetime] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	[AdTermID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.StudentSectionErrors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[StudentSectionErrors] ADD  DEFAULT ((0)) FOR [AdTermID]
GO

ALTER TABLE [dbo].[StudentSectionErrors]  WITH CHECK ADD  CONSTRAINT [FK_dbo.StudentSectionErrors_dbo.Jobs_JobID] FOREIGN KEY([JobID])
REFERENCES [dbo].[Jobs] ([Id])
GO

ALTER TABLE [dbo].[StudentSectionErrors] CHECK CONSTRAINT [FK_dbo.StudentSectionErrors_dbo.Jobs_JobID]
GO

ALTER TABLE [dbo].[StudentSectionErrors]  WITH CHECK ADD  CONSTRAINT [FK_dbo.StudentSectionErrors_dbo.Status_StatusID] FOREIGN KEY([StatusID])
REFERENCES [dbo].[Status] ([ID])
GO

ALTER TABLE [dbo].[StudentSectionErrors] CHECK CONSTRAINT [FK_dbo.StudentSectionErrors_dbo.Status_StatusID]
GO

CREATE TABLE [dbo].[StudentSectionErrorsArchive](
	[PK] [int] IDENTITY(1,1) NOT NULL,
	[Id] [int] NOT NULL,
	[AdCourseID] [int] NULL,
	[CourseCode] [nvarchar](100) NULL,
	[SectionCode] [nvarchar](100) NULL,
	[AdClassSchedID] [int] NULL,
	[OldSectionCode] [nvarchar](100) NULL,
	[OldSectionId] [int] NULL,
	[NewSectionId] [int] NULL,
	[StartDate] [datetime] NULL,
	[ProgramStartMonthAndDay] [nvarchar](10) NULL,
	[ProgramStartDay] [int] NULL,
	[ProgramStartMonth] [int] NULL,
	[ProgramLetterInitial] [nvarchar](10) NULL,
	[SyStudentID] [int] NULL,
	[Si_PseudoRegistratonTrackingID] [int] NULL,
	[ForcastedNumberOfStudents] [int] NULL,
	[CVueMaxStudents] [int] NULL,
	[TargetStudentCount] [int] NULL,
	[AdProgramVersionID] [int] NULL,
	[LastAdClassSchedIDTaken] [int] NULL,
	[ScheduleGroupName] [nvarchar](100) NULL,
	[AdEnrollID] [int] NULL,
	[AdEnrollSchedID] [int] NULL,
	[GroupNumber] [uniqueidentifier] NULL,
	[GroupTypeKey] [int] NULL,
	[CounselorLocationCode] [int] NULL,
	[GroupStatusKey] [int] NULL,
	[GroupTargetStudentCount] [int] NULL,
	[GroupMinimumStudentCount] [int] NULL,
	[HasInstructor] [bit] NULL,
	[InstructorAssignedStatusID] [int] NULL,
	[AdTeacherID] [int] NULL,
	[PrimaryInstructor] [nvarchar](100) NULL,
	[FacultyType] [int] NULL,
	[FacultyTypeName] [nvarchar](100) NULL,
	[Term] [nvarchar](100) NULL,
	[Campus] [nvarchar](100) NULL,
	[NewGroupCategory] [int] NULL,
	[IsTheCourseTaughtByFullTimeFaculty] [bit] NULL,
	[SysLmsVendorID] [int] NULL,
	[ProcessID] [uniqueidentifier] NULL,
	[JobID] [int] NULL,
	[StatusID] [int] NULL,
	[OneDayBeforeRunningLive] [bit] NULL,
	[SectionGuid] [uniqueidentifier] NULL,
	[EndDate] [datetime] NULL,
	[LmsExtractStatus] [char](1) NULL,
	[StudentNumber] [nvarchar](100) NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[ErrorMessage] [nvarchar](MAX) NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[UpdatedOn] [datetime] NULL,
	[UpdatedBy] [nvarchar](100) NULL,
	[AdTermID] [int] NOT NULL,
	[AddedOn] [datetime] NULL,
 CONSTRAINT [PK_StudentSectionErrorsArchive] PRIMARY KEY CLUSTERED 
(
	[PK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[StudentSectionErrorsArchive] ADD  CONSTRAINT [DF_StudentSectionErrorsArchive_AddedOn]  DEFAULT (getdate()) FOR [AddedOn]

