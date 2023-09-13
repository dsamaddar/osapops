<%@ Page Language="VB" MasterPageFile="~/OsapMaster.master" AutoEventWireup="false"
    CodeFile="frmMonitoring.aspx.vb" Inherits="frmMonitoring" Title=".:OSAP:Monitoring:." %>

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
        <tr valign="top">
            <td style="width: 30%">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <div class="widget-title">
                                User List</div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div id="divgrd" onscroll="SetDivPosition()" style="max-height: 500px; max-width: 100%;
                                overflow: auto">
                                <asp:BulletedList ID="bulReportList" runat="server" BorderStyle="None" DisplayMode="LinkButton">
                                </asp:BulletedList>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <table style="width: 100%;">
                    <tr>
                        <td style="width: 70%">
                            <div class="widget-title">
                                <asp:Label ID="lblPendingTaskList" runat="server" Text="Pending List"></asp:Label>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 70%">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 70%">
                            <div style="max-height: 250px; max-width: 100%; overflow: auto">
                                <asp:GridView ID="grdPendingTaskList" runat="server" CssClass="mGrid" AutoGenerateColumns="False">
                                    <Columns>
                                        <asp:BoundField DataField="ApplicationId" HeaderText="Tracking#" />
                                        <asp:TemplateField HeaderText="Memo">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="hpMemo" runat="server" CssClass="linkbtn" NavigateUrl='<%# ConfigurationManager.AppSettings("osap_storage") + Eval("FileName") %>'
                                                    Target="_blank">Memo</asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Type" HeaderText="Type" />
                                        <asp:BoundField DataField="Description" HeaderText="Description" />
                                        <asp:BoundField DataField="Initiator" HeaderText="Initiator" />
                                        <asp:BoundField DataField="Status" HeaderText="Status" />
                                        <asp:BoundField DataField="CreatedDate" HeaderText="CreatedDate" />
                                        <asp:BoundField DataField="Decision" HeaderText="Decision" />
                                        <asp:BoundField DataField="Comment" HeaderText="Comment" />
                                        <asp:BoundField DataField="DecisionDate" HeaderText="DecisionDate" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 70%">
                            <div class="widget-title">
                                <asp:Label ID="lblPerformedTask" runat="server" Text="Performed List"></asp:Label>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 70%">
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 70%">
                            <div style="max-height: 250px; max-width: 100%; overflow: auto">
                                <asp:GridView ID="grdPerformedTaskList" runat="server" CssClass="mGrid" AutoGenerateColumns="False">
                                    <Columns>
                                        <asp:BoundField DataField="ApplicationId" HeaderText="Tracking#" />
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
                                        <asp:BoundField DataField="Comment" HeaderText="Comment" />
                                        <asp:BoundField DataField="DecisionDate" HeaderText="DecisionDate" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 70%">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptPlaceHolder" runat="Server">
</asp:Content>
