﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using YR.Common.DotNetUI;
using System.Text;
using YR.Busines;
using YR.Common.DotNetCode;
using YR.Busines.DAL;
using YR.Busines.IDAO;
using YR.Web.App_Code;

namespace YR.Web.YRBase.SysAppend
{
    public partial class AppendProperty_List : PageBase
    {
        YR_System_IDAO systemidao = new YR_System_Dal();
        public string _Function;
        protected void Page_Load(object sender, EventArgs e)
        {
            _Function = Server.UrlDecode(Request["Function"]);
            if (!IsPostBack)
            {
                InitData();
                if (string.IsNullOrEmpty(_Function))
                {
                    _Function = "未选择";
                }
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            DataTable dt = new DataTable();
            if (!string.IsNullOrEmpty(_Function))
            {
                dt = systemidao.AppendProperty_List(_Function);
            }
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
        }
        protected void rp_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label title = e.Item.FindControl("lblProperty_Control_Type") as Label;
                if (title != null)
                {
                    string text = title.Text;
                    text = text.Replace("1", "文本框");
                    text = text.Replace("2", "下拉框");
                    text = text.Replace("3", "日期框");
                    text = text.Replace("4", "标  签");
                    title.Text = text;
                }
            }
        }
    }
}