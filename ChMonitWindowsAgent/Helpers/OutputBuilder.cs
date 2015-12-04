using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ChMonitoring.Helpers {
    class OutputBuilder {
        private StringBuilder m_template = null;
        public List<string> Tags { get; set; }

        public OutputBuilder(string template = null) {
            if(string.IsNullOrEmpty(template))
                throw new ArgumentNullException();

            m_template = new StringBuilder(template);
            Tags = new List<string>();

            var matches = new Regex("{[A-Z_]+}").Matches(template);
            foreach(Match match in matches) {
                Tags.Add(match.Value.Substring(1, match.Value.Length - 2));
            }
        }

        public string GetResult() {
            return m_template.ToString();
        }

        public void ReplaceAllTags(params object[] replacements) {
            int minimumLength = Math.Min(Tags.Count, replacements.Length);
            for(int i = 0; i < minimumLength; i++) {
                ReplaceTagInStringBuilder(Tags[i], replacements[i]);
            }
        }

        public void ReplaceTag(string tag, object replacement) {
            if(Tags.Contains(tag))
                ReplaceTagInStringBuilder(tag, replacement);
        }

        private void ReplaceTagInStringBuilder(string tag, object replacement) {
            if(replacement == null)
                m_template.Replace("{" + tag + "}", "null");
            else
                m_template.Replace("{" + tag + "}", replacement.ToString());
        }
    }
}
