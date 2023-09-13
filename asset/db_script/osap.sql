----------------------------- Module User -----------------------------

create proc spInsertModuleUser
@Guid nvarchar(100),
@DisplayName nvarchar(100),
@UserName nvarchar(50),
@Email nvarchar(50)
as
begin
	if not exists(select * from ModuleUser m where m.UserName = @UserName)
	begin
		insert into ModuleUser(Guid,DisplayName,UserName,Email,IsVisible)
		values(@Guid,@DisplayName,@UserName,@Email,1);
	end
end

----------------------------- Module User -----------------------------

----------------------------- Application Type ------------------------
alter proc spGetApplicationTypes
as
begin
	Select t.ApplicationTypeId,t.ApplicationTypeText,isnull(t.ApplicationTypeDescription,'') as ApplicationTypeDescription,
	isnull(t.Email,'') as Email,t.IsVisible
	from ApplicationType t order by t.ApplicationTypeText
end
-- exec spGetApplicationTypes
GO

alter proc spInsertApplicationType
@ApplicationTypeText nvarchar(150),
@ApplicationTypeDescription nvarchar(200),
@Email nvarchar(100),
@IsVisible bit
as
begin
	declare @max_app_type_id as int set @max_app_type_id = 0;

	select @max_app_type_id = max(ApplicationTypeId)+1 from ApplicationType;
	
	Insert into ApplicationType(ApplicationTypeId,ApplicationTypeText,ApplicationTypeDescription,Email,IsVisible)
	Values(@max_app_type_id,@ApplicationTypeText,@ApplicationTypeDescription,@Email,@IsVisible)

end

GO

alter proc spUpdateApplicationType
@ApplicationTypeId int,
@ApplicationTypeText nvarchar(150),
@ApplicationTypeDescription nvarchar(200),
@Email nvarchar(100),
@IsVisible bit
as
begin
	Update ApplicationType set ApplicationTypeText = @ApplicationTypeText,ApplicationTypeDescription = @ApplicationTypeDescription,
	Email = @Email,IsVisible = @IsVisible
	where ApplicationTypeId = @ApplicationTypeId;
end

GO
----------------------------- Application Type ------------------------

GO

create function fnGetUserNameById(@ModuleUserId int)
returns nvarchar(50)
as
begin
	declare @user_name as nvarchar(50) set @user_name = '';
	select @user_name = DisplayName from ModuleUser where ModuleUserId = @ModuleUserId

	return @user_name;
end
GO

alter function fnGetEmailByUserID(@ModuleUserId int)
returns nvarchar(50)
as
begin
	declare @email as nvarchar(50) set @email = '';
	select @email = Email from ModuleUser where ModuleUserId = @ModuleUserId;
	return @email;
end

GO

-- select dbo.fnGetFileNameByApplicationId(50308)

alter function fnGetFileNameByApplicationId(@ApplicationId int)
returns nvarchar(100)
as
begin
	declare @file_name as nvarchar(100) set @file_name = '';
	select @file_name = FileName from Application where ApplicationId = @ApplicationId;
	return @file_name;
end

-- select dbo.fnGetEmailByUserID(50308)

GO

alter proc spInitiateApplication
@ApplicantId int,
@Description nvarchar(4000),
@ApplicationTypeId int,
@ApplicationStatusId int,
@FileName nvarchar(100),
@CreatedBy int,
@DocumentWorkFlow nvarchar(1000)
as
begin
	declare @ApplicationId as int Set @ApplicationId = 0;
begin tran

	insert into Application(ApplicantId,Title,Description,Amount,ApplicationTypeId,ApplicationStatusId,FileName,CreatedDate,CreatedBy)
	Values(@ApplicantId,'',@Description,0,@ApplicationTypeId,@ApplicationStatusId,@FileName,GETDATE(),@ApplicantId)
	IF (@@ERROR <> 0) GOTO ERR_HANDLER

	select @ApplicationId = max(ApplicationId) from Application;

	exec spInsertMultipleDocWorkFlow @ApplicationId,@DocumentWorkFlow;
	IF (@@ERROR <> 0) GOTO ERR_HANDLER

	select @ApplicationId as 'ApplicationId';

COMMIT TRAN
RETURN 0

ERR_HANDLER:
ROLLBACK TRAN
RETURN 1
end

GO

alter proc spPermanentRejectApplication
@ApplicationId int,
@RejectionRemarks nvarchar(500)
as
begin
	Update ProcessFlow set  Comment = ISNULL(Comment,'') + ' - Reject by initiator', ProcessFlowDecisionId=3 where ApplicationId = @ApplicationId;
	update Application set Description += '[ Rejection Remarks : ' + @RejectionRemarks + ' ]', ApplicationStatusId = 4 where ApplicationId = @ApplicationId;
end

GO

alter proc spInsertMultipleDocWorkFlow
@ApplicationId int,
@DocumentWorkFlow nvarchar(1000)
as
begin
	Declare @Index as int
	Declare @CurrentData as nvarchar(4000)
	Declare @RestData as nvarchar(4000)
	Declare @RestPortion as nvarchar(4000)

	Declare @SupervisorID as nvarchar(50)
	Declare @DepartmentID as nvarchar(50)
	Declare @BranchID as nvarchar(50)
	Declare @seq_counter as int set @seq_counter = 1;

	Declare @ApproverId int, @RoleId int, @Sequence int, @ProcessFlowDecisionId int

