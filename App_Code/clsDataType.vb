Imports Microsoft.VisualBasic

Public Class clsDataType

    Dim _data As String
    Dim _data_type As String

    Public Property data() As String
        Get
            Return _data
        End Get
        Set(ByVal value As String)
            _data = value
        End Set
    End Property

    Public Property data_type() As String
        Get
            Return _data_type
        End Get
        Set(ByVal value As String)
            _data_type = value
        End Set
    End Property

End Class
