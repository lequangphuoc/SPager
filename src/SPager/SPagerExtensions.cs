using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace SPager
{
    public static class SPagerExtensions
    {
        public static MvcHtmlString Pager(this AjaxHelper ajaxHelper, Func<int,string> generateUrl, AjaxOptions ajaxOptions, int totalItems, int currentPage,int pageSize = 10)
        {
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var lastPage = totalPages;
            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");

            //previous
            if (currentPage > 1)
            {
                ul.InnerHtml += AddLink(currentPage - 1, generateUrl, false, "«", ajaxOptions);
            }

            #region page links
            //if total pages less than 6, show all pagination
            if (lastPage < 6)
            {
                 for(int page = 1; page <= lastPage; page++)
                 {
                     ul.InnerHtml += AddLink(page, generateUrl, page == currentPage, page.ToString(),ajaxOptions);
                 }
            }
            else //if total pages more than 6, show with ...
            {
                if (currentPage < 5)//show 1 2 3 4 5 ... 10
                {
                    for(int page = 1; page < 6; page++)
                    {
                            ul.InnerHtml += AddLink(page, generateUrl, page == currentPage, page.ToString(), ajaxOptions);
                    }
                    ul.InnerHtml += "<li><span>...</span></li>";
                    ul.InnerHtml += AddLink(lastPage, generateUrl, false, lastPage.ToString(), ajaxOptions);
                }
                else if(currentPage > lastPage - 4)//show 1 ... 6 7 8 9 10
                {
                    ul.InnerHtml += AddLink(1, generateUrl, false, "1", ajaxOptions);
                    ul.InnerHtml += "<li><span>...</span></li>";
                    for(int page = lastPage - 4; page <= lastPage; page++)
                    {
                            ul.InnerHtml += AddLink(page, generateUrl, page == currentPage, page.ToString(), ajaxOptions);
                    }
                }
                else// show 1 ... 4 5 6 ... 10
                {
                    ul.InnerHtml += AddLink(1, generateUrl, false, "1", ajaxOptions);
                    ul.InnerHtml += "<li><span>...</span></li>";
                    ul.InnerHtml += AddLink(currentPage-1, generateUrl, false, (currentPage-1).ToString(), ajaxOptions);
                    ul.InnerHtml += AddLink(currentPage, generateUrl, true, currentPage.ToString(), ajaxOptions);
                    ul.InnerHtml += AddLink(currentPage+1, generateUrl, false, (currentPage+1).ToString(), ajaxOptions);
                    ul.InnerHtml += "<li><span>...</span></li>";
                    ul.InnerHtml += AddLink(lastPage, generateUrl, false, lastPage.ToString(), ajaxOptions);
                }
            }
            #endregion
            //next
            if (currentPage < lastPage)
            {
                ul.InnerHtml += AddLink(currentPage + 1, generateUrl, false, "»", ajaxOptions);
            }

            

            return MvcHtmlString.Create(ul.ToString());
        }

        private static TagBuilder AddLink(int page, Func<int,string> generateUrl, bool active, string linkText, AjaxOptions ajaxOptions)
        {
            var li = new TagBuilder("li");
            if (active)
                li.MergeAttribute("class", "active");
            var a = new TagBuilder("a");
            a.MergeAttribute("href", active ? "javascript:void()" : generateUrl(page));
            a.SetInnerText(linkText);
            foreach (var option in ajaxOptions.ToUnobtrusiveHtmlAttributes())
            {
                a.Attributes.Add(option.Key, option.Value.ToString());
            }
            li.InnerHtml = a.ToString();
            return li;
        }

       
    }
}
