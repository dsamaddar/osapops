Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient

Public Class clsApplication

    Dim con As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("OSAPCon").ConnectionString)

    Dim _ApplicationId, _ApplicantId, _ApplicationTypeId, _ApplicationStatusId, _CreatedBy As Integer

    Public Property ApplicationId() As Integer
        Get
            Return _ApplicationId
        End Get
        Set(ByVal value As Integer)
            _ApplicationId = value
        End Set
    End Property

    Public Property ApplicantId() As Integer
        Get
            Return _ApplicantId
        End Get
        Set(ByVal value As Integer)
            _ApplicantId = value
        End Set
    End Property

    Public Property ApplicationTypeId() As Integer
        Get
            Return _ApplicationTypeId
        End Get
        Set(ByVal value As Integer)
            _ApplicationTypeId = value
        End Set
    End Property

    Public Property ApplicationStatusId() As Integer
        Get
            Return _ApplicationStatusId
        End Get
        Set(ByVal value As Integer)
            _ApplicationStatusId = value
        End Set
    End Property

    Public Property CreatedBy() As Integer
        Get
            Return _CreatedBy
        End Get
        Set(ByVal value As Integer)
            _CreatedBy = value
        End Set
    End Property

    Dim _Initiator, _Type, _Status, _Title, _Description, _FileName, _ApprovedFileName, _DocumentWorkFlow As String

    Public Property Initiator() As String
        Get
            Return _Initiator
        End Get
        Set(ByVal value As String)
            _Initiator = value
        End Set
    End Property

    Public Property Type() As String
        Get
            Return _Type
        End Get
        Set(ByVal value As String)
            _Type = value
        End Set
    End Property

    Public Property Status() As String
        Get
            Return _Status
        End Get
        Set(ByVal value As String)
            _Status = value
        End Set
    End Property

    Public Property Title() As String
        Get
            Return _Title
        End Get
        Set(ByVal value As String)
            _Title = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return _Description
        End Get
        Set(ByVal value As String)
            _Description = value
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

    Public Property ApprovedFileName() As String
        Get
            Return _ApprovedFileName
        End Get
        Set(ByVal value As String)
            _ApprovedFileName = value
        End Set
    End Property

    Public Property DocumentWorkFlow() As String
        Get
            Return _DocumentWorkFlow
        End Get
        Set(ByVal value As String)
            _DocumentWorkFlow = value
        End Set
    End Property

    Dim _Amount As Double

    Public Property Amount() As Double
        Get
            Return _Amount
        End Get
        Set(ByVal value As Double)
            _Amount = value
        End Set
    End Property

    Dim _CreatedDate As DateTime

    Public Property CreatedDate() As DateTime
        Get
            Return _CreatedDate
        End Get
        Set(ByVal value As DateTime)
            _CreatedDate = value
        End Set
    End Property

#Region " Initiate Application"

    Public Function fnInitiateApplication(ByVal app_info As clsApplication) As clsResult
        Dim result As New clsResult()
        Try
            Dim dr As SqlDataReader
            Dim cmd As SqlCommand = New SqlCommand("spInitiateApplication", con)
            con.Open()
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("@ApplicantId", app_info.ApplicantId)
            cmd.Parameters.AddWithValue("@Description", app_info.Description)
            cmd.Parameters.AddWithValue("@ApplicationTypeId", app_info.ApplicationTypeId)
            cmd.Parameters.AddWithValue("@ApplicationStatusId", app_info.ApplicationStatusId)
            cmd.Parameters.AddWithValue("@FileName", app_info.FileName)
            cmd.Parameters.AddWithValue("@CreatedBy", app_info.CreatedBy)
            cmd.Parameters.AddWithValue("@DocumentWorkFlow", app_info.DocumentWorkFlow)
            dr = cmd.ExecuteReader()
            While dr.Read()
                result.ApplicationId = dr.Item("ApplicationId")
            End While
            con.Close()
            result.Success = True
            result.Message = "Application Initiated Successfully!"
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


