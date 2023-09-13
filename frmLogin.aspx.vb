Imports System
Imports System.Collections
Imports System.Configuration
Imports System.Data
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Runtime.InteropServices
Imports System.Data.SqlClient
Imports System.Net.Dns
Imports System.Security.Principal
Imports System.Security.Cryptography
Imports System.DirectoryServices
Imports System.DirectoryServices.ActiveDirectory
Imports System.DirectoryServices.AccountManagement

Partial Class frmLogin
    Inherits System.Web.UI.Page

    Dim Cypher As New clsCaesarCypher()
    Dim EmpInfoData As New clsEmployeeInfo()
    Dim DashBoardData As New clsDashBoard()
    Dim ModuleUserData As New clsModuleUser()

    Private Sub MessageBox(ByVal strMsg As String)
        Dim lbl As New System.Web.UI.WebControls.Label
        lbl.Text = "<script language='javascript'>" & Environment.NewLine _
                   & "window.alert(" & "'" & strMsg & "'" & ")</script>"
        Page.Controls.Add(lbl)
    End Sub

    <DllImport("ADVAPI32.dll", EntryPoint:="LogonUserW", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Public Shared Function LogonUser(ByVal lpszUsername As String, ByVal lpszDomain As String, ByVal lpszPassword As String, ByVal dwLogonType As Integer, ByVal dwLogonProvider As Integer, ByRef phToken As IntPtr) As Boolean
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            FailureText.Text = "Use Your Windows ID & Password. 3 Bad attemps will Lock your windows ID."
            Session("ModuleUserId") = ""
            Session("Guid") = ""
            Session("DisplayName") = ""
            Session("UserName") = ""
            Session("Email") = ""
        End If
    End Sub

    Protected Sub LoginButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LoginButton.Click
        Try
            Dim module_user As New clsModuleUser()

            If AuthenticateUser(UserName.Text, Password.Text) = False Then
                MsgBox("Provide Correct Credentials")
            Else
                module_user = ModuleUserData.fnCheckUserLogin(UserName.Text)
                Session("ModuleUserId") = module_user.ModuleUserId
                Session("Guid") = module_user.Guid
                Session("DisplayName") = module_user.DisplayName
                Session("UserName") = module_user.UserName
                Session("Email") = module_user.Email

                Response.Redirect("frmInitiate.aspx")
            End If
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Function AuthenticateUser(ByVal user As String, ByVal pass As String) As Boolean

        Dim value As Boolean = False

        Try
            Using pc = New PrincipalContext(ContextType.Domain, "MFILBD.COM", "DC=MFILBD,DC=COM")
                value = pc.ValidateCredentials(user, pass)

                If value = False Then
                    Return value
                End If
            End Using
            Return value
        Catch ex As Exception
            MessageBox(ex.Message)
            Return False
        End Try
    End Function

End Class
