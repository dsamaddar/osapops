<%@ Page Language="VB" MasterPageFile="~/OsapMaster.master" AutoEventWireup="false"
    Theme="CommonSkin" CodeFile="frmView.aspx.vb" Inherits="frmView" Title=".:OSAP:Viewer:." %>

<asp:Content ID="Content1" ContentPlaceHolderID="headerPlaceHolder" runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyPlaceHolder" runat="Server">
    <table width="100%">
        <tr>
            <td style="width: 30%; vertical-align: top">
                <table style="width: 100%;">
                    <tr>
                        <td style="vertical-align: top">
                            <asp:Panel ID="Panel1" runat="server" SkinID="pnlInner">
                                <table style="width: 100%;">
                                    <tr>
                                        <td colspan="2" style="vertical-align: top">
                                            <div class="widget-title">
                                                OSAP Viewer</div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 30%">
                                        </td>
                                        <td style="width: 70%">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 30%">
                                        </td>
                                        <td style="width: 70%">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 30%" class="label">
                                            Tracking Number
                                        </td>
                                        <td style="width: 70%">
                                            <asp:TextBox ID="txtTrackingNo" runat="server" CssClass="InputTxtBox"></asp:TextBox>
                                            &nbsp;<asp:Button ID="btnShow" runat="server" CssClass="styled-button-1" 
                                                Text="Show" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="label">
                                            Status</td>
                                        <td>
                                            <asp:Label ID="lblStatus" runat="server" CssClass="label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="label">
                                            Initiator</td>
                                        <td>
                                            <asp:Label ID="lblInitiator" runat="server" CssClass="label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="label">
                                            Initiation Date</td>
                                        <td>
                                            <asp:Label ID="lblInitiationDate" runat="server" CssClass="label"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="label">
                                            Description</td>
                                        <td>
                                            <asp:TextBox ID="txtDescription" runat="server" CssClass="InputTxtBox" 
                                                Height="100px" ReadOnly="True" TextMode="MultiLine" Width="90%"></asp:TextBox>
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
                            <asp:Panel ID="pnlApprovalWorkflow" runat="server" SkinID="pnlInner">
                                <table style="width: 100%;">
                                    <tr>
                                        <td colspan="2">
                                            <div class="widget-title">
                                                Approval Workflow
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:GridView ID="grdApprovalFlow" runat="server" AutoGenerateColumns="False" CssClass="mGrid">
                                                <Columns>
                                                    <asp:BoundField DataField="Sequence" HeaderText="Sequence" />
                                                    <asp:BoundField DataField="Approver" HeaderText="Approver" />
                                                    <asp:BoundField DataField="Role" HeaderText="Role" />
                                                    <asp:BoundField DataField="Status" HeaderText="Status" />
                                                    <asp:BoundField DataField="Comment" HeaderText="Comment" />
                                                    <asp:BoundField DataField="DecisionDate" HeaderText="DecisionDate" />
                                                </Columns>
                                            </asp:GridView>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 70%; vertical-align: top">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <asp:Literal ID="ltEmbed" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" runat="Server">
</asp:Content>
