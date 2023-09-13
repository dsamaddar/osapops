Imports Microsoft.VisualBasic

Public Class clsResult

    Dim _ApplicationId, _ProcessFlowId, _PApproverId, _TApproverId As Integer

    Public Property ApplicationId() As Integer
        Get
            Return _ApplicationId
        End Get
        Set(ByVal value As Integer)
            _ApplicationId = value
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

    Public Property PApproverId() As Integer
        Get
            Return _PApproverId
        End Get
        Set(ByVal value As Integer)
            _PApproverId = value
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

    Dim _Message, _FileName, _Comment As String

    Public Property Message() As String
        Get
            Return _Message
        End Get
        Set(ByVal value As String)
            _Message = value
        End Set
    End Property

    Public Property FileName() As String
        Get
            Return _FileName
        End Get
        Set(ByVal value As String)
            _FileName = value
        End Set
    End Property

    Public Property Comment() As String
        Get
            Return _Comment
        End Get
        Set(ByVal value As String)
            _Comment = value
        End Set
    End Property

    Dim _Success, _IsFinalStage As Boolean

    Public Property Success() As Boolean
        Get
            Return _Success
        End Get
        Set(ByVal value As Boolean)
            _Success = value
        End Set
    End Property

    Public Property IsFinalStage() As Boolean
        Get
            Return _IsFinalStage
        End Get
        Set(ByVal value As Boolean)
            _IsFinalStage = value
        End Set
    End Property

End Class
