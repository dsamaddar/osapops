Imports System.Net.Mail

Partial Class frmTransfer
    Inherits System.Web.UI.Page

    Dim ModuleUserData As New clsModuleUser()
    Dim ApplicationData As New clsApplication()
    Dim ProcessTransferData As New clsProcessTransfer()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ModuleUserId As String = Session("ModuleUserId")

        If ModuleUserId = "" Then
            Response.Redirect("~\frmLogin.aspx")
        End If

        If Not IsPostBack Then
            GetTransferableApps(Session("ModuleUserId"))
            GetModuleUserList()
            btnTransfer.Enabled = False
        End If
    End Sub

    Protected Sub GetTransferableApps(ByVal ModuleUserId As Integer)
        Try
            grdTransferableApps.DataSource = ApplicationData.fnGetTransferableApplications(ModuleUserId)
            grdTransferableApps.DataBind()
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub GetModuleUserList()
        drpUserList.DataTextField = "DisplayName"
        drpUserList.DataValueField = "ModuleUserId"
        drpUserList.DataSource = ModuleUserData.fnGetModuleUserList()
        drpUserList.DataBind()
    End Sub

    Private Sub MessageBox(ByVal strMsg As String)
        Dim lbl As New System.Web.UI.WebControls.Label
        lbl.Text = "<script language='javascript'>" & Environment.NewLine _
                   & "window.alert(" & "'" & strMsg & "'" & ")</script>"
        Page.Controls.Add(lbl)
    End Sub

    Protected Sub btnTransfer_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTransfer.Click
        Try
            If grdTransferableApps.SelectedIndex = -1 Then
                MessageBox("Select an application first.")
                Exit Sub
            End If

            If txtComments.Text = "" Then
                MessageBox("Comment is required")
                Exit Sub
            End If

            Dim lblApplicationId, lblProcessFlowId, lblApproverId As New Label
            Dim result As New clsResult()
            Dim MailProp As New clsMailProperty()

            lblApplicationId = grdTransferableApps.SelectedRow.FindControl("lblApplicationId")
            lblProcessFlowId = grdTransferableApps.SelectedRow.FindControl("lblProcessFlowId")
            lblApproverId = grdTransferableApps.SelectedRow.FindControl("lblApproverId")

            Dim process_transfer As New clsProcessTransfer()
            process_transfer.ApplicationId = lblApplicationId.Text
            process_transfer.ProcessFlowId = lblProcessFlowId.Text
            process_transfer.Comment = txtComments.Text
            process_transfer.TApproverId = drpUserList.SelectedValue
            process_transfer.TransferBy = Session("ModuleUserId")

            result = ProcessTransferData.fnTransferApplication(process_transfer)

            If result.Success = True Then
                MailProp = ApplicationData.fnGetOsapTransferMail(result)
                SendMail(MailProp)
                ClearForm()
            End If

            MessageBox(result.Message)

        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub ClearForm()
        grdTransferableApps.SelectedIndex = -1
        txtComments.Text = ""
        drpUserList.SelectedIndex = -1
        btnTransfer.Enabled = False

        GetTransferableApps(Session("ModuleUserId"))
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

    Protected Sub grdTransferableApps_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdTransferableApps.SelectedIndexChanged
        Try
            Dim lblApproverId As New Label

            lblApproverId = grdTransferableApps.SelectedRow.FindControl("lblApproverId")
            drpUserList.SelectedValue = lblApproverId.Text
            btnTransfer.Enabled = True
            txtComments.Text = ""
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try

    End Sub

End Class
