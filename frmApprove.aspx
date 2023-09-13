<%@ Page Language="VB" MasterPageFile="~/OsapMaster.master" AutoEventWireup="false"
    Theme="CommonSkin" CodeFile="frmApprove.aspx.vb" Inherits="frmApprove" Title=".:OSAP:Approve:." %>

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
            <td style="width: 30%; vertical-align: top">
                <table style="width: 100%;">
                    <tr>
                        <td>
                            <asp:Panel ID="pnlPendingApprovalInformation" runat="server" SkinID="pnlInner">
                                <table style="width: 100%;">
                                    <tr>
                                        <td>
                                            <div class="widget-title">
                                                Pending Approval Information</div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div id="divgrd" onscroll="SetDivPosition()" style="max-height: 150px; max-width: 100%;
                                                overflow: auto">
                                                <asp:GridView ID="grdingWaitingList" runat="server" AutoGenerateColumns="False" CssClass="mGrid"
                                                    Width="100%">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="ID" ShowHeader="False">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select"
                                                                    Text='<%# Bind("ApplicationId") %>'></asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="ProcessFlowId" Visible="False">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblProcessFlowId" runat="server" Text='<%# Bind("ProcessFlowId") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="ApplicationId" Visible="False">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblApplicationId" runat="server" Text='<%# Bind("ApplicationId") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="ApproverId" HeaderText="ApproverId" Visible="False" />
                                                        <asp:TemplateField HeaderText="ApplicationTypeId" Visible="False">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblApplicationTypeId" runat="server" Text='<%# Bind("ApplicationTypeId") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Type" HeaderText="App Type" />
                                                        <asp:BoundField DataField="Initiator" HeaderText="Initiator" />
                                                        <asp:BoundField DataField="Status" HeaderText="Status" />
                                                        <asp:BoundField DataField="CreatedDate" HeaderText="Dated" />
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
                            <asp:Panel ID="pnlDescription" runat="server" SkinID="pnlInner">
                                <table style="width: 100%;">
                                    <tr>
                                        <td>
                                            <div class="widget-title">
                                                Description</div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtDescription" runat="server" Height="100px" TextMode="MultiLine"
                                                Width="250px" Enabled="False"></asp:TextBox>
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
                            <asp:Panel ID="pnlApprovalStatus" runat="server" SkinID="pnlInner">
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <div class="widget-title">
                                                Approval Status</div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div id="divDocWorkFlow" onscroll="SetDivPosition()" style="max-height: 150px; max-width: 100%;
                                                overflow: auto">
                                                <asp:GridView ID="grdDocumentWorkflow" runat="server" AutoGenerateColumns="False"
                                                    CssClass="mGrid" EmptyDataText="No Application Found" Width="100%">
                                                    <Columns>
                                                        <asp:BoundField DataField="Sequence" HeaderText="ID" />
                                                        <asp:BoundField DataField="Approver" HeaderText="Approver" />
                                                        <asp:BoundField DataField="Role" HeaderText="Role" />
                                                        <asp:BoundField DataField="Status" HeaderText="Status" />
                                                        <asp:BoundField DataField="Comment" HeaderText="Comment" />
                                                        <asp:BoundField DataField="DecisionDate" HeaderText="DecisionDate" />
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 70%; vertical-align: top">
                <table style="width: 100%; height: 100%">
                    <tr>
                        <td>
                            <div>
                                <asp:Literal ID="ltEmbed" runat="server" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="pnlRemarks" runat="server" SkinID="pnlInner">
                                <table style="width: 100%;">
                                    <tr>
                                        <td colspan="2">
                                            <div class="widget-title">
                                                Remarks</div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 60%">
                                            <asp:TextBox ID="txtComments" runat="server" Height="80px" TextMode="MultiLine" Width="300px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="reqFldComments" runat="server" ControlToValidate="txtComments"
                                                ErrorMessage="Comments Required" ValidationGroup="Reject" CssClass="RequiredLabel"></asp:RequiredFieldValidator>
                                        </td>
                                        <td style="width: 40%">
                                            <asp:Button ID="btnReject" runat="server" BackColor="#FF7777" Text="Reject" Height="25px"
                                                Width="100px" OnClientClick="if (!confirm('Are you sure to reject the request ?')) return false"
                                                ValidationGroup="Reject" />
                                            <br />
                                            <br />
                                            <asp:Button ID="btnAccept" runat="server" BackColor="#8BD88B" Text="Accept" Height="25px"
                                                Width="100px" OnClientClick="if (!confirm('Are you sure to approve the request ?')) return false" />
                                            <br />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" runat="Server">
</asp:Content>
