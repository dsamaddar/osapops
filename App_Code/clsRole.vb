Imports Microsoft.VisualBasic
Imports System.Data

Public Class clsRole

    Dim _RoleId As Integer
    Dim Common As New clsCommon()

    Public Property RoleId() As Integer
        Get
            Return _RoleId
        End Get
        Set(ByVal value As Integer)
            _RoleId = value
        End Set
    End Property

    Dim _RoleText, _RoleDescription As String

    Public Property RoleText() As String
        Get
            Return _RoleText
        End Get
        Set(ByVal value As String)
            _RoleText = value
        End Set
    End Property

    Public Property RoleDescription() As String
        Get
            Return _RoleDescription
        End Get
        Set(ByVal value As String)
            _RoleDescription = value
        End Set
    End Property

    Public Function fnGetRoleList() As DataSet
        Return Common.fnReturnDataSet("spGetRoleList")
    End Function

End Class