begin tran

	set @RestData=@DocumentWorkFlow
	while @RestData<>''
	begin
		set @Index=CHARINDEX('|',@RestData)
		set @CurrentData=substring(@RestData,1,@Index-1)
		set @RestData=substring(@RestData,@Index+1,len(@RestData))		
		
		set @RestPortion=@CurrentData
		
		set @Index=CHARINDEX('~',@RestPortion)		
		set @ApproverId=substring(@RestPortion,1,@Index-1)
		set @RestPortion=substring(@RestPortion,@Index+1,len(@RestPortion))	
		
		set @Index=CHARINDEX('~',@RestPortion)		
		set @RoleId=convert(int,substring(@RestPortion,1,@Index-1))
		set @RestPortion=substring(@RestPortion,@Index+1,len(@RestPortion))
		
		set @Index=CHARINDEX('~',@RestPortion)		
		--set @Sequence=substring(@RestPortion,1,@Index-1)
		set @Sequence=@seq_counter
		set @RestPortion=substring(@RestPortion,@Index+1,len(@RestPortion))	

		set @Index=CHARINDEX('~',@RestPortion)		
		set @ProcessFlowDecisionId=substring(@RestPortion,1,@Index-1)
		set @RestPortion=substring(@RestPortion,@Index+1,len(@RestPortion))	
		
		insert into ProcessFlow(ApplicationId,ApproverId,RoleId,Sequence,ProcessFlowDecisionId)
		Values(@ApplicationId,@ApproverId,@RoleId,@Sequence,@ProcessFlowDecisionId)
		IF (@@ERROR <> 0) GOTO ERR_HANDLER
		
		Set @ApproverId = 0;
		Set @RoleId = 0;
		Set @Sequence = 0;
		Set @ProcessFlowDecisionId = 0;
		set @seq_counter  = @seq_counter + 1;
	end

COMMIT TRAN
RETURN 0

ERR_HANDLER:
ROLLBACK TRAN
RETURN 1
end

GO
-- exec spGetOsapInitiatorMail 41722
alter proc spGetOsapInitiatorMail
@ApplicationId int
as
begin
	Declare @MailBody as nvarchar(4000) Set @MailBody = ''
	Declare @MailSubject as nvarchar(200) Set @MailSubject = ''
	Declare @MailTo as nvarchar(200) Set @MailTo = ''
	Declare @MailFrom as nvarchar(200) Set @MailFrom =''
	Declare @MailCC as nvarchar(200) Set @MailCC = ''
	Declare @SoftwareLink  as nvarchar(500) Set @SoftwareLink = 'http://ext.mfilbd.com/osap/'

	Declare @Initiator as nvarchar(100) Set @Initiator = '';
	Declare @InitiatorMail as nvarchar(50) set @InitiatorMail = '';
	Declare @Description  as nvarchar(1000) Set @Description='';
	Declare @InitiationDate as datetime

	Select @Initiator = m.DisplayName,@InitiatorMail=m.Email,@Description = a.Description, @InitiationDate = a.CreatedDate,
	@MailCC = 'divit@meridianfinancebd.com'
	from Application a 
	inner join ModuleUser m on a.ApplicantId = m.ModuleUserId
	inner join ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	where ApplicationId = @ApplicationId;

	Set @MailBody = '
	<html>
	<body>
	<br/>Please be kind enough to approve the memo.<br/>
	<table border=''1'' width=''60%''>
	<tr>
		<td>Tracking No</td>
		<td>' + Convert(nvarchar,@ApplicationId) + '</td>
	</tr>
	<tr>
		<td>Description</td>
		<td>' + @Description + '</td>
	</tr>
	<tr>
		<td>Initiation Date</td>
		<td>' + convert(nvarchar,@InitiationDate) + '</td>
	</tr>
	<tr>
		<td>Software Link</td>
		<td><a href='+@SoftwareLink+'>Link</a></td>
	</tr>
	</table>
	</body>
	</html>'

	Set @MailSubject = '[OSAP: '+ convert(nvarchar,@ApplicationId) +'] Require your attention'
	Set @MailFrom =@InitiatorMail
	Select @MailTo = ISNULL(Email,'')  from ModuleUser Where ModuleUserId = (
	select ApproverId from ProcessFlow where ApplicationId = @ApplicationId and Sequence = 1
	)

	if @MailTo = ''
		Set @MailTo = 'divit@meridianfinancebd.com';

	Select @MailSubject as 'MailSubject',@MailBody as 'MailBody' ,Case When @MailFrom='' then 'info@meridianfinancebd.com' else @MailFrom end  as 'MailFrom',
	Case When @MailTo='' then 'divit@meridianfinancebd.com' else @MailTo end as 'MailTo',
	Case When @MailCC='' then 'divit@meridianfinancebd.com' else @MailCC end as 'MailCC',
	'dsamaddar@meridianfinancebd.com' as 'MailBCC'

end

GO

