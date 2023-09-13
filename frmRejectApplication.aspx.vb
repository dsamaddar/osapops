Imports System.Net.Mail

Partial Class frmRejectApplication
    Inherits System.Web.UI.Page

    Dim ApplicationData As New clsApplication()
    Dim ProcessFlowData As New clsProcessFlow()

    Protected Sub btnReject_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReject.Click
        Try
            Dim lblApplicationId As New Label
            Dim MailProp As New clsMailProperty()
            Dim result As New clsResult()

            lblApplicationId = grdInitiatedApplications.SelectedRow.FindControl("lblApplicationId")

            If lblApplicationId.Text = "" Then
                MessageBox("Select an application first.")
                Exit Sub
            End If

            result = ApplicationData.fnPermanentRejectApplication(Convert.ToInt32(lblApplicationId.Text), txtRejectionRemarks.Text)
            If result.Success = True Then
                MailProp = ApplicationData.fnGetOsapPermanentRejectionMail(Convert.ToInt32(lblApplicationId.Text), txtRejectionRemarks.Text)
                SendMail(MailProp)
                ClearForm()
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

            Dim mail_to_adr As String() = MailProp.MailTo.Split(New Char() {";"c})
            Dim mail_to As String
            For Each mail_to In mail_to_adr
                If mail_to <> "" Then
                    mail.CC.Add(mail_to)
                End If
            Next

            Dim mail_cc_adr As String() = MailProp.MailCC.Split(New Char() {";"c})
            Dim mail_cc As String
            For Each mail_cc In mail_cc_adr
                If mail_cc <> "" Then
                    mail.CC.Add(mail_cc)
                End If
            Next

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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ModuleUserId As String = Session("ModuleUserId")

        If ModuleUserId = "" Then
            Response.Redirect("~\frmLogin.aspx")
        End If

        If Not IsPostBack Then
            txtDateFrom.Text = Now.Month.ToString() & "/1/" & Now.Year.ToString()
            txtDateTo.Text = Now.Date
            btnReject.Enabled = False
            GetInitiatedList(Convert.ToInt32(Session("ModuleUserId")), Convert.ToDateTime(txtDateFrom.Text), Convert.ToDateTime(txtDateTo.Text))

        End If
    End Sub

    Protected Sub GetInitiatedList(ByVal ApplicantId As Integer, ByVal StartDt As Date, ByVal EndDt As Date)
        grdInitiatedApplications.DataSource = ApplicationData.fnGetRejectableApplicationByUser(ApplicantId, StartDt, EndDt)
        grdInitiatedApplications.DataBind()
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
        txtRejectionRemarks.Text = ""
        btnReject.Enabled = False

        GetInitiatedList(Convert.ToInt32(Session("ModuleUserId")), Convert.ToDateTime(txtDateFrom.Text), Convert.ToDateTime(txtDateTo.Text))
    End Sub

    Protected Sub grdInitiatedApplications_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdInitiatedApplications.SelectedIndexChanged
        Try
            Dim lblApplicationId As New Label

            lblApplicationId = grdInitiatedApplications.SelectedRow.FindControl("lblApplicationId")
            GetDocumentWorkFlow(Convert.ToInt32(lblApplicationId.Text))
            'btnReject.Enabled = True
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub GetDocumentWorkFlow(ByVal ApplicationId As Integer)
        grdDocumentWorkflow.DataSource = ProcessFlowData.fnGetApplicationWorkFlowInfo(ApplicationId)
        grdDocumentWorkflow.DataBind()
    End Sub


    Private Sub MessageBox(ByVal strMsg As String)
        Dim lbl As New System.Web.UI.WebControls.Label
        lbl.Text = "<script language='javascript'>" & Environment.NewLine _
                   & "window.alert(" & "'" & strMsg & "'" & ")</script>"
        Page.Controls.Add(lbl)
    End Sub

End Class
