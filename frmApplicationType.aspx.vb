
Partial Class frmApplicationType
    Inherits System.Web.UI.Page

    Dim ApplicationTypeData As New clsApplicationType()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ModuleUserId As String = Session("ModuleUserId")

        If ModuleUserId = "" Then
            Response.Redirect("~\frmLogin.aspx")
        End If

        If Not IsPostBack Then
            GetApplicationTypes()
            ClearForm()
        End If
    End Sub

    Protected Sub ClearForm()
        btnAdd.Enabled = True
        btnUpdate.Enabled = False

        hdFldApplicationTypeId.Value = ""
        txtApplicationType.Text = ""
        txtDescription.Text = ""
        txtEmail.Text = ""
        chkBxIsVisible.Checked = True
        grdAppType.SelectedIndex = -1
    End Sub

    Protected Sub GetApplicationTypes()
        grdAppType.DataSource = ApplicationTypeData.fnGetApplicationTypes()
        grdAppType.DataBind()
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Try
            Dim app_type As New clsApplicationType()
            Dim result As New clsResult()

            app_type.ApplicationTypeText = txtApplicationType.Text
            app_type.ApplicationTypeDescription = txtDescription.Text
            app_type.Email = txtEmail.Text

            If chkBxIsVisible.Checked = True Then
                app_type.IsVisible = True
            Else
                app_type.IsVisible = False
            End If

            result = ApplicationTypeData.fnInsertApplicationType(app_type)

            If result.Success = True Then
                ClearForm()
                GetApplicationTypes()
            End If

            MessageBox(result.Message)
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        Try
            If hdFldApplicationTypeId.Value = "" Then
                MessageBox("Remember to select an Item first.")
                Exit Sub
            End If

            Dim app_type As New clsApplicationType()
            Dim result As New clsResult()

            app_type.ApplicationTypeId = hdFldApplicationTypeId.Value
            app_type.ApplicationTypeText = txtApplicationType.Text
            app_type.ApplicationTypeDescription = txtDescription.Text
            app_type.Email = txtEmail.Text

            If chkBxIsVisible.Checked = True Then
                app_type.IsVisible = True
            Else
                app_type.IsVisible = False
            End If

            result = ApplicationTypeData.fnUpdateApplicationType(app_type)

            If result.Success = True Then
                ClearForm()
                GetApplicationTypes()
            End If

            MessageBox(result.Message)

        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub grdAppType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdAppType.SelectedIndexChanged
        Try
            Dim lblApplicationTypeId, lblApplicationTypeText, lblApplicationTypeDescription, lblEmail, lblIsVisible As New Label
            lblApplicationTypeId = grdAppType.SelectedRow.FindControl("lblApplicationTypeId")
            lblApplicationTypeText = grdAppType.SelectedRow.FindControl("lblApplicationTypeText")
            lblApplicationTypeDescription = grdAppType.SelectedRow.FindControl("lblApplicationTypeDescription")
            lblEmail = grdAppType.SelectedRow.FindControl("lblEmail")
            lblIsVisible = grdAppType.SelectedRow.FindControl("lblIsVisible")

            hdFldApplicationTypeId.Value = lblApplicationTypeId.Text
            txtApplicationType.Text = lblApplicationTypeText.Text
            txtDescription.Text = lblApplicationTypeDescription.Text
            txtEmail.Text = lblEmail.Text

            If lblIsVisible.Text = "True" Then
                chkBxIsVisible.Checked = True
            Else
                chkBxIsVisible.Checked = False
            End If

            btnAdd.Enabled = False
            btnUpdate.Enabled = True
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Private Sub MessageBox(ByVal strMsg As String)
        Dim lbl As New System.Web.UI.WebControls.Label
        lbl.Text = "<script language='javascript'>" & Environment.NewLine _
                   & "window.alert(" & "'" & strMsg & "'" & ")</script>"
        Page.Controls.Add(lbl)
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        ClearForm()
    End Sub
End Class
