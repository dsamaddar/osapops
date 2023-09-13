Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class clsRptConfigMast

    Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("OSAPCon").ConnectionString)

    Dim _function_id, _fast_path As Integer

    Public Property function_id() As Integer
        Get
            Return _function_id
        End Get
        Set(ByVal value As Integer)
            _function_id = value
        End Set
    End Property

    Public Property fast_path() As Integer
        Get
            Return _fast_path
        End Get
        Set(ByVal value As Integer)
            _fast_path = value
        End Set
    End Property

    Dim _function_name, _report_file, _report_header, _report_footer As String

    Public Property function_name() As String
        Get
            Return _function_name
        End Get
        Set(ByVal value As String)
            _function_name = value
        End Set
    End Property

    Public Property report_file() As String
        Get
            Return _report_file
        End Get
        Set(ByVal value As String)
            _report_file = value
        End Set
    End Property

    Public Property report_header() As String
        Get
            Return _report_header
        End Get
        Set(ByVal value As String)
            _report_header = value
        End Set
    End Property

    Public Property report_footer() As String
        Get
            Return _report_footer
        End Get
        Set(ByVal value As String)
            _report_footer = value
        End Set
    End Property

    Public Function fnGetRptConfigMast(ByVal fast_path As String) As clsRptConfigMast
        Dim rpt_config As New clsRptConfigMast()
        Dim sp As String = "rspGetRptConfigMast"
        Dim dr As SqlDataReader
        Try
            con.Open()
            Using cmd As SqlCommand = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@fast_path", fast_path)
                dr = cmd.ExecuteReader()
                While dr.Read()
                    rpt_config.function_id = dr.Item("function_id")
                    rpt_config.function_name = dr.Item("function_name")
                    rpt_config.report_file = dr.Item("report_file")
                    rpt_config.report_header = dr.Item("report_header")
                    rpt_config.report_footer = dr.Item("report_footer")
                    rpt_config.fast_path = dr.Item("fast_path")
                End While
                con.Close()
                Return rpt_config
            End Using
        Catch ex As Exception
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            Return Nothing
        End Try
    End Function

End Class
