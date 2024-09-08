using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application = System.Windows.Application;
namespace Civ6TranslationToolWPF.Levie
{
    public static class ResourceHelper
    {
        public static string T(string key, params string[] args)
        {
            try
            {
                string? resourceString = Application.Current.FindResource(key) as string;

                if (resourceString != null)
                {
                    return string.Format(resourceString, args);
                }

                return KeyToText(key);
            }
            catch (Exception)
            {
                return KeyToText(key);
            }
        }

        public static string T(string key)
        {
            string? resourceString = Application.Current.FindResource(key) as string;

            return resourceString ?? KeyToText(key);
        }

        private static string KeyToText(string key)
        {
            if (string.IsNullOrEmpty(key))
                return key;

            string result = Regex.Replace(key, @"[_\-.]", " ");

            StringBuilder formattedText = new StringBuilder();

            for (int i = 0; i < result.Length; i++)
            {
                char currentChar = result[i];

                if (char.IsUpper(currentChar))
                {
                    if (i > 0 && !char.IsUpper(result[i - 1]))
                    {
                        formattedText.Append(" ");
                    }
                    else if (i > 0 && char.IsUpper(result[i]) && (i + 1 < result.Length && !char.IsUpper(result[i + 1])))
                    {
                        formattedText.Append(" ");
                    }
                }

                formattedText.Append(currentChar);
            }

            return formattedText.ToString().Trim();
        }
    }

}
