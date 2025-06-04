USE [ARB]
GO

DELETE Rules

GO
-- *********************************** DNP-960 BEGIN ***********************************
INSERT INTO [dbo].[Rules] 
VALUES (
1--PK
,'LastAdCourseIDTaken'--FieldName
,'32254'--FieldExpectedValue
,'int'--FieldExpectedValueType
,32255--AdCourseID, int,>
,'DNP-960'--CourseCode, varchar(100),
,'P'--SectionPrefix
,'5'--SectionBaseCounterFirstDigitString
,1--Priority
,1--<IsActive, bit,>
,GETDATE()--<CreatedOn, datetime,>
,'RuleSetUp'--<CreatedBy, varchar(150),>
,GETDATE()--<UpdatedOn, datetime,>
,'RuleSetUp')--<UpdatedBy, varchar(150),>

GO

INSERT INTO [dbo].[Rules] 
VALUES (
2--PK
,'LastAdCourseIDTaken'--FieldName
,'32255'--FieldExpectedValue
,'int'--FieldExpectedValueType
,32255--AdCourseID, int,>
,'DNP-960'--CourseCode, varchar(100),
,'P'--SectionPrefix
,'5'--SectionBaseCounterFirstDigitString
,2--Priority
,1--<IsActive, bit,>
,GETDATE()--<CreatedOn, datetime,>
,'RuleSetUp'--<CreatedBy, varchar(150),>
,GETDATE()--<UpdatedOn, datetime,>
,'RuleSetUp')--<UpdatedBy, varchar(150),>

GO
-- *********************************** DNP-960 END *************************************

-- *********************************** DNP-965 BEGIN ***********************************

INSERT INTO [dbo].[Rules] 
VALUES (
3--PK
,'LastAdCourseIDTaken'--FieldName
,'32255'--FieldExpectedValue
,'int'--FieldExpectedValueType
,32256--AdCourseID, int,>
,'DNP-965'--CourseCode, varchar(100),
,'P'--SectionPrefix
,'5'--SectionBaseCounterFirstDigitString
,1--Priority
,1--<IsActive, bit,>
,GETDATE()--<CreatedOn, datetime,>
,'RuleSetUp'--<CreatedBy, varchar(150),>
,GETDATE()--<UpdatedOn, datetime,>
,'RuleSetUp')--<UpdatedBy, varchar(150),>

GO
INSERT INTO [dbo].[Rules] 
VALUES (
4--PK
,'LastAdCourseIDTaken'--FieldName
,'32256'--FieldExpectedValue
,'int'--FieldExpectedValueType
,32256--AdCourseID, int,>
,'DNP-965'--CourseCode, varchar(100),
,'P'--SectionPrefix
,'5'--SectionBaseCounterFirstDigitString
,2--Priority
,1--<IsActive, bit,>
,GETDATE()--<CreatedOn, datetime,>
,'RuleSetUp'--<CreatedBy, varchar(150),>
,GETDATE()--<UpdatedOn, datetime,>
,'RuleSetUp')--<UpdatedBy, varchar(150),>

GO

-- *********************************** DNP-965 END **************************************


-- *********************************** DNP-960A BEGIN ***********************************

INSERT INTO [dbo].[Rules] 
VALUES (
5--PK
,'LastAdCourseIDTaken'--FieldName
,'34613'--FieldExpectedValue
,'int'--FieldExpectedValueType
,34614--AdCourseID, int,>
,'DNP-960A'--CourseCode, varchar(100),
,'P'--SectionPrefix
,'5'--SectionBaseCounterFirstDigitString
,1--Priority
,1--<IsActive, bit,>
,GETDATE()--<CreatedOn, datetime,>
,'RuleSetUp'--<CreatedBy, varchar(150),>
,GETDATE()--<UpdatedOn, datetime,>
,'RuleSetUp')--<UpdatedBy, varchar(150),>

GO
INSERT INTO [dbo].[Rules] 
VALUES (
6--PK
,'LastAdCourseIDTaken'--FieldName
,'34614'--FieldExpectedValue
,'int'--FieldExpectedValueType
,34614--AdCourseID, int,>
,'DNP-960A'--CourseCode, varchar(100),
,'P'--SectionPrefix
,'5'--SectionBaseCounterFirstDigitString
,2--Priority
,1--<IsActive, bit,>
,GETDATE()--<CreatedOn, datetime,>
,'RuleSetUp'--<CreatedBy, varchar(150),>
,GETDATE()--<UpdatedOn, datetime,>
,'RuleSetUp')--<UpdatedBy, varchar(150),>

GO

