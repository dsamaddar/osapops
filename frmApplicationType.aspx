<%@ Page Language="VB" MasterPageFile="~/OsapMaster.master" AutoEventWireup="false"
    CodeFile="frmApplicationType.aspx.vb" Inherits="frmApplicationType" Title=".:OSAP:Application Type:."
    Theme="CommonSkin" %>

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
                <asp:Panel ID="pnlNewAppType" runat="server" SkinID="pnlInner">
                    <table style="width: 100%;">
                        <tr>
                            <td colspan="4">
                                <div class="widget-title">
                                    Define Application Type</div>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 30%; height: 15px;">
                            </td>
                            <td style="height: 15px">
                            </td>
                            <td style="height: 15px">
                                <asp:HiddenField ID="hdFldApplicationTypeId" runat="server" />
                            </td>
                            <td style="width: 30%; height: 15px;">
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td class="label">
                                Application Type
                            </td>
                            <td>
                                <asp:TextBox ID="txtApplicationType" runat="server" CssClass="InputTxtBox" Width="250px"></asp:TextBox>
                            </td>
                            <td>
                                &nbsp;<asp:RequiredFieldValidator ID="reqFldAppType" runat="server" 
                                    ControlToValidate="txtApplicationType" CssClass="RequiredLabel" 
                                    ErrorMessage="*" ValidationGroup="AppType"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td class="label">
                                Description
                            </td>
                            <td>
                                <asp:TextBox ID="txtDescription" runat="server" CssClass="InputTxtBox" Height="50px"
                                    TextMode="MultiLine" Width="250px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="reqFldDescription" runat="server" 
                                    ControlToValidate="txtDescription" CssClass="RequiredLabel" ErrorMessage="*" 
                                    ValidationGroup="AppType"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td class="label">
                                Email
                            </td>
                            <td>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="InputTxtBox" Width="200px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="reqFldEmail" runat="server" 
                                    ControlToValidate="txtEmail" CssClass="RequiredLabel" ErrorMessage="*" 
                                    ValidationGroup="AppType"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td class="label">
                                Is Visible
                            </td>
                            <td>
                                <asp:CheckBox ID="chkBxIsVisible" runat="server" CssClass="chkText" Text="Yes/No" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                                <asp:Button ID="btnAdd" runat="server" CssClass="styled-button-1" Text="ADD" 
                                    ValidationGroup="AppType" />
                                &nbsp;<asp:Button ID="btnUpdate" runat="server" CssClass="styled-button-1" 
                                    Text="Update" ValidationGroup="AppType" />
                                &nbsp;<asp:Button ID="btnCancel" runat="server" CssClass="styled-button-1" 
                                    Text="Cancel" />
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
        <tr>
            <td>
                <div class="widget-title">
                    Application Type List</div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Panel ID="pnlAppList" runat="server" SkinID="pnlInner">
                    <div id="divgrd" onscroll="SetDivPosition()" style="max-height: 250px; max-width: 100%;
                        overflow: auto">
                        <asp:GridView ID="grdAppType" runat="server" CssClass="mGrid" AutoGenerateColumns="False"
                            Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="Select" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select"
                                            Text="Select"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Id" Visible="False">
                                    <ItemTemplate>
                                        <asp:Label ID="lblApplicationTypeId" runat="server" Text='<%# Bind("ApplicationTypeId") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Application Type">
                                    <ItemTemplate>
                                        <asp:Label ID="lblApplicationTypeText" runat="server" Text='<%# Bind("ApplicationTypeText") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Description">
                                    <ItemTemplate>
                                        <asp:Label ID="lblApplicationTypeDescription" runat="server" Text='<%# Bind("ApplicationTypeDescription") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Email">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEmail" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="IsVisible">
                                    <ItemTemplate>
                                        <asp:Label ID="lblIsVisible" runat="server" Text='<%# Bind("IsVisible") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" runat="Server">
</asp:Content>
