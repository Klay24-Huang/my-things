using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace OtherService.WebAction
{
    public class RedirectAndPost : ActionResult
    {
        private readonly ServerPage _serverPage;

        private HtmlGenericControl _currentForm;
        private HtmlGenericControl CurrentForm
        {
            get
            {
                if (_currentForm is null)
                {
                    _currentForm = new HtmlGenericControl("form");
                }
                return _currentForm;
            }
            set
            {
                this._currentForm = value;
            }
        }

        public RedirectAndPost(ServerPage serverPage)
        {
            _serverPage = serverPage;
            SetForm();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            List<string> list = new List<string>();
            list.AddRange(_serverPage.ValidateSendText());

            if (list.Count > 0)
            {
                throw new ArgumentNullException(string.Join(";",list));
            }

            HttpResponseBase response = context.HttpContext.Response;
            string s = BuildPostForm();
            context.HttpContext.Response.Write(s);
        }

        private string BuildPostForm()
        {
            if(CurrentForm is null || CurrentForm.InnerHtml is null)
            {
                SetForm();
            }

            var page = new Page();
            Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(_serverPage.SendText);

            RenderControlAndParamenter(dictionary);
            HtmlGenericControl scriptSrc = new HtmlGenericControl("script");
            scriptSrc.Attributes.Add("type", "text/javascript");
            scriptSrc.Attributes.Add("src", page.ClientScript.GetWebResourceUrl(this.GetType(), "OtherService.WebAction.Res.jquery-3.4.1.min.js"));

            HtmlGenericControl scriptSubmit = new HtmlGenericControl("script");
            scriptSubmit.Attributes.Add("type", "text/javascript");
            scriptSubmit.InnerHtml = string.Empty;
            scriptSubmit.InnerHtml += "\r\n//<![CDATA[";
            scriptSubmit.InnerHtml += "\r\n    $(document).ready(function () {";
            scriptSubmit.InnerHtml += $"\r\n        $(\"#{_serverPage.FormId}\").each(function (i) ";
            scriptSubmit.InnerHtml += "\r\n         { ";
            scriptSubmit.InnerHtml += "\r\n            this.name = this.id;";
            scriptSubmit.InnerHtml += "\r\n        });";
            scriptSubmit.InnerHtml += "\r\n";
            scriptSubmit.InnerHtml += "\r\n        $(\"input[type=hidden]:not(.__parameter)\").each(function (i) {";
            scriptSubmit.InnerHtml += "\r\n            $(this).remove();";
            scriptSubmit.InnerHtml += "\r\n        });";
            scriptSubmit.InnerHtml += "\r\n";
            scriptSubmit.InnerHtml += $"\r\n        $(\"#{_serverPage.FormId}\").submit();";
            scriptSubmit.InnerHtml += "\r\n    });";
            scriptSubmit.InnerHtml += "\r\n//]]>";
            scriptSubmit.InnerHtml += "\r\n";

            var sb = new System.Text.StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var xhtmlTextWriter = new XhtmlTextWriter(sw))
                {
                    scriptSrc.RenderControl(xhtmlTextWriter);
                    scriptSubmit.RenderControl(xhtmlTextWriter);
                    CurrentForm.RenderControl(xhtmlTextWriter);
                }
            }
            return sb.ToString();
        }

        private void SetForm()
        {
            List<string> list = new List<string>();
            list.AddRange(PropertyValidator.Validate(_serverPage));
            if(list.Count == 0)
            { 
                CurrentForm = CurrentForm ?? new HtmlGenericControl("form");
                CurrentForm.Attributes.Add("id", _serverPage.FormId);
                CurrentForm.Attributes.Add("name", _serverPage.FormId);
                CurrentForm.Attributes.Add("action", _serverPage.Url);
                CurrentForm.Attributes.Add("method", _serverPage.ServiceMethod.Method);
                CurrentForm.Attributes.Add("target", _serverPage.Target);
            }
            else
            {
                throw new ArgumentNullException(string.Join(";", list));
            }
        }
        

        private string BuildParamenter(string id, string value)
        {
            string arg = value ?? "";

            return $"&{id}={arg}";
        }

        private string RenderControlAndParamenter(Dictionary<string, string> parameters)
        {
            string text = string.Empty;

            foreach (var item in parameters)
            {
                string value = string.Empty;
                if (item.Value != null)
                {
                    value = item.Value;

                    if (CurrentForm != null)
                    {
                        HtmlInputHidden htmlInputHidden = null;
                        htmlInputHidden = new HtmlInputHidden();
                        htmlInputHidden.ID = item.Key;
                        htmlInputHidden.Value = value;
                        htmlInputHidden.Attributes["class"] = "__parameter";
                        CurrentForm.Controls.Add(htmlInputHidden);
                    }
                }

                text += BuildParamenter(item.Key, item.Value);
            }

            return text;
        }
    }
}
