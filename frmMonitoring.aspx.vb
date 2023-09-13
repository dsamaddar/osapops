
Partial Class frmMonitoring
    Inherits System.Web.UI.Page

    Dim Common As New clsCommon()
    Dim ProcessFlowData As New clsProcessFlow()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ModuleUserId As String = Session("ModuleUserId")

        If ModuleUserId = "" Then
            Response.Redirect("~\frmLogin.aspx")
        End If

        If Not IsPostBack Then
            GetEmpTaskCountList()
        End If
    End Sub

    Protected Sub GetEmpTaskCountList()
        bulReportList.DataTextField = "UserName"
        bulReportList.DataValueField = "ModuleUserId"
        bulReportList.DataSource = Common.fnLoadDataSet("spGetEmpTaskCountList")
        bulReportList.DataBind()
    End Sub

    Protected Sub bulReportList_Click(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.BulletedListEventArgs) Handles bulReportList.Click
        Try
            lblPendingTaskList.Text = "Pending task list of " & bulReportList.Items(e.Index).Text
            lblPerformedTask.Text = "Performed task list of " & bulReportList.Items(e.Index).Text
            GetPendingTaskListByUser(bulReportList.Items(e.Index).Value)
            GetPerformedTaskListByUser(bulReportList.Items(e.Index).Value)
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub GetPendingTaskListByUser(ByVal ModuleUserId As Integer)
        grdPendingTaskList.DataSource = ProcessFlowData.fnGetPendingTaskListByUser(ModuleUserId)
        grdPendingTaskList.DataBind()
    End Sub

    Protected Sub GetPerformedTaskListByUser(ByVal ModuleUserId As Integer)
        grdPerformedTaskList.DataSource = ProcessFlowData.fnGetPerformedTaskListByUser(ModuleUserId)
        grdPerformedTaskList.DataBind()
    End Sub

    Private Sub MessageBox(ByVal strMsg As String)
        Dim lbl As New System.Web.UI.WebControls.Label
        lbl.Text = "<script language='javascript'>" & Environment.NewLine _
                   & "window.alert(" & "'" & strMsg & "'" & ")</script>"
        Page.Controls.Add(lbl)
    End Sub
End Class
