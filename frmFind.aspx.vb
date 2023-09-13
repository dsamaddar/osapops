Imports System.IO
Imports System.Drawing

Partial Class frmFind
    Inherits System.Web.UI.Page

    Dim ApplicationTypeData As New clsApplicationType()
    Dim ModuleUserData As New clsModuleUser()
    Dim ProcessFlowData As New clsProcessFlow()
    Dim ProcessFlowDecisionData As New clsProcessFlowDecision()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ModuleUserId As String = Session("ModuleUserId")

        If ModuleUserId = "" Then
            Response.Redirect("~\frmLogin.aspx")
        End If

        If Not IsPostBack Then
            GetApplicationTypeList()
            GetModuleUserList()
            GetProcessFlowDecisionList()
            txtDateFrom.Text = Now.Month.ToString() & "/01/" & Now.Year.ToString()
            txtDateTo.Text = Now.Date
        End If
    End Sub

    Protected Sub ExportToExcel(ByVal sender As Object, ByVal e As EventArgs)
        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition", "attachment;filename=osap_search_result.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Using sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)

            'To Export all pages
            grdSearchResult.HeaderRow.BackColor = Color.White
            For Each cell As TableCell In grdSearchResult.HeaderRow.Cells
                cell.BackColor = grdSearchResult.HeaderStyle.BackColor
            Next
            For Each row As GridViewRow In grdSearchResult.Rows
                row.BackColor = Color.White
                For Each cell As TableCell In row.Cells
                    If row.RowIndex Mod 2 = 0 Then
                        cell.BackColor = grdSearchResult.AlternatingRowStyle.BackColor
                    Else
                        cell.BackColor = grdSearchResult.RowStyle.BackColor
                    End If
                    cell.CssClass = "textmode"
                Next
            Next

            grdSearchResult.RenderControl(hw)
            'style to format numbers to string
            Dim style As String = "<style> .textmode { } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.[End]()
        End Using
    End Sub

    Public Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
        ' Verifies that the control is rendered
    End Sub

    Private Sub MessageBox(ByVal strMsg As String)
        Dim lbl As New System.Web.UI.WebControls.Label
        lbl.Text = "<script language='javascript'>" & Environment.NewLine _
                   & "window.alert(" & "'" & strMsg & "'" & ")</script>"
        Page.Controls.Add(lbl)
    End Sub

    Protected Sub GetApplicationTypeList()
        Try
            drpDocumentType.DataTextField = "ApplicationTypeText"
            drpDocumentType.DataValueField = "ApplicationTypeId"
            drpDocumentType.DataSource = ApplicationTypeData.fnGetApplicationTypeList()
            drpDocumentType.DataBind()

            Dim A As New ListItem
            A.Value = "0"
            A.Text = "ALL"

            drpDocumentType.Items.Insert(0, A)
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try

    End Sub

    Protected Sub GetModuleUserList()
        Try
            drpApprover.DataTextField = "DisplayName"
            drpApprover.DataValueField = "ModuleUserId"
            drpApprover.DataSource = ModuleUserData.fnGetModuleUserList()
            drpApprover.DataBind()

            Dim A As New ListItem
            A.Value = "0"
            A.Text = "ALL"

            drpApprover.Items.Insert(0, A)
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try

    End Sub

    Protected Sub GetProcessFlowDecisionList()
        Try
            drpTaskStatus.DataTextField = "ProcessFlowDecisionText"
            drpTaskStatus.DataValueField = "ProcessFlowDecisionId"
            drpTaskStatus.DataSource = ProcessFlowDecisionData.fnGetProcessFlowDecisionList()
            drpTaskStatus.DataBind()

            Dim A As New ListItem
            A.Value = "0"
            A.Text = "ALL"

            drpTaskStatus.Items.Insert(0, A)
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try

    End Sub

    Protected Sub ClearForm()
        drpApprover.SelectedIndex = -1
        drpDocumentType.SelectedIndex = -1
        drpTaskStatus.SelectedIndex = -1
        txtDateFrom.Text = Now.Month.ToString() & "/01/" & Now.Year.ToString()
        txtDateTo.Text = Now.Date
    End Sub

    Protected Sub btnFind_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFind.Click
        Try
            grdSearchResult.DataSource = ProcessFlowData.fnFindTasksAndStatus(txtDescription.Text, drpDocumentType.SelectedValue, drpApprover.SelectedValue, drpTaskStatus.SelectedValue, Convert.ToDateTime(txtDateFrom.Text), Convert.ToDateTime(txtDateTo.Text))
            grdSearchResult.DataBind()
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

End Class
