<%@ Page Language="VB" MasterPageFile="~/OsapMaster.master" AutoEventWireup="false"
    Theme="CommonSkin" CodeFile="frmLoadRptConfig.aspx.vb" Inherits="frmLoadRptConfig"
    Title=".:Bankulator:Core Report:." %>

<asp:Content ID="Content1" ContentPlaceHolderID="headerPlaceHolder" runat="Server">

    <script type="text/javascript">
        $(function () {
            $(".datepicker").datepicker({
                showOn: 'both',
                buttonImageOnly: true,
                buttonImage: 'asset/img/calendar.gif'
             
            });
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="bodyPlaceHolder" runat="Server">
    <table style="width: 100%;">
        <tr>
            <td style="width: 70%">
                <table style="width: 100%;">
                    <tr>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="pnlLoadReport" runat="server" SkinID="pnlInner">
                                <table style="width: 100%;">
                                    <tr align="left">
                                        <td colspan="4">
                                            <div class="widget-title">
                                                <asp:Label ID="lblReportName" runat="server" Text="Load Report"></asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 50px">
                                        </td>
                                        <td style="width: 200px">
                                        </td>
                                        <td>
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            Fast Path
                                        </td>
                                        <td align="left">
                                            <asp:TextBox ID="txtFastPath" runat="server" AutoPostBack="True" Width="50px" autocomplete="off"></asp:TextBox>
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
                                            <asp:HiddenField ID="hdfldReportFile" runat="server" />
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
                    <tr align="left">
                        <td>
                            <div class="widget-title">
                                Parameter Selection
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="pnlParameters" runat="server" SkinID="pnlInner">
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="pnlExports" runat="server" SkinID="pnlInner">
                                <table style="width: 100%;">
                                    <tr align="left">
                                        <td colspan="4">
                                            <div class="widget-title">
                                                Export Options
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 50px">
                                        </td>
                                        <td style="width: 200px">
                                            <asp:RadioButtonList ID="rdbtnExportOptions" runat="server" RepeatDirection="Horizontal"
                                                Visible="False">
                                                <asp:ListItem>Preview</asp:ListItem>
                                                <asp:ListItem>Export</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                        <td style="width: 200px">
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td align="left">
                                            <asp:DropDownList ID="drpExportOptions" runat="server" Width="150px">
                                                <asp:ListItem Text="PDF" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="Rich Text" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="Word for Windows" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="Excel" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="Excel Record" Value="8"></asp:ListItem>
                                                <asp:ListItem Text="HTML 3.2" Value="6"></asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td align="left">
                                            <asp:Button ID="btnGenerate" runat="server" Text="Generate" ValidationGroup="ParamValidation" />
                                        </td>
                                        <td align="left">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td align="left">
                                            <input type="text" id="txtReturn" style="display: none;" onchange="PlaceAgreementID();" />
                                        </td>
                                        <td>
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
                            <div style="max-height: 200px; max-width: 100%; overflow: auto">
                                <asp:GridView ID="grdReport" runat="server">
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 30%" align="left" valign="top">
                <asp:BulletedList ID="bulReportList" runat="server" BorderStyle="Outset" DisplayMode="HyperLink">
                </asp:BulletedList>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" runat="Server">
</asp:Content>
