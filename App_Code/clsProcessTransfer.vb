Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class clsProcessTransfer

    Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("OSAPCon").ConnectionString)
    Dim _TransferId, _ProcessFlowId, _ApplicationId, _PApproverId, _RoleId, _Sequence, _ProcessFlowDecisionId, _TApproverId, _TransferBy As Integer

    Public Property TransferId() As Integer
        Get
            Return _TransferId
        End Get
        Set(ByVal value As Integer)
            _TransferId = value
        End Set
    End Property

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

    Public Property PApproverId() As Integer
        Get
            Return _PApproverId
        End Get
        Set(ByVal value As Integer)
            _PApproverId = value
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

    Public Property TApproverId() As Integer
        Get
            Return _TApproverId
        End Get
        Set(ByVal value As Integer)
            _TApproverId = value
        End Set
    End Property

    Public Property TransferBy() As Integer
        Get
            Return _TransferBy
        End Get
        Set(ByVal value As Integer)
            _TransferBy = value
        End Set
    End Property

    Dim _TransferDate As DateTime

    Public Property TransferDate() As DateTime
        Get
            Return _TransferDate
        End Get
        Set(ByVal value As DateTime)
            _TransferDate = value
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


#Region " Transfer Application "

    Public Function fnTransferApplication(ByVal process_transfer As clsProcessTransfer) As clsResult
        Dim result As New clsResult()
        Try
            Dim dr As SqlDataReader
            Dim cmd As SqlCommand = New SqlCommand("spTransferApplication", con)
            con.Open()
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("@ApplicationId", process_transfer.ApplicationId)
            cmd.Parameters.AddWithValue("@ProcessFlowId", process_transfer.ProcessFlowId)
            cmd.Parameters.AddWithValue("@Comment", process_transfer.Comment)
            cmd.Parameters.AddWithValue("@TApproverId", process_transfer.TApproverId)
            cmd.Parameters.AddWithValue("@ModuleUserId", process_transfer.TransferBy)
            dr = cmd.ExecuteReader()
            While dr.Read()
                result.ApplicationId = dr.Item("ApplicationId")
                result.ProcessFlowId = dr.Item("ProcessFlowId")
                result.PApproverId = dr.Item("PApproverId")
                result.TApproverId = dr.Item("TApproverId")
                result.Comment = dr.Item("Comment")
            End While
            con.Close()
            result.Success = True
            result.Message = "Application Transfered Successfully!"
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
