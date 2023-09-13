Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient

Public Class clsModuleUser

    Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("OSAPCon").ConnectionString)

    Dim Common As New clsCommon()

    Dim _ModuleUserId As Integer

    Public Property ModuleUserId() As Integer
        Get
            Return _ModuleUserId
        End Get
        Set(ByVal value As Integer)
            _ModuleUserId = value
        End Set
    End Property

    Dim _Guid As Guid

    Public Property Guid() As Guid
        Get
            Return _Guid
        End Get
        Set(ByVal value As Guid)
            _Guid = value
        End Set
    End Property

    Dim _DisplayName, _UserName, _Email, _Branch, _Department, _Role, _CreatedBy, _SignFile As String

    Public Property DisplayName() As String
        Get
            Return _DisplayName
        End Get
        Set(ByVal value As String)
            _DisplayName = value
        End Set
    End Property

    Public Property UserName() As String
        Get
            Return _UserName
        End Get
        Set(ByVal value As String)
            _UserName = value
        End Set
    End Property

    Public Property Email() As String
        Get
            Return _Email
        End Get
        Set(ByVal value As String)
            _Email = value
        End Set
    End Property

    Public Property Branch() As String
        Get
            Return _Branch
        End Get
        Set(ByVal value As String)
            _Branch = value
        End Set
    End Property

    Public Property Department() As String
        Get
            Return _Department
        End Get
        Set(ByVal value As String)
            _Department = value
        End Set
    End Property

    Public Property Role() As String
        Get
            Return _Role
        End Get
        Set(ByVal value As String)
            _Role = value
        End Set
    End Property

    Public Property CreatedBy() As String
        Get
            Return _CreatedBy
        End Get
        Set(ByVal value As String)
            _CreatedBy = value
        End Set
    End Property

    Public Property SignFile() As String
        Get
            Return _SignFile
        End Get
        Set(ByVal value As String)
            _SignFile = value
        End Set
    End Property

    Dim _CreatedDate As Date

    Public Property CreatedDate() As Date
        Get
            Return _CreatedDate
        End Get
        Set(ByVal value As Date)
            _CreatedDate = value
        End Set
    End Property

    Dim _IsVisible As Boolean

    Public Property IsVisible() As String
        Get
            Return _IsVisible
        End Get
        Set(ByVal value As String)
            _IsVisible = value
        End Set
    End Property

    Public Function fnGetModuleUserList() As DataSet
        Return Common.fnReturnDataSet("spGetModuleUserList")
    End Function

    Public Function fnCheckUserLogin(ByVal UserName As String) As clsModuleUser
        Dim sp As String = "spCheckUserLogin"

        Dim module_user As New clsModuleUser()

        Dim dr As SqlDataReader
        Try
            con.Open()
            Using cmd = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@UserName", UserName)
                dr = cmd.ExecuteReader()
                While dr.Read()
                    module_user.ModuleUserId = dr.Item("ModuleUserId")
                    module_user.Guid = dr.Item("Guid")
                    module_user.DisplayName = dr.Item("DisplayName")
                    module_user.UserName = dr.Item("UserName")
                    module_user.Email = dr.Item("Email")
                End While
                con.Close()
                Return module_user
            End Using
        Catch ex As Exception
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            Return Nothing
        End Try
    End Function


#Region " Insert Module User "

    Public Function fnInsertModuleUser(ByVal module_user As clsModuleUser) As clsResult
        Dim result As New clsResult()
        Try
            Dim cmd As SqlCommand = New SqlCommand("spInsertModuleUser", con)
            con.Open()
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@Guid", module_user.Guid)
            cmd.Parameters.AddWithValue("@DisplayName", module_user.DisplayName)
            cmd.Parameters.AddWithValue("@UserName", module_user.UserName)
            cmd.Parameters.AddWithValue("@Email", module_user.Email)
            cmd.ExecuteNonQuery()
            con.Close()
            result.Success = True
            result.Message = "User Added Successfully!"
            Return result
        Catch ex As Exception
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            result.ApplicationId = 0
            result.Success = False
            result.Message = ex.Message
            Return result
        End Try
    End Function

#End Region
End Class
