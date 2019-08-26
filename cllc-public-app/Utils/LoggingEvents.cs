using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Utils
{
    public static class LoggingEvents
    {
        public const int HttpGet = 1000;
        public const int HttpPost = 1001;
        public const int HttpPut = 1002;
        public const int HttpDelete = 1003;

        public const int Get = 2000;
        public const int Save = 2001;
        public const int Update = 2002;
        public const int Delete = 2003;

        public const int NotFound = 4000;

        public const int Error = 5000;
        public const int BadRequest = 5001;

        public static string GetHeaders (HttpRequest request)
        {
            StringBuilder html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<body>");
            html.AppendLine("<b>Request Headers:</b>");
            html.AppendLine("<ul style=\"list-style-type:none\">");
            foreach (var item in request.Headers)
            {
                html.AppendFormat("<li><b>{0}</b> = {1}</li>\r\n", item.Key, ExpandValue(item.Value));
            }
            html.AppendLine("</ul>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");
            return html.ToString();
        }

        /// <summary>
        /// Utility function used to expand headers.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static string ExpandValue(IEnumerable<string> values)
        {
            StringBuilder value = new StringBuilder();

            foreach (string item in values)
            {
                if (value.Length > 0)
                {
                    value.Append(", ");
                }
                value.Append(item);
            }
            return value.ToString();
        }


    }
}