#Region " Permanent Reject Application"

    Public Function fnPermanentRejectApplication(ByVal ApplicationId As Integer, ByVal RejectionRemarks As String) As clsResult
        Dim result As New clsResult()
        Try
            Dim dr As SqlDataReader
            Dim cmd As SqlCommand = New SqlCommand("spPermanentRejectApplication", con)
            con.Open()
            cmd.CommandType = CommandType.StoredProcedure

            cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId)
            cmd.Parameters.AddWithValue("@RejectionRemarks", RejectionRemarks)
            cmd.ExecuteNonQuery()
            con.Close()
            result.Success = True
            result.Message = "Application Permanently Rejected."
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

#Region " Reject Application"

    Public Function fnRejectApplication(ByVal ProcessFlowId As Integer, ByVal Comment As String) As clsResult
        Dim result As New clsResult()
        Try
            Dim dr As SqlDataReader
            Dim cmd As SqlCommand = New SqlCommand("spRejectApplication", con)
            con.Open()
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@ProcessFlowId", ProcessFlowId)
            cmd.Parameters.AddWithValue("@Comment", Comment)
            dr = cmd.ExecuteReader()
            While dr.Read()
                result.ApplicationId = dr.Item("ApplicationId")
            End While
            con.Close()
            result.Success = True
            result.Message = "Application Rejected Successfully!"
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

#Region " Approve Application "

    Public Function fnApproveApplication(ByVal ProcessFlowId As Integer, ByVal Comment As String) As clsResult
        Dim result As New clsResult()
        Try
            Dim dr As SqlDataReader
            Dim cmd As SqlCommand = New SqlCommand("spApproveApplication", con)
            con.Open()
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@ProcessFlowId", ProcessFlowId)
            cmd.Parameters.AddWithValue("@Comment", Comment)
            dr = cmd.ExecuteReader()
            While dr.Read()
                result.ApplicationId = dr.Item("ApplicationId")
                result.FileName = dr.Item("FileName")
                result.IsFinalStage = dr.Item("IsFinalStage")
            End While
            con.Close()
            result.Success = True
            result.Message = "Application Accepted Successfully!"
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

#Region " sp Get Initiated Application By User "

    Public Function fnGetInitiatedApplicationByUser(ByVal ApplicantId As Integer, ByVal StartDt As Date, ByVal EndDt As Date) As DataSet

        Dim sp As String = "spGetInitiatedApplicationByUser"
        Dim da As SqlDataAdapter = New SqlDataAdapter()
        Dim ds As DataSet = New DataSet()
        Try
            con.Open()
            Using cmd = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ApplicantId", ApplicantId)
                cmd.Parameters.AddWithValue("@StartDt", StartDt)
                cmd.Parameters.AddWithValue("@EndDt", EndDt)
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

#Region " Get Rejectable Application By User "

    Public Function fnGetRejectableApplicationByUser(ByVal ApplicantId As Integer, ByVal StartDt As Date, ByVal EndDt As Date) As DataSet

        Dim sp As String = "spGetRejectableApplicationByUser"
        Dim da As SqlDataAdapter = New SqlDataAdapter()
        Dim ds As DataSet = New DataSet()
        Try
            con.Open()
            Using cmd = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ApplicantId", ApplicantId)
                cmd.Parameters.AddWithValue("@StartDt", StartDt)
                cmd.Parameters.AddWithValue("@EndDt", EndDt)
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


#Region " Get Transferable Applications "

    Public Function fnGetTransferableApplications(ByVal ModuleUserId As Integer) As DataSet

        Dim sp As String = "spGetTransferableApplications"
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

#Region " Get Waiting List By User "

    Public Function fnGetWaitingListByUser(ByVal ModuleUserId As Integer) As DataSet

        Dim sp As String = "spGetWaitingListByUser"
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

