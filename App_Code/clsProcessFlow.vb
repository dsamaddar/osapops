Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class clsProcessFlow

    Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("OSAPCon").ConnectionString)

    Dim _ProcessFlowId, _ApplicationId, _ApproverId, _RoleId, _Sequence, _ProcessFlowDecisionId As Integer

    Public Property ProcessFlowId() As Integer
        Get
            Return _ProcessFlowId
        End Get
        Set(ByVal value As Integer)
            _ProcessFlowId = value
        End Set
    End Property

    Public Property ApplicationId() As Integer
        Get
            Return _ApplicationId
        End Get
        Set(ByVal value As Integer)
            _ApplicationId = value
        End Set
    End Property

    Public Property ApproverId() As Integer
        Get
            Return _ApproverId
        End Get
        Set(ByVal value As Integer)
            _ApproverId = value
        End Set
    End Property

    Public Property RoleId() As Integer
        Get
            Return _RoleId
        End Get
        Set(ByVal value As Integer)
            _RoleId = value
        End Set
    End Property

    Public Property Sequence() As Integer
        Get
            Return _Sequence
        End Get
        Set(ByVal value As Integer)
            _Sequence = value
        End Set
    End Property

    Public Property ProcessFlowDecisionId() As Integer
        Get
            Return _ProcessFlowDecisionId
        End Get
        Set(ByVal value As Integer)
            _ProcessFlowDecisionId = value
        End Set
    End Property

    Dim _DecisionDate As DateTime

    Public Property DecisionDate() As DateTime
        Get
            Return _DecisionDate
        End Get
        Set(ByVal value As DateTime)
            _DecisionDate = value
        End Set
    End Property

    Dim _Comment As String

    Public Property Comment() As String
        Get
            Return _Comment
        End Get
        Set(ByVal value As String)
            _Comment = value
        End Set
    End Property

    Dim _ApproverName, _RoleName As String

    Public Property ApproverName() As String
        Get
            Return _ApproverName
        End Get
        Set(ByVal value As String)
            _ApproverName = value
        End Set
    End Property

    Public Property RoleName() As String
        Get
            Return _RoleName
        End Get
        Set(ByVal value As String)
            _RoleName = value
        End Set
    End Property

#Region " Get Application Work Flow Info "

    Public Function fnGetApplicationWorkFlowInfo(ByVal ApplicationId As Integer) As DataSet

        Dim sp As String = "spGetApplicationWorkFlowInfo"
        Dim da As SqlDataAdapter = New SqlDataAdapter()
        Dim ds As DataSet = New DataSet()
        Try
            con.Open()
            Using cmd = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId)
                da.SelectCommand = cmd
                da.Fill(ds)
                con.Close()
                Return ds
            End Using
        Catch ex As Exception
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            Return Nothing
        End Try
    End Function

#End Region

#Region " Get Pending Task List By User "

    Public Function fnGetPendingTaskListByUser(ByVal ModuleUserId As Integer) As DataSet

        Dim sp As String = "spGetPendingTaskListByUser"
        Dim da As SqlDataAdapter = New SqlDataAdapter()
        Dim ds As DataSet = New DataSet()
        Try
            con.Open()
            Using cmd = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ModuleUserId", ModuleUserId)
                da.SelectCommand = cmd
                da.Fill(ds)
                con.Close()
                Return ds
            End Using
        Catch ex As Exception
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            Return Nothing
        End Try
    End Function

#End Region

#Region " Get Performed Task List By User "

    Public Function fnGetPerformedTaskListByUser(ByVal ModuleUserId As Integer) As DataSet

        Dim sp As String = "spGetPerformedTaskListByUser"
        Dim da As SqlDataAdapter = New SqlDataAdapter()
        Dim ds As DataSet = New DataSet()
        Try
            con.Open()
            Using cmd = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ModuleUserId", ModuleUserId)
                da.SelectCommand = cmd
                da.Fill(ds)
                con.Close()
                Return ds
            End Using
        Catch ex As Exception
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            Return Nothing
        End Try
    End Function

#End Region

#Region " Find Tasks And Status "

    Public Function fnFindTasksAndStatus(ByVal Description As String, ByVal ApplicationTypeId As Integer, ByVal ApproverId As Integer, ByVal ProcessFlowDecisionId As Integer, ByVal StartDate As Date, ByVal EndDate As Date) As DataSet

        Dim sp As String = "spFindTasksAndStatus"
        Dim da As SqlDataAdapter = New SqlDataAdapter()
        Dim ds As DataSet = New DataSet()
        Try
            con.Open()
            Using cmd = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@Description", Description)
                cmd.Parameters.AddWithValue("@ApplicationTypeId", ApplicationTypeId)
                cmd.Parameters.AddWithValue("@ApproverId", ApproverId)
                cmd.Parameters.AddWithValue("@ProcessFlowDecisionId", ProcessFlowDecisionId)
                cmd.Parameters.AddWithValue("@StartDate", StartDate)
                cmd.Parameters.AddWithValue("@EndDate", EndDate)
                da.SelectCommand = cmd
                da.Fill(ds)
                con.Close()
                Return ds
            End Using
        Catch ex As Exception
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            Return Nothing
        End Try
    End Function

#End Region

End Class
