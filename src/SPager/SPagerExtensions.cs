using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace SPager
{
    public static class SPagerExtensions
    {
        private static TagBuilder GenerateLink(int page, Func<int,string> generateUrl, bool active, string linkText, AjaxOptions ajaxOptions)
        {
            var li = new TagBuilder("li");
            if (active)
                li.MergeAttribute("class", "active");
            var a = new TagBuilder("a");
            a.MergeAttribute("href", active ? "javascript:void()" : generateUrl(page));
            a.SetInnerText(linkText);
            if(ajaxOptions != null)
                a.MergeAttributes<string, object>(ajaxOptions.ToUnobtrusiveHtmlAttributes());
            li.InnerHtml = a.ToString();
            return li;
        }

        private static string GeneratePager(Func<int, string> generateUrl, AjaxOptions ajaxOptions, int totalItems, int currentPage, int pageSize = 10)
        {
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var lastPage = totalPages;
            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");

            //previous
            if (currentPage > 1)
            {
                ul.InnerHtml += GenerateLink(currentPage - 1, generateUrl, false, "«", ajaxOptions);
            }

            #region page links
            //if total pages less than 6, show all pagination
            if (lastPage < 6)
            {
                for (int page = 1; page <= lastPage; page++)
                {
                    ul.InnerHtml += GenerateLink(page, generateUrl, page == currentPage, page.ToString(), ajaxOptions);
                }
            }
            else //if total pages more than 6, show with ...
            {
                if (currentPage < 5)//show 1 2 3 4 5 ... 10
                {
                    for (int page = 1; page < 6; page++)
                    {
                        ul.InnerHtml += GenerateLink(page, generateUrl, page == currentPage, page.ToString(), ajaxOptions);
                    }
                    ul.InnerHtml += "<li><span>...</span></li>";
                    ul.InnerHtml += GenerateLink(lastPage, generateUrl, false, lastPage.ToString(), ajaxOptions);
                }
                else if (currentPage > lastPage - 4)//show 1 ... 6 7 8 9 10
                {
                    ul.InnerHtml += GenerateLink(1, generateUrl, false, "1", ajaxOptions);
                    ul.InnerHtml += "<li><span>...</span></li>";
                    for (int page = lastPage - 4; page <= lastPage; page++)
                    {
                        ul.InnerHtml += GenerateLink(page, generateUrl, page == currentPage, page.ToString(), ajaxOptions);
                    }
                }
                else// show 1 ... 4 5 6 ... 10
                {
                    ul.InnerHtml += GenerateLink(1, generateUrl, false, "1", ajaxOptions);
                    ul.InnerHtml += "<li><span>...</span></li>";
                    ul.InnerHtml += GenerateLink(currentPage - 1, generateUrl, false, (currentPage - 1).ToString(), ajaxOptions);
                    ul.InnerHtml += GenerateLink(currentPage, generateUrl, true, currentPage.ToString(), ajaxOptions);
                    ul.InnerHtml += GenerateLink(currentPage + 1, generateUrl, false, (currentPage + 1).ToString(), ajaxOptions);
                    ul.InnerHtml += "<li><span>...</span></li>";
                    ul.InnerHtml += GenerateLink(lastPage, generateUrl, false, lastPage.ToString(), ajaxOptions);
                }
            }
            #endregion
            //next
            if (currentPage < lastPage)
            {
                ul.InnerHtml += GenerateLink(currentPage + 1, generateUrl, false, "»", ajaxOptions);
            }
            return ul.ToString(TagRenderMode.Normal);
        }

        public static MvcHtmlString Pager(this HtmlHelper htmlHelper, Func<int, string> generateUrl, int totalItems, int currentPage, int pageSize = 10)
        {
            return MvcHtmlString.Create(GeneratePager(generateUrl, null, totalItems, currentPage, pageSize));
        }

        public static MvcHtmlString Pager(this AjaxHelper ajaxHelper, Func<int, string> generateUrl, AjaxOptions ajaxOptions, int totalItems, int currentPage, int pageSize = 10)
        {
            return MvcHtmlString.Create(GeneratePager(generateUrl, ajaxOptions, totalItems, currentPage, pageSize));
        }
       
    }
}