#Region " Get Application Info ById "

    Public Function fnGetApplicationInfoById(ByVal ApplicationId As Integer) As clsApplication

        Dim sp As String = "spGetApplicationInfoById"
        Dim app_info As New clsApplication()
        Dim dr As SqlDataReader

        Try
            con.Open()
            Using cmd = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId)
                dr = cmd.ExecuteReader()
                While dr.Read()
                    app_info.ApplicationId = dr.Item("ApplicationId")
                    app_info.ApplicantId = dr.Item("ApplicantId")
                    app_info.Initiator = dr.Item("Initiator")
                    app_info.Title = dr.Item("Title")
                    app_info.Description = dr.Item("Description")
                    app_info.Amount = dr.Item("Amount")
                    app_info.ApplicationTypeId = dr.Item("ApplicationTypeId")
                    app_info.Type = dr.Item("Type")
                    app_info.ApplicationStatusId = dr.Item("ApplicationStatusId")
                    app_info.Status = dr.Item("Status")
                    app_info.FileName = dr.Item("FileName")
                    app_info.ApprovedFileName = dr.Item("ApprovedFileName")
                    app_info.CreatedDate = dr.Item("CreatedDate")
                End While
                con.Close()
                Return app_info
            End Using
        Catch ex As Exception
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            Return Nothing
        End Try
    End Function

#End Region

#Region " Get Osap Initiator Mail "

    Public Function fnGetOsapInitiatorMail(ByVal ApplicationId As Integer) As clsMailProperty
        Dim sp As String = "spGetOsapInitiatorMail"
        Dim dr As SqlDataReader
        Dim MailProp As New clsMailProperty()
        Try
            con.Open()

            Using cmd = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId)
                dr = cmd.ExecuteReader()
                While dr.Read()
                    MailProp.MailSubject = dr.Item("MailSubject")
                    MailProp.MailBody = dr.Item("MailBody")
                    MailProp.MailFrom = dr.Item("MailFrom")
                    MailProp.MailTo = dr.Item("MailTo")
                    MailProp.MailCC = dr.Item("MailCC")
                    MailProp.MailBCC = dr.Item("MailBCC")
                End While
                con.Close()

                Return MailProp
            End Using
        Catch ex As Exception
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            Return Nothing
        End Try
    End Function

#End Region

#Region " Get Osap Permanent Rejection Mail "

    Public Function fnGetOsapPermanentRejectionMail(ByVal ApplicationId As Integer, ByVal RejectionRemarks As String) As clsMailProperty
        Dim sp As String = "spGetOsapPermanentRejectionMail"
        Dim dr As SqlDataReader
        Dim MailProp As New clsMailProperty()
        Try
            con.Open()

            Using cmd = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId)
                cmd.Parameters.AddWithValue("@RejectionRemarks", RejectionRemarks)
                dr = cmd.ExecuteReader()
                While dr.Read()
                    MailProp.MailSubject = dr.Item("MailSubject")
                    MailProp.MailBody = dr.Item("MailBody")
                    MailProp.MailFrom = dr.Item("MailFrom")
                    MailProp.MailTo = dr.Item("MailTo")
                    MailProp.MailCC = dr.Item("MailCC")
                    MailProp.MailBCC = dr.Item("MailBCC")
                End While
                con.Close()

                Return MailProp
            End Using
        Catch ex As Exception
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            Return Nothing
        End Try
    End Function

#End Region

