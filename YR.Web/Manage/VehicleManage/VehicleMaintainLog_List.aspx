﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehicleMaintainLog_List.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.VehicleMaintainLog_List" %>

<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>保养记录管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            //divresize(63);
            FixedTableHeader("#dnd-example", $(window).height() - 91);
        })

        //新增
        function add() {
            var url = "/Manage/VehicleManage/VehicleMaintainLog_Form.aspx";
            top.openDialog(url, 'Pits_Form', '保养记录 - 添加', 700, 350, 50, 50);
        }
        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/VehicleManage/VehicleMaintainLog_Form.aspx?key=" + key;
                top.openDialog(url, 'Pits_Form', '保养记录 - 编辑', 700, 350, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            if (IsDelData(key)) {
                var delparm = 'action=Virtualdelete&module=用户管理&tableName=YR_UserInfo&pkName=ID&pkVal=' + key;
                delConfig('/Ajax/Common_Ajax.ashx', delparm)
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="script1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel runat="server" ID="UpdatePanel2"> 
        <ContentTemplate>
    <div class="btnbartitle">
        <div>
            保养纪录管理
        </div>
    </div>
    <div class="btnbarcontetn">
        <div style="text-align: right">
            <uc1:LoadButton ID="LoadButton1" runat="server" />
        </div>
                <div style="float: left;">
            <table class="tabCondition">
                <tr>
                    <th>
                        维修站
                    </th>
                    <td>
                        <input type="text" id="txtName" runat="server" placeholder="维修站名称" tabindex="1" autocomplete="off" />
                    </td>
                    <th>
                        保养车辆
                    </th>
                    <td>
                        <input type="text" id="txtVehicleName" runat="server" placeholder="保养车辆名称" tabindex="1" autocomplete="off" />
                    </td>
                    <th>
                        保养项目
                    </th>
                    <td>
                        <input type="text" id="txtMaintainItems" runat="server" placeholder="保养项目" tabindex="1" autocomplete="off" />
                    </td>
                    <th>
                        保养时间
                    </th>
                    <td>
                        <input id="txtStartCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 100px" placeholder="开始时间" tabindex="1" autocomplete="off" />-
                        <input id="txtEndCreateTime" runat="server" type="text" class="txt" onfocus="WdatePicker({dateFmt: 'yyyy-MM-dd' })"
                            checkexpession="NotNull" style="width: 100px" placeholder="结束时间" tabindex="1" autocomplete="off" />
                    </td>
                </tr>
                <tr>
                    <th>
                        保养人
                    </th>
                    <td>
                        <input type="text" id="txtMaintainPeople" runat="server" placeholder="保养人" tabindex="1" autocomplete="off" />
                    </td>
                    <th>
                        保养人电话
                    </th>
                    <td colspan="7">
                        <input type="text" id="txtLinkPhone" runat="server" placeholder="保养人电话" tabindex="1" autocomplete="off" />
                    </td>
                </tr>
                <tr style="text-align: center">
                    <td colspan="8">
                        <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click"><span class="icon-botton"
            style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;">
        </span>查 询</asp:LinkButton><asp:LinkButton ID="lbtInit" runat="server" class="button green"
            OnClick="lbtInit_Click"><span class="icon-botton">
        </span>重 置</asp:LinkButton>
                    </td>
                </tr>
            </table>
        </div>

    </div>
    <div class="div-body">
        <table id="table1" class="grid" singleselect="true">
            <thead>
                <tr>
                    <td style="width: 20px; text-align: center;">
                        选择
                    </td>
                    <td style="width: 60px; text-align: center;">
                        维修站
                    </td>
                    <td style="width: 60px; text-align: center;">
                        保养车辆
                    </td>
                    <td style="width: 60px; text-align: center;">
                        保养项目
                    </td>
                    <td style="width: 60px; text-align: center;">
                        保养时间
                    </td>
                    <td style="width: 100px; text-align: center;">
                        保养人
                    </td>
                    <td style="width: 180px;text-align: center;">
                        创建时间
                    </td>
                </tr>
            </thead>
            <tbody>
            
                <asp:Repeater ID="rp_Item" runat="server" OnItemDataBound="rp_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td style="width: 20px; text-align: center;">
                                <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                            </td>
                            <td style="width: 60px; text-align: center;">
                                <%#Eval("PitsName")%>
                            </td>
                            <td style="width: 80px; text-align: center;">
                                <a href="javascript:void()">
                                <%#Eval("VehicleName")%></a>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%#Eval("MaintainItems")%>
                            </td>
                            <td style="width: 200px; text-align: center;">
                                <%#Eval("MaintainTime")%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%#Eval("MaintainPeople")%>
                            </td>
                            <td style="width: 50px; text-align: center;">
                                <%#Eval("CreateTime")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <% if (rp_Item != null)
                           {
                               if (rp_Item.Items.Count == 0)
                               {
                                   Response.Write("<tr><td colspan='7' style='color:red;text-align:center'>没有找到您要的相关数据！</td></tr>");
                               }
                           } %>
                    </FooterTemplate>
                </asp:Repeater>
            </tbody>
        </table><br />
    <uc1:PageControl ID="PageControl1" runat="server" />
    </div>
            </ContentTemplate>
    <Triggers>
    <asp:AsyncPostBackTrigger ControlID="lbtSearch" EventName="Click" />
    <asp:AsyncPostBackTrigger ControlID="lbtInit" EventName="Click" />
    <asp:AsyncPostBackTrigger ControlID="rp_Item" EventName="ItemDataBound" />
    </Triggers>
    </asp:UpdatePanel>
    </form>
</body>
</html>
