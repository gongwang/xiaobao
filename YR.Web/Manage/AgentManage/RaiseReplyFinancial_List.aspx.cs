﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using YR.Busines.IDAO;
using YR.Busines.DAL;
using YR.Common.DotNetCode;
using YR.Common.DotNetUI;
using YR.Web.App_Code;
using Asiasofti.SmartVehicle.Manager;

namespace YR.Web.Manage.AgentManage
{
    public partial class RaiseReplyFinancial_List : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageControl1.pageHandler += new EventHandler(pager_PageChanged);
            if (!IsPostBack)
            {
                DataBindGrid();
            }
        }


        /// <summary>
        /// 绑定数据，分页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void pager_PageChanged(object sender, EventArgs e)
        {
            DataBindGrid();
        }

        /// <summary>
        /// 绑定数据源
        /// </summary>
        private void DataBindGrid(bool isQuery = false)
        {
            UserRaiseFinancialManager userRaiseFinancialManager = new UserRaiseFinancialManager();
            int count = 0;
            int pageIndex = isQuery ? 1 : PageControl1.PageIndex;
            KeyValuePair<StringBuilder, IList<SqlParam>> keyValue = InitCondition();

            DataTable dt = userRaiseFinancialManager.GetUserRaiseFinancialInfoPage(keyValue.Key, keyValue.Value, pageIndex, PageControl1.PageSize, ref count);
            ControlBindHelper.BindRepeaterList(dt, rp_Item);
            this.PageControl1.PageIndex = pageIndex;
            this.PageControl1.RecordCount = Convert.ToInt32(count);
            this.PageControl1.PageChecking();
        }

        /// <summary>
        /// 绑定后激发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rp_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblUser_Sex = e.Item.FindControl("lblUser_Sex") as Label;
                Label lblDeleteMark = e.Item.FindControl("lblDeleteMark") as Label;
                if (lblUser_Sex != null)
                {
                    string text = lblUser_Sex.Text;
                    text = text.Replace("1", "男士");
                    text = text.Replace("0", "女士");
                    lblUser_Sex.Text = text;

                    string textDeleteMark = lblDeleteMark.Text;
                    textDeleteMark = textDeleteMark.Replace("1", "<span style='color:Blue'>启用</span>");
                    textDeleteMark = textDeleteMark.Replace("2", "<span style='color:red'>停用</span>");
                    lblDeleteMark.Text = textDeleteMark;
                }
            }
        }

        /// <summary>
        /// 初始化查询条件
        /// </summary>
        /// <returns></returns>
        private KeyValuePair<StringBuilder, IList<SqlParam>> InitCondition()
        {
            StringBuilder sb = new StringBuilder();
            List<SqlParam> IList_param = new List<SqlParam>();

            // 关键字（登录名、真实姓名、手机号）
            if (!string.IsNullOrEmpty(txt_Search.Value))
            {
                sb.Append(" and (b.UserName like @name or b.RealName like @name or b.BindPhone like @name) ");
                IList_param.Add(new SqlParam("@name", '%' + txt_Search.Value.Trim() + '%'));
            }
            // 操作者
            if (!string.IsNullOrEmpty(txtOperation.Value))
            {
                sb.Append(" and c.OperatorName like @OperatorName ");
                IList_param.Add(new SqlParam("@OperatorName", '%' + txtOperation.Value.Trim() + '%'));
            }
            // 订单号
            if (!string.IsNullOrEmpty(txtOrdersNo.Value))
            {
                sb.Append(" and d.OrderNum like @OrdersNo ");
                IList_param.Add(new SqlParam("@OrdersNo", '%' + txtOrdersNo.Value.Trim() + '%'));
            }
            // 变动类型
            if (selChangesType.Value != "-1")
            {
                sb.Append(" and a.ChangesType=@ChangesType ");
                IList_param.Add(new SqlParam("@ChangesType", selChangesType.Value));
            }
            // 操作者类型
            if (selOperatorType.Value != "-1")
            {
                sb.Append(" and a.OperatorType=@OperatorType ");
                IList_param.Add(new SqlParam("@OperatorType", selOperatorType.Value));
            }
            // 操作渠道
            if (selOperatorWay.Value != "-1")
            {
                sb.Append(" and a.OperatorWay=@OperatorWay ");
                IList_param.Add(new SqlParam("@OperatorWay", selOperatorWay.Value));
            }
            // 状态
            if (selFinancialState.Value != "-1")
            {
                sb.Append(" and a.State=@State ");
                IList_param.Add(new SqlParam("@State", selFinancialState.Value));
            }
            // 变动起始时间
            if (!string.IsNullOrEmpty(txtStartChangesTime.Value))
            {
                sb.Append(" and DATEDIFF(d,@StartChangesTime,ChangesTime)>=0");
                IList_param.Add(new SqlParam("@StartChangesTime", txtStartChangesTime.Value));
            }
            // 变动结束时间
            if (!string.IsNullOrEmpty(txtEndChangesTime.Value))
            {
                sb.Append(" and DATEDIFF(d,ChangesTime,@EndChangesTime)>=0 ");
                IList_param.Add(new SqlParam("@EndChangesTime", txtEndChangesTime.Value));
            }
            // 变动起始余额
            if (!string.IsNullOrEmpty(txtStartMoney.Value))
            {
                sb.Append(" and ChangesCoin>=@StartChangesAmount ");
                IList_param.Add(new SqlParam("@StartChangesAmount", txtStartMoney.Value));
            }
            // 变动结束余额
            if (!string.IsNullOrEmpty(txtEndMoney.Value))
            {
                sb.Append(" and ChangesCoin<=@EndChangesAmount ");
                IList_param.Add(new SqlParam("@EndChangesAmount", txtEndMoney.Value));
            }
            return new KeyValuePair<StringBuilder, IList<SqlParam>>(sb, IList_param);
        }

        /// <summary>
        /// 清空输入框
        /// </summary>
        public void ClearInput()
        {
            txt_Search.Value = "";
            txtEndChangesTime.Value = "";
            txtEndMoney.Value = "";
            txtOperation.Value = "";
            txtOrdersNo.Value = "";
            txtStartChangesTime.Value = "";
            txtStartMoney.Value = "";
            selFinancialState.Value = "-1";
            selOperatorType.Value = "-1";
            selOperatorWay.Value = "-1";
            selChangesType.Value = "-1";
        }

        // 搜索
        protected void lbtSearch_Click(object sender, EventArgs e)
        {
            this.DataBindGrid(true);
        }
        // 重置
        protected void lbtInit_Click(object sender, EventArgs e)
        {
            this.ClearInput();
        }
    }
}