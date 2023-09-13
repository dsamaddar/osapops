Imports System.Data
Imports System.Data.SqlClient
Imports CrystalDecisions.CrystalReports.Engine

Partial Class frmLoadRptConfig
    Inherits System.Web.UI.Page

    Dim RptConfig As New clsRptConfigParam()
    Dim RptMasterData As New clsRptConfigMast()
    Dim RptMaster As New clsRptConfigMast()
    Dim Common As New clsCommon()

    Protected Sub btnGenerate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGenerate.Click
        Dim params As String = ""
        Dim myReport As New ReportDocument()
        Dim folder As String
        Dim f As String
        Dim repName As String
        Dim dt As New DataSet()
        Dim serverName As [String], uid As [String], pwd As [String], dbName As [String]

        Dim conStr As String = ConfigurationManager.ConnectionStrings("OSAPCon").ConnectionString
        Dim retArr As [String](), usrArr As [String](), pwdArr As [String](), serverArr As [String](), dbArr As [String]()

        Try
            dt = RptConfig.fnGetRptConfigParams(Convert.ToInt32(txtFastPath.Text))

            f = "~/reports/"
            folder = Server.MapPath(f)
            repName = folder & hdfldReportFile.Value
            myReport.Load(repName)

            retArr = conStr.Split(";")
            serverArr = retArr(0).Split("=")
            dbArr = retArr(1).Split("=")
            usrArr = retArr(2).Split("=")
            pwdArr = retArr(3).Split("=")

            serverName = serverArr(1)
            uid = usrArr(1)
            pwd = pwdArr(1)
            dbName = dbArr(1)

            myReport.SetDatabaseLogon(uid, pwd, serverName, dbName)

            Dim parameters As CrystalDecisions.Web.Parameter = New CrystalDecisions.Web.Parameter()
            Dim txt As New TextBox

            For Each Row As DataRow In dt.Tables(0).Rows
                txt = CType(FindControlRecursive(Me, Row("control_id")), TextBox)

                If Row("parameter_type") = "date" And txt.Text = "" Then
                    txt.Text = Now.Date
                End If

                myReport.SetParameterValue(Row("parameter_name"), txt.Text)

                params &= Row("parameter_name") & " : " & txt.Text
            Next

            Common.fn_insert_rpt_gen_log(Convert.ToInt32(txtFastPath.Text), "system", params)
            myReport.ExportToHttpResponse(drpExportOptions.SelectedValue, Response, True, lblReportName.Text.Replace(" ", "_") & "_" & Now.Ticks)
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Function FindControlRecursive(ByVal ctrl As Control, ByVal id As String) As Control
        Dim c As Control = Nothing

        If ctrl.ID = id Then
            c = ctrl
        Else
            For Each childCtrl In ctrl.Controls
                Dim resCtrl As Control = FindControlRecursive(childCtrl, id)
                If resCtrl IsNot Nothing Then c = resCtrl
            Next
        End If

        Return c
    End Function

    Protected Sub txtFastPath_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtFastPath.TextChanged

        Try
            GetReportProfile(Convert.ToInt32(txtFastPath.Text))
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub GetDynamicControls()
        Try
            If txtFastPath.Text <> "" Then
                Dim dt As New DataSet()
                dt = RptConfig.fnGetRptConfigParams(Convert.ToInt32(txtFastPath.Text))

                For Each Row As DataRow In dt.Tables(0).Rows
                    AddControl(Row("control_id"), Row("parameter_label"), Row("is_mandatory"), Row("parameter_type"), Row("default_value"), Row("wild_card"))
                Next
            End If

        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub GetReportProfile(ByVal fast_path As Integer)
        Try
            RptMaster = RptMasterData.fnGetRptConfigMast(fast_path)

            hdfldReportFile.Value = RptMaster.report_file
            lblReportName.Text = RptMaster.function_name
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub AddControl(ByVal control_id As String, ByVal parameter_label As String, ByVal is_mandatory As Boolean, ByVal parameter_type As String, ByVal default_value As String, ByVal wild_card As String)
        Dim lt As New Literal()
        Dim lbl_str As String = ""

        Dim lbl, lbl_wild_card As New Label()
        lbl.ID = "lbl" & control_id

        lbl_str = parameter_label
        If is_mandatory = True Then
            lbl_str = " * " & lbl_str
        End If


        lbl_str = lbl_str.PadLeft(27, "-")
        lbl.Text = lbl_str & "  :  "


        pnlParameters.Controls.Add(lbl)

        Dim txt As New TextBox()
        txt.ID = control_id
        txt.Text = default_value
        txt.AutoCompleteType = AutoCompleteType.Disabled

        If txt.ID = "txtModuleUserId" Then
            txt.Text = Session("ModuleUserId")
            txt.Enabled = False
        End If

        If is_mandatory = True Then
            txt.ValidationGroup = "ParamValidation"
        End If

        If parameter_type = "date" Then
            txt.Attributes.Add("class", "datepicker")
            If default_value = "" Or default_value Is Nothing Then
                txt.Text = Now.Date
            End If
        End If

        pnlParameters.Controls.Add(txt)

        If wild_card <> "" Then
            lbl_wild_card.Text = " [" & wild_card & "]"
            pnlParameters.Controls.Add(lbl_wild_card)
        End If

        If is_mandatory = True Then
            AddValidator(control_id, parameter_label)
        End If

        lt.Text &= "<br /><br />"
        pnlParameters.Controls.Add(lt)

    End Sub

    Protected Sub AddValidator(ByVal control_id As String, ByVal parameter_label As String)
        Dim reqFldVal As New RequiredFieldValidator()

        reqFldVal.ID = "reqFldVal" & control_id
        reqFldVal.ControlToValidate = control_id
        reqFldVal.SetFocusOnError = True
        reqFldVal.ForeColor = Drawing.Color.Red
        reqFldVal.ErrorMessage = "  Required: " & parameter_label
        'reqFldVal.EnableClientScript = False
        reqFldVal.ValidationGroup = "ParamValidation"
        reqFldVal.Enabled = True
        pnlParameters.Controls.Add(reqFldVal)

    End Sub

    Private Sub MessageBox(ByVal strMsg As String)
        Dim lbl As New System.Web.UI.WebControls.Label
        lbl.Text = "<script language='javascript'>" & Environment.NewLine _
                   & "window.alert(" & "'" & strMsg & "'" & ")</script>"
        Page.Controls.Add(lbl)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ModuleUserId As String = Session("ModuleUserId")

        If ModuleUserId = "" Then
            Response.Redirect("~\frmLogin.aspx")
        End If

        GetDynamicControls()
        If Not IsPostBack Then
            If Request.QueryString("fast_path") <> "" Then
                txtFastPath.Text = Request.QueryString("fast_path")
                GetDynamicControls()
                GetReportProfile(Convert.ToInt32(txtFastPath.Text))
            End If

            GetReportList()
            SetFocus(txtFastPath)
            rdbtnExportOptions.SelectedIndex = 0
        End If

    End Sub

    Protected Sub GetReportList()
        bulReportList.DataTextField = "function_name"
        bulReportList.DataValueField = "fast_path"
        bulReportList.DataSource = Common.fnLoadDataSet("rspGetReportList")
        bulReportList.DataBind()
    End Sub
End Class
