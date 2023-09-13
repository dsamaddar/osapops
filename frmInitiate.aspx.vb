Imports System.Data
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Net.Mail
Imports System.Threading

Partial Class frmInitiate
    Inherits System.Web.UI.Page

    Dim ApplicationTypeData As New clsApplicationType()
    Dim ModuleUserData As New clsModuleUser()
    Dim RoleData As New clsRole()
    Dim ApplicationData As New clsApplication()
    Dim ProcessFlowData As New clsProcessFlow()
    Dim Common As New clsCommon()

    Public Shared Sequence As Integer = 0

    Protected Function FormatApprovalFlowTable() As DataTable
        Dim dt As DataTable = New DataTable()
        dt.Columns.Add("sequence_id", System.Type.GetType("System.Decimal"))
        dt.Columns.Add("module_user_id", System.Type.GetType("System.Decimal"))
        dt.Columns.Add("module_user", System.Type.GetType("System.String"))
        dt.Columns.Add("role_id", System.Type.GetType("System.Decimal"))
        dt.Columns.Add("role_name", System.Type.GetType("System.String"))
        Return dt
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ModuleUserId As String = Session("ModuleUserId")

        If ModuleUserId = "" Then
            Response.Redirect("~\frmLogin.aspx")
        End If

        If Not IsPostBack Then
            GetApplicationTypeList()
            GetModuleUserList()
            GetRoleList()

            Sequence = 0
            btnSave.Enabled = False
            txtDateFrom.Text = Now.Month.ToString() & "/1/" & Now.Year.ToString()
            txtDateTo.Text = Now.Date

            GetInitiatedList(Convert.ToInt32(Session("ModuleUserId")), Convert.ToDateTime(txtDateFrom.Text), Convert.ToDateTime(txtDateTo.Text))

            Session("dtApprovalFlow") = ""

            Dim dtApprovalFlow As DataTable = New DataTable()
            dtApprovalFlow = FormatApprovalFlowTable()
            Session("dtApprovalFlow") = dtApprovalFlow

        End If
    End Sub

    Protected Function AddApprovalFlow(ByVal ProcessFlow As clsProcessFlow) As DataTable

        Dim dtApprovalFlow As DataTable = New DataTable()
        dtApprovalFlow = Session("dtApprovalFlow")

        '' Chq If Item Already Exists
        If ChqItemAlreadyExists(ProcessFlow.ApproverId) = 1 Then
            MessageBox("Approver Already In The List.")
            Return dtApprovalFlow
        End If

        Dim dr As DataRow
        dr = dtApprovalFlow.NewRow()

        Sequence += 1
        dr("sequence_id") = Sequence
        dr("module_user_id") = ProcessFlow.ApproverId
        dr("module_user") = ProcessFlow.ApproverName
        dr("role_id") = ProcessFlow.RoleId
        dr("role_name") = ProcessFlow.RoleName

        dtApprovalFlow.Rows.Add(dr)
        dtApprovalFlow.AcceptChanges()
        btnSave.Enabled = True
        Return dtApprovalFlow

    End Function

    Protected Function ChqItemAlreadyExists(ByVal ApproverId As Integer) As Integer

        Dim dtApprovalFlow As DataTable = New DataTable()
        dtApprovalFlow = Session("dtApprovalFlow")

        Dim IsExists As Boolean = False
        Dim ExistingApproverId As String = ""

        For Each rw As DataRow In dtApprovalFlow.Rows
            ExistingApproverId = rw.Item("module_user_id")

            If ExistingApproverId = ApproverId Then
                IsExists = True
                Exit For
            End If
        Next

        If IsExists = True Then
            Return 1
        Else
            Return 0
        End If

    End Function

    Private Sub MessageBox(ByVal strMsg As String)
        Dim lbl As New System.Web.UI.WebControls.Label
        lbl.Text = "<script language='javascript'>" & Environment.NewLine _
                   & "window.alert(" & "'" & strMsg & "'" & ")</script>"
        Page.Controls.Add(lbl)
    End Sub

    Protected Sub GetApplicationTypeList()
        drpDocumentType.DataTextField = "ApplicationTypeText"
        drpDocumentType.DataValueField = "ApplicationTypeId"
        drpDocumentType.DataSource = ApplicationTypeData.fnGetApplicationTypeList()
        drpDocumentType.DataBind()
    End Sub

    Protected Sub GetModuleUserList()
        drpUserList.DataTextField = "DisplayName"
        drpUserList.DataValueField = "ModuleUserId"
        drpUserList.DataSource = ModuleUserData.fnGetModuleUserList()
        drpUserList.DataBind()
    End Sub

    Protected Sub GetRoleList()
        drpRoleList.DataTextField = "RoleText"
        drpRoleList.DataValueField = "RoleId"
        drpRoleList.DataSource = RoleData.fnGetRoleList()
        drpRoleList.DataBind()
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Try

            Dim ProcessFlow As New clsProcessFlow()

            ProcessFlow.ApproverId = drpUserList.SelectedValue
            ProcessFlow.ApproverName = drpUserList.SelectedItem.Text
            ProcessFlow.RoleId = drpRoleList.SelectedValue
            ProcessFlow.RoleName = drpRoleList.SelectedItem.Text

            Dim dtApprovalFlow As DataTable = New DataTable()

            dtApprovalFlow = AddApprovalFlow(ProcessFlow)
            Session("dtApprovalFlow") = dtApprovalFlow

            grdApprovalFlow.DataSource = dtApprovalFlow
            grdApprovalFlow.DataBind()

            If grdApprovalFlow.Rows.Count > 0 Then
                btnSave.Enabled = True
            Else
                btnSave.Enabled = False
            End If
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try

    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Try
            Dim app_info As New clsApplication()
            Dim result As New clsResult()
            Dim MailProp As New clsMailProperty()
            Dim file_name As String = Session("UserName") & "_" & Guid.NewGuid.ToString() & ".pdf"
            Dim counter As Integer = 0
            Dim ProcessFlowDecisionId As Integer = 0
            Dim data_type As New clsDataType()
            Dim sleep_time As Integer = 0


            If Not flupAttachment.HasFile Then
                MessageBox("Remember to attach file!")
                Exit Sub
            End If

            'slowing down application for specific application type
            data_type = Common.fnGetValue("spGetApplicationSleepTime", drpDocumentType.SelectedValue)
            sleep_time = Convert.ToInt32(data_type.data) * 1000
            Thread.Sleep(sleep_time)

            app_info.ApplicantId = Convert.ToInt32(Session("ModuleUserId"))
            app_info.Description = txtDescription.Text
            app_info.ApplicationTypeId = drpDocumentType.SelectedValue
            app_info.ApplicationStatusId = 2
            app_info.FileName = file_name
            app_info.CreatedBy = Convert.ToInt32(Session("ModuleUserId"))

            Dim DocumentWorkFlow As String = ""

            Dim lblApproverId, lblRoleId, lblSequence As New System.Web.UI.WebControls.Label()
            For Each rw As GridViewRow In grdApprovalFlow.Rows
                lblApproverId = rw.FindControl("lblmodule_user_id")
                lblRoleId = rw.FindControl("lblrole_id")
                lblSequence = rw.FindControl("lblsequence_id")

                If counter = 0 Then
                    ProcessFlowDecisionId = 4
                Else
                    ProcessFlowDecisionId = 2
                End If
                DocumentWorkFlow += lblApproverId.Text & "~" & lblRoleId.Text & "~" & lblSequence.Text & "~" & ProcessFlowDecisionId.ToString() & "~|"
                counter += 1
            Next

            app_info.DocumentWorkFlow = DocumentWorkFlow

            result = ApplicationData.fnInitiateApplication(app_info)

            If result.Success = True Then
                FtpUpload(file_name)
                MailProp = ApplicationData.fnGetOsapInitiatorMail(result.ApplicationId)
                SendMail(MailProp)
                ClearDocUpload()
            End If

            MessageBox(result.Message)

        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub SendMail(ByVal MailProp As clsMailProperty)
        Dim mail As New Net.Mail.MailMessage()
        Try
            mail.From = New MailAddress(MailProp.MailFrom)
            mail.To.Add(MailProp.MailTo)
            mail.CC.Add(MailProp.MailCC)
            mail.Bcc.Add(MailProp.MailBCC)
            mail.Subject = MailProp.MailSubject
            mail.Body = MailProp.MailBody
            mail.IsBodyHtml = True
            Dim smtp As New SmtpClient("192.168.1.14", 25)
            smtp.Send(mail)
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub grdApprovalFlow_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles grdApprovalFlow.RowDeleting
        Dim i As Integer
        Dim dtApprovalFlow As DataTable = New DataTable()

        dtApprovalFlow = Session("dtApprovalFlow")

        i = e.RowIndex

        Dim del_seq As Integer = 0
        del_seq = Convert.ToInt32(dtApprovalFlow.Rows(i).Item("sequence_id").ToString())

        If del_seq < Sequence Then
            MessageBox("Delete Last Item/Approver First.")
            Exit Sub
        End If

        dtApprovalFlow.Rows(i).Delete()
        dtApprovalFlow.AcceptChanges()

        Session("dtApprovalFlow") = dtApprovalFlow

        grdApprovalFlow.DataSource = dtApprovalFlow
        grdApprovalFlow.DataBind()

        Sequence -= 1

        If grdApprovalFlow.Rows.Count > 0 Then
            btnSave.Enabled = True
        Else
            btnSave.Enabled = False
        End If
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        ClearForm()
        GetInitiatedList(Convert.ToInt32(Session("ModuleUserId")), Convert.ToDateTime(txtDateFrom.Text), Convert.ToDateTime(txtDateTo.Text))
    End Sub

    Protected Sub ClearForm()
        grdInitiatedApplications.SelectedIndex = -1
        grdInitiatedApplications.DataSource = ""
        grdInitiatedApplications.DataBind()

        grdDocumentWorkflow.DataSource = ""
        grdDocumentWorkflow.DataBind()
    End Sub

    Protected Sub ClearDocUpload()
        Try
            Sequence = 0
            grdInitiatedApplications.SelectedIndex = -1
            grdInitiatedApplications.DataSource = ""
            grdInitiatedApplications.DataBind()

            grdDocumentWorkflow.DataSource = ""
            grdDocumentWorkflow.DataBind()

            drpDocumentType.SelectedIndex = -1
            drpRoleList.SelectedIndex = -1
            drpUserList.SelectedIndex = -1
            txtDescription.Text = ""

            Session("dtApprovalFlow") = ""
            Dim dtApprovalFlow As DataTable = New DataTable()
            dtApprovalFlow = FormatApprovalFlowTable()
            Session("dtApprovalFlow") = dtApprovalFlow

            grdApprovalFlow.DataSource = ""
            grdApprovalFlow.DataBind()

            GetInitiatedList(Convert.ToInt32(Session("ModuleUserId")), Convert.ToDateTime(txtDateFrom.Text), Convert.ToDateTime(txtDateTo.Text))
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub GetInitiatedList(ByVal ApplicantId As Integer, ByVal StartDt As Date, ByVal EndDt As Date)
        grdInitiatedApplications.DataSource = ApplicationData.fnGetInitiatedApplicationByUser(ApplicantId, StartDt, EndDt)
        grdInitiatedApplications.DataBind()
    End Sub

    Protected Sub grdInitiatedApplications_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdInitiatedApplications.SelectedIndexChanged
        Try
            Dim lblApplicationId As New Label

            lblApplicationId = grdInitiatedApplications.SelectedRow.FindControl("lblApplicationId")
            GetDocumentWorkFlow(Convert.ToInt32(lblApplicationId.Text))
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub GetDocumentWorkFlow(ByVal ApplicationId As Integer)
        grdDocumentWorkflow.DataSource = ProcessFlowData.fnGetApplicationWorkFlowInfo(ApplicationId)
        grdDocumentWorkflow.DataBind()
    End Sub

    Protected Sub SimpleFTPUpload(ByVal file_name As String)
        Try
            Dim ftp As String = ConfigurationManager.AppSettings("osap_ftp_storage")

            Dim Client As New WebClient()

            Client.Credentials = New NetworkCredential("ftpuser", "Farc1lgh#")
            Client.UploadFile(ftp & file_name, Path.GetFileName(flupAttachment.PostedFile.FileName))
            MessageBox("File Upload Complete")


        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Public Sub FtpUpload(ByVal file_name As String)
        Try
            Dim clsRequest As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(ConfigurationManager.AppSettings("osap_ftp_storage") & file_name), System.Net.FtpWebRequest)
            clsRequest.Credentials = New System.Net.NetworkCredential(ConfigurationManager.AppSettings("osap_ftp_user"), ConfigurationManager.AppSettings("osap_ftp_password"))
            clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile

            Dim fs As System.IO.Stream = flupAttachment.PostedFile.InputStream
            Dim br As New System.IO.BinaryReader(fs)
            Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Integer))

            Using writer As System.IO.Stream = clsRequest.GetRequestStream
                writer.Write(bytes, 0, bytes.Length)
                writer.Close()
            End Using
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try

    End Sub

End Class
