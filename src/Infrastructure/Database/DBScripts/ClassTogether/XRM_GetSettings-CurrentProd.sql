USE [ARB]
GO

/****** Object:  StoredProcedure [dbo].[XRM_GetSettings]    Script Date: 7/8/2024 12:51:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO


ALTER PROCEDURE [dbo].[XRM_GetSettings]
	@ProcessID	UNIQUEIDENTIFIER 
AS 

    SET NOCOUNT ON

	TRUNCATE TABLE [dbo].[IMPORT_AdClassSched];
	TRUNCATE TABLE [dbo].[IMPORT_AdEnrollSched]
	TRUNCATE TABLE [dbo].[CRM_SchedulingGroup];
	TRUNCATE TABLE [dbo].[PreFilteredXrmCourse]
	TRUNCATE TABLE [dbo].[PseudoRegistraton]
	TRUNCATE TABLE [dbo].[PreLoadStudentSections] 
	
			
		 
       DECLARE		@CreatedOn								DATETIME,
                    @ModifiedOn								DATETIME,
                    @setting_statecode						INT,
                    @seeting_statuscode						INT,
                    @gcu_CourseRosterBalanceSettingCode		NVARCHAR(100),--UNIQUEIDENTIFIER,
                    @gcu_EarliestClassStartDate				DATETIME,
                    @gcu_LatestClassStartDate				DATETIME,
                    @gcu_TargetStudentCount					INT,
                    @gcu_MaxStudentCount					INT,
                    @gcu_MinStudentCount					INT,
                    @gcu_RosterBalanceDaysInAdvance			INT,
                    @gcu_GlobalRosterBalanceSettingId		UNIQUEIDENTIFIER,
					@CurrentDateTime						DATETIME = GETDATE(),
					@gcu_ReuseEmptySections					BIT;
			IF(@ProcessID IS NULL)
			BEGIN
				SET @ProcessID	= NEWID();
			END

			INSERT INTO [dbo].[ImportProcess]
			   (
				    [ProcessID]
				   ,[StartDate]
				   ,[HasError]
				   ,[Remarks]
				   ,[HasWarning]
			   )
				VALUES
			   (
					@ProcessID
				   ,@CurrentDateTime
				   ,0
				   ,'Process start~'
				   ,0
			   );

             SELECT TOP 1
					   @gcu_GlobalRosterBalanceSettingId = [gcu_GlobalRosterBalanceSettingId]
                      ,@CreatedOn = [CreatedOn]
                      ,@ModifiedOn = [ModifiedOn]
                      ,@setting_statecode = [statecode]
                      ,@seeting_statuscode = [statuscode]
                      ,@gcu_CourseRosterBalanceSettingCode = [gcu_GlobalRosterBalanceSettingCode]
                      ,@gcu_EarliestClassStartDate = [gcu_EarliestClassStartDate]
                      ,@gcu_LatestClassStartDate = [gcu_LatestClassStartDate]
                      ,@gcu_TargetStudentCount = [gcu_TargetStudentCount]
                      ,@gcu_MaxStudentCount = [gcu_MaxStudentCount]
                      ,@gcu_MinStudentCount = [gcu_MinStudentCount]
                      ,@gcu_RosterBalanceDaysInAdvance = [gcu_RosterBalanceDaysInAdvance]
					  ,@gcu_ReuseEmptySections = [gcu_ReuseEmptySections]
             FROM [dbo].[XrmGlobalCourseSettings] 
			 WHERE 
				[statecode] = 0  
					AND [statuscode] = 1;
			
			IF(@@ROWCOUNT = 0)
			BEGIN
				UPDATE [dbo].[ImportProcess]
					SET
						[HasWarning] = 1,
						[Remarks] = [Remarks] + 'Zero XRM global settings retrieved~'
					WHERE ProcessID = @ProcessID;
			END

             INSERT INTO [dbo].[PreFilteredXrmCourse]
				   (
						[gcu_CourseId]
					   ,[gcu_CampusVueNumber]
					   ,[gcu_IsRosterBalanced]
					   ,[statecode]
					   ,[statuscode]
					   ,[Id]
					   ,[gcu_CourseRosterBalanceSettingId]
					   ,[CreatedOn]
					   ,[ModifiedOn]
					   ,[setting_statecode]
					   ,[setting_statuscode]
					   ,[gcu_CourseRosterBalanceSettingCode]
					   ,[gcu_EarliestClassStartDate]
					   ,[gcu_LatestClassStartDate]
					   ,[gcu_TargetStudentCountOverride]
					   ,[gcu_MaxStudentCountOverride]
					   ,[gcu_MinStudentCountOverride]
					   ,[gcu_RosterBalanceDaysInAdvanceOverride]
					   ,[SettingSourceTable]
					   ,ProcessID
					   ,[gcu_ReuseEmptySections]
				   )
             SELECT c.[gcu_CourseId]
                      ,c.[gcu_CampusVueNumber]
                      ,c.[gcu_IsRosterBalanced]
                      ,c.[statecode]
                      ,c.[statuscode]
                      ,c.[Id]
                      ,s.[gcu_CourseRosterBalanceSettingId]
                      ,s.[CreatedOn]
                      ,s.[ModifiedOn]
                      ,s.[statecode] as setting_statecode
                      ,s.[statuscode] as setting_statuscode
                      ,s.[gcu_CourseRosterBalanceSettingCode]
                      ,s.[gcu_EarliestClassStartDate]
                      ,s.[gcu_LatestClassStartDate]
                      ,s.[gcu_TargetStudentCountOverride]
                      ,s.[gcu_MaxStudentCountOverride]
                      ,s.[gcu_MinStudentCountOverride]
                      ,s.[gcu_RosterBalanceDaysInAdvanceOverride]
                      ,'XrmCourseSettings' AS SettingSourceTable
					  ,@ProcessID
					  ,s.[gcu_ReuseEmptySections]
               FROM [dbo].[XrmCourses] c 
					INNER JOIN [dbo].[XrmCourseSettings] s ON c.gcu_CourseId = s.gcu_CourseID AND s.[statecode] = 0 AND s.[statuscode] = 1
						WHERE 
							c.[statecode] = 0 
								AND s.[statuscode] = 1 
									AND c.[gcu_IsRosterBalanced] = 1;

          INSERT INTO [dbo].[PreFilteredXrmCourse]
				   (
						[gcu_CourseId]
					   ,[gcu_CampusVueNumber]
					   ,[gcu_IsRosterBalanced]
					   ,[statecode]
					   ,[statuscode]
					   ,[Id]
					   ,[gcu_CourseRosterBalanceSettingId]
					   ,[CreatedOn]
					   ,[ModifiedOn]
					   ,[setting_statecode]
					   ,[setting_statuscode]
					   ,[gcu_CourseRosterBalanceSettingCode]
					   ,[gcu_EarliestClassStartDate]
					   ,[gcu_LatestClassStartDate]
					   ,[gcu_TargetStudentCountOverride]
					   ,[gcu_MaxStudentCountOverride]
					   ,[gcu_MinStudentCountOverride]
					   ,[gcu_RosterBalanceDaysInAdvanceOverride]
					   ,[SettingSourceTable]
					   ,ProcessID
					   ,[gcu_ReuseEmptySections]
				   )
               SELECT c.[gcu_CourseId]
                      ,c.[gcu_CampusVueNumber]
                      ,c.[gcu_IsRosterBalanced]
                      ,c.[statecode]
                      ,c.[statuscode]
                      ,c.[Id]
                      ,@gcu_GlobalRosterBalanceSettingId as gcu_GlobalRosterBalanceSettingId
                      ,@CreatedOn as CreatedOn
                      ,@ModifiedOn as ModifiedOn
                      ,@setting_statecode as setting_statecode
                      ,@seeting_statuscode as seeting_statuscode
                      ,@gcu_CourseRosterBalanceSettingCode as gcu_CourseRosterBalanceSettingCode
                      ,@gcu_EarliestClassStartDate as gcu_EarliestClassStartDate
                      ,@gcu_LatestClassStartDate as gcu_LatestClassStartDate
                      ,@gcu_TargetStudentCount as gcu_TargetStudentCountOverride
                      ,@gcu_MaxStudentCount as gcu_MaxStudentCountOverride
                      ,@gcu_MinStudentCount as gcu_MinStudentCountOverride
                      ,@gcu_RosterBalanceDaysInAdvance as gcu_RosterBalanceDaysInAdvanceOverride
                      ,'XrmGlobalCourseSettings' AS SettingSourceTable
					  ,@ProcessID
					  ,@gcu_ReuseEmptySections
               FROM [dbo].[XrmCourses] c 
					WHERE NOT EXISTS (select [gcu_CourseId] from [dbo].[PreFilteredXrmCourse] Y WHERE Y.gcu_CourseId = C.[gcu_CourseId])
							AND c.[gcu_IsRosterBalanced] = 1  
								AND	c.[statecode] = 0  
									AND c.[statuscode] = 1; 


		INSERT INTO [dbo].[IMPORT_AdClassSched]
			   (
					[AdClassSchedID]
				   ,[Code]
				   ,[Section]
				   ,[Descrip]
				   ,[AdCourseID]
				   ,[SyCampusID]
				   ,[MaxStudents]
				   ,[RegStudents]
				   ,[AdTeacherID]
				   ,[AttendanceType]
				   ,[LDA]
				   ,[NextDateAtt]
				   ,[StartDate]
				   ,[EndDate]
				   ,[ModFlag]
				   ,[Active]
				   ,[UserID]
				   ,[DateAdded]
				   ,[DateLstMod]
				   ,[SchedComment]
				   ,[UnschedDays]
				   ,[DropAbsentPct]
				   ,[DropConsAbsent]
				   ,[DropCumAbsent]
				   ,[WarnAbsentPct]
				   ,[WarnConsAbsent]
				   ,[WarnCumAbsent]
				   ,[MakeUpMaxType]
				   ,[MakeUpMaxNum]
				   ,[WaitlistExpiredOrCleared]
				   ,[StartTime]
				   ,[DaysFlag]
				   ,[SyllabusDocument]
				   ,[FinalRegStudents]
				   ,[FinalRegDropStudents]
				   ,[AllowWaitListing]
				   ,[WaitListMaxNumOfSeats]
				   ,[AllowReservations]
				   ,[adDeliveryMethodID]
				   ,[HideFaculty]
				   ,[HideLocation]
				   ,[BlindGrading]
				   ,[SendToLMS]
				   ,[AuditAdvisementRequired]
				   ,[AdShiftID]
				   ,[PassFailSetting]
				   ,[DefaultMeetingLengthStudentSpecific]
				   ,[AllowStudentSpecificMeeting]
				   ,[AddedAdClassSchedId]
				   ,[ThresholdType]
				   ,[ThresholdNumOfSeats]
				   ,[SyLmsVendorID]
				   ,[LmsExtractStatus]
				   ,[ClassSchedLength]
				   ,[MaxSections]
				   ,[AutoAdded]
				   ,[EnrollStatusCredits]
				   ,[EnrollStatusHours]
				   ,[AddDropDate]
				   ,[AddDropManual]
				   ,[LDWDate]
				   ,[LDWManual]
				   ,[EnforceAttendanceLDW]
				   ,[AutoDropWarningForLDW]
				   ,[Credits]
				   ,[Hours]
				   ,[AllowOverride]
				   ,[MinCredits]
				   ,[MaxCredits]
				   ,[ProcessID]
				 
			   )
			SELECT 
				   DISTINCT cv.[AdClassSchedID] 
				  ,cv.[Code]
				  ,cv.[Section]
				  ,cv.[Descrip]
				  ,cv.[AdCourseID]
				  ,cv.[SyCampusID]
				  ,cv.[MaxStudents]
				  ,cv.[RegStudents]
				  ,cv.[AdTeacherID]
				  ,cv.[AttendanceType]
				  ,cv.[LDA]
				  ,cv.[NextDateAtt]
				  ,cv.[StartDate]
				  ,cv.[EndDate]
				  ,cv.[ModFlag]
				  ,cv.[Active]
				  ,cv.[UserID]
				  ,cv.[DateAdded]
				  ,cv.[DateLstMod]
				  --,cv.[AdClassSchedID]
				  ,cv.[SchedComment]
				  ,cv.[UnschedDays]
				  ,cv.[DropAbsentPct]
				  ,cv.[DropConsAbsent]
				  ,cv.[DropCumAbsent]
				  ,cv.[WarnAbsentPct]
				  ,cv.[WarnConsAbsent]
				  ,cv.[WarnCumAbsent]
				  ,cv.[MakeUpMaxType]
				  ,cv.[MakeUpMaxNum]
				  ,cv.[WaitlistExpiredOrCleared]
				  ,cv.[StartTime]
				  ,cv.[DaysFlag]
				  ,cv.[SyllabusDocument]
				  ,cv.[FinalRegStudents]
				  ,cv.[FinalRegDropStudents]
				  ,cv.[AllowWaitListing]
				  ,cv.[WaitListMaxNumOfSeats]
				  ,cv.[AllowReservations]
				  ,cv.[adDeliveryMethodID]
				  ,cv.[HideFaculty]
				  ,cv.[HideLocation]
				  ,cv.[BlindGrading]
				  ,cv.[SendToLMS]
				  ,cv.[AuditAdvisementRequired]
				  ,cv.[AdShiftID]
				  ,cv.[PassFailSetting]
				  ,cv.[DefaultMeetingLengthStudentSpecific]
				  ,cv.[AllowStudentSpecificMeeting]
				  ,cv.[AddedAdClassSchedId]
				  ,cv.[ThresholdType]
				  ,cv.[ThresholdNumOfSeats]
				  ,cv.[SyLmsVendorID]
				  ,cv.[LmsExtractStatus]
				  ,cv.[ClassSchedLength]
				  ,cv.[MaxSections]
				  ,cv.[AutoAdded]
				  ,cv.[EnrollStatusCredits]
				  ,cv.[EnrollStatusHours]
				  ,cv.[AddDropDate]
				  ,cv.[AddDropManual]
				  ,cv.[LDWDate]
				  ,cv.[LDWManual]
				  ,cv.[EnforceAttendanceLDW]
				  ,cv.[AutoDropWarningForLDW]
				  ,cv.[Credits]
				  ,cv.[Hours]
				  ,cv.[AllowOverride]
				  ,cv.[MinCredits]
				  ,cv.[MaxCredits]
				  ,@ProcessID
				
			   FROM [CAMPUSVUE].[dbo].[AdClassSched] AS cv WITH (NOLOCK)
					INNER JOIN [dbo].[PreFilteredXrmCourse] AS xrm  WITH (NOLOCK) 
						ON xrm.[gcu_CampusVueNumber] = cv.[AdCourseID] and cv.[adDeliveryMethodID] IN (1)
							AND cv.[StartDate] BETWEEN @CurrentDateTime	
								AND DATEADD(d, xrm.[gcu_RosterBalanceDaysInAdvanceOverride], @CurrentDateTime) 
									AND cv.SyCampusID = 6 AND cv.Active = 1
				
	UPDATE [dbo].[CourseSections]
			SET
				[Active] = 0,
				[UpdatedOn] = GETDATE(),
				[UpdatedBy] = 'ARB Loader'
		FROM [dbo].[CourseSections] CS
			INNER JOIN [CAMPUSVUE].[dbo].[AdClassSched] AC ON CS.AdClassSchedId = AC.AdClassSchedId
				WHERE AC.Active = 0;

		--remove all sections with code 'OL%'
		DELETE FROM [dbo].[IMPORT_AdClassSched]	WHERE UPPER([Section])	LIKE 'OL%';
		
		--remove any sections previously created by ARB in CampusVue in case the Live Job has to run again.
		delete from [dbo].[IMPORT_AdClassSched]	where UPPER([Section])	LIKE 'EO%';
		delete from [dbo].[IMPORT_AdClassSched]	where UPPER([Section])	LIKE 'IO%';
		delete from [dbo].[IMPORT_AdClassSched]	where UPPER([Section])	LIKE 'O5%';
		delete from [dbo].[IMPORT_AdClassSched]	where UPPER([Section])	LIKE 'O6%';
		delete from [dbo].[IMPORT_AdClassSched]	where UPPER([Section])	LIKE 'O7%';
		delete from [dbo].[IMPORT_AdClassSched]	where UPPER([Section])	LIKE 'O8%';
		delete from [dbo].[IMPORT_AdClassSched]	where UPPER([Section])	LIKE 'P5%';
		delete from [dbo].[IMPORT_AdClassSched]	where UPPER([Section])	LIKE 'XO%';
		delete from [dbo].[IMPORT_AdClassSched]	where UPPER([Section])	LIKE 'XC%';
					
		UPDATE [dbo].[IMPORT_AdClassSched]
		SET
			 [gcu_FacultyType] = CDM.gcu_FacultyType
			,[gcu_FacultyTypeName] = CDM.gcu_FacultyTypeName
		FROM 
		[dbo].[IMPORT_AdClassSched] CS 
			INNER JOIN [dbo].[XrmCourses] C ON CS.AdCourseID = C.gcu_CampusVueNumber
				INNER JOIN [dbo].[XrmCourseDeliveryMethods] CDM  ON CDM.gcu_CourseCodeLKP = C.[gcu_CourseId]
					AND CDM.[gcu_DeliveryMethodLKPName] = 'ONLINE'

	
			 

	INSERT INTO [dbo].[IMPORT_AdEnrollSched]
           ([SyStudentID]
           ,[AdEnrollID]
           ,[AdClassSchedID]
           ,[AdCourseID]
           ,[AdTermID]
           ,[AcademicYear]
           ,[Descrip]
           ,[StartDate]
           ,[ExpectedEndDate]
           ,[EndDate]
           ,[LDA]
           ,[DateGradePosted]
           ,[DropDate]
           ,[DateBilled]
           ,[Extended]
           ,[ExtensionBilled]
           ,[Status]
           ,[PreviousStatus]
           ,[Comments]
           ,[TranscriptComment]
           ,[Cost]
           ,[MinutesAttended]
           ,[MinutesAbsent]
           ,[CostScheduled]
           ,[Credits]
           ,[Hours]
           ,[NumericGrade]
           ,[AdGradeLetterCode]
           ,[RetakeFlag]
           ,[Speed]
           ,[Points]
           ,[QualityPoints]
           ,[CreditsAttempt]
           ,[CreditsEarned]
           ,[HoursAttempt]
           ,[HoursEarned]
           ,[GPACalc]
           ,[PayStatus]
           ,[ModFlag]
           ,[UserID]
           ,[DateAdded]
           ,[DateLstMod]
           ,[AdEnrollSchedID]
           ,[AdProgramCourseID]
           ,[AmCollegeID]
           ,[Substitute]
           ,[SubstituteAdCourseID]
           ,[TransferCredit]
           ,[AdDependentClassSchedID]
           ,[AutoDropFlag]
           ,[ConsecutiveMinutesAbsent]
           ,[RosterFlag]
           ,[RevenueRefNo]
           ,[Include]
           ,[RetakeOverride]
           ,[AdGradeScaleID]
           ,[MinutesMakeUp]
           ,[DateReqMet]
           ,[TransferredCourseCode]
           ,[TransferredCourseDescrip]
           ,[AdEnrollRegistrationID]
           ,[DateAdvised]
           ,[AfterAddDrop]
           ,[AmCollegeTransferID]
           ,[IsCrossRef]
           ,[CrossRefAdCourseID]
           ,[MidTermNumericGrade]
           ,[MidTermGradeLetterCode]
           ,[MidTermGradeComments]
           ,[DateMidTermGradePosted]
           ,[IsAudit]
           ,[AuditEffectiveDate]
           ,[IsPassFail]
           ,[LmsExtractStatus]
           ,[InProgressGrade]
           ,[AdCourseFeeSchedID]
           ,[EnrollStatusCredits]
           ,[EnrollStatusHours]
           ,[FaStudentAyPaymentPeriodId]
           ,[RetakeFeeWaived]
           ,[CourseRefundPolicyUsed]
           ,[ExpDeadlineDate]
           ,[ReplacedIncompleteGrade]
           ,[AmHighSchoolID]
           ,[AmTransferTypeID]
           ,[RetakeTIV2ndCredEarnZeroed]
           ,[RetakeTIV2ndCredEarnValue]
           ,[UseOnlyRateSchedule]
           ,[OverrideCourseProgression]
           ,[StuNum]
			,[FirstName]
			,[LastName]
           ,[ProcessID],
		   [LastAdClassSchedIDTaken]
		   ,adProgramVersionID)
	SELECT cv_es.[SyStudentID]
		  ,cv_es.[AdEnrollID]
		  ,cv_es.[AdClassSchedID]
		  ,cv_es.[AdCourseID]
		  ,cv_es.[AdTermID]
		  ,cv_es.[AcademicYear]
		  ,cv_es.[Descrip]
		  ,cv_es.[StartDate]
		  ,cv_es.[ExpectedEndDate]
		  ,cv_es.[EndDate]
		  ,cv_es.[LDA]
		  ,cv_es.[DateGradePosted]
		  ,cv_es.[DropDate]
		  ,cv_es.[DateBilled]
		  ,cv_es.[Extended]
		  ,cv_es.[ExtensionBilled]
		  ,cv_es.[Status]
		  ,cv_es.[PreviousStatus]
		  ,cv_es.[Comments]
		  ,cv_es.[TranscriptComment]
		  ,cv_es.[Cost]
		  ,cv_es.[MinutesAttended]
		  ,cv_es.[MinutesAbsent]
		  ,cv_es.[CostScheduled]
		  ,cv_es.[Credits]
		  ,cv_es.[Hours]
		  ,cv_es.[NumericGrade]
		  ,cv_es.[AdGradeLetterCode]
		  ,cv_es.[RetakeFlag]
		  ,cv_es.[Speed]
		  ,cv_es.[Points]
		  ,cv_es.[QualityPoints]
		  ,cv_es.[CreditsAttempt]
		  ,cv_es.[CreditsEarned]
		  ,cv_es.[HoursAttempt]
		  ,cv_es.[HoursEarned]
		  ,cv_es.[GPACalc]
		  ,cv_es.[PayStatus]
		  ,cv_es.[ModFlag]
		  ,cv_es.[UserID]
		  ,cv_es.[DateAdded]
		  ,cv_es.[DateLstMod]
		  ,cv_es.[AdEnrollSchedID]
		  ,cv_es.[AdProgramCourseID]
		  ,cv_es.[AmCollegeID]
		  ,cv_es.[Substitute]
		  ,cv_es.[SubstituteAdCourseID]
		  ,cv_es.[TransferCredit]
		  ,cv_es.[AdDependentClassSchedID]
		  ,cv_es.[AutoDropFlag]
		  ,cv_es.[ConsecutiveMinutesAbsent]
		  ,cv_es.[RosterFlag]
		  ,cv_es.[RevenueRefNo]
		  ,cv_es.[Include]
		  ,cv_es.[RetakeOverride]
		  ,cv_es.[AdGradeScaleID]
		  ,cv_es.[MinutesMakeUp]
		  ,cv_es.[DateReqMet]
		  ,cv_es.[TransferredCourseCode]
		  ,cv_es.[TransferredCourseDescrip]
		  ,cv_es.[AdEnrollRegistrationID]
		  ,cv_es.[DateAdvised]
		  ,cv_es.[AfterAddDrop]
		  ,cv_es.[AmCollegeTransferID]
		  ,cv_es.[IsCrossRef]
		  ,cv_es.[CrossRefAdCourseID]
		  ,cv_es.[MidTermNumericGrade]
		  ,cv_es.[MidTermGradeLetterCode]
		  ,cv_es.[MidTermGradeComments]
		  ,cv_es.[DateMidTermGradePosted]
		  ,cv_es.[IsAudit]
		  ,cv_es.[AuditEffectiveDate]
		  ,cv_es.[IsPassFail]
		  ,cv_es.[LmsExtractStatus]
		  ,cv_es.[InProgressGrade]
		  ,cv_es.[AdCourseFeeSchedID]
		  ,cv_es.[EnrollStatusCredits]
		  ,cv_es.[EnrollStatusHours]
		  ,cv_es.[FaStudentAyPaymentPeriodId]
		  ,cv_es.[RetakeFeeWaived]
		  ,cv_es.[CourseRefundPolicyUsed]
		  ,cv_es.[ExpDeadlineDate]
		  ,cv_es.[ReplacedIncompleteGrade]
		  ,cv_es.[AmHighSchoolID]
		  ,cv_es.[AmTransferTypeID]
		  ,cv_es.[RetakeTIV2ndCredEarnZeroed]
		  ,cv_es.[RetakeTIV2ndCredEarnValue]
		  ,cv_es.[UseOnlyRateSchedule]
		  ,cv_es.[OverrideCourseProgression]
		   ,cv_s.[StuNum]
          ,cv_s.[FirstName]
		  ,cv_s.[LastName]
		  ,@ProcessID
		   ,NULL AS LastAdClassSchedIDTaken
		   ,AE.adProgramVersionID
	  FROM [CAMPUSVUE].[dbo].[AdEnrollSched] as cv_es WITH (NOLOCK)
		INNER JOIN [IMPORT_AdClassSched] cs on cs.AdClassSchedID = cv_es.[AdClassSchedID]
			LEFT JOIN [CAMPUSVUE].[dbo].[SyStudent] cv_s WITH (NOLOCK) ON cv_es.[SyStudentID] = cv_s.[SyStudentID]
				LEFT JOIN  [CAMPUSVUE].[dbo].AdEnroll ae  WITH (NOLOCK) ON ae.[AdEnrollID] = cv_es.[AdEnrollID]
					WHERE cv_es.[TransferCredit] = 0

		IF OBJECT_ID('tempdb..#ClassesTaken') IS NOT NULL
		  DROP TABLE #ClassesTaken

		SELECT cv.AdClassSchedID, cv.SyStudentID, cv.StartDate, cv.EndDate, adsched.Section, 
			   cv.AdCourseID, cv.DropDate, cv.Status, cv.AdGradeLetterCode
		INTO #ClassesTaken 
			FROM [CAMPUSVUE].[dbo].AdEnrollSched cv WITH (NOLOCK)
				INNER JOIN IMPORT_AdEnrollSched temp ON temp.SyStudentID = cv.SyStudentID
					INNER JOIN [CAMPUSVUE].[dbo].AdClassSched adsched ON cv.AdClassSchedID = adsched.AdClassSchedID
						WHERE (cv.StartDate IS NOT NULL AND cv.StartDate < @CurrentDateTime)
							ORDER BY cv.startdate DESC 

		UPDATE IMPORT_AdEnrollSched
		SET LastAdClassSchedIDTaken = 
			(SELECT top 1 ct.AdClassSchedID
				FROM #ClassesTaken ct 
					WHERE IMPORT_AdEnrollSched.SyStudentID = ct.SyStudentID
						AND (ct.StartDate IS NOT NULL AND ct.StartDate < @CurrentDateTime)
							AND LTRIM(RTRIM(ct.Section)) <> 'OGCU' 
								AND LTRIM(RTRIM(ct.Status)) != 'D' AND LTRIM(RTRIM(ct.AdGradeLetterCode)) != 'F' 
									AND LTRIM(RTRIM(ct.AdGradeLetterCode)) != 'I' AND LTRIM(RTRIM(ct.AdGradeLetterCode)) != 'W'
										ORDER BY ct.startdate DESC) 

		UPDATE T1
			SET T1.LastAdCourseIDTaken = T2.AdCourseID
		FROM IMPORT_AdEnrollSched T1 
			INNER JOIN #ClassesTaken T2 on T1.LastAdClassSchedIDTaken = T2.AdClassSchedID
		
			
	 	UPDATE [dbo].[IMPORT_AdClassSched]
		SET
			[PrimaryInstructor] = t.Code
		FROM 
		[dbo].[IMPORT_AdClassSched] CS 
			INNER JOIN [CAMPUSVUE].[dbo].[AdTeacher] t WITH (NOLOCK) ON t.AdTeacherId = cs.AdTeacherID
		
		UPDATE [dbo].[IMPORT_AdEnrollSched]
			SET
				[TermCode] = at.Code
		FROM [dbo].[IMPORT_AdEnrollSched] ae
			INNER JOIN [CAMPUSVUE].[dbo].AdTerm at WITH (NOLOCK) ON at.AdTermID = ae.AdTermID


  		UPDATE [dbo].[ImportProcess]
			SET
				[EndDate] = GETDATE(),
				[Remarks] = [Remarks] + 'Process complete~'
		WHERE ProcessID = @ProcessID;

		WITH pseudo AS
		(	 SELECT 
			 PT.[Si_PseudoRegistratonTrackingID]
			,PT.[SyStudentID]
			,PT.[LMSAdClassSchedID]
			,PT.[CvueAdClassSchedID]
			,PT.[ValidFromDate]
			,PT.ValidUptoDate
			,PT.UserId
			,PT.DateAdded
			,PT.DateLstMod
			,@ProcessID as ProcessID
			,SS.[StuNum]
			,ss.[FirstName]
			,ss.[LastName]
			,RowNum = ROW_NUMBER() 
				OVER(
						PARTITION BY PT.[SyStudentID], [LMSAdClassSchedID], [CvueAdClassSchedID] 
							ORDER BY PT.DateLstMod DESC
					)
		FROM [CAMPUSVUE].DBO.[Si_PseudoRegistratonTracking] PT WITH (NOLOCK) 
			INNER JOIN [dbo].[IMPORT_AdClassSched] AC ON PT.[LMSAdClassSchedID] = AC.[AdClassSchedID]
				INNER JOIN  [CAMPUSVUE].DBO.AdEnrollSched AE ON AE.[AdClassSchedID] = PT.[CvueAdClassSchedID]
					INNER JOIN  [CAMPUSVUE].DBO.[SyStudent] SS ON SS.[SyStudentID] = PT.[SyStudentID]
						WHERE PT.ValidUptoDate IS NULL AND AE.[Status] IN ('L','C','S') 
		)
		INSERT INTO  [dbo].[PseudoRegistraton] 
		(
				[Si_PseudoRegistratonTrackingID]
			   ,[SyStudentID]
			   ,[LMSAdClassSchedID]
			   ,[CvueAdClassSchedID]
			   ,[ValidFromDate]
			   ,[ValidUptoDate]
			   ,[UserId]
			   ,[DateAdded]
			   ,[DateLstMod]
			   ,[ProcessID]
			   ,[StuNum]
			   ,[FirstName]
			   ,[LastName]
		)
		
			SELECT 
				 [Si_PseudoRegistratonTrackingID]
			   ,[SyStudentID]
			   ,[LMSAdClassSchedID]
			   ,[CvueAdClassSchedID]
			   ,[ValidFromDate]
			   ,[ValidUptoDate]
			   ,[UserId]
			   ,[DateAdded]
			   ,[DateLstMod]
			   ,[ProcessID]
			   ,[StuNum]
			   ,[FirstName]
			   ,[LastName]
			FROM   pseudo 
			WHERE
			   RowNum = 1
			ORDER BY 
			   DateLstMod DESC;

    SET NOCOUNT OFF


GO


