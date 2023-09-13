Imports Microsoft.VisualBasic
Imports System.Data

Public Class clsProcessFlowDecision

    Dim Common As New clsCommon()
    Dim _ProcessFlowDecisionId As Integer

    Public Property ProcessFlowDecisionId() As Integer
        Get
            Return _ProcessFlowDecisionId
        End Get
        Set(ByVal value As Integer)
            _ProcessFlowDecisionId = value
        End Set
    End Property

    Dim _ProcessFlowDecisionText, _ProcessFlowDecisionDescription As String

    Public Property ProcessFlowDecisionText() As String
        Get
            Return _ProcessFlowDecisionText
        End Get
        Set(ByVal value As String)
            _ProcessFlowDecisionText = value
        End Set
    End Property

    Public Property ProcessFlowDecisionDescription() As String
        Get
            Return _ProcessFlowDecisionDescription
        End Get
        Set(ByVal value As String)
            _ProcessFlowDecisionDescription = value
        End Set
    End Property

    Public Function fnGetProcessFlowDecisionList() As DataSet
        Return Common.fnReturnDataSet("spGetProcessFlowDecisionList")
    End Function

End Class