-- exec spGetOsapPermanentRejectionMail 42167,''
alter proc spGetOsapPermanentRejectionMail
@ApplicationId int,
@RejectionRemarks nvarchar(500)
as
begin
	Declare @MailBody as nvarchar(4000) Set @MailBody = ''
	Declare @MailSubject as nvarchar(200) Set @MailSubject = ''
	Declare @MailTo as nvarchar(200) Set @MailTo = ''
	Declare @MailFrom as nvarchar(200) Set @MailFrom =''
	Declare @MailCC as nvarchar(200) Set @MailCC = ''
	Declare @SoftwareLink  as nvarchar(500) Set @SoftwareLink = 'http://ext.mfilbd.com/osap/'

	Declare @Initiator as nvarchar(100) Set @Initiator = '';
	Declare @InitiatorMail as nvarchar(50) set @InitiatorMail = '';
	Declare @Description  as nvarchar(1000) Set @Description='';
	Declare @Comment as nvarchar(1000) set @Comment = '';
	Declare @ApproverId as int Set @ApproverId = 0;
	Declare @ApproverMail as nvarchar(50) set @ApproverMail = '';
	Declare @ApproverName as nvarchar(100) set @ApproverName = '';
	Declare @InitiationDate as datetime

	Select @Initiator = m.DisplayName,@InitiatorMail=m.Email,@Description = a.Description, @InitiationDate = a.CreatedDate,
	@MailCC = 'divit@meridianfinancebd.com' + ';'
	from Application a 
	inner join ModuleUser m on a.ApplicantId = m.ModuleUserId
	inner join ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	where ApplicationId = @ApplicationId;

	select @MailTo += dbo.fnGetEmailByUserID(ApproverId) + ';' from ProcessFlow where ApplicationId = @ApplicationId

	Set @MailBody = '
	<html>
	<body style=''font-family: Gill Sans MT, sans-serif;background-color: red;''>
	<br/>Your document has been Rejected.<br/>
	<table border=''1'' width=''40%''>
	<tr>
		<td>Tracking No</td>
		<td>' + Convert(nvarchar,@ApplicationId) + '</td>
	</tr>
	<tr>
		<td>Description</td>
		<td>' + @Description + '</td>
	</tr>
	<tr>
		<td>Initiation Date</td>
		<td>' + convert(nvarchar,@InitiationDate) + '</td>
	</tr>
	<tr>
		<td>Final Rejection remarks</td>
		<td>' + @RejectionRemarks + '</td>
	</tr>
	</table>
	</body>
	</html>'

	Set @MailSubject = '[OSAP: '+ convert(nvarchar,@ApplicationId) +'] Rejected by ' + @Initiator
	set @MailFrom = @InitiatorMail;

	if @MailTo = ''
		Set @MailTo = 'divit@meridianfinancebd.com';

	Select @MailSubject as 'MailSubject',@MailBody as 'MailBody' ,Case When @MailFrom='' then 'info@meridianfinancebd.com' else @MailFrom end  as 'MailFrom',
	Case When @MailTo='' then 'divit@meridianfinancebd.com' else @MailTo end as 'MailTo',
	Case When @MailCC='' then 'divit@meridianfinancebd.com' else @MailCC end as 'MailCC',
	'dsamaddar@meridianfinancebd.com' as 'MailBCC'

end


GO

alter proc spGetOsapTransferMail
@ApplicationId int,
@ProcessFlowId int,
@PApproverId int,
@TApproverId int,
@Comment nvarchar(1000)
as
begin
	Declare @MailBody as nvarchar(4000) Set @MailBody = ''
	Declare @MailSubject as nvarchar(200) Set @MailSubject = ''
	Declare @MailTo as nvarchar(200) Set @MailTo = ''
	Declare @MailFrom as nvarchar(200) Set @MailFrom =''
	Declare @MailCC as nvarchar(200) Set @MailCC = ''
	Declare @SoftwareLink  as nvarchar(500) Set @SoftwareLink = 'http://192.168.11.241/osap/'

	Declare @Initiator as nvarchar(100) Set @Initiator = '';
	Declare @InitiatorMail as nvarchar(50) set @InitiatorMail = '';
	Declare @Description  as nvarchar(1000) Set @Description='';
	Declare @ApproverId as int Set @ApproverId = 0;
	Declare @ApproverMail as nvarchar(50) set @ApproverMail = '';
	Declare @ApproverName as nvarchar(100) set @ApproverName = '';
	Declare @InitiationDate as datetime

	Select @Initiator = m.DisplayName,@InitiatorMail=m.Email,@Description = a.Description, @InitiationDate = a.CreatedDate,
	@MailCC = ISNULL(t.Email,'divit@meridianfinancebd.com')
	from Application a 
	inner join ModuleUser m on a.ApplicantId = m.ModuleUserId
	inner join ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	where ApplicationId = @ApplicationId;

	Set @MailBody = '
	<html>
	<body style=''font-family: Gill Sans MT, sans-serif;background-color: AliceBlue;''>
	<br/>New task assignment<br/>
	<table border=''1'' width=''40%''>
	<tr>
		<td>Tracking No</td>
		<td>' + Convert(nvarchar,@ApplicationId) + '</td>
	</tr>
	<tr>
		<td>Description</td>
		<td>' + @Description + '</td>
	</tr>
	<tr>
		<td>Initiation Date</td>
		<td>' + convert(nvarchar,@InitiationDate) + '</td>
	</tr>
	<tr>
		<td>Transferred from</td>
		<td>' + dbo.fnGetUserNameById(@PApproverId) + '</td>
	</tr>
	<tr>
		<td>Transferred to</td>
		<td>' + dbo.fnGetUserNameById(@TApproverId) + '</td>
	</tr>
	<tr>
		<td>Comments</td>
		<td>' + @Comment + '</td>
	</tr>
	<tr>
		<td>Software Link</td>
		<td>' + @SoftwareLink + '</td>
	</tr>
	</table>
	</body>
	</html>'

	Set @MailSubject = '[OSAP: '+ convert(nvarchar,@ApplicationId) +'] Require your attention'
	set @MailFrom = dbo.fnGetEmailByUserID(@PApproverId);
	set @MailTo = dbo.fnGetEmailByUserID(@TApproverId);

	if @MailTo = ''
		Set @MailTo = 'divit@meridianfinancebd.com';

	Select @MailSubject as 'MailSubject',@MailBody as 'MailBody' ,Case When @MailFrom='' then 'info@meridianfinancebd.com' else @MailFrom end  as 'MailFrom',
	Case When @MailTo='' then 'divit@meridianfinancebd.com' else @MailTo end as 'MailTo',
	Case When @MailCC='' then 'divit@meridianfinancebd.com' else @MailCC end as 'MailCC',
	'dsamaddar@meridianfinancebd.com' as 'MailBCC'

end

GO

