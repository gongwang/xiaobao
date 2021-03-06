﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VehicleModel_List.aspx.cs" Inherits="YR.Web.Manage.VehicleManage.VehicleModel_List" %>


<%@ Register Src="../../UserControl/PageControl.ascx" TagName="PageControl" TagPrefix="uc1" %>
<%@ Register Src="../../UserControl/LoadButton.ascx" TagName="LoadButton" TagPrefix="uc1" %>

<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>字典管理</title>
    <link href="/Themes/Styles/Site.css" rel="stylesheet" type="text/css" />
    <script src="/Themes/Scripts/jquery-1.8.2.min.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/jquery.pullbox.js" type="text/javascript"></script>
    <script src="/Themes/Scripts/FunctionJS.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $('#txt_Search').bind('keypress', function (event) {
                if (event.keyCode == "13") {
                    load();
                }
            });
        })

        //新增
        function add() {
            var url = "/Manage/VehicleManage/VehicleModel_Form.aspx";
            top.openDialog(url, 'VehicleModel_Form', '车型信息 - 添加', 800, 570, 50, 50);
        }
        //编辑
        function edit() {
            var key = CheckboxValue();
            if (IsEditdata(key)) {
                var url = "/Manage/VehicleManage/VehicleModel_Form.aspx?key=" + key;
                top.openDialog(url, 'VehicleModel_Form', '车型信息 - 编辑', 800, 570, 50, 50);
            }
        }
        //删除
        function Delete() {
            var key = CheckboxValue();
            var selValue = key.split(',');
            if (IsDelData(selValue[0])) {
                var delparm = 'action=Virtualdelete&module=车型管理&tableName=YR_VehicleModel&pkName=ID&pkVal=' + key;
                delConfigNoFresh('/Ajax/Common_Ajax.ashx', delparm)
            }
        }
        //执行查询
        function load() {
            __doPostBack('lbtSearch', '');
        }

        function ReviewInfo(url) {
            top.openDialog(url, 'VehicleModel_List', '车型提示 - 预览', 900, 600, 50, 50);
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
                        车型列表
                    </div>
                </div>
                <div class="btnbarcontetn">
                    <div style="float: left;">
                        车型名称：
                         <input type="text" id="txtName" runat="server" style="width: 176px;" />
                        <asp:LinkButton ID="lbtSearch" runat="server" class="button green" OnClick="lbtSearch_Click"><span class="icon-botton"
            style="background: url('../../Themes/images/Search.png') no-repeat scroll 0px 4px;"> </span>查 询</asp:LinkButton>
                    </div>
                    <div style="text-align: right">
                        <uc1:LoadButton ID="LoadButton1" runat="server" />
                    </div>
                </div>
                <div class="div-body">
                    <table id="table1" class="grid" singleselect="true">
                        <thead>
                            <tr>
                                <td style="width: 5%; text-align: center;">选择</td>
                                <td style="width: 25%; text-align: center;">名称</td>
                                <td style="width: 35%; text-align: center;">简介</td>
                                <td style="width: 10%; text-align: center;">图片</td>
                                <td style="width: 10%; text-align: center;">显示顺序</td>
                                <td style="width: 10%; text-align: center;">创建时间</td>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rp_Item" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td style="text-align: center;">
                                            <input type="checkbox" value="<%#Eval("ID")%>" name="checkbox" />
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("Name")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("Intro")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("Images")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("Sort")%>
                                        </td>
                                        <td style="text-align: center;">
                                            <%#Eval("CreateTime")%>
                                        </td>

                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <% if (rp_Item != null)
                                       {
                                           if (rp_Item.Items.Count == 0)
                                           {
                                               Response.Write("<tr><td colspan='5' style='color:red;text-align:center'>没有找到您要的相关数据！</td></tr>");
                                           }
                                       } %>
                                </FooterTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                    <br />
                    <uc1:PageControl ID="PageControl1" runat="server" />
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="lbtSearch" EventName="Click" />
                <asp:PostBackTrigger ControlID="PageControl1" />
            </Triggers>
        </asp:UpdatePanel>
    </form>
</body>
</html>
