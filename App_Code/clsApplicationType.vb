Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient

Public Class clsApplicationType

    Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("OSAPCon").ConnectionString)

    Dim _ApplicationTypeId As Integer
    Dim _ApplicationTypeText, _ApplicationTypeDescription, _Email As String
    Dim _IsVisible As Boolean
    Dim _SleepTime As Integer
    Dim Common As New clsCommon()


    Public Property ApplicationTypeId() As Integer
        Get
            Return _ApplicationTypeId
        End Get
        Set(ByVal value As Integer)
            _ApplicationTypeId = value
        End Set
    End Property

    Public Property ApplicationTypeText() As String
        Get
            Return _ApplicationTypeText
        End Get
        Set(ByVal value As String)
            _ApplicationTypeText = value
        End Set
    End Property

    Public Property ApplicationTypeDescription() As String
        Get
            Return _ApplicationTypeDescription
        End Get
        Set(ByVal value As String)
            _ApplicationTypeDescription = value
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

    Public Property IsVisible() As Boolean
        Get
            Return _IsVisible
        End Get
        Set(ByVal value As Boolean)
            _IsVisible = value
        End Set
    End Property

    Public Property SleepTime() As Integer
        Get
            Return _SleepTime
        End Get
        Set(ByVal value As Integer)
            _SleepTime = value
        End Set
    End Property

#Region " Insert Application Type"

    Public Function fnInsertApplicationType(ByVal app_type As clsApplicationType) As clsResult
        Dim result As New clsResult()
        Try
            Dim cmd As SqlCommand = New SqlCommand("spInsertApplicationType", con)
            con.Open()
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@ApplicationTypeText", app_type.ApplicationTypeText)
            cmd.Parameters.AddWithValue("@ApplicationTypeDescription", app_type.ApplicationTypeDescription)
            cmd.Parameters.AddWithValue("@Email", app_type.Email)
            cmd.Parameters.AddWithValue("@IsVisible", app_type.IsVisible)
            cmd.ExecuteNonQuery()
            con.Close()
            result.Success = True
            result.Message = "Application Type Initiated Successfully!"
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

#Region " Update Application Type"

    Public Function fnUpdateApplicationType(ByVal app_type As clsApplicationType) As clsResult
        Dim result As New clsResult()
        Try
            Dim cmd As SqlCommand = New SqlCommand("spUpdateApplicationType", con)
            con.Open()
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@ApplicationTypeId", app_type.ApplicationTypeId)
            cmd.Parameters.AddWithValue("@ApplicationTypeText", app_type.ApplicationTypeText)
            cmd.Parameters.AddWithValue("@ApplicationTypeDescription", app_type.ApplicationTypeDescription)
            cmd.Parameters.AddWithValue("@Email", app_type.Email)
            cmd.Parameters.AddWithValue("@IsVisible", app_type.IsVisible)
            cmd.ExecuteNonQuery()
            con.Close()
            result.Success = True
            result.Message = "Application Type Updated Successfully!"
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

    Public Function fnGetApplicationTypeList() As DataSet
        Return Common.fnReturnDataSet("spGetApplicationTypeList")
    End Function

    Public Function fnGetApplicationTypes() As DataSet
        Return Common.fnReturnDataSet("spGetApplicationTypes")
    End Function

End Class
