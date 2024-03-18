using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.LinkModels
{
    public class Link
    {
        public string? Href { get; set; }       // represents a target URI
        public string? Rel { get; set; }        // link relation type - descirbes how current context is related to target resource
        public string? Method { get; set; }     // HTTP method

        public Link() { }

        public Link(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
