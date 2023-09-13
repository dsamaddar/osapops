
-- drop table tbl_rpt_config_mast
create table tbl_rpt_config_mast(
function_id int identity(1,1) primary key,
function_name nvarchar(50),
report_file nvarchar(50),
report_header nvarchar(200),
report_footer nvarchar(200),
fast_path int unique(fast_path)
);

GO

insert into tbl_rpt_config_mast(function_name,report_file,report_header,report_footer,fast_path)
values
('Initiated by Current User', 'rptInitiatedByUser.rpt','Initiated by Current User','',1001);

go
-- drop proc rspGetReportList
create proc rspGetReportList
as
select 'frmLoadRptConfig.aspx?fast_path=' + convert(nvarchar,fast_path) as fast_path,convert(nvarchar,fast_path) + ' - ' + function_name as function_name from tbl_rpt_config_mast;

go
-- exec rspGetReportList;

go

-- drop proc rspGetRptConfigMast
create proc rspGetRptConfigMast
@fast_path int
as
begin
	select * from tbl_rpt_config_mast where fast_path = @fast_path
end

go

-- drop table tbl_rpt_config_param;
create table tbl_rpt_config_param(
config_id int identity(1,1) primary key,
function_id int foreign key references tbl_rpt_config_mast(function_id),
control_id nvarchar(50),
parameter_label nvarchar(50),
parameter_name nvarchar(50),
parameter_type nvarchar(50),
parameter_sl int,
is_mandatory bit default 1,
default_value nvarchar(50),
wild_card nvarchar(50),
validation_exp nvarchar(50),
);

go

insert into tbl_rpt_config_param(function_id,control_id,parameter_label,parameter_name,parameter_type,parameter_sl,is_mandatory,default_value,wild_card,validation_exp)
values
(1,'txtModuleUserId','User ID','@ModuleUserId','text',1,1,'','',''),
(1,'txtStartDt','Start Date','@start_dt','date',2,1,'04/01/2022','',''),
(1,'txtEndDt','End Date','@end_dt','date',3,1,'04/17/2022','','');



--select * from tbl_rpt_config_mast;
--select * from tbl_rpt_config_param;

go

-- drop proc rspGetRptConfigParams
create procedure rspGetRptConfigParams
@fast_path int
as
begin
	select c.function_id,c.function_name,c.report_file,c.report_header,c.report_footer,p.control_id,p.parameter_label,
	p.parameter_name,p.parameter_type,p.parameter_sl,p.is_mandatory,p.default_value,p.wild_card,p.validation_exp
	from tbl_rpt_config_mast c inner join tbl_rpt_config_param p on c.function_id = p.function_id
	where c.fast_path = @fast_path
	order by p.parameter_sl;
end

go

-- exec rspGetRptConfigParams 1004
-- drop table tbl_rpt_gen_log;
create table tbl_rpt_gen_log(
id int identity(1,1),
fast_path int,
rpt_user nvarchar(50),
params nvarchar(200),
log_dt datetime default getdate()
);

go

-- drop proc rsp_insert_rpt_gen_log
create proc rsp_insert_rpt_gen_log
@fast_path int,
@rpt_user nvarchar(50),
@params nvarchar(200)
as
begin
	insert into tbl_rpt_gen_log(fast_path,rpt_user,params)
	values(@fast_path,@rpt_user,@params)
end

