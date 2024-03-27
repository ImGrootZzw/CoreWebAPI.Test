using System;
using System.Collections.Generic;
using System.Text;

namespace CoreWebAPI.Common.Helper
{
    public static class UtilConvert
    {
        #region 基础数据类型转换
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool GetIsEmptyOrNull(this object thisValue)
        {
            return GetCString(thisValue) == "" || GetCString(thisValue) == "undefined" || GetCString(thisValue) == "null";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool GetIsNotEmptyOrNull(this object thisValue)
        {
            return GetCString(thisValue) != "" && GetCString(thisValue) != "undefined" && GetCString(thisValue) != "null";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static int GetCInt(this object thisValue)
        {
            int reval = 0;
            if (thisValue == null) return 0;
            if (thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        public static int GetCInt(this object thisValue, int errorValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out int reval))
            {
                return reval;
            }
            return errorValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static double GetCMoney(this object thisValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), System.Globalization.NumberStyles.Float, null, out double reval))
            {
                return reval;
            }
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        public static double GetCMoney(this object thisValue, double errorValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), System.Globalization.NumberStyles.Float, null, out double reval))
            {
                return reval;
            }
            return errorValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static string GetCString(this object thisValue)
        {
            if (thisValue != null) return thisValue.ToString().Trim();
            return "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        public static string GetCString(this object thisValue, string errorValue)
        {
            if (thisValue != null) return thisValue.ToString().Trim();
            return errorValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static Decimal GetCDecimal(this object thisValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && decimal.TryParse(thisValue.ToString(), System.Globalization.NumberStyles.Float, null, out decimal reval))
            {
                return reval;
            }
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        public static Decimal GetCDecimal(this object thisValue, decimal errorValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && decimal.TryParse(thisValue.ToString(), System.Globalization.NumberStyles.Float, null, out decimal reval))
            {
                return reval;
            }
            return errorValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static double GetCDouble(this object thisValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), System.Globalization.NumberStyles.Float, null, out double reval))
            {
                return reval;
            }
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        public static double GetCDouble(this object thisValue, double errorValue)
        {
            if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), System.Globalization.NumberStyles.Float, null, out double reval))
            {
                return reval;
            }
            return errorValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static DateTime GetCDate(this object thisValue)
        {
            DateTime reval = DateTime.MinValue;
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
            {
                reval = Convert.ToDateTime(thisValue);
            }
            return reval;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="errorValue"></param>
        /// <returns></returns>
        public static DateTime GetCDate(this object thisValue, DateTime errorValue)
        {
            DateTime reval;
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return errorValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool GetCBool(this object thisValue)
        {
            bool reval = false;
            if (thisValue != null && thisValue != DBNull.Value && bool.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        }
        #endregion
    }
}
