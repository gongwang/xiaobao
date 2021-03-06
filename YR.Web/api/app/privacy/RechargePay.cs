﻿using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using YR.Common.DotNetCache;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;
using YR.Common.DotNetJson;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.app.privacy
{
    /// <summary>
    /// 充值支付
    /// </summary>
    public class RechargePay : IApiAction2
    {
        //用户id
        private string uid = string.Empty;

        //支付方式,1:支付宝,2:微信支付
        private int payway = 0;

        //交易金额
        private decimal money = 0.00m;

        private Hashtable res = null;

        private string weixinNotifyUrl;

        private string alipayNotifyUrl;

        private string webRoot;

        private static Log Logger = LogFactory.GetLogger(typeof(RechargePay));

        private string cacheKey = "RechargePay_";

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["UID"].ToString().Trim().Length <= 0 ||
                res["PayWay"] == null || res["PayWay"].ToString().Trim().Length <= 0 ||
                res["Money"] == null || res["Money"].ToString().Trim().Length <= 0)
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                int.TryParse(res["PayWay"].ToString().Trim(), out payway);
                cacheKey += uid;
                decimal.TryParse(res["Money"].ToString().Trim(), out money);

                //在充值时进行身份验证
                UserFinancialManager ufm = new UserFinancialManager();
                UserInfoManager uim = new UserInfoManager();
                Hashtable user = uim.GetUserInfoByUserID(uid);
                if (user == null || user.Keys.Count <= 0)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "您不是有效会员");
                }
                else
                {
                    //验证用户是否经过身份认证
                    if ("4".CompareTo(user["REALNAMECERTIFICATION"].ToString()) != 0)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "您当前未完成实名认证，请完成实名认证后重试");
                    }
                    //验证用户状态是否有效
                    if ("0".CompareTo(user["USERSTATE"].ToString()) == 0)
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "faild", "您当前已被禁用无法充值");
                    }
                }
                //----------------------------------------------------------------------------------

                #region 检测前一笔未确认支付交易是否成功，成功则调用支付成功业务逻辑

                Hashtable financial_ht = ufm.GetLatestUserFinancialInfo(uid, UserFinancialState.NewSubmit, UserFinancialChangesType.Recharge);
                if (financial_ht != null && financial_ht.Keys.Count > 0)
                {
                    string order_payid = SiteHelper.GetHashTableValueByKey(financial_ht, "OrderPayID");
                    string operator_way = SiteHelper.GetHashTableValueByKey(financial_ht, "OperatorWay");
                    if (operator_way == UserFinancialOperatorWay.WeixinPay.GetHashCode().ToString())
                    {
                        WxOrderQuery orderQuery = new WxOrderQuery();
                        OrderQueryResult queryResult = orderQuery.Query(order_payid);
                        if (queryResult.trade_state == TradeStateEnum.SUCCESS)
                        {
                            Hashtable hashuf = new Hashtable();
                            hashuf["ID"] = SiteHelper.GetHashTableValueByKey(financial_ht, "ID");
                            hashuf["UserID"] = SiteHelper.GetHashTableValueByKey(financial_ht, "UserID");
                            hashuf["State"] = UserFinancialState.Effect.GetHashCode();
                            hashuf["TradeNo"] = queryResult.transaction_id;
                            hashuf["TotalFee"] = queryResult.total_fee;
                            hashuf["PayWay"] = UserFinancialOperatorWay.WeixinPay;
                            decimal changesAmount = 0.00m;
                            decimal.TryParse(SiteHelper.GetHashTableValueByKey(financial_ht, "ChangesAmount"), out changesAmount);
                            bool isSuccess = false;
                            if (Math.Abs(changesAmount) == queryResult.total_fee)
                                isSuccess = uim.RechargeCallBack(hashuf);
                        }
                    }
                    else if (operator_way == UserFinancialOperatorWay.Alipay.GetHashCode().ToString())
                    {
                        AlipayOrderQuery orderQuery = new AlipayOrderQuery();
                        OrderQueryResult queryResult = orderQuery.Query(order_payid);
                        if (queryResult.trade_state == TradeStateEnum.SUCCESS)
                        {
                            Hashtable hashuf = new Hashtable();
                            hashuf["ID"] = SiteHelper.GetHashTableValueByKey(financial_ht, "ID");
                            hashuf["UserID"] = SiteHelper.GetHashTableValueByKey(financial_ht, "UserID");
                            hashuf["State"] = UserFinancialState.Effect.GetHashCode();
                            hashuf["TradeNo"] = queryResult.transaction_id;
                            hashuf["TotalFee"] = queryResult.total_fee;
                            hashuf["PayWay"] = UserFinancialOperatorWay.Alipay;
                            decimal changesAmount = 0.00m;
                            decimal.TryParse(SiteHelper.GetHashTableValueByKey(financial_ht, "ChangesAmount"), out changesAmount);
                            bool isSuccess = false;
                            if (Math.Abs(changesAmount) == queryResult.total_fee)
                                isSuccess = uim.RechargeCallBack(hashuf);
                        }
                    }
                }

                #endregion

                webRoot = SiteHelper.GetWebRoot();
                weixinNotifyUrl = string.Format("{0}{1}", webRoot, "/api/app/pay/wxpay/recharge_notify_url.aspx");
                alipayNotifyUrl = string.Format("{0}{1}", webRoot, "/api/app/pay/alipay/recharge_notify_url.aspx");

                string result = "";
                ICache cache = null;
                switch (payway)
                {
                    case 1:
                        cache = CacheFactory.GetCache();
                        if (!string.IsNullOrEmpty(cache.Get<string>(cacheKey)))
                        {
                            cache.Dispose();
                            Logger.Error("充值支付宝支付5秒内，" + uid);
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "5秒内请勿重复点击");
                        }
                        cache.Dispose();
                        result = AliPay(uid, money);
                        break;
                    case 2:
                        cache = CacheFactory.GetCache();
                        if (!string.IsNullOrEmpty(cache.Get<string>(cacheKey)))
                        {
                            cache.Dispose();
                            Logger.Error("充值微信支付5秒内，" + uid);
                            return SiteHelper.GetJsonFromHashTable(null, "faild", "5秒内请勿重复点击");
                        }
                        cache.Dispose();
                        result = WxPay(uid, money);
                        break;
                    default:
                        break;
                }
                return result;
            }
        }

        //支付宝
        private string AliPay(string uid, decimal money)
        {
            UserFinancialManager ufm = new UserFinancialManager();
            Hashtable hashuf = new Hashtable();
            hashuf["ID"] = CommonHelper.GetGuid;
            hashuf["UserID"] = uid;
            hashuf["OrderNum"] = SiteHelper.GenerateOrderNum();
            hashuf["OrderPayID"] = SiteHelper.GeneratePayID();
            hashuf["ChangesAmount"] = money;
            hashuf["ChangesTime"] = SiteHelper.GetWebServerCurrentTime();
            hashuf["ChangesType"] = (int)UserFinancialChangesType.Recharge;
            hashuf["Remark"] = "用户充值";
            hashuf["Operator"] = uid;
            hashuf["OperatorType"] = (int)UserFinancialOperatorType.User;
            hashuf["OperatorWay"] = (int)UserFinancialOperatorWay.Alipay;
            hashuf["State"] = (int)UserFinancialState.NewSubmit;
            bool isSuccess = ufm.AddOrEditUserFinancialInfo(hashuf, null);
            if (isSuccess)
            {
                Hashtable uf = ufm.GetUserFinancialPayInfoByPayID(hashuf["OrderPayID"].ToString());
                if (uf == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "生成支付订单失败");
                }
                else
                {
                    Hashtable result = new Hashtable();
                    result["PayWay"] = payway;
                    result["orderInfo"] = YR.Web.api.app.pay.alipay.Core.GetOrderInfo(hashuf["OrderPayID"].ToString(), "余额充值", string.Format("用户充值{0:N2}元", money), alipayNotifyUrl, money);
                    Logger.Info("RechargePay,AliPay orderInfo:" + result["orderInfo"]);

                    ICache cache = CacheFactory.GetCache();
                    DateTime dt = DateTime.Now.AddSeconds(5);
                    cache.Set(cacheKey, uid, dt - DateTime.Now);
                    cache.Dispose();

                    return SiteHelper.GetJsonFromHashTable(result, "success", "生成支付订单成功", "RechargePay");
                }
            }
            else
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "生成支付订单失败");
            }
        }

        //微信
        private string WxPay(string uid, decimal money)
        {
            UserFinancialManager ufm = new UserFinancialManager();
            Hashtable hashuf = new Hashtable();
            hashuf["ID"] = CommonHelper.GetGuid;
            hashuf["UserID"] = uid;
            hashuf["OrderNum"] = SiteHelper.GenerateOrderNum();
            hashuf["OrderPayID"] = SiteHelper.GeneratePayID();
            hashuf["ChangesAmount"] = money;
            hashuf["ChangesTime"] = SiteHelper.GetWebServerCurrentTime();
            hashuf["ChangesType"] = (int)UserFinancialChangesType.Recharge;
            hashuf["Remark"] = "用户充值";
            hashuf["Operator"] = uid;
            hashuf["OperatorType"] = (int)UserFinancialOperatorType.User;
            hashuf["OperatorWay"] = (int)UserFinancialOperatorWay.WeixinPay;
            hashuf["State"] = (int)UserFinancialState.NewSubmit;
            bool isSuccess = ufm.AddOrEditUserFinancialInfo(hashuf, null);
            if (isSuccess)
            {
                Hashtable uf = ufm.GetUserFinancialPayInfoByPayID(hashuf["OrderPayID"].ToString());
                if (uf == null)
                {
                    return SiteHelper.GetJsonFromHashTable(null, "faild", "生成支付订单失败");
                }
                else
                {
                    WxUtil wxUtil = new WxUtil();
                    string prepayid = "", sign = "";
                    prepayid = wxUtil.GetPrepayId(hashuf["OrderPayID"].ToString(), "余额充值", money, weixinNotifyUrl,HttpContext.Current.Request.UserHostAddress);
                    Hashtable result = new Hashtable();
                    result["PayWay"] = payway;
                    result["appId"] = pay.wxpay.WxConfig.AppId;
                    result["partnerId"] = pay.wxpay.WxConfig.MchId;
                    result["prepayId"] = prepayid;
                    result["packageValue"] = "Sign=WXPay";
                    result["nonceStr"] = wxUtil.genNonceStr();
                    result["timeStamp"] = wxUtil.genTimeStamp();

                    SortedDictionary<string, string> dic = new SortedDictionary<string, string>();
                    dic.Add("appid", result["appId"].ToString());
                    dic.Add("noncestr", result["nonceStr"].ToString());
                    dic.Add("package", result["packageValue"].ToString());
                    dic.Add("partnerid", result["partnerId"].ToString());
                    dic.Add("prepayid", result["prepayId"].ToString());
                    dic.Add("timestamp", result["timeStamp"].ToString());
                    sign = wxUtil.getSign(dic);

                    result["sign"] = sign;
                    Logger.Info("RechargePay,WxPay prepayId:" + result["prepayId"]);

                    ICache cache = CacheFactory.GetCache();
                    DateTime dt = DateTime.Now.AddSeconds(5);
                    cache.Set(cacheKey, uid, dt - DateTime.Now);
                    cache.Dispose();

                    return SiteHelper.GetJsonFromHashTable(result, "success", "生成支付订单成功", "RechargePay");
                }
            }
            else
            {
                return SiteHelper.GetJsonFromHashTable(null, "faild", "生成支付订单失败");
            }
        }
    }
}