-- exec spGetOsapRejectionMail 41934,57150
alter proc spGetOsapRejectionMail
@ApplicationId int,
@ProcessFlowId int
as
begin
	Declare @MailBody as nvarchar(4000) Set @MailBody = ''
	Declare @MailSubject as nvarchar(200) Set @MailSubject = ''
	Declare @MailTo as nvarchar(200) Set @MailTo = ''
	Declare @MailFrom as nvarchar(200) Set @MailFrom =''
	Declare @MailCC as nvarchar(200) Set @MailCC = ''
	Declare @SoftwareLink  as nvarchar(500) Set @SoftwareLink = 'http://ext.mfilbd.com/osap/'

	Declare @Initiator as nvarchar(100) Set @Initiator = '';
	Declare @InitiatorMail as nvarchar(50) set @InitiatorMail = '';
	Declare @Description  as nvarchar(1000) Set @Description='';
	Declare @Comment as nvarchar(1000) set @Comment = '';
	Declare @ApproverId as int Set @ApproverId = 0;
	Declare @ApproverMail as nvarchar(50) set @ApproverMail = '';
	Declare @ApproverName as nvarchar(100) set @ApproverName = '';
	Declare @InitiationDate as datetime

	Select @Initiator = m.DisplayName,@InitiatorMail=m.Email,@Description = a.Description, @InitiationDate = a.CreatedDate,
	@MailCC = 'divit@meridianfinancebd.com'
	from Application a 
	inner join ModuleUser m on a.ApplicantId = m.ModuleUserId
	inner join ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	where ApplicationId = @ApplicationId;

	select @Comment = ISNULL(Comment,'') ,@ApproverId = ApproverId from ProcessFlow where ProcessFlowId = @ProcessFlowId;
	select @ApproverMail = Email,@ApproverName = DisplayName from ModuleUser where ModuleUserId = @ApproverId;

	Set @MailBody = '
	<html>
	<body style=''font-family: Gill Sans MT, sans-serif;background-color: lightcoral;''>
	<br/>Your document has been rejected.<br/>
	<table border=''1'' width=''40%''>
	<tr>
		<td>Tracking No</td>
		<td>' + Convert(nvarchar,@ApplicationId) + '</td>
	</tr>
	<tr>
		<td>Description</td>
		<td>' + @Description + '</td>
	</tr>
	<tr>
		<td>Initiation Date</td>
		<td>' + convert(nvarchar,@InitiationDate) + '</td>
	</tr>
	<tr>
		<td>Rejected by</td>
		<td>' + @ApproverName + '</td>
	</tr>
	<tr>
		<td>Rejection Remarks</td>
		<td>' + @Comment + '</td>
	</tr>
	<tr>
		<td>Software Link</td>
		<td><a href='+@SoftwareLink+'>Link</a></td>
	</tr>
	</table>
	</body>
	</html>'

	Set @MailSubject = '[OSAP: '+ convert(nvarchar,@ApplicationId) +'] Require your attention'
	set @MailFrom = @ApproverMail;
	set @MailTo = @InitiatorMail;

	if @MailTo = ''
		Set @MailTo = 'divit@meridianfinancebd.com';

	Select @MailSubject as 'MailSubject',@MailBody as 'MailBody' ,Case When @MailFrom='' then 'info@meridianfinancebd.com' else @MailFrom end  as 'MailFrom',
	Case When @MailTo='' then 'divit@meridianfinancebd.com' else @MailTo end as 'MailTo',
	Case When @MailCC='' then 'divit@meridianfinancebd.com' else @MailCC end as 'MailCC',
	'dsamaddar@meridianfinancebd.com' as 'MailBCC'

end

GO


alter proc spGetOsapMidApprovalMail
@ApplicationId int,
@ProcessFlowId int
as
begin
	Declare @MailBody as nvarchar(4000) Set @MailBody = ''
	Declare @MailSubject as nvarchar(200) Set @MailSubject = ''
	Declare @MailTo as nvarchar(200) Set @MailTo = ''
	Declare @MailFrom as nvarchar(200) Set @MailFrom =''
	Declare @MailCC as nvarchar(200) Set @MailCC = ''
	Declare @SoftwareLink  as nvarchar(500) Set @SoftwareLink = 'http://ext.mfilbd.com/osap/'

	Declare @Initiator as nvarchar(100) Set @Initiator = '';
	Declare @InitiatorMail as nvarchar(50) set @InitiatorMail = '';
	Declare @Description  as nvarchar(1000) Set @Description='';
	Declare @Comment as nvarchar(1000) set @Comment = '';
	Declare @Sequence as int Set @Sequence = 0;
	Declare @ApproverId as int Set @ApproverId = 0;
	Declare @ApproverMail as nvarchar(50) set @ApproverMail = '';
	Declare @NextApproverMail as nvarchar(50) set @NextApproverMail = '';
	Declare @ApproverName as nvarchar(100) set @ApproverName = '';
	Declare @InitiationDate as datetime

	Select @Initiator = m.DisplayName,@InitiatorMail=m.Email,@Description = a.Description, @InitiationDate = a.CreatedDate,
	@MailCC = 'divit@meridianfinancebd.com'
	from Application a 
	inner join ModuleUser m on a.ApplicantId = m.ModuleUserId
	inner join ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	where ApplicationId = @ApplicationId;

	select @Comment = ISNULL(Comment,'') ,@ApproverId = ApproverId,@Sequence = Sequence+1 from ProcessFlow where ProcessFlowId = @ProcessFlowId;
	select @ApproverMail = Email,@ApproverName = DisplayName from ModuleUser where ModuleUserId = @ApproverId;

	-- Finding Next approver
	select @NextApproverMail = Email from ModuleUser where ModuleUserId = (
	select ApproverId from ProcessFlow where ApplicationId = @ApplicationId and Sequence = @Sequence
	)

	Set @MailBody = '
	<html>
	<body style=''font-family: Gill Sans MT, sans-serif;background-color: white;''>
	<br/>Please be kind enough to approve the memo.<br/>
	<table border=''1'' width=''40%''>
	<tr>
		<td>Tracking No</td>
		<td>' + Convert(nvarchar,@ApplicationId) + '</td>
	</tr>
	<tr>
		<td>Description</td>
		<td>' + @Description + '</td>
	</tr>
	<tr>
		<td>Initiation Date</td>
		<td>' + convert(nvarchar,@InitiationDate) + '</td>
	</tr>
	<tr>
		<td>Approved by</td>
		<td>' + @ApproverName + '</td>
	</tr>
	<tr>
		<td>Comments</td>
		<td>' + @Comment + '</td>
	</tr>
	<tr>
		<td>Software Link</td>
		<td><a href='+@SoftwareLink+'>Link</a></td>
	</tr>
	</table>
	</body>
	</html>'

	Set @MailSubject = '[OSAP: '+ convert(nvarchar,@ApplicationId) +'] Require your attention'
	set @MailFrom = @ApproverMail;
	set @MailTo = @NextApproverMail;

	if @MailTo = ''
		Set @MailTo = 'divit@meridianfinancebd.com';

	Select @MailSubject as 'MailSubject',@MailBody as 'MailBody' ,Case When @MailFrom='' then 'info@meridianfinancebd.com' else @MailFrom end  as 'MailFrom',
	Case When @MailTo='' then 'divit@meridianfinancebd.com' else @MailTo end as 'MailTo',
	Case When @MailCC='' then 'divit@meridianfinancebd.com' else @MailCC end as 'MailCC',
	'dsamaddar@meridianfinancebd.com' as 'MailBCC'

