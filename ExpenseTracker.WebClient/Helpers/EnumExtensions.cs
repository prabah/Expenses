using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using ExpenseTracker.WebClient.Controllers;

namespace ExpenseTracker.WebClient.Helpers
{
    public static class EnumExtensions
    {
        public static string ToDescriptionString(this ClientsController.ImageFileType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}