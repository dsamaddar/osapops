Imports System.Net.Mail
Imports System.IO
'Imports iTextSharp.text.pdf
'Imports iTextSharp.text
Imports System.Net
Imports PdfSharp
Imports MigraDoc.DocumentObjectModel
Imports MigraDoc.DocumentObjectModel.Tables
Imports MigraDoc.Rendering
Imports PdfSharp.Drawing
Imports PdfSharp.Pdf
Imports PdfSharp.Pdf.IO
Imports System.Data
Imports System.Threading

Partial Class frmApprove
    Inherits System.Web.UI.Page

    Dim ApplicationData As New clsApplication()
    Dim ProcessFlowData As New clsProcessFlow()
    Dim Common As New clsCommon()

    Protected Function FormatApprovalFlowTable() As DataTable
        Dim dt As DataTable = New DataTable()
        dt.Columns.Add("Approver", System.Type.GetType("System.String"))
        dt.Columns.Add("Role", System.Type.GetType("System.String"))
        dt.Columns.Add("DecisionDate", System.Type.GetType("System.String"))
        dt.Columns.Add("Comment", System.Type.GetType("System.String"))
        Return dt
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ModuleUserId As String = Session("ModuleUserId")

        If ModuleUserId = "" Then
            Response.Redirect("~\frmLogin.aspx")
        End If

        If Not IsPostBack Then
            GetWaitingList(Convert.ToInt32(Session("ModuleUserId")))
            btnAccept.Enabled = False
            btnReject.Enabled = False
        End If
    End Sub

    Protected Sub GetWaitingList(ByVal ModuleUserId As Integer)
        grdingWaitingList.DataSource = ApplicationData.fnGetWaitingListByUser(ModuleUserId)
        grdingWaitingList.DataBind()
    End Sub

    Protected Sub grdingWaitingList_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdingWaitingList.SelectedIndexChanged
        Try
            Dim lblApplicationId As New Label

            lblApplicationId = grdingWaitingList.SelectedRow.FindControl("lblApplicationId")
            GetAppInfo(Convert.ToInt32(lblApplicationId.Text))
            btnAccept.Enabled = True
            btnReject.Enabled = True
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub GetDocumentWorkFlow(ByVal ApplicationId As Integer)
        grdDocumentWorkFlow.DataSource = ProcessFlowData.fnGetApplicationWorkFlowInfo(ApplicationId)
        grdDocumentWorkflow.DataBind()
    End Sub

    Private Sub MessageBox(ByVal strMsg As String)
        Dim lbl As New System.Web.UI.WebControls.Label
        lbl.Text = "<script language='javascript'>" & Environment.NewLine _
                   & "window.alert(" & "'" & strMsg & "'" & ")</script>"
        Page.Controls.Add(lbl)
    End Sub

    Protected Sub GetAppInfo(ByVal tracking_no As Integer)
        Try
            Dim file_path As String = ""
            Dim app_info As New clsApplication()

            app_info = ApplicationData.fnGetApplicationInfoById(tracking_no)

            txtDescription.Text = app_info.Description

            GetDocumentWorkFlow(tracking_no)

            If app_info.Status <> "Approved" Then
                file_path = ConfigurationManager.AppSettings("osap_storage") & app_info.FileName
                LoadPDF(file_path)
            Else
                file_path = ConfigurationManager.AppSettings("osap_storage") & app_info.ApprovedFileName
                LoadPDF(file_path)
            End If

        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub LoadPDF(ByVal file_path As String)
        Try
            Dim embed As String = "<object data=""{0}"" type=""application/pdf"" width=""100%"" height=""395px"">"
            embed += "If you are unable to view file, you can download from <a href = ""{0}"">here</a>"
            embed += " or download <a target = ""_blank"" href = ""http://get.adobe.com/reader/"">Adobe PDF Reader</a> to view the file."
            embed += "</object>"
            ltEmbed.Text = String.Format(embed, ResolveUrl(file_path))
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try

    End Sub

    Protected Sub btnReject_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReject.Click
        Dim result As New clsResult()
        Dim MailProp As New clsMailProperty()
        Try
            Dim lblProcessFlowId As New Label

            If grdingWaitingList.SelectedIndex = -1 Then
                MessageBox("Select a process first.")
                Exit Sub
            End If

            If txtComments.Text = "" Then
                MessageBox("Rejection requires a comment.")
                Exit Sub
            End If

            lblProcessFlowId = grdingWaitingList.SelectedRow.FindControl("lblProcessFlowId")
            result = ApplicationData.fnRejectApplication(Convert.ToInt32(lblProcessFlowId.Text), txtComments.Text)

            If result.Success = True Then
                MailProp = ApplicationData.fnGetOsapRejectionMail(result.ApplicationId, Convert.ToInt32(lblProcessFlowId.Text))
                SendMail(MailProp)
                ClearForm()
            End If

            MessageBox(result.Message)
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try

    End Sub

    Protected Sub btnAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAccept.Click
        Dim result As New clsResult()
        Dim MailProp As New clsMailProperty()
        Dim data_type As New clsDataType()
        Dim sleep_time As Integer = 0

        Try
            Dim lblProcessFlowId, lblApplicationTypeId As New Label

            If grdingWaitingList.SelectedIndex = -1 Then
                MessageBox("Select a process first.")
                Exit Sub
            End If

            lblApplicationTypeId = grdingWaitingList.SelectedRow.FindControl("lblApplicationTypeId")

            'slowing down application for specific application type
            data_type = Common.fnGetValue("spGetApplicationSleepTime", lblApplicationTypeId.Text)
            sleep_time = Convert.ToInt32(data_type.data) * 1000
            Thread.Sleep(sleep_time)

            lblProcessFlowId = grdingWaitingList.SelectedRow.FindControl("lblProcessFlowId")
            result = ApplicationData.fnApproveApplication(Convert.ToInt32(lblProcessFlowId.Text), txtComments.Text)

            If result.Success = True And result.IsFinalStage = False Then
                'mid approval mail
                MailProp = ApplicationData.fnGetOsapMidApprovalMail(result.ApplicationId, Convert.ToInt32(lblProcessFlowId.Text))
                SendMail(MailProp)
            ElseIf result.Success = True And result.IsFinalStage = True Then
                'FINAL approval mail
                'FtpUpload(result.FileName, result.ApplicationId)
                GenerateApprovedPDFAndMarge(result.ApplicationId, result.FileName)
                MailProp = ApplicationData.fnGetOsapFinalApprovalMail(result.ApplicationId, Convert.ToInt32(lblProcessFlowId.Text))
                SendMail(MailProp)
            Else

            End If
            ClearForm()
            MessageBox(result.Message)
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub SendMail(ByVal MailProp As clsMailProperty)
        Dim mail As New Net.Mail.MailMessage()
        Try
            mail.From = New MailAddress(MailProp.MailFrom)
            mail.To.Add(MailProp.MailTo)

            Dim mail_cc_adr As String() = MailProp.MailCC.Split(New Char() {";"c})
            Dim mail_cc As String
            For Each mail_cc In mail_cc_adr
                If mail_cc <> "" Then
                    mail.CC.Add(mail_cc)
                End If
            Next

            mail.Bcc.Add(MailProp.MailBCC)
            mail.Subject = MailProp.MailSubject
            mail.Body = MailProp.MailBody
            mail.IsBodyHtml = True
            Dim smtp As New SmtpClient("192.168.1.14", 25)
            smtp.Send(mail)
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

    Protected Sub ClearForm()

        txtComments.Text = ""
        txtDescription.Text = ""
        grdingWaitingList.SelectedIndex = -1
        GetWaitingList(Convert.ToInt32(Session("ModuleUserId")))

        grdDocumentWorkflow.DataSource = ""
        grdDocumentWorkflow.DataBind()
        ltEmbed.Text = ""
        btnAccept.Enabled = False
        btnReject.Enabled = False

    End Sub

    'Protected Sub AddTrackingNo(ByVal file_name As String, ByVal tracking_no As Integer)
    '    Dim bytes As Byte() = File.ReadAllBytes("D:\PDFs\Test.pdf")
    '    Dim blackFont As Font = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK)
    '    Using stream As New MemoryStream()
    '        Dim reader As New PdfReader(bytes)
    '        Using stamper As New PdfStamper(reader, stream)
    '            Dim pages As Integer = reader.NumberOfPages
    '            For i As Integer = 1 To pages
    '                ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, New Phrase(i.ToString(), blackFont), 568.0F, 15.0F, 0)
    '            Next
    '        End Using
    '        bytes = stream.ToArray()
    '    End Using
    '    File.WriteAllBytes("D:\PDFs\Test_1.pdf", bytes)
    'End Sub

    'Protected Sub AddTable(ByVal file_name As String)
    '    file_name = Replace(file_name, ".pdf", "_approved.pdf")
    '    Dim username As String = ConfigurationManager.AppSettings("osap_ftp_user")
    '    Dim password As String = ConfigurationManager.AppSettings("osap_ftp_password")
    '    Dim bytes As Byte()

    '    Using client As New WebClient()
    '        client.Credentials = New NetworkCredential(username, password)
    '        bytes = client.DownloadData(file_name)
    '    End Using

    '    Dim fs As New FileStream(file_name, FileMode.Create, FileAccess.Write)

    '    Dim document As Document = New Document(PageSize.A4, 25, 25, 30, 30)
    '    Dim writer As PdfWriter = PdfWriter.GetInstance(document, fs)



    '    'Dim table As PdfTable = New pdfta

    '    'PdfPTable(Table = New PdfPTable(3))

    '    'Table.AddCell("Row 1, Col 1")
    '    'Table.AddCell("Row 1, Col 2")
    '    'Table.AddCell("Row 1, Col 3")

    '    'Table.AddCell("Row 2, Col 1")
    '    'Table.AddCell("Row 2, Col 2")
    '    'Table.AddCell("Row 2, Col 3")

    '    'Table.AddCell("Row 3, Col 1")
    '    'Table.AddCell("Row 3, Col 2")
    '    'Table.AddCell("Row 3, Col 3")
    '    'Document.Add(Table)
    'End Sub

    'Protected Function PostTrackingNo(ByVal file_name As String, ByVal tracking_no As Integer) As Byte()
    '    Try
    '        Dim username As String = ConfigurationManager.AppSettings("osap_ftp_user")
    '        Dim password As String = ConfigurationManager.AppSettings("osap_ftp_password")
    '        'Dim downloaded_file As String = ConfigurationManager.AppSettings("local_dw_storage") & Now.Ticks.ToString() & ".pdf"
    '        Dim bytes As Byte()

    '        Using client As New WebClient()
    '            client.Credentials = New NetworkCredential(username, password)
    '            bytes = client.DownloadData(file_name)
    '        End Using

    '        Dim blackFont As Font = FontFactory.GetFont("Arial", 16, Font.NORMAL, BaseColor.BLACK)
    '        Dim headerFont As Font = FontFactory.GetFont("Arial", 26, Font.NORMAL, BaseColor.BLACK)
    '        Dim contentFont As Font = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK)
    '        Dim footerFont As Font = FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)
    '        Using stream As New MemoryStream()
    '            Dim reader As New PdfReader(bytes)
    '            Dim rectangle As Rectangle = reader.GetPageSize(1)
    '            Using stamper As New PdfStamper(reader, stream)
    '                stamper.InsertPage(reader.NumberOfPages + 1, rectangle)

    '                Dim pages As Integer = reader.NumberOfPages
    '                For i As Integer = 1 To pages

    '                    If i < pages Then
    '                        ColumnText.ShowTextAligned(stamper.GetOverContent(i), Element.ALIGN_RIGHT, New Phrase(" APPROVED | TRACKING NO: " & tracking_no.ToString(), blackFont), 568.0F, 15.0F, 0)
    '                    End If

    '                    If i = pages Then
    '                        ' add last page content
    '                        ColumnText.ShowTextAligned(stamper.GetOverContent(i), Element.ALIGN_RIGHT, New Phrase(GenFooterString(tracking_no), footerFont), 568.0F, 15.0F, 0)
    '                        ColumnText.ShowTextAligned(stamper.GetOverContent(i), Element.ALIGN_LEFT, New Phrase(" APPROVED | TRACKING NO: " & tracking_no.ToString(), headerFont), 35.0F, 670.0F, 0)
    '                        ColumnText.ShowTextAligned(stamper.GetOverContent(i), Element.ALIGN_LEFT, New Phrase("HI", contentFont), 35.0F, 600.0F, 0)

    '                    End If
    '                Next

    '            End Using
    '            bytes = stream.ToArray()
    '        End Using
    '        'File.WriteAllBytes("D:\PDFs\Test_1.pdf", bytes)
    '        Return bytes
    '    Catch ex As Exception
    '        MessageBox(ex.Message)
    '        Return Nothing
    '    End Try
    'End Function

    'Private Function GenFooterString(ByVal ApplicationId As Integer) As String
    '    Dim footer_text As String = ""
    '    Dim app_info As clsApplication = ApplicationData.fnGetApplicationInfoById(ApplicationId)
    '    footer_text = "This document has been Automatically generated by Online Service Approval System (OSAP) and does not require any signature(s)\n"
    '    footer_text &= "Tracking ID # " & app_info.ApplicationId.ToString() & " | FILEID #" & app_info.FileName & " | Created On #" & app_info.CreatedDate
    '    Return footer_text
    'End Function

    'Private Sub writeText(ByVal cb As PdfContentByte, ByVal Text As String, ByVal X As Integer, ByVal Y As Integer, ByVal font As BaseFont, ByVal Size As Integer)
    '    cb.SetFontAndSize(font, Size)
    '    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Text, X, Y, 0)
    'End Sub

    'Public Sub FtpUpload(ByVal file_name As String, ByVal tracking_no As Integer)
    '    Try
    '        Dim original_file_name As String = ConfigurationManager.AppSettings("osap_ftp_storage") & file_name
    '        Dim approved_file_name As String = ConfigurationManager.AppSettings("osap_ftp_storage") & Replace(file_name, ".pdf", "_approved.pdf")
    '        Dim osap_file_storage As String = ConfigurationManager.AppSettings("osap_http_storage") & file_name

    '        Dim clsRequest As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(approved_file_name), System.Net.FtpWebRequest)
    '        clsRequest.Credentials = New System.Net.NetworkCredential(ConfigurationManager.AppSettings("osap_ftp_user"), ConfigurationManager.AppSettings("osap_ftp_password"))
    '        clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile

    '        'Dim fs As System.IO.Stream = flupAttachment.PostedFile.InputStream
    '        'Dim br As New System.IO.BinaryReader(fs)
    '        Dim bytes As Byte() = PostTrackingNo(osap_file_storage, tracking_no)

    '        Using writer As System.IO.Stream = clsRequest.GetRequestStream
    '            writer.Write(bytes, 0, bytes.Length)
    '            writer.Close()
    '        End Using
    '    Catch ex As Exception
    '        MessageBox(ex.Message)
    '    End Try

    'End Sub

    'Public Shared Function InsertPages(ByVal sourcePdf As String, _
    '                                   ByVal pagesToInsert As Dictionary(Of Integer, iTextSharp.text.pdf.PdfImportedPage), _
    '                                   ByVal outPdf As String) As Boolean
    '    Dim result As Boolean = False
    '    Dim reader As iTextSharp.text.pdf.PdfReader = Nothing
    '    Dim doc As iTextSharp.text.Document = Nothing
    '    Dim copier As iTextSharp.text.pdf.PdfCopy = Nothing
    '    Try
    '        reader = New iTextSharp.text.pdf.PdfReader(sourcePdf)
    '        doc = New iTextSharp.text.Document(reader.GetPageSizeWithRotation(1))
    '        copier = New iTextSharp.text.pdf.PdfCopy(doc, New IO.FileStream(outPdf, IO.FileMode.Create))
    '        doc.Open()
    '        For i As Integer = 1 To reader.NumberOfPages
    '            If pagesToInsert.ContainsKey(i) Then
    '                copier.AddPage(pagesToInsert(i))
    '            End If
    '            copier.AddPage(copier.GetImportedPage(reader, i))
    '        Next
    '        doc.Close()
    '        reader.Close()
    '        result = True
    '    Catch ex As Exception
    '        'Put your own code to handle exception here
    '        'Debug.Write(ex.Message)
    '    End Try
    '    Return result
    'End Function

    Private Sub GenerateApprovedPDFAndMarge(ByVal ApplicationId As Integer, ByVal file_name As String)
        Try

            Dim original_file_name As String = ConfigurationManager.AppSettings("osap_ftp_storage") & file_name
            Dim approved_file_name As String = ConfigurationManager.AppSettings("osap_ftp_storage") & Replace(file_name, ".pdf", "_approved.pdf")
            Dim osap_file_storage As String = ConfigurationManager.AppSettings("osap_http_storage") & file_name
            Dim RootPath As String = "\\FILESERV.mfilbd.com\hiddenMFILGatewayApplicationStorage$\OSAP\"

            Dim app_info As New clsApplication()
            app_info = ApplicationData.fnGetApplicationInfoById(ApplicationId)

            Dim dtApprovalFlow As DataTable = New DataTable()
            'dtApprovalFlow = FormatApprovalFlowTable()
            dtApprovalFlow = ProcessFlowData.fnGetApplicationWorkFlowInfo(ApplicationId).Tables(0)

            Dim document As Document = New Document()
            Dim section As MigraDoc.DocumentObjectModel.Section = document.AddSection()
            Dim style As MigraDoc.DocumentObjectModel.Style = document.Styles("Normal")
            style.Font.Name = "Gill Sans MT"
            style = document.Styles(StyleNames.Header)
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right)
            style = document.Styles(StyleNames.Footer)
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Center)
            style = document.Styles.AddStyle("Table", "Normal")
            style.Font.Name = "Gill Sans MT"
            style.Font.Size = 12
            style = document.Styles.AddStyle("Reference", "Normal")
            style.ParagraphFormat.SpaceBefore = "5mm"
            style.ParagraphFormat.SpaceAfter = "5mm"
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right)
            style = document.Styles.AddStyle("HeadOfThePage", "Normal")
            style.Font.Name = "Gill Sans MT"
            style.Font.Size = 24
            Dim sectionWidth As Integer = CInt(document.DefaultPageSetup.PageWidth) - CInt(document.DefaultPageSetup.LeftMargin) - CInt(document.DefaultPageSetup.RightMargin)
            sectionWidth = sectionWidth / 2
            Dim tblHead As MigraDoc.DocumentObjectModel.Tables.Table = section.AddTable()
            Dim headcolumn As Column = tblHead.AddColumn(sectionWidth)
            Dim headcolumn2 As Column = tblHead.AddColumn(sectionWidth)
            Dim headrow As Row = tblHead.AddRow()
            Dim headp1 = headrow.Cells(0).AddParagraph()
            headp1.AddFormattedText("APPROVED", "HeadOfThePage")
            Dim headp2 = headrow.Cells(1).AddParagraph()
            headp2.AddFormattedText("TRACKING ID: " & ApplicationId.ToString(), "HeadOfThePage")
            section.AddParagraph(Environment.NewLine)
            section.AddParagraph(Environment.NewLine)
            section.AddParagraph("Initiated by: " & app_info.Initiator)
            section.AddParagraph("Created On: " & app_info.CreatedDate.ToString("dd-MMM-yyyy hh:ss tt"))
            section.AddParagraph("Description: " & app_info.Description)
            section.AddParagraph(Environment.NewLine)

            For Each rw As DataRow In dtApprovalFlow.Rows
                Dim tbl As MigraDoc.DocumentObjectModel.Tables.Table = section.AddTable()
                tbl.TopPadding = "3"
                tbl.BottomPadding = "3"
                tbl.LeftPadding = "3"
                tbl.RightPadding = "3"
                tbl.Borders.Color = MigraDoc.DocumentObjectModel.Color.FromRgb(0, 0, 0)
                tbl.Style = "Table"
                Dim column As Column = tbl.AddColumn(sectionWidth)
                Dim column2 As Column = tbl.AddColumn(sectionWidth)
                Dim row As Row = tbl.AddRow()
                Dim p1 = row.Cells(0).AddParagraph("")
                p1.AddFormattedText(rw.Item("Role") & ":", TextFormat.Italic)
                p1.AddFormattedText(Environment.NewLine + rw.Item("Approver"), TextFormat.Bold)
                p1.AddFormattedText(Environment.NewLine)
                p1.AddFormattedText("Approval Date: " & Environment.NewLine, TextFormat.Italic)
                p1.AddFormattedText()

                'if the approver is Board do not show approval date - requested by Mahmudul Bhuiyan - 26 Apr 2022
                If rw.Item("Approver") = "Board" Then
                    p1.AddFormattedText("")
                Else
                    p1.AddFormattedText(rw.Item("CreatedOn").ToString())
                End If

                Dim p2 = row.Cells(1).AddParagraph()
                p2.AddFormattedText("Comment:", TextFormat.Italic)
                p2.AddFormattedText(Environment.NewLine)
                Dim cmnt As String

                If String.IsNullOrEmpty(rw.Item("Comment")) Then
                    cmnt = ""
                Else
                    cmnt = rw.Item("Comment")
                End If

                p2.AddFormattedText(cmnt)
                section.AddParagraph(Environment.NewLine)
            Next rw

            Dim footerpara As Paragraph = section.Footers.Primary.AddParagraph()
            footerpara.Format.Font.Size = 9
            footerpara.AddFormattedText("This document has been Automatically generated by Online Service Approval System (OSAP) and does not require any signature(s)")
            footerpara.AddFormattedText(Environment.NewLine)
            footerpara.AddFormattedText("Tracking ID # ")
            footerpara.AddFormattedText(ApplicationId.ToString(), TextFormat.Bold)
            footerpara.AddFormattedText(" | ")
            footerpara.AddFormattedText("FILEID # ")
            footerpara.AddFormattedText(app_info.FileName.Replace(".pdf", ""))
            footerpara.AddFormattedText(" | Created On # ")
            footerpara.AddFormattedText(DateTime.Now.ToString("dd-MMM-yyyy hh:ss tt"))
            Dim renderer As PdfDocumentRenderer = New PdfDocumentRenderer()
            renderer.Document = document
            renderer.RenderDocument()
            Dim FinalReportPDFFile As String = ApplicationId.ToString() & ".pdf"
            renderer.PdfDocument.Save(FinalReportPDFFile)
            Dim font As XFont = New XFont("Gill Sans MT", 20, XFontStyle.Bold)
            Dim PDFDoc As PdfSharp.Pdf.PdfDocument = PdfSharp.Pdf.IO.PdfReader.Open(RootPath & file_name, PdfDocumentOpenMode.Import)
            Dim PDFNewDoc As PdfSharp.Pdf.PdfDocument = New PdfSharp.Pdf.PdfDocument()

            Using ReportPDFDoc As PdfDocument = PdfSharp.Pdf.IO.PdfReader.Open(FinalReportPDFFile, PdfDocumentOpenMode.Import)

                For Pg As Integer = 0 To ReportPDFDoc.Pages.Count - 1
                    PDFNewDoc.AddPage(ReportPDFDoc.Pages(Pg))
                Next
            End Using

            For Pg As Integer = 0 To PDFDoc.Pages.Count - 1
                Dim page As PdfPage = PDFDoc.Pages(Pg)
                PDFNewDoc.AddPage(page)
            Next

            For pg As Integer = 0 To PDFNewDoc.Pages.Count - 1
                Dim gfx As XGraphics = XGraphics.FromPdfPage(PDFNewDoc.Pages(pg), XGraphicsPdfPageOptions.Append)
                Dim ft As XFont = New XFont("Gill Sans MT", 20, XFontStyle.Bold)
                gfx.DrawString("APPROVED | TRACKING : " & ApplicationId.ToString(), font, XBrushes.Black, New XRect(0, 0, PDFNewDoc.Pages(pg).Width, PDFNewDoc.Pages(pg).Height), XStringFormats.BottomRight)
            Next

            Dim outputfile As String = (RootPath & file_name).Replace(".pdf", "") & "_approved.pdf"
            PDFNewDoc.Save(outputfile)
            File.Delete(FinalReportPDFFile)
        Catch ex As Exception
            MessageBox(ex.Message)
        End Try
    End Sub

End Class