end

GO

-- exec spGetOsapFinalApprovalMail 41938,57162
alter proc spGetOsapFinalApprovalMail
@ApplicationId int,
@ProcessFlowId int
as
begin
	Declare @MailBody as nvarchar(4000) Set @MailBody = ''
	Declare @MailSubject as nvarchar(200) Set @MailSubject = ''
	Declare @MailTo as nvarchar(200) Set @MailTo = ''
	Declare @MailFrom as nvarchar(200) Set @MailFrom =''
	Declare @MailCC as nvarchar(200) Set @MailCC = ''
	Declare @SoftwareLink  as nvarchar(500) Set @SoftwareLink = 'http://ext.mfilbd.com/osap/'

	Declare @Initiator as nvarchar(100) Set @Initiator = '';
	Declare @InitiatorMail as nvarchar(50) set @InitiatorMail = '';
	Declare @Description  as nvarchar(1000) Set @Description='';
	Declare @Comment as nvarchar(1000) set @Comment = '';
	Declare @ApproverId as int Set @ApproverId = 0;
	Declare @ApproverMail as nvarchar(50) set @ApproverMail = '';
	Declare @ApproverName as nvarchar(100) set @ApproverName = '';
	Declare @InitiationDate as datetime

	Select @Initiator = m.DisplayName,@InitiatorMail=m.Email,@Description = a.Description, @InitiationDate = a.CreatedDate,
	@MailCC = 'divit@meridianfinancebd.com' + ';'
	from Application a 
	inner join ModuleUser m on a.ApplicantId = m.ModuleUserId
	inner join ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	where ApplicationId = @ApplicationId;

	select @Comment = ISNULL(Comment,'') ,@ApproverId = ApproverId from ProcessFlow where ProcessFlowId = @ProcessFlowId;
	select @ApproverMail = Email,@ApproverName = DisplayName from ModuleUser where ModuleUserId = @ApproverId;

	select @MailCC += dbo.fnGetEmailByUserID(ApproverId) + ';' from ProcessFlow where ApplicationId = @ApplicationId

	Set @MailBody = '
	<html>
	<body style=''font-family: Gill Sans MT, sans-serif;background-color: lightgreen;''>
	<br/>Your document has been approved.<br/>
	<table border=''1'' width=''40%''>
	<tr>
		<td>Tracking No</td>
		<td>' + Convert(nvarchar,@ApplicationId) + '</td>
	</tr>
	<tr>
		<td>Description</td>
		<td>' + @Description + '</td>
	</tr>
	<tr>
		<td>Initiation Date</td>
		<td>' + convert(nvarchar,@InitiationDate) + '</td>
	</tr>
	<tr>
		<td>Software Link</td>
		<td><a href='+@SoftwareLink+'>Link</a></td>
	</tr>
	</table>
	</body>
	</html>'

	Set @MailSubject = '[OSAP: '+ convert(nvarchar,@ApplicationId) +'] Require your attention'
	set @MailFrom = @ApproverMail;
	set @MailTo = @InitiatorMail;

	if @MailTo = ''
		Set @MailTo = 'divit@meridianfinancebd.com';

	Select @MailSubject as 'MailSubject',@MailBody as 'MailBody' ,Case When @MailFrom='' then 'info@meridianfinancebd.com' else @MailFrom end  as 'MailFrom',
	Case When @MailTo='' then 'divit@meridianfinancebd.com' else @MailTo end as 'MailTo',
	Case When @MailCC='' then 'divit@meridianfinancebd.com' else @MailCC end as 'MailCC',
	'dsamaddar@meridianfinancebd.com' as 'MailBCC'

end



GO

alter proc spGetWaitingListByUser
@ModuleUserId int
as
begin
	select p.ProcessFlowId,p.ApproverId,a.ApplicationId,t.ApplicationTypeText as Type,m.DisplayName as Initiator,s.ApplicationStatusText as Status,
	convert(nvarchar,a.CreatedDate,106) as CreatedDate
	from Application a 
	inner join ProcessFlow p on a.ApplicationId = p.ApplicationId
	inner join ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	inner join ModuleUser m on a.ApplicantId = m.ModuleUserId
	inner join ApplicationStatus s on a.ApplicationStatusId = s.ApplicationStatusId
	Where a.ApplicationStatusId IN (2) and p.ApproverId = @ModuleUserId and p.ProcessFlowDecisionId IN (4)
	order by a.CreatedDate
end
GO

-- exec spGetWaitingListByUser 400

GO

create proc spRejectApplication
@ProcessFlowId int,
@Comment nvarchar(1000)
as
begin
	declare @ApplicationId as int set @ApplicationId = 0;
	declare @Sequence as int set @Sequence = 0;

	select @ApplicationId = ApplicationId,@Sequence = Sequence from ProcessFlow where ProcessFlowId=@ProcessFlowId;

	update ProcessFlow set ProcessFlowDecisionId = 3,Comment = @Comment,DecisionDate=GETDATE() where ProcessFlowId=@ProcessFlowId
	update ProcessFlow set ProcessFlowDecisionId = 5 where ApplicationId=@ApplicationId and Sequence > @Sequence;

	update Application set ApplicationStatusId= 4 where ApplicationId = @ApplicationId;

	select @ApplicationId as ApplicationId;
