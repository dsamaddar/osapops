Imports System.Web
Imports System.Web.Security
Imports System.Security.Principal
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Net

Partial Class frmView
    Inherits System.Web.UI.Page

    Dim ApplicationData As New clsApplication()
    Dim ProcessFlowData As New clsProcessFlow()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ModuleUserId As String = Session("ModuleUserId")

        If ModuleUserId = "" Then
            Response.Redirect("~\frmLogin.aspx")
        End If

        If Not IsPostBack Then

        End If
    End Sub

    Protected Sub GetAppInfo(ByVal tracking_no As Integer)
        Try
            Dim file_path As String = ""
            Dim app_info As New clsApplication()

            app_info = ApplicationData.fnGetApplicationInfoById(tracking_no)

            lblInitiationDate.Text = app_info.CreatedDate
            lblInitiator.Text = app_info.Initiator
            lblStatus.Text = app_info.Status
            txtDescription.Text = app_info.Description

            GetDocumentWorkFlow(tracking_no)

            If app_info.Status <> "Approved" Then
                'file_path = ConfigurationManager.AppSettings("osap_storage") & app_info.FileName
                'file_path = ConfigurationManager.AppSettings("osap_root_path") & app_info.FileName
                file_path = ConfigurationManager.AppSettings("osap_http_storage") & app_info.FileName
                LoadPDF(file_path)
            Else
                'file_path = ConfigurationManager.AppSettings("osap_storage") & app_info.ApprovedFileName
                'file_path = ConfigurationManager.AppSettings("osap_root_path") & app_info.ApprovedFileName
                file_path = ConfigurationManager.AppSettings("osap_http_storage") & app_info.ApprovedFileName
                LoadPDF(file_path)
            End If

        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub ShowPDF(ByVal file_name As String)
        
    End Sub

    Private Sub MessageBox(ByVal strMsg As String)
        Dim lbl As New System.Web.UI.WebControls.Label
        lbl.Text = "<script language='javascript'>" & Environment.NewLine _
                   & "window.alert(" & "'" & strMsg & "'" & ")</script>"
        Page.Controls.Add(lbl)
    End Sub

    Protected Sub LoadPDF(ByVal file_path As String)
        Try
            Dim embed As String = "<object data=""{0}"" type=""application/pdf"" width=""100%"" height=""450px"">"
            embed += "If you are unable to view file, you can download from <a href = ""{0}"">here</a>"
            embed += " or download <a target = ""_blank"" href = ""http://get.adobe.com/reader/"">Adobe PDF Reader</a> to view the file."
            embed += "</object>"
            ltEmbed.Text = String.Format(embed, ResolveUrl(file_path))
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub GetDocumentWorkFlow(ByVal ApplicationId As Integer)
        grdApprovalFlow.DataSource = ProcessFlowData.fnGetApplicationWorkFlowInfo(ApplicationId)
        grdApprovalFlow.DataBind()
    End Sub

    Protected Sub btnShow_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnShow.Click
        GetAppInfo(Convert.ToInt32(txtTrackingNo.Text))
    End Sub

    Protected Sub FTPViewPDF(ByVal sender As Object, ByVal e As EventArgs)
        Dim fileName As String = (TryCast(sender, LinkButton)).CommandArgument
        Dim ftp As String = "ftp://192.168.11.36/"
        Dim ftpFolder As String = ""

        Try
            Dim request As FtpWebRequest = CType(WebRequest.Create(ftp & ftpFolder & fileName), FtpWebRequest)
            request.Method = WebRequestMethods.Ftp.DownloadFile
            request.Credentials = New NetworkCredential("ftpuser", "Farc1lgh#")
            request.UsePassive = True
            request.UseBinary = True
            request.EnableSsl = False
            Dim webResponse As FtpWebResponse = CType(request.GetResponse(), FtpWebResponse)

            Using stream As MemoryStream = New MemoryStream()
                CopyStream(webResponse.GetResponseStream(), stream)
                Dim bytes As Byte() = stream.ToArray()
                Response.Cache.SetCacheability(HttpCacheability.NoCache)
                Response.ContentType = "application/pdf"
                Response.BinaryWrite(bytes)
                Response.Flush()
                Response.End()
            End Using

        Catch ex As WebException
            Throw New Exception((TryCast(ex.Response, FtpWebResponse)).StatusDescription)
        End Try
    End Sub

    Public Shared Sub CopyStream(ByVal input As Stream, ByVal output As Stream)
        Dim buffer As Byte() = New Byte(4095) {}
        While True
            Dim read As Integer = input.Read(buffer, 0, buffer.Length)
            If read <= 0 Then
                Return
            End If
            output.Write(buffer, 0, read)
        End While
    End Sub

End Class
