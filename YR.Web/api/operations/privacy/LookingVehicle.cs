﻿using Asiasofti.SmartVehicle.Common;
using Asiasofti.SmartVehicle.Common.Enum;
using Asiasofti.SmartVehicle.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using YR.Common.DotNetCode;
using YR.Common.DotNetData;
using YR.Common.DotNetJson;
using YR.Common.DotNetLog;
using YR.Web.api.api_class;
using YR.Web.api.app.pay.alipay;
using YR.Web.api.app.pay.wxpay;

namespace YR.Web.api.operations.privacy
{
    /// <summary>
    /// 寻找车
    /// </summary>
    public class LookingVehicle : IApiAction2
    {
        private string uid = string.Empty;
        private string vid = string.Empty;

        public string Execute(Hashtable params_ht)
        {
            Hashtable res = params_ht;

            if (res["UID"] == null || res["VID"] == null || res["UID"].ToString().Trim().Length <= 0 || res["VID"].ToString().Trim().Length <= 0)
            {
                Hashtable result = new Hashtable();
                result["ErrCode"] = "00";//服务器异常
                return SiteHelper.GetJsonFromHashTable(result, "faild", "参数不完整");
            }
            else
            {
                uid = res["UID"].ToString().Trim();
                vid = res["VID"].ToString().Trim();
                VehicleManager vm = new VehicleManager();
                Hashtable ht = vm.GetVehicleInfoByID(vid);
                if (ht == null)
                {
                    Hashtable result = new Hashtable();
                    result["ErrCode"] = "01";//车辆未找到
                    return SiteHelper.GetJsonFromHashTable(result, "faild", "未找到相关车辆信息");
                }
                else
                {
                    //占用车辆禁止操作
                    /*if (ht["USESTATE"].ToString() != "1")
                    {
                        OrdersManager om = new OrdersManager();
                        Hashtable user_ht = om.GetLatestUserByVehicleID(vid);
                        if (user_ht != null && user_ht.Keys.Count > 0)
                        {
                            string vehicle_uid = SiteHelper.GetHashTableValueByKey(user_ht, "UserID");
                            if (uid.CompareTo(vehicle_uid) != 0)
                            {
                                Hashtable result = new Hashtable();
                                result["ErrCode"] = "02";//车辆已被占用
                                return SiteHelper.GetJsonFromHashTable(result, "faild", "指令发送失败,车辆已被占用");
                            }
                        }
                    }*/

                    bool isSuccess = vm.FindVehicle(vid);
                    if (!isSuccess)
                    {
                        Hashtable result = new Hashtable();
                        result["ErrCode"] = "04";//指令发送失败
                        return SiteHelper.GetJsonFromHashTable(result, "faild", "指令发送失败");
                    }
                    else
                    {
                        return SiteHelper.GetJsonFromHashTable(null, "success", "指令发送成功");
                    }
                }
            }
        }
    }
}