end

GO

alter proc spApproveApplication
@ProcessFlowId int,
@Comment nvarchar(1000)
as
begin
	declare @ApplicationId as int set @ApplicationId = 0;
	declare @Sequence as int set @Sequence = 0;
	declare @MaxSequence as int Set @MaxSequence = 0;
	declare @IsFinalStage as bit set @IsFinalStage = 0;

	select @ApplicationId = ApplicationId,@Sequence = Sequence from ProcessFlow where ProcessFlowId=@ProcessFlowId;
	select @MaxSequence = Max(Sequence) from ProcessFlow where ApplicationId=@ApplicationId;
	
	if @MaxSequence = @Sequence
	begin
		update ProcessFlow set ProcessFlowDecisionId = 1,Comment = @Comment,DecisionDate=GETDATE() where ProcessFlowId = @ProcessFlowId;
		update Application set ApplicationStatusId = 3 where ApplicationId = @ApplicationId;
		Set @IsFinalStage = 1;
	end
	else
	begin
		update ProcessFlow set ProcessFlowDecisionId = 1,Comment = @Comment,DecisionDate = GETDATE() where ProcessFlowId = @ProcessFlowId;
		update ProcessFlow set ProcessFlowDecisionId = 4 where ApplicationId = @ApplicationId and Sequence = (@Sequence+1);
	end

	select @ApplicationId as ApplicationId, @IsFinalStage as IsFinalStage,dbo.fnGetFileNameByApplicationId(@ApplicationId) as 'FileName';
end

GO

create proc spCheckUserLogin
@UserName nvarchar(100)
as
begin
	select * from ModuleUser where UserName = @UserName;
end

GO

create proc spGetApplicationTypeList
as
begin
	select ApplicationTypeId,ApplicationTypeText from ApplicationType order by ApplicationTypeText
end

-- exec spGetApplicationTypeList;

GO

create proc spGetModuleUserList
as
begin
	select ModuleUserId,DisplayName from ModuleUser where IsVisible=1 order by DisplayName;
end

GO

create proc spGetRoleList
as
begin
	select RoleId,RoleText from Role order by RoleId;
end

GO

alter proc spGetInitiatedApplicationByUser
@ApplicantId int,
@StartDt date,
@EndDt date
as
begin
	select a.ApplicationId as Tracking,t.ApplicationTypeText as 'Type',a.Description,
	s.ApplicationStatusText as 'Status',convert(nvarchar,a.CreatedDate,106) as CreatedDate,
	a.FileName,CASE a.ApplicationStatusId WHEN 3 THEN replace(a.FileName,'.pdf','_approved.pdf') ELSE a.FileName END as ApprovedFileName
	from Application a 
	inner join ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	inner join ApplicationStatus s on a.ApplicationStatusId = s.ApplicationStatusId
	where ApplicantId = @ApplicantId and CreatedDate between @StartDt and DATEADD(DAY,1,@EndDt)
	order by a.ApplicationId desc;
end

GO


create proc spGetRejectableApplicationByUser
@ApplicantId int,
@StartDt date,
@EndDt date
as
begin
	select a.ApplicationId as Tracking,t.ApplicationTypeText as 'Type',a.Description,
	s.ApplicationStatusText as 'Status',convert(nvarchar,a.CreatedDate,106) as CreatedDate,
	a.FileName,CASE a.ApplicationStatusId WHEN 3 THEN replace(a.FileName,'.pdf','_approved.pdf') ELSE a.FileName END as ApprovedFileName
	from Application a 
	inner join ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	inner join ApplicationStatus s on a.ApplicationStatusId = s.ApplicationStatusId
	where ApplicantId = @ApplicantId and CreatedDate between @StartDt and DATEADD(DAY,1,@EndDt)
	and a.ApplicationStatusId IN (2,3)
	order by a.ApplicationId desc;
end

GO

alter proc spGetApplicationInfoById
@ApplicationId int
as
begin
	select a.ApplicationId,a.ApplicantId,m.DisplayName as 'Initiator',ISNULL(a.Title,'') as Title,
	ISNULL(a.Description,'') as Description,ISNULL(a.Amount,0) as Amount,t.ApplicationTypeId,
	t.ApplicationTypeText as 'Type',a.ApplicationStatusId,s.ApplicationStatusText as 'Status',a.FileName,
	replace(a.FileName,'.pdf','_approved.pdf') as ApprovedFileName,convert(nvarchar,a.CreatedDate,106) as CreatedDate
	from Application a 
	inner join ModuleUser m on a.ApplicantId = m.ModuleUserId
	inner join ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	inner join ApplicationStatus s on a.ApplicationStatusId = s.ApplicationStatusId
	where ApplicationId = @ApplicationId;
end

-- exec spGetApplicationInfoById 41722

GO

alter proc spGetApplicationWorkFlowInfo
@ApplicationId int
as
begin
	select f.ProcessFlowId,f.ApplicationId,f.ApproverId,m.DisplayName as Approver,f.RoleId,r.RoleText as Role,
	f.Sequence,f.ProcessFlowDecisionId,fd.ProcessFlowDecisionText as 'Status',convert(nvarchar,f.DecisionDate,106) as DecisionDate,DecisionDate as CreatedOn,
	ISNULL(f.Comment,'') as Comment
	from ProcessFlow f 
	inner join ModuleUser m on f.ApproverId= m.ModuleUserId
	inner join Role r on f.RoleId = r.RoleId
	inner join ProcessFlowDecision fd on f.ProcessFlowDecisionId = fd.ProcessFlowDecisionId
	where ApplicationId = @ApplicationId
	order by Sequence;
