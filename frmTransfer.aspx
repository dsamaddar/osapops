<%@ Page Language="VB" MasterPageFile="~/OsapMaster.master" AutoEventWireup="false"
    Theme="CommonSkin" CodeFile="frmTransfer.aspx.vb" Inherits="frmTransfer" Title=".:OSAP:Transfer:." %>

<asp:Content ID="Content1" ContentPlaceHolderID="headerPlaceHolder" runat="Server">

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
                <asp:Panel ID="pnlTransferableApplications" runat="server" SkinID="pnlInner">
                    <table style="width: 100%;">
                        <tr>
                            <td>
                                <div class="widget-title">
                                    Transferable Applications</div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div id="divgrd" onscroll="SetDivPosition()" style="max-height: 200px; max-width: 100%;
                                    overflow: auto">
                                    <asp:GridView ID="grdTransferableApps" runat="server" CssClass="mGrid" AutoGenerateColumns="False">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Select" ShowHeader="False">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkBtnSelect" runat="server" CausesValidation="False" CommandName="Select"
                                                        Text='<%# Bind("ApplicationId") %>'></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="ApplicationId" Visible="False">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblApplicationId" runat="server" Text='<%# Bind("ApplicationId") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="ProcessFlowId" Visible="False">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblProcessFlowId" runat="server" Text='<%# Bind("ProcessFlowId") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Type" HeaderText="Type" />
                                            <asp:BoundField DataField="Initiator" HeaderText="Initiator" />
                                            <asp:BoundField DataField="CreatedDate" HeaderText="Initiation Date" />
                                            <asp:BoundField DataField="Description" HeaderText="Description" />
                                            <asp:TemplateField HeaderText="ApproverId" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblApproverId" runat="server" Text='<%# Bind("ApproverId") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Approver" HeaderText="Approver" />
                                            <asp:BoundField DataField="Status" HeaderText="Status" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
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
                <asp:Panel ID="pnlTransferDestination" runat="server" SkinID="pnlInner">
                    <table style="width: 100%;">
                        <tr>
                            <td colspan="4">
                                <div class="widget-title">
                                    Transfer To</div>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 200px">
                            </td>
                            <td style="width: 120px">
                            </td>
                            <td style="width: 300px">
                            </td>
                            <td align="left">
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td class="label">
                                Transfer to
                            </td>
                            <td>
                                <asp:DropDownList ID="drpUserList" runat="server" Width="250px">
                                </asp:DropDownList>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td class="label">
                                Comments
                            </td>
                            <td>
                                <asp:TextBox ID="txtComments" runat="server" Height="60px" TextMode="MultiLine" Width="250px"></asp:TextBox>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td class="label">
                            </td>
                            <td>
                                <asp:Button ID="btnTransfer" runat="server" CssClass="styled-button-1" Text="Transfer"
                                    Width="100px" />
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" runat="Server">
</asp:Content>