#Region " Get Osap Transfer Mail "

    Public Function fnGetOsapTransferMail(ByVal result As clsResult) As clsMailProperty
        Dim sp As String = "spGetOsapTransferMail"
        Dim dr As SqlDataReader
        Dim MailProp As New clsMailProperty()
        Try
            con.Open()

            Using cmd = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ApplicationId", result.ApplicationId)
                cmd.Parameters.AddWithValue("@ProcessFlowId", result.ProcessFlowId)
                cmd.Parameters.AddWithValue("@PApproverId", result.PApproverId)
                cmd.Parameters.AddWithValue("@TApproverId", result.TApproverId)
                cmd.Parameters.AddWithValue("@Comment", result.Comment)
                dr = cmd.ExecuteReader()
                While dr.Read()
                    MailProp.MailSubject = dr.Item("MailSubject")
                    MailProp.MailBody = dr.Item("MailBody")
                    MailProp.MailFrom = dr.Item("MailFrom")
                    MailProp.MailTo = dr.Item("MailTo")
                    MailProp.MailCC = dr.Item("MailCC")
                    MailProp.MailBCC = dr.Item("MailBCC")
                End While
                con.Close()

                Return MailProp
            End Using
        Catch ex As Exception
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            Return Nothing
        End Try
    End Function

#End Region

#Region " Get Osap Rejection Mail "

    Public Function fnGetOsapRejectionMail(ByVal ApplicationId As Integer, ByVal ProcessFlowId As Integer) As clsMailProperty
        Dim sp As String = "spGetOsapRejectionMail"
        Dim dr As SqlDataReader
        Dim MailProp As New clsMailProperty()
        Try
            con.Open()

            Using cmd = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId)
                cmd.Parameters.AddWithValue("@ProcessFlowId", ProcessFlowId)
                dr = cmd.ExecuteReader()
                While dr.Read()
                    MailProp.MailSubject = dr.Item("MailSubject")
                    MailProp.MailBody = dr.Item("MailBody")
                    MailProp.MailFrom = dr.Item("MailFrom")
                    MailProp.MailTo = dr.Item("MailTo")
                    MailProp.MailCC = dr.Item("MailCC")
                    MailProp.MailBCC = dr.Item("MailBCC")
                End While
                con.Close()

                Return MailProp
            End Using
        Catch ex As Exception
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            Return Nothing
        End Try
    End Function

#End Region

#Region " Get Osap Mid Approval Mail "

    Public Function fnGetOsapMidApprovalMail(ByVal ApplicationId As Integer, ByVal ProcessFlowId As Integer) As clsMailProperty
        Dim sp As String = "spGetOsapMidApprovalMail"
        Dim dr As SqlDataReader
        Dim MailProp As New clsMailProperty()
        Try
            con.Open()

            Using cmd = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId)
                cmd.Parameters.AddWithValue("@ProcessFlowId", ProcessFlowId)
                dr = cmd.ExecuteReader()
                While dr.Read()
                    MailProp.MailSubject = dr.Item("MailSubject")
                    MailProp.MailBody = dr.Item("MailBody")
                    MailProp.MailFrom = dr.Item("MailFrom")
                    MailProp.MailTo = dr.Item("MailTo")
                    MailProp.MailCC = dr.Item("MailCC")
                    MailProp.MailBCC = dr.Item("MailBCC")
                End While
                con.Close()

                Return MailProp
            End Using
        Catch ex As Exception
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            Return Nothing
        End Try
    End Function

#End Region

#Region " Get Osap Final Approval Mail "

    Public Function fnGetOsapFinalApprovalMail(ByVal ApplicationId As Integer, ByVal ProcessFlowId As Integer) As clsMailProperty
        Dim sp As String = "spGetOsapFinalApprovalMail"
        Dim dr As SqlDataReader
        Dim MailProp As New clsMailProperty()
        Try
            con.Open()

            Using cmd = New SqlCommand(sp, con)
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@ApplicationId", ApplicationId)
                cmd.Parameters.AddWithValue("@ProcessFlowId", ProcessFlowId)
                dr = cmd.ExecuteReader()
                While dr.Read()
                    MailProp.MailSubject = dr.Item("MailSubject")
                    MailProp.MailBody = dr.Item("MailBody")
                    MailProp.MailFrom = dr.Item("MailFrom")
                    MailProp.MailTo = dr.Item("MailTo")
                    MailProp.MailCC = dr.Item("MailCC")
                    MailProp.MailBCC = dr.Item("MailBCC")
                End While
                con.Close()

                Return MailProp
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
