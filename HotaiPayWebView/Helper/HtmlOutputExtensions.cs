//******************************************************************
//*  author：Umeko
//*  Function：將指定字串轉成 <p> 或 <br> 或 <ul> 或 <ol>  延伸方法
//*  Create Date：2020/07/01
//*  Modify Record：
//*  <author>            <time>            <TaskID>                <desc>
//*  Umeko               2020/8/26         CAIP2-867                處理空字串噴錯問題
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace HotaiPayWebView.Helper
{
    public static class HtmlOutputExtensions
    {
        /// <summary>
        /// @Html.ToParagraphFor(m => m.otherInfo, new { @class = "class1", id = "p1" })
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ToParagraphFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            return ToParagraphFor(htmlHelper, expression, '\n', htmlAttributes);
        }
        /// <summary>
        /// @Html.ToParagraphFor(m => m.otherInfo2,Model.charspit, new { @class = "class1", id = "p1" })
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="spitchar"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ToParagraphFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,char spitchar, object htmlAttributes = null)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string name = ExpressionHelper.GetExpressionText(expression);
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            
            return HtmlFormateControlUtil.GenerateHtml(fullHtmlFieldName, "ptag", htmlHelper.Encode(metadata.Model), spitchar, (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes));
        }
        /// <summary>
        /// @Html.ToParagraphs(Model.otherInfo, new { @class = "class2", id = "p2" })
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="stateValue"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ToParagraphs(this HtmlHelper htmlHelper, string stateValue, object htmlAttributes = null)
        {
            return ToParagraphs(htmlHelper, stateValue, '\n', htmlAttributes);
        }
        /// <summary>
        ///  @Html.ToParagraphs(Model.otherInfo2,Model.charspit, new { @class = "class2", id = "p2" })
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="stateValue"></param>
        /// <param name="spitchar"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ToParagraphs(this HtmlHelper htmlHelper, string stateValue, char spitchar, object htmlAttributes = null)
        {
            return HtmlFormateControlUtil.GenerateHtml("", "ptag", htmlHelper.Encode(stateValue), spitchar, (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes));
        }
        /// <summary>
        /// @Html.ToLineBreak(Model.otherInfo)
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="stateValue"></param>
        /// <param name="spitchar"></param>
        /// <returns></returns>
        public static MvcHtmlString ToLineBreak(this HtmlHelper htmlHelper, string stateValue, char spitchar = '\n')
        {
            object htmlAttributes = null;
            return HtmlFormateControlUtil.GenerateHtml("", "brtag", htmlHelper.Encode(stateValue), spitchar,(IDictionary<string, object>)new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// @Html.ToUnorderedList(Model.otherInfo, new { @class = "class3", id = "ul1" })
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="stateValue"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ToUnorderedList(this HtmlHelper htmlHelper, string stateValue, object htmlAttributes = null)
        {

            return ToUnorderedList(htmlHelper, stateValue, '\n', htmlAttributes);
        }

        /// <summary>
        /// @Html.ToUnorderedList(Model.otherInfo2,Model.charspit, new { @class = "class3", id = "ul1" })
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="stateValue"></param>
        /// <param name="spitchar"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ToUnorderedList(this HtmlHelper htmlHelper, string stateValue, char spitchar , object htmlAttributes = null)
        {

            return HtmlFormateControlUtil.GenerateListHtml("", "ultag", htmlHelper.Encode(stateValue), spitchar, (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes));
        }

        /// <summary>
        /// @Html.ToOrderedList(Model.otherInfo, new { @class = "class3", id = "ol1", liclass = "xxx" })
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="stateValue"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ToOrderedList(this HtmlHelper htmlHelper, string stateValue, object htmlAttributes = null)
        {
            return ToOrderedList(htmlHelper, stateValue, '\n', htmlAttributes);
        }

        /// <summary>
        /// @Html.ToOrderedList(Model.otherInfo2,Model.charspit, new { @class = "class3", id = "ol1", liclass = "xxx" })
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="stateValue"></param>
        /// <param name="spitchar"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ToOrderedList(this HtmlHelper htmlHelper, string stateValue, char spitchar, object htmlAttributes = null)
        { 
            return HtmlFormateControlUtil.GenerateListHtml("", "oltag", htmlHelper.Encode(stateValue), spitchar, (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes));
        }
        
    }

    public static class HtmlFormateControlUtil
    {
        public static MvcHtmlString GenerateHtml(string name, string type, string stateValue, char spitchar, IDictionary<string, object> htmlAttributes)
        {
            var Lines = stateValue.Split(spitchar).Where(a => a.Trim() != string.Empty);

            StringBuilder sb = new StringBuilder();

            if (Lines.Count() > 1)
            {
                int i = 0;
                foreach (var Line in Lines)
                {
                    i++;
                    string id =
                        htmlAttributes.ContainsKey("id") ? $"{htmlAttributes["id"]}_{i:00}" : "";
                    string n_name = string.IsNullOrWhiteSpace(name) ? "" : $"{name}_{i:00}";

                    switch (type)
                    {
                        case "ptag":
                            sb.Append(GenerateParagraphHtml(n_name, id, Line, (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)));
                            break;
                        case "brtag":
                            sb.Append(GenerateLineBreakHtml(Line));
                            break;
                    }
                }
            }
            else
            {
                string id =
                        htmlAttributes.ContainsKey("id") ? htmlAttributes["id"].ToString() : "";

                string lineInner = (Lines.Count() == 1)? Lines.First() : "";

                switch (type)
                {
                    case "ptag":
                        sb.Append(GenerateParagraphHtml(name, id, lineInner, htmlAttributes));
                        break;
                    case "brtag":
                        sb.Append(GenerateLineBreakHtml(lineInner));
                        break;
                }
            }
            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString GenerateListHtml(string name, string type, string stateValue, char spitchar, IDictionary<string, object> htmlAttributes)
        {
            var Lines = stateValue.Split(spitchar).Where(a => a.Trim() != string.Empty);

            TagBuilder tag;
            switch (type)
            {
                case "oltag":
                    tag = new TagBuilder("ol");
                    break;
                case "ultag":
                default:
                    tag = new TagBuilder("ul");
                    break;

            }

            string id = htmlAttributes.ContainsKey("id") ? htmlAttributes["id"].ToString(): "";
            if (!string.IsNullOrWhiteSpace(id))
            {
                tag.GenerateId(id);
                htmlAttributes.Remove("id");
            }
            string nclass =
                        htmlAttributes.ContainsKey("class") ? htmlAttributes["class"].ToString() : "";

            if (!string.IsNullOrWhiteSpace(nclass))
            {
                tag.AddCssClass(nclass);
                htmlAttributes.Remove("class");
            }

            StringBuilder sb = new StringBuilder();
            if (Lines.Count() > 1)
            {
                int i = 0;
                foreach (var Line in Lines)
                {
                    i++;
                    sb.Append(GenerateListItemhHtml("", "", Line, (IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)));
                }
            }
            else
            {
                string lineInner = (Lines.Count() == 1) ? Lines.First() : "";

                sb.Append(GenerateListItemhHtml("", "", lineInner, htmlAttributes));
            }

            tag.InnerHtml += sb.ToString();

            return new MvcHtmlString(tag.ToString());

        }

        private static string GenerateParagraphHtml(string name, string id, string value, IDictionary<string, object> htmlAttributes)
        {
            TagBuilder tag = new TagBuilder("p");
            string nclass =
                        htmlAttributes.ContainsKey("class") ? htmlAttributes["class"].ToString() : "";

            if (!string.IsNullOrWhiteSpace(name))
                tag.MergeAttribute("name", name);

            if (!string.IsNullOrWhiteSpace(id))
            {
                tag.GenerateId(id);
                htmlAttributes.Remove("id");
            }
            if (!string.IsNullOrWhiteSpace(nclass))
            {
                tag.AddCssClass(nclass);
                htmlAttributes.Remove("class");
            }

            tag.MergeAttributes<string, object>(htmlAttributes);
            tag.SetInnerText(value);

            return tag.ToString();
        }

        private static string GenerateLineBreakHtml(string value)
        {
            return value+"<br />";
        }

        private static string GenerateListItemhHtml(string name, string id, string value, IDictionary<string, object> htmlAttributes)
        {
            TagBuilder tag = new TagBuilder("li");
            
            string nclass =
                        htmlAttributes.ContainsKey("liclass") ?htmlAttributes["liclass"].ToString() : "";

            if (!string.IsNullOrWhiteSpace(name))
                tag.MergeAttribute("name", name);

            if (!string.IsNullOrWhiteSpace(id))
            { 
                tag.GenerateId(id);
                htmlAttributes.Remove("id");
            }
            if (!string.IsNullOrWhiteSpace(nclass))
            {
                tag.AddCssClass(nclass);
                htmlAttributes.Remove("liclass");
            }
               
            tag.MergeAttributes<string, object>(htmlAttributes);
            tag.SetInnerText(value);
            
            return tag.ToString();
        }
    }
}