end

-- exec spGetApplicationWorkFlowInfo 41941           

GO

alter proc spGetTransferableApplications
@ModuleUserId int
as
begin
	select a.ApplicationId,f.ProcessFlowId,a.ApplicantId,t.ApplicationTypeText as Type,dbo.fnGetUserNameById(a.ApplicantId) as Initiator,
	convert(nvarchar,a.CreatedDate,106) as CreatedDate,a.Description,f.ApproverId,dbo.fnGetUserNameById(f.ApproverId) as Approver,d.ProcessFlowDecisionText as 'Status'
	from Application a inner join ProcessFlow f on a.ApplicationId = f.ApplicationId
	inner join ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	inner join ProcessFlowDecision d on f.ProcessFlowDecisionId = d.ProcessFlowDecisionId
	Where f.ApproverId = @ModuleUserId and f.ProcessFlowDecisionId = 4
end

-- exec spGetTransferableApplications 361
GO

alter proc spTransferApplication
@ApplicationId int,
@ProcessFlowId int,
@Comment nvarchar(500),
@TApproverId int,
@ModuleUserId int
as
begin
	declare @ApplicantId as int set @ApplicantId = 0;
	declare @PApproverId as int set @PApproverId = 0;
	declare @RoleId as int Set @RoleId = 0;
	declare @Sequence as int set @Sequence = 0;
	declare @ProcessFlowDecisionId as int set @ProcessFlowDecisionId = 0;

	select @PApproverId = f.ApproverId,@RoleId = f.RoleId,@Sequence = f.Sequence,
	@ProcessFlowDecisionId = f.ProcessFlowDecisionId
	from ProcessFlow f where f.ProcessFlowId = @ProcessFlowId

	insert into ProcessTransfer(ProcessFlowId,ApplicationId,PApproverId,RoleId,Sequence,ProcessFlowDecisionId,TApproverId,TransferBy,TransferDate,Comment)
	values(@ProcessFlowId,@ApplicationId,@PApproverId,@RoleId,@Sequence,@ProcessFlowDecisionId,@TApproverId,@ModuleUserId,GETDATE(),@Comment);

	Update ProcessFlow set ApproverId = @TApproverId where ProcessFlowId = @ProcessFlowId;

	select @ApplicationId as ApplicationId,@ProcessFlowId as ProcessFlowId,@PApproverId as PApproverId,@TApproverId as TApproverId,@Comment as Comment
end


GO

--------------------- Monitoring ---------------------------------

GO

create function fnCountPendingTaskByUserId(@ModuleUserId int)
returns int
as
begin
	declare @count as int set @count = 0;

	select @count = count(*) from ProcessFlow f where f.ApproverId = @ModuleUserId and f.ProcessFlowDecisionId = 4;

	return @count;
end

-- select dbo.fnCountPendingTaskByUserId(332)

GO

create function fnCountPerformedTaskByUserId(@ModuleUserId int)
returns int
as
begin
	declare @count as int set @count = 0;
	select @count = count(*) from ProcessFlow f where f.ApproverId = @ModuleUserId and f.ProcessFlowDecisionId <> 4;

	return @count;
end

-- select dbo.fnCountPerformedTaskByUserId(332)

GO

GO
alter proc spGetEmpTaskCountList
as
begin
	/*
	select  ModuleUserId, DisplayName + ' ( ' + convert(nvarchar,Total) + ' )' as UserName from (
	select m.ModuleUserId,m.DisplayName, count(*) as Total
	from ProcessFlow f inner join ModuleUser m on f.ApproverId = m.ModuleUserId
	where f.ProcessFlowDecisionId = 4
	group by m.DisplayName,m.ModuleUserId
	) as x order by Total desc;
	*/


	Select  ModuleUserId, DisplayName + ' P:' + convert(nvarchar,PendingTask) + '|C:' + convert(nvarchar,CompletedTask) + ' )' as UserName 
	from (
	select m.ModuleUserId,m.DisplayName,dbo.fnCountPendingTaskByUserId(m.ModuleUserId) as PendingTask,
	dbo.fnCountPerformedTaskByUserId(m.ModuleUserId) as CompletedTask
	from ModuleUser m where m.IsVisible=1
	and m.ModuleUserId not in (239,252,270,374)
	) as x order by PendingTask desc;

end

GO

alter proc spGetPendingTaskListByUser
@ModuleUserId int
as
begin
	select p.ProcessFlowId,p.ApproverId,a.ApplicationId,a.FileName,replace(a.FileName,'.pdf','_approved.pdf') as ApprovedFileName,
	t.ApplicationTypeText as Type,a.Description,m.DisplayName as Initiator,s.ApplicationStatusText as Status,
	convert(nvarchar,a.CreatedDate,106) as CreatedDate,p.ProcessFlowDecisionId,d.ProcessFlowDecisionText as Decision,
	ISNULL(p.Comment,'') as Comment,p.DecisionDate
	from Application a 
	inner join ProcessFlow p on a.ApplicationId = p.ApplicationId
	inner join ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	inner join ModuleUser m on a.ApplicantId = m.ModuleUserId
	inner join ApplicationStatus s on a.ApplicationStatusId = s.ApplicationStatusId
	inner join ProcessFlowDecision d on p.ProcessFlowDecisionId = d.ProcessFlowDecisionId
	Where a.ApplicationStatusId IN (2) and p.ApproverId = @ModuleUserId and p.ProcessFlowDecisionId IN (4)
	order by a.CreatedDate
end

-- exec spGetPendingTaskListByUser 332

select * from ModuleUser where DisplayName like '%Riyad%'

GO

