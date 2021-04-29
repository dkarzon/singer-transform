using Octostache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingerTransform.Octostache
{
    public class OctostacheDictionary
    {
        private readonly VariableDictionary _dictionary;

        public OctostacheDictionary()
        {
            _dictionary = new VariableDictionary();

            _dictionary.AddExtension("cleanurl", CleanUrl);
        }

        public void Add(string key, string value)
        {
            _dictionary.Add(key, value);
        }

        public string Evaluate(string expressionOrVariableOrText)
        {
            return _dictionary.Evaluate(expressionOrVariableOrText);
        }

        private static string CleanUrl(string argument, string[] options)
        {
            var parsedUrl = argument?.ToLower();

            if (parsedUrl?.Contains('?') ?? false)
            {
                parsedUrl = parsedUrl.Substring(0, parsedUrl.IndexOf('?'));
            }

            if (parsedUrl?.Contains('#') ?? false)
            {
                parsedUrl = parsedUrl.Substring(0, parsedUrl.IndexOf('#'));
            }

            return parsedUrl;
        }
    }
}
