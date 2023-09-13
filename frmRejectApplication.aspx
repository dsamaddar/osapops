<%@ Page Language="VB" MasterPageFile="~/OsapMaster.master" AutoEventWireup="false"
    Theme="CommonSkin" CodeFile="frmRejectApplication.aspx.vb" Inherits="frmRejectApplication"
    Title=".:OSAP:Reject Application:." %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headerPlaceHolder" runat="Server">
    <style type="text/css">
        /*Calendar Control CSS*/.MyCalendarCss .ajax__calendar_container
        {
            background-color: #DEF1F4;
            border: solid 1px #77D5F7;
        }
        .MyCalendarCss .ajax__calendar_header
        {
            background-color: #ffffff;
            margin-bottom: 4px;
        }
        .MyCalendarCss .ajax__calendar_title, .MyCalendarCss .ajax__calendar_next, .MyCalendarCss .ajax__calendar_prev
        {
            color: #004080;
            padding-top: 3px;
        }
        .MyCalendarCss .ajax__calendar_body
        {
            background-color: #ffffff;
            border: solid 1px #77D5F7;
        }
        .MyCalendarCss .ajax__calendar_dayname
        {
            text-align: center;
            font-weight: bold;
            margin-bottom: 4px;
            margin-top: 2px;
            color: #004080;
        }
        .MyCalendarCss .ajax__calendar_day
        {
            color: #004080;
            text-align: center;
        }
        .MyCalendarCss .ajax__calendar_hover .ajax__calendar_day, .MyCalendarCss .ajax__calendar_hover .ajax__calendar_month, .MyCalendarCss .ajax__calendar_hover .ajax__calendar_year, .MyCalendarCss .ajax__calendar_active
        {
            color: #004080;
            font-weight: bold;
            background-color: #DEF1F4;
        }
        .MyCalendarCss .ajax__calendar_today
        {
            font-weight: bold;
        }
        .MyCalendarCss .ajax__calendar_other, .MyCalendarCss .ajax__calendar_hover .ajax__calendar_today, .MyCalendarCss .ajax__calendar_hover .ajax__calendar_title
        {
            color: #bbbbbb;
        }
    </style>

    <script type="text/javascript">
        window.onload = function() {
            var strCook = document.cookie;
            if (strCook.indexOf("!~") != 0) {
                var intS = strCook.indexOf("!~");
                var intE = strCook.indexOf("~!");
                var strPos = strCook.substring(intS + 2, intE);
                document.getElementById("divgrd").scrollTop = strPos;
            }
        }
        function SetDivPosition() {
            var intY = document.getElementById("divgrd").scrollTop;
            document.title = intY;
            document.cookie = "yPos=!~" + intY + "~!";
        }

        window.scrollBy(100, 100); 
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyPlaceHolder" runat="Server">
    <table style="width: 100%;">
        <tr>
            <td>
                <asp:ScriptManager ID="ScriptManager1" runat="server">
                </asp:ScriptManager>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="pnlInitiatedApps" runat="server" SkinID="pnlInner">
                    <table style="width: 100%;">
                        <tr>
                            <td>
                                <table style="width: 100%;">
                                    <tr>
                                        <td colspan="6">
                                            <div class="widget-title">
                                                Find Applications</div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 50%">
                                        </td>
                                        <td class="label">
                                            From
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtDateFrom" runat="server" CssClass="InputTxtBox"></asp:TextBox>
                                            <cc1:CalendarExtender ID="txtDateFrom_CalendarExtender" runat="server" Enabled="True"
                                                TargetControlID="txtDateFrom" CssClass="MyCalendarCss">
                                            </cc1:CalendarExtender>
                                        </td>
                                        <td class="label">
                                            To
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtDateTo" runat="server" CssClass="InputTxtBox"></asp:TextBox>
                                            <cc1:CalendarExtender ID="txtDateTo_CalendarExtender" runat="server" Enabled="True"
                                                TargetControlID="txtDateTo" CssClass="MyCalendarCss">
                                            </cc1:CalendarExtender>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="styled-button-1"
                                                ValidationGroup="Search" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="widget-title">
                                    Reject Initiated Application</div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div>
                                    <asp:GridView ID="grdInitiatedApplications" runat="server" AutoGenerateColumns="False"
                                        CssClass="mGrid" EmptyDataText="No Application Found" Width="100%">
                                        <Columns>
                                            <asp:TemplateField HeaderText="" ShowHeader="False">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select"
                                                        Text='<%# Bind("Tracking") %>'></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Tracking" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblApplicationId" runat="server" Text='<%# Bind("Tracking") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Memo">
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="hpMemo" runat="server" CssClass="linkbtn" NavigateUrl='<%# ConfigurationManager.AppSettings("osap_storage") + Eval("FileName") %>'
                                                        Target="_blank">Memo</asp:HyperLink>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Approved Memo">
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="hpApprovedMemo" runat="server" CssClass="linkbtn" NavigateUrl='<%# ConfigurationManager.AppSettings("osap_storage") + Eval("ApprovedFileName") %>'
                                                        Target="_blank">Approved Memo</asp:HyperLink>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Type" HeaderText="Document Type" />
                                            <asp:BoundField DataField="Description" HeaderText="Description" />
                                            <asp:BoundField DataField="Status" HeaderText="Status" />
                                            <asp:BoundField DataField="CreatedDate" HeaderText="CreatedDate" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="pnlWorkFlowInformation" runat="server" SkinID="pnlInner">
                    <div class="widget-title">
                        Documents workflow information
                    </div>
                    <div id="divDocWorkflow" onscroll="SetDivPosition()" style="max-height: 150px; max-width: 100%;
                        overflow: auto">
                        <asp:GridView ID="grdDocumentWorkflow" runat="server" AutoGenerateColumns="False"
                            Width="100%" EmptyDataText="No Application Found" CssClass="mGrid">
                            <Columns>
                                <asp:BoundField DataField="Sequence" HeaderText="Sequence" />
                                <asp:BoundField DataField="Approver" HeaderText="Approver" />
                                <asp:BoundField DataField="Role" HeaderText="Role" />
                                <asp:BoundField DataField="Status" HeaderText="Status" />
                                <asp:BoundField DataField="Comment" HeaderText="Comment" />
                                <asp:BoundField DataField="DecisionDate" HeaderText="DecisionDate" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="pnlRejectionCause" runat="server" SkinID="pnlInner">
                    <table style="width: 100%;">
                        <tr>
                            <td style="width: 200px">
                            </td>
                            <td style="width: 150px">
                            </td>
                            <td style="width: 300px">
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td class="label">
                                Rejection Remarks
                            </td>
                            <td>
                                <asp:TextBox ID="txtRejectionRemarks" runat="server" CssClass="InputTxtBox" Height="50px"
                                    TextMode="MultiLine" ValidationGroup="Reject" Width="250px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="reqFldRejectionRemarks" runat="server" CssClass="RequiredLabel"
                                    ErrorMessage="*" ControlToValidate="txtRejectionRemarks" ValidationGroup="Reject"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:Button ID="btnReject" runat="server" CssClass="styled-button-1" Text="Reject"
                                    ValidationGroup="Reject" Enabled="False" />
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" runat="Server">
</asp:Content>
