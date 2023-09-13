<%@ Page Language="VB" MasterPageFile="~/OsapMaster.master" AutoEventWireup="false"
    Theme="CommonSkin" CodeFile="frmFind.aspx.vb" Inherits="frmFind" Title=".:OSAP:Find:." %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headerPlaceHolder" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyPlaceHolder" runat="Server">
    <table style="width: 100%;">
        <tr>
            <td style="width: 35%; vertical-align: top">
                <asp:Panel ID="pnlParameter" runat="server" SkinID="pnlInner">
                    <table style="width: 100%;">
                        <tr>
                            <td colspan="2">
                                <div class="widget-title">
                                    Search By</div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:ScriptManager ID="ScriptManager1" runat="server">
                                </asp:ScriptManager>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                Type/Description/Content
                            </td>
                            <td>
                                <asp:TextBox ID="txtDescription" runat="server" CssClass="InputTxtBox" Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                Document Type
                            </td>
                            <td>
                                <asp:DropDownList ID="drpDocumentType" runat="server" CssClass="InputTxtBox" Width="200px">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                Approver
                            </td>
                            <td>
                                <asp:DropDownList ID="drpApprover" runat="server" CssClass="InputTxtBox" Width="200px">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                Task Status
                            </td>
                            <td>
                                <asp:DropDownList ID="drpTaskStatus" runat="server" CssClass="InputTxtBox" Width="150px">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                Date From
                            </td>
                            <td>
                                <asp:TextBox ID="txtDateFrom" runat="server" CssClass="InputTxtBox" Width="100px"></asp:TextBox>
                                <cc1:CalendarExtender ID="txtDateFrom_CalendarExtender" runat="server" Enabled="True"
                                    TargetControlID="txtDateFrom" Format="MM/dd/yyyy">
                                </cc1:CalendarExtender>
                                &nbsp;<asp:RequiredFieldValidator ID="reqFldStartDt" runat="server" ControlToValidate="txtDateFrom"
                                    CssClass="RequiredLabel" ErrorMessage="*" ValidationGroup="Search"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                Date To
                            </td>
                            <td>
                                <asp:TextBox ID="txtDateTo" runat="server" CssClass="InputTxtBox" Width="100px"></asp:TextBox>
                                <cc1:CalendarExtender ID="txtDateTo_CalendarExtender" runat="server" Enabled="True"
                                    Format="MM/dd/yyyy" TargetControlID="txtDateTo">
                                </cc1:CalendarExtender>
                                &nbsp;<asp:RequiredFieldValidator ID="reqFldEndDt" runat="server" ControlToValidate="txtDateTo"
                                    CssClass="RequiredLabel" ErrorMessage="*" ValidationGroup="Search"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:Button ID="btnFind" runat="server" CssClass="styled-button-1" Text="Search"
                                    ValidationGroup="Search" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
            <td style="width: 65%; vertical-align: top">
                <div>
                    <asp:Button ID="btnExport" runat="server" Text="Export To Excel" OnClick="ExportToExcel" /></div>
                <div id="divgrd" onscroll="SetDivPosition()" style="max-height: 400px; max-width: 100%;
                    overflow: auto">
                    
                    <asp:GridView ID="grdSearchResult" runat="server" CssClass="mGrid" EmptyDataText="No search result"
                        AutoGenerateColumns="False">
                        <Columns>
                            <asp:BoundField DataField="ApplicationId" HeaderText="TrackingNo" />
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
                            <asp:BoundField DataField="Type" HeaderText="Type" />
                            <asp:BoundField DataField="Description" HeaderText="Description" />
                            <asp:BoundField DataField="Initiator" HeaderText="Initiator" />
                            <asp:BoundField DataField="Status" HeaderText="Status" />
                            <asp:BoundField DataField="CreatedDate" HeaderText="CreatedDate" />
                            <asp:BoundField DataField="Decision" HeaderText="Decision" />
                            <asp:BoundField DataField="Authorizer" HeaderText="Authorizer" />
                            <asp:BoundField DataField="Comment" HeaderText="Comment" />
                            <asp:BoundField DataField="DecisionDate" HeaderText="DecisionDate" />
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" runat="Server">
</asp:Content>
