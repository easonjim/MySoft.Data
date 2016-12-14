using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MySoft.Data
{
    /// <summary>
    /// 分页样式
    /// </summary>
    public enum HtmlPagerStyle
    {
        /// <summary>
        /// Custom Style
        /// </summary>
        [EnumDescription("custom")]
        Custom,
        /// <summary>
        /// Digg Style
        /// </summary>
        [EnumDescription("digg")]
        Digg,
        /// <summary>
        /// Yahoo Style
        /// </summary>
        [EnumDescription("yahoo")]
        Yahoo,
        /// <summary>
        /// Meneame Style
        /// </summary>
        [EnumDescription("meneame")]
        Meneame,
        /// <summary>
        /// Flickr Style
        /// </summary>
        [EnumDescription("flickr")]
        Flickr,
        /// <summary>
        /// Sabrosus Style
        /// </summary>
        [EnumDescription("sabrosus")]
        Sabrosus,
        /// <summary>
        /// Pagination Style
        /// </summary>
        [EnumDescription("pagination")]
        Pagination,
        /// <summary>
        /// Scott Style
        /// </summary>
        [EnumDescription("scott")]
        Scott,
        /// <summary>
        /// Quotes Style
        /// </summary>
        [EnumDescription("quotes")]
        Quotes,
        /// <summary>
        /// Black Style
        /// </summary>
        [EnumDescription("black")]
        Black,
        /// <summary>
        /// Black2 Style
        /// </summary>
        [EnumDescription("black2")]
        Black2,
        /// <summary>
        /// BlackRed Style
        /// </summary>
        [EnumDescription("black-red")]
        BlackRed,
        /// <summary>
        /// Grayr Style
        /// </summary>
        [EnumDescription("grayr")]
        Grayr,
        /// <summary>
        /// Yellow Style
        /// </summary>
        [EnumDescription("yellow")]
        Yellow,
        /// <summary>
        /// Jogger Style
        /// </summary>
        [EnumDescription("jogger")]
        Jogger,
        /// <summary>
        /// Starcraft2 Style
        /// </summary>
        [EnumDescription("starcraft2")]
        Starcraft2,
        /// <summary>
        /// Tres Style
        /// </summary>
        [EnumDescription("tres")]
        Tres,
        /// <summary>
        /// Megas512 Style
        /// </summary>
        [EnumDescription("megas512")]
        Megas512,
        /// <summary>
        /// Technorati Style
        /// </summary>
        [EnumDescription("technorati")]
        Technorati,
        /// <summary>
        /// Youtube Style
        /// </summary>
        [EnumDescription("youtube")]
        Youtube,
        /// <summary>
        /// Msdn Style
        /// </summary>
        [EnumDescription("msdn")]
        Msdn,
        /// <summary>
        /// Badoo Style
        /// </summary>
        [EnumDescription("badoo")]
        Badoo,
        /// <summary>
        /// Manu Style
        /// </summary>
        [EnumDescription("manu")]
        Manu,
        /// <summary>
        /// GreenBlack Style
        /// </summary>
        [EnumDescription("green-black")]
        GreenBlack,
        /// <summary>
        /// Viciao Style
        /// </summary>
        [EnumDescription("viciao")]
        Viciao,
        /// <summary>
        /// Yahoo2 Style
        /// </summary>
        [EnumDescription("yahoo2")]
        Yahoo2
    }

    /// <summary>
    /// 创建分页的html
    /// </summary>
    public class HtmlPager
    {
        /// <summary>
        /// 定义一个IDataPage属性
        /// </summary>
        private DataPage dataPage;
        /// <summary>
        /// 翻页时连接字符串
        /// </summary>
        private string linkFormat;
        /// <summary>
        /// 翻页时连接字符串
        /// </summary>
        public string LinkFormat
        {
            get { return linkFormat; }
            set { linkFormat = value; }
        }

        /// <summary>
        /// 每屏连接条数
        /// </summary>
        private int linkSize;
        /// <summary>
        /// 每屏连接条数
        /// </summary>
        public int LinkSize
        {
            get { return linkSize; }
            set { linkSize = value; }
        }

        private HtmlPagerStyle style = HtmlPagerStyle.Custom;
        /// <summary>
        /// 页面样式
        /// </summary>
        public HtmlPagerStyle Style
        {
            get { return style; }
            set { style = value; }
        }

        private string pageID = "$Page";
        /// <summary>
        /// 页面参数ID
        /// </summary>
        public string PageID
        {
            get { return pageID; }
            set { pageID = value; }
        }

        private string prevPageTitle = "上一页";
        /// <summary>
        /// 上一页标题
        /// </summary>
        public string PrevPageTitle
        {
            set { prevPageTitle = value; }
        }

        private string nextPageTitle = "下一页";
        /// <summary>
        /// 下一页标题
        /// </summary>
        public string NextPageTitle
        {
            set { nextPageTitle = value; }
        }

        private bool showBracket = true;
        /// <summary>
        /// 是否显示链接中的括号
        /// </summary>
        public bool ShowBracket
        {
            set { showBracket = value; }
        }

        private bool showGotoPage = true;
        /// <summary>
        /// 显示转到某页
        /// </summary>
        public bool ShowGotoPage
        {
            set { showGotoPage = value; }
        }

        private bool showRecord = true;
        /// <summary>
        /// 显示记录信息
        /// </summary>
        public bool ShowRecord
        {
            set { showRecord = value; }
        }

        private string pagerCss;
        /// <summary>
        /// 分页样式
        /// </summary>
        public string PagerCss
        {
            set { pagerCss = value; }
        }

        /// <summary>
        /// 获取或设置DataPage
        /// </summary>
        public DataPage DataPage
        {
            get { return dataPage; }
            set { dataPage = value; }
        }

        /// <summary>
        /// 初始化HtmlPager对象
        /// </summary>
        /// <param name="dataPage"></param>
        public HtmlPager(DataPage dataPage)
        {
            this.dataPage = dataPage;
            this.linkSize = 10;
            this.style = HtmlPagerStyle.Custom;
        }

        /// <summary>
        /// 初始化HtmlPager对象
        /// </summary>
        /// <param name="page">对应的数据源</param>
        /// <param name="linkFormat">对应的翻页格式{0}必须设置，对应当前页</param>
        public HtmlPager(DataPage dataPage, string linkFormat)
            : this(dataPage)
        {
            this.linkFormat = linkFormat;
        }

        /// <summary>
        /// 初始化HtmlPager对象
        /// </summary>
        /// <param name="page">对应的数据源</param>
        /// <param name="linkFormat">对应的翻页格式{0}必须设置，对应当前页</param>
        /// <param name="linkSize">每屏连接条数</param>
        public HtmlPager(DataPage dataPage, string linkFormat, int linkSize)
            : this(dataPage, linkFormat)
        {
            this.linkSize = linkSize;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (dataPage.CurrentPageIndex <= 0) dataPage.CurrentPageIndex = 1;
            if (dataPage.RowCount < 0) dataPage.RowCount = 0;

            int halfSize = Convert.ToInt32(Math.Floor(linkSize / 2.0));
            if (linkSize % 2 == 0) halfSize--;

            string html = string.Empty;

            //生成分页的html
            if (style == HtmlPagerStyle.Custom)
            {
                sb.Append("<div id='htmlPager' class=\"" + (pagerCss ?? EnumDescriptionAttribute.GetDescription(style)) + "\">\n");
            }
            else
            {
                sb.Append("<div id='htmlPager' class=\"" + EnumDescriptionAttribute.GetDescription(style) + "\">\n");
            }

            if (dataPage.PageCount == 0)
            {
                sb.Append("<span class=\"disabled\">上一页</span>\n");
                sb.Append("<span class=\"current\">1</span>\n");
                sb.Append("<span class=\"disabled\">下一页</span>\n");
            }
            else
            {
                if (!dataPage.IsFirstPage)
                {
                    sb.Append("<a href=\"" + GetHtmlLink(dataPage.CurrentPageIndex - 1) + "\" title=\"上一页\">" + prevPageTitle + "</a>\n");
                }
                else
                {
                    sb.Append("<span class=\"disabled\">" + prevPageTitle + "</span>\n");
                }

                int startPage = dataPage.CurrentPageIndex;
                if (startPage <= halfSize || dataPage.PageCount <= linkSize) startPage = halfSize + 1;
                else if (startPage + halfSize >= dataPage.PageCount)
                {
                    startPage = dataPage.PageCount - halfSize;
                    if (linkSize % 2 == 0) startPage--;
                }

                int beginIndex = startPage - halfSize;
                int endIndex = startPage + halfSize;

                if (linkSize % 2 == 0) endIndex++;

                if (beginIndex - 1 > 0)
                {
                    if (beginIndex - 1 == 1)
                    {
                        sb.Append("<a href=\"" + GetHtmlLink(1) + "\" title=\"第1页\">[1]</a>\n");
                    }
                    else
                    {
                        sb.Append("<a href=\"" + GetHtmlLink(1) + "\" title=\"第1页\">[1]</a></span>...&nbsp;\n");
                    }
                }

                for (int index = beginIndex; index <= endIndex; index++)
                {
                    if (index > dataPage.PageCount) break;
                    if (index == dataPage.CurrentPageIndex)
                    {
                        sb.Append("<span class=\"current\">");
                        sb.Append(index);
                        sb.Append("</span>\n");
                    }
                    else
                    {
                        sb.Append("<a href=\"" + GetHtmlLink(index) + "\" title=\"第" + index + "页\">[" + index + "]</a>\n");
                    }
                }

                if (endIndex + 1 <= dataPage.PageCount)
                {
                    if (endIndex + 1 == dataPage.PageCount)
                    {
                        sb.Append("<a href=\"" + GetHtmlLink(endIndex + 1) + "\" title=\"第" + (endIndex + 1) + "页\">[" + (endIndex + 1) + "]</a>\n");
                    }
                    else
                    {
                        sb.Append("...&nbsp;<a href=\"" + GetHtmlLink(dataPage.PageCount) + "\" title=\"第" + dataPage.PageCount + "页\">[" + dataPage.PageCount + "]</a>\n");
                    }
                }

                if (!dataPage.IsLastPage)
                {
                    sb.Append("<a href=\"" + GetHtmlLink(dataPage.CurrentPageIndex + 1) + "\" title=\"下一页\">" + nextPageTitle + "</a>\n");
                }
                else
                {
                    sb.Append("<span class=\"disabled\">" + nextPageTitle + "</span>\n");
                }
            }

            if (showGotoPage)
            {
                sb.Append("&nbsp;/&nbsp;第&nbsp;<select id=\"pageSelect\" onchange=\"" + GetHtmlLink("this.value") + "\">\n");
                if (dataPage.PageCount == 0)
                {
                    sb.Append("<option value=\"1\" selected=\"selected\">1</option>\n");
                }
                else
                {
                    for (int index = 1; index <= dataPage.PageCount; index++)
                    {
                        if (index == dataPage.CurrentPageIndex)
                        {
                            sb.Append("<option value=\"" + index + "\" selected=\"selected\">");
                            sb.Append(index);
                            sb.Append("</option>\n");
                        }
                        else
                        {
                            sb.Append("<option value=\"" + index + "\">");
                            sb.Append(index);
                            sb.Append("</option>\n");
                        }
                    }
                }
                sb.Append("</select>&nbsp;页&nbsp;\n");
            }

            if (showRecord)
            {
                sb.Append("&nbsp;共<span class=\"red\">" + dataPage.RowCount + "</span>条&nbsp;/&nbsp;每页<span class=\"red\">" + dataPage.PageSize + "</span>条\n");
            }

            sb.Append("<input type=\"hidden\" id=\"currentPage\" value=\"" + dataPage.CurrentPageIndex + "\"/>\n");
            sb.Append("</div>\n");

            html = sb.ToString();
            if (!showBracket)
            {
                Regex reg = new Regex(@"\[([\d]+)\]");
                html = reg.Replace(html, "$1");
            }

            return html;
        }

        private string GetHtmlLink(int value)
        {
            return linkFormat.Replace(pageID, value.ToString());
        }

        private string GetHtmlLink(string value)
        {
            if (linkFormat.Contains("javascript:"))
            {
                string format = linkFormat.Replace("javascript:", string.Format("javascript:var selectValue={0};", value));
                value = "selectValue";
                return format.Replace(pageID, value);
            }
            else
            {
                return string.Concat("javascript:location.href='", linkFormat.Replace(pageID, ("'+" + value + "+'")), "';");
            }
        }
    }
}