-- *********************************** DNP-960A END *************************************

GO

-- *********************************** DNP-965A BEGIN ***********************************

INSERT INTO [dbo].[Rules] 
VALUES (
7--PK
,'LastAdCourseIDTaken'--FieldName
,'34614'--FieldExpectedValue
,'int'--FieldExpectedValueType
,34615--AdCourseID, int,>
,'DNP-965A'--CourseCode, varchar(100),
,'P'--SectionPrefix
,'5'--SectionBaseCounterFirstDigitString
,1--Priority
,1--<IsActive, bit,>
,GETDATE()--<CreatedOn, datetime,>
,'RuleSetUp'--<CreatedBy, varchar(150),>
,GETDATE()--<UpdatedOn, datetime,>
,'RuleSetUp')--<UpdatedBy, varchar(150),>

GO
INSERT INTO [dbo].[Rules] 
VALUES (
8--PK
,'LastAdCourseIDTaken'--FieldName
,'34615'--FieldExpectedValue
,'int'--FieldExpectedValueType
,34615--AdCourseID, int,>
,'DNP-965A'--CourseCode, varchar(100),
,'P'--SectionPrefix
,'5'--SectionBaseCounterFirstDigitString
,2--Priority
,1--<IsActive, bit,>
,GETDATE()--<CreatedOn, datetime,>
,'RuleSetUp'--<CreatedBy, varchar(150),>
,GETDATE()--<UpdatedOn, datetime,>
,'RuleSetUp')--<UpdatedBy, varchar(150),>


-- *********************************** DNP-965A END *************************************


GO

-- *********************************** CNL_664A BEGIN ***********************************
INSERT INTO [dbo].[Rules] 
VALUES (
9--PK
,'LastAdCourseIDTaken'--FieldName
,'34044'--FieldExpectedValue
,'int'--FieldExpectedValueType
,34046--AdCourseID, int,>
,'CNL-664A'--CourseCode, varchar(100),
,'P'--SectionPrefix
,'5'--SectionBaseCounterFirstDigitString
,1--Priority
,1--<IsActive, bit,>
,GETDATE()--<CreatedOn, datetime,>
,'RuleSetUp'--<CreatedBy, varchar(150),>
,GETDATE()--<UpdatedOn, datetime,>
,'RuleSetUp')--<UpdatedBy, varchar(150),>

GO
INSERT INTO [dbo].[Rules] 
VALUES (
10--PK
,'LastAdCourseIDTaken'--FieldName
,'34046'--FieldExpectedValue
,'int'--FieldExpectedValueType
,34046--AdCourseID, int,>
,'CNL-664A'--CourseCode, varchar(100),
,'P'--SectionPrefix
,'5'--SectionBaseCounterFirstDigitString
,2--Priority
,1--<IsActive, bit,>
,GETDATE()--<CreatedOn, datetime,>
,'RuleSetUp'--<CreatedBy, varchar(150),>
,GETDATE()--<UpdatedOn, datetime,>
,'RuleSetUp')--<UpdatedBy, varchar(150),>


-- *********************************** CNL_664A END *************************************
GO

-- *********************************** CNL_664B BEGIN ***********************************

INSERT INTO [dbo].[Rules] 
VALUES (
11--PK
,'LastAdCourseIDTaken'--FieldName
,'34046'--FieldExpectedValue
,'int'--FieldExpectedValueType
,34047--AdCourseID, int,>
,'CNL-664B'--CourseCode, varchar(100),
,'P'--SectionPrefix
,'5'--SectionBaseCounterFirstDigitString
,1--Priority
,1--<IsActive, bit,>
,GETDATE()--<CreatedOn, datetime,>
,'RuleSetUp'--<CreatedBy, varchar(150),>
,GETDATE()--<UpdatedOn, datetime,>
,'RuleSetUp')--<UpdatedBy, varchar(150),>

GO
INSERT INTO [dbo].[Rules] 
VALUES (
12--PK
,'LastAdCourseIDTaken'--FieldName
,'34047'--FieldExpectedValue
,'int'--FieldExpectedValueType
,34047--AdCourseID, int,>
,'CNL-664B'--CourseCode, varchar(100),
,'P'--SectionPrefix
,'5'--SectionBaseCounterFirstDigitString
,2--Priority
,1--<IsActive, bit,>
,GETDATE()--<CreatedOn, datetime,>
,'RuleSetUp'--<CreatedBy, varchar(150),>
,GETDATE()--<UpdatedOn, datetime,>
,'RuleSetUp')--<UpdatedBy, varchar(150),>

-- *********************************** CNL_664B END *************************************
GO