alter proc spGetPerformedTaskListByUser
@ModuleUserId int
as
begin
	select p.ProcessFlowId,p.ApproverId,a.ApplicationId,a.FileName,	replace(a.FileName,'.pdf','_approved.pdf') as ApprovedFileName,
	t.ApplicationTypeText as Type,a.Description,m.DisplayName as Initiator,s.ApplicationStatusText as Status,
	convert(nvarchar,a.CreatedDate,106) as CreatedDate,p.ProcessFlowDecisionId,d.ProcessFlowDecisionText as Decision,
	ISNULL(p.Comment,'') as Comment,p.DecisionDate
	from Application a 
	inner join ProcessFlow p on a.ApplicationId = p.ApplicationId
	inner join ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	inner join ModuleUser m on a.ApplicantId = m.ModuleUserId
	inner join ApplicationStatus s on a.ApplicationStatusId = s.ApplicationStatusId
	inner join ProcessFlowDecision d on p.ProcessFlowDecisionId = d.ProcessFlowDecisionId
	Where a.ApplicationStatusId NOT IN (2) and p.ApproverId = @ModuleUserId and p.ProcessFlowDecisionId NOT IN (4)
	order by a.CreatedDate
end

-- exec spGetPerformedTaskListByUser 332

GO
-- exec spFindTasksAndStatus '',6,0,1,'01/01/2020','06/16/2023'
-- select * from ApplicationType where ApplicationTypeText like '%Interest Rate%'
-- select * from ProcessFlowDecision

alter proc spFindTasksAndStatus
@Description nvarchar(100),
@ApplicationTypeId int,
@ApproverId int,
@ProcessFlowDecisionId int,
@StartDate date,
@EndDate date
as
begin

	declare @ApplicationTypeIdParam as nvarchar(100) set @ApplicationTypeIdParam = '';
	declare @ApplicationTypeTextParam as nvarchar(100) set @ApplicationTypeTextParam = '';
	declare @ApproverIdParam as nvarchar(50) set @ApproverIdParam = '';
	declare @ProcessFlowDecisionIdParam as nvarchar(50) set @ProcessFlowDecisionIdParam = '';


	if @ApplicationTypeId = 0
		set @ApplicationTypeTextParam = '%';
	else
	begin
		select @ApplicationTypeTextParam = ApplicationTypeText from ApplicationType where ApplicationTypeId = @ApplicationTypeId;
		--select ApplicationTypeText from ApplicationType where ApplicationTypeId = @ApplicationTypeId;
		set @ApplicationTypeTextParam = '%' + convert(nvarchar,@ApplicationTypeTextParam) + '%';
		--print @ApplicationTypeId;
		--print @ApplicationTypeTextParam;
		--print @StartDate;
		--print @EndDate;
	end

	if @ApproverId = 0
		set @ApproverIdParam = '%';
	else
		set @ApproverIdParam = '%' + convert(nvarchar,@ApproverId) + '%';

	if @ProcessFlowDecisionId = 0
		set @ProcessFlowDecisionIdParam = '%';
	else
		set @ProcessFlowDecisionIdParam = '%' + convert(nvarchar,@ProcessFlowDecisionId) + '%';

	--select @ProcessFlowDecisionIdParam;

	select p.ProcessFlowId,p.ApproverId,a.ApplicationId,a.FileName,replace(a.FileName,'.pdf','_approved.pdf') as ApprovedFileName,
	t.ApplicationTypeId,t.ApplicationTypeText as Type,a.Description,m.DisplayName as Initiator,s.ApplicationStatusText as Status,
	convert(nvarchar,a.CreatedDate,106) as CreatedDate,p.ProcessFlowDecisionId,d.ProcessFlowDecisionText as Decision,
	dbo.fnGetUserNameById(p.ApproverId) as 'Authorizer',ISNULL(p.Comment,'') as Comment,convert(nvarchar,p.DecisionDate,106) as DecisionDate
	from dbo.Application a 
	inner join dbo.ProcessFlow p on a.ApplicationId = p.ApplicationId
	inner join dbo.ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	inner join dbo.ModuleUser m on a.ApplicantId = m.ModuleUserId
	inner join dbo.ApplicationStatus s on a.ApplicationStatusId = s.ApplicationStatusId
	inner join dbo.ProcessFlowDecision d on p.ProcessFlowDecisionId = d.ProcessFlowDecisionId
	Where 
	t.ApplicationTypeText like @ApplicationTypeTextParam
	and (t.ApplicationTypeText like '%' + @Description + '%' or a.Description like '%' + @Description + '%' or p.Comment like '%' + @Description + '%')
	and p.ApproverId like @ApproverIdParam
	and p.ProcessFlowDecisionId like @ProcessFlowDecisionIdParam
	and a.CreatedDate between @StartDate and DATEADD(day,1,@EndDate)
	and a.ApplicationTypeId not in (38,42,50,52,60,61,70,121,200)
	order by a.CreatedDate
end

GO
--select * from ProcessFlow where ApproverId = 332

GO

--------------------- Monitoring ---------------------------------

--------------------- Report Queries -----------------------------

alter proc rspInitiatedByUser
@ModuleUserId int,
@start_dt date,
@end_dt date
as
begin
	select a.ApplicationId,a.ApplicantId,dbo.fnGetUserNameById(a.ApplicantId) as Initiator,a.Description,t.ApplicationTypeText Type,
	s.ApplicationStatusText as Status,convert(nvarchar,a.CreatedDate,106) as CreatedDate
	from Application a 
	inner join ApplicationType t on a.ApplicationTypeId = t.ApplicationTypeId
	inner join ApplicationStatus s on a.ApplicationStatusId = s.ApplicationStatusId
	where a.ApplicantId = @ModuleUserId and CreatedDate between @start_dt and DATEADD(DAY,1,@end_dt)
end

-- exec rspInitiatedByUser 361,'4/1/2022','4/30/2022'
GO


--------------------- Report Queries -----------------------------
-- drop proc spGetProcessFlowDecisionList
Create proc spGetProcessFlowDecisionList
as
begin
	select ProcessFlowDecisionId,ProcessFlowDecisionText from ProcessFlowDecision
	order by ProcessFlowDecisionText;
end
-- exec spGetProcessFlowDecisionList
