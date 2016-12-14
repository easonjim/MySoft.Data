
#region File Comment
// +-------------------------------------------------------------------------+
// + Copyright (C), xxx Co., Ltd.
// +-------------------------------------------------------------------------+
// + FileName:DebugView.aspx.cs
// +-------------------------------------------------------------------------+
// + Author:wanghao   Version:1.0   Date:2011-03-22
// +-------------------------------------------------------------------------+
// + Description:
// +            Fro DebugView,查看调试信息
// +-------------------------------------------------------------------------+
// + History:
// +         <author>     <time>     <desc>
// +-------------------------------------------------------------------------+
#endregion
                
using System;
using System.Collections.Generic;
using System.Text;

namespace MySoft.Data
{
   public class DebugView
    {
        #region  调用WinAPI'kernel32'输出调试信息到内存
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private extern static void OutputDebugString(string lpszOutputString);

        public static void PrintDebug(string ErrorInfo)
        {
            OutputDebugString(ErrorInfo);
        }
        public static void PrintDebug(string ErrorInfoA, string ErrorInfoB)
        {
            OutputDebugString(ErrorInfoA + ":---" + ErrorInfoB);
        }
        #endregion
    }
}
