using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace QuickCode.Demo.Portal.Helpers
{
    public static class HtmlExtensions
    {
        public static IHtmlContent Image(this IHtmlHelper html, byte[] image)
        {
            if (image != null)
            {
                var img = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(image));
                return new HtmlString("<img src='" + img + "' height=\"80px\" />");
            }
            else
            {
                return new HtmlString("No Image");
            }
        }

        public static string ImageData(this IHtmlHelper html, byte[] image)
        {
            if (image != null)
            {
                var img = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(image));
                return img;
            }
            else
            {
                return "No Image";
            }
        }


            /// <summary>
            /// Produces the markup for an image element that displays a QR Code image, as provided by Google's chart API.
            /// </summary>
            /// <param name="htmlHelper"></param>
            /// <param name="data">The data to be encoded, as a string.</param>
            /// <param name="size">The square length of the resulting image, in pixels.</param>
            /// <param name="margin">The width of the border that surrounds the image, measured in rows (not pixels).</param>
            /// <param name="errorCorrectionLevel">The amount of error correction to build into the image.  Higher error correction comes at the expense of reduced space for data.</param>
            /// <param name="htmlAttributes">Optional HTML attributes to include on the image element.</param>
            /// <returns></returns>
        public static IHtmlContent QRCode(this IHtmlHelper htmlHelper, string data, int size = 80, int margin = 4, QRCodeErrorCorrectionLevel errorCorrectionLevel = QRCodeErrorCorrectionLevel.Low, object htmlAttributes = null)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (size < 1)
                throw new ArgumentOutOfRangeException("size", size, "Must be greater than zero.");
            if (margin < 0)
                throw new ArgumentOutOfRangeException("margin", margin, "Must be greater than or equal to zero.");
            if (!Enum.IsDefined(typeof(QRCodeErrorCorrectionLevel), errorCorrectionLevel))
                throw new InvalidEnumArgumentException("errorCorrectionLevel", (int)errorCorrectionLevel, typeof(QRCodeErrorCorrectionLevel));

            var url = string.Format("http://chart.apis.google.com/chart?cht=qr&chld={2}|{3}&chs={0}x{0}&chl={1}", size, WebUtility.UrlEncode(data), errorCorrectionLevel.ToString()[0], margin);

            var tag = new TagBuilder("img");
            if (htmlAttributes != null)
                tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            tag.Attributes.Add("src", url);
            tag.Attributes.Add("width", size.ToString());
            tag.Attributes.Add("height", size.ToString());

            return new HtmlString(tag.ToString());
        }
 

        public enum QRCodeErrorCorrectionLevel
        {
            /// <summary>Recovers from up to 7% erroneous data.</summary>
            Low,
            /// <summary>Recovers from up to 15% erroneous data.</summary>
            Medium,
            /// <summary>Recovers from up to 25% erroneous data.</summary>
            QuiteGood,
            /// <summary>Recovers from up to 30% erroneous data.</summary>
            High
        }

        public static IHtmlContent MenuLink(this IHtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string activeClass = "select", bool checkAction = true, string rootAction = null)
        {
            var currentController = htmlHelper.ViewContext.RouteData.Values["Controller"].ToString();
            var currentAction = htmlHelper.ViewContext.RouteData.Values["Action"].ToString();

            if (string.Compare(controllerName, currentController, StringComparison.OrdinalIgnoreCase) == 0
                && (((!checkAction) || string.Compare(actionName, currentAction, StringComparison.OrdinalIgnoreCase) == 0) || ( string.Compare(actionName, rootAction, StringComparison.OrdinalIgnoreCase) == 0)))
            {
                if (rootAction != null)
                {
                    return htmlHelper.ActionLink(linkText, rootAction, controllerName, null, new { @class = activeClass });
                }
                else
                {
                    return htmlHelper.ActionLink(linkText, actionName, controllerName, null, new { @class = activeClass });
                }
            }

            return htmlHelper.ActionLink(linkText, actionName, controllerName);

        }

        public static string FormatAuthType(this IHtmlHelper htmlHelper, string value)
        {
            string returnValue = string.Empty;
            switch (value)
            {
                case "Detail":
                    returnValue = "Detay";
                    break;
                case "List":
                    returnValue = "Liste";
                    break;
                case "Delete":
                    returnValue = "Sil";
                    break;
                case "Update":
                    returnValue = "G�ncelle";
                    break;
                case "Insert":
                    returnValue = "Ekle";
                    break;
            }
            return returnValue;
        }

        public static string FormatCurrency(this IHtmlHelper htmlHelper,String currency)
        {
            string formattedCurrency = currency;
            switch (currency)
            {
                case "TRY":
                    formattedCurrency = "TL";
                    break;                
            }
            return formattedCurrency;
        }  

        //public static  MvcHtmlString Calendar(this HtmlHelper helper, DateTime dateToShow)
        //{
        //    DateTimeFormatInfo cinfo = DateTimeFormatInfo.CurrentInfo;
        //    StringBuilder sb = new StringBuilder();
        //    DateTime date = new DateTime(dateToShow.Year, dateToShow.Month, 1);
        //    int emptyCells = ((int)date.DayOfWeek + 7 - (int)cinfo.FirstDayOfWeek) % 7;
        //    int days = DateTime.DaysInMonth(dateToShow.Year, dateToShow.Month);
        //     sb.Append("<table class='cal'><tr><th colspan='7'>" + cinfo.MonthNames[date.Month - 1] + " " + dateToShow.Year + "</th></tr>");
        //    for (int i = 0; i < ((days + emptyCells) > 35 ? 42 : 35); i++)
        //    {
        //        if (i % 7 == 0)
        //        {
        //            if (i > 0)  sb.Append("</tr>");
        //             sb.Append("<tr>");
        //        }

        //        if (i < emptyCells || i >= emptyCells + days)
        //        {
        //             sb.Append("<td class='cal-empty'>&nbsp;</td>");
        //        }
        //        else
        //        {
        //             sb.Append("<td class='cal-day'>" + date.Day + "</td>");
        //            date = date.AddDays(1);
        //        }
        //    }
        //     sb.Append("</tr></table>");
        //    return new MvcHtmlString( sb.ToString());
        //    //return helper.Raw( sb.ToString());
        //}
    }
}