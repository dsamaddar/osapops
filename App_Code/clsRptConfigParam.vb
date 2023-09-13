Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class clsRptConfigParam

    Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("OSAPCon").ConnectionString)

    Dim _config_id, _function_id, _parameter_sl As Integer

    Public Property config_id() As Integer
        Get
            Return _config_id
        End Get
        Set(ByVal value As Integer)
            _config_id = value
        End Set
    End Property

    Public Property function_id() As Integer
        Get
            Return _function_id
        End Get
        Set(ByVal value As Integer)
            _function_id = value
        End Set
    End Property

    Public Property parameter_sl() As Integer
        Get
            Return _parameter_sl
        End Get
        Set(ByVal value As Integer)
            _parameter_sl = value
        End Set
    End Property

    Dim _function_name, _report_file, _report_header, _report_footer, _control_id, _parameter_label, _parameter_name, _parameter_type, _default_value, _wild_card, _validation_exp As String

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

    Public Property control_id() As String
        Get
            Return _control_id
        End Get
        Set(ByVal value As String)
            _control_id = value
        End Set
    End Property

    Public Property parameter_label() As String
        Get
            Return _parameter_label
        End Get
        Set(ByVal value As String)
            _parameter_label = value
        End Set
    End Property

    Public Property parameter_name() As String
        Get
            Return _parameter_name
        End Get
        Set(ByVal value As String)
            _parameter_name = value
        End Set
    End Property

    Public Property parameter_type() As String
        Get
            Return _parameter_type
        End Get
        Set(ByVal value As String)
            _parameter_type = value
        End Set
    End Property

    Public Property default_value() As String
        Get
            Return _default_value
        End Get
        Set(ByVal value As String)
            _default_value = value
        End Set
    End Property

    Public Property wild_card() As String
        Get
            Return _wild_card
        End Get
        Set(ByVal value As String)
            _wild_card = value
        End Set
    End Property

    Public Property validation_exp() As String
        Get
            Return _validation_exp
        End Get
        Set(ByVal value As String)
            _validation_exp = value
        End Set
    End Property

    Dim _is_mandatory As Boolean

    Public Property is_mandatory() As Boolean
        Get
            Return _is_mandatory
        End Get
        Set(ByVal value As Boolean)
            _is_mandatory = value
        End Set
    End Property

#Region " Get Rpt Config Params "

    Public Function fnGetRptConfigParams(ByVal fast_path As Integer) As DataSet
        Dim sp As String = "rspGetRptConfigParams"
        Dim da As SqlDataAdapter = New SqlDataAdapter()
        Dim ds As DataSet = New DataSet()
        Try
            con.Open()
            Using cmd As SqlCommand = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@fast_path", fast_path)
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
