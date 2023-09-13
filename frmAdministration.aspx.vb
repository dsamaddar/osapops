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

Partial Class frmAdministration
    Inherits System.Web.UI.Page

    Dim ModuleUserData As New clsModuleUser()

    Protected Sub btnAdSync_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdSync.Click
        Try
            If Session("UserName") = "dsamaddar" Then
                SyncADUsers()
            End If

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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ModuleUserId As String = Session("ModuleUserId")

        If ModuleUserId = "" Then
            Response.Redirect("~\frmLogin.aspx")
        End If

        If Not IsPostBack Then

        End If
    End Sub

    Public Sub SyncADUsers()
        Static DomainPath As String = "LDAP://MFILBD.COM"
        Static AdminUser As String = "matbor"
        Static Pass As String = "$$2o17$$VerticalLimit"
        Dim rel As New clsResult()
        Dim module_user As New clsModuleUser()

        Try

            Dim searchRoot As DirectoryEntry = New DirectoryEntry(DomainPath, AdminUser, Pass)
            Dim search As DirectorySearcher = New DirectorySearcher(searchRoot)
            search.Filter = "(&(objectClass=user)(objectCategory=person))"
            search.PropertiesToLoad.Add("samaccountname")
            search.PropertiesToLoad.Add("mail")
            search.PropertiesToLoad.Add("usergroup")
            search.PropertiesToLoad.Add("displayname") 'first name
            search.PropertiesToLoad.Add("objectGUID")

            Dim resultCol As SearchResultCollection = search.FindAll()

            If resultCol.Count > 0 Then
                For Each result As SearchResult In resultCol
                    If result.Properties.Contains("samaccountname") And result.Properties.Contains("mail") And result.Properties.Contains("displayname") Then
                        module_user.Guid = UnicodeBytesToString(result.Properties("objectGUID")(0))
                        module_user.DisplayName = result.Properties("displayname")(0)
                        module_user.UserName = result.Properties("samaccountname")(0)
                        module_user.Email = result.Properties("mail")(0)
                        rel = ModuleUserData.fnInsertModuleUser(module_user)
                    End If
                Next
            End If

            MessageBox("Sync Complete")
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try

    End Sub

    Private Function UnicodeBytesToString(ByVal bytes() As Byte) As Guid
        Return New Guid(bytes)
    End Function

End Class
