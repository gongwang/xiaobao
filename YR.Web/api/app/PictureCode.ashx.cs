﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using YR.Common.DotNetCache;
using YR.Common.DotNetCode;

namespace YR.Web.api.app
{
    /// <summary>
    /// PictureCode 的摘要说明
    /// </summary>
    public class PictureCode : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string mobile = context.Request["mobile"];
            if (!ValidateUtil.IsMobilePhone(mobile))
            {
                context.Response.Write("手机格式不正确");
                context.Response.End();
            }
            int codeW = 200;
            int codeH = 50;
            int fontSize = 32;
            string chkCode = string.Empty;
            //颜色列表，用于验证码、噪线、噪点 
            Color[] color = { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.Brown, Color.DarkBlue };
            //字体列表，用于验证码 
            string[] font = { "Times New Roman", "Verdana", "Arial", "Gungsuh", "Impact" };
            //验证码的字符集，去掉了一些容易混淆的字符 
            char[] character = { '2', '3', '4', '5', '6', '8', '9', 'a', 'b', 'd', 'e', 'f', 'h', 'k', 'm', 'n', 'r', 'x', 'y', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' };
            Random rnd = new Random();
            //生成验证码字符串 
            for (int i = 0; i < 4; i++)
            {
                chkCode += character[rnd.Next(character.Length)];
            }

            string lowerCode = chkCode.ToLower();
            ICache cache = CacheFactory.GetCache();
            string loginCodeKey = "login_code_" + mobile;
            DateTime dt = DateTime.Now.AddSeconds(120);
            cache.Set(loginCodeKey, lowerCode, dt - DateTime.Now);
            cache.Dispose();

            //创建画布
            Bitmap bmp = new Bitmap(codeW, codeH);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            //画噪线 
            for (int i = 0; i < 1; i++)
            {
                int x1 = rnd.Next(codeW);
                int y1 = rnd.Next(codeH);
                int x2 = rnd.Next(codeW);
                int y2 = rnd.Next(codeH);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawLine(new Pen(clr), x1, y1, x2, y2);
            }
            //画验证码字符串 
            for (int i = 0; i < chkCode.Length; i++)
            {
                string fnt = font[rnd.Next(font.Length)];
                Font ft = new Font(fnt, fontSize);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawString(chkCode[i].ToString(), ft, new SolidBrush(clr), (float)i *50 + 2, (float)0);
            }
            //画噪点 
            for (int i = 0; i < 200; i++)
            {
                int x = rnd.Next(bmp.Width);
                int y = rnd.Next(bmp.Height);
                Color clr = color[rnd.Next(color.Length)];
                bmp.SetPixel(x, y, clr);
            }
            //清除该页输出缓存，设置该页无缓存 
            context.Response.Buffer = true;
            context.Response.ExpiresAbsolute = System.DateTime.Now.AddMilliseconds(0);
            context.Response.Expires = 0;
            context.Response.CacheControl = "no-cache";
            context.Response.AppendHeader("Pragma", "No-Cache");
            //将验证码图片写入内存流，并将其以 "image/Png" 格式输出 
            MemoryStream ms = new MemoryStream();
            try
            {
                bmp.Save(ms, ImageFormat.Png);
                context.Response.ClearContent();
                context.Response.ContentType = "image/Png";
                context.Response.BinaryWrite(ms.ToArray());
            }
            finally
            {

                //显式释放资源 
                if (cache != null)
                {
                    cache.Dispose();
                }
                //cache.Dispose();
                bmp.Dispose();
                g.Dispose();
            }
            if (cache != null)
            {
                cache.Dispose();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}