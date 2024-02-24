using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Clean.Console
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public Selector Parent { get; set; }
        public Selector Child { get; set; }
        //פונקציה אשר בונה selector בצורה היררכית
        public static Selector ParseSelectorString(string selectorString)
        {
            Selector rootSelector = new Selector();
            Selector currentSelector = rootSelector;
            //פירוק הstring selector למערך של מחרוזות
            string[] partsSelector = selectorString.Split(" ");
            foreach (string part in partsSelector)
            {
                //(פרוק המחרוזת לחלקים לפי המפרידים # ו-. (נקודה
                string[] selectors = new Regex("(?=[#\\.])").Split(part).Where(p => p.Length > 0).ToArray();
                foreach (string selector in selectors)
                {
                    //selector השמת נתוני ה
                    if (selector.StartsWith("#"))
                        currentSelector.Id = selector.Substring(1);
                    else if (selector.StartsWith("."))
                        currentSelector.Classes.Add(selector.Substring(1));
                    else if (HtmlHelper.Instance.HtmlTags.Contains(selector))
                        currentSelector.TagName = selector;
                    else
                        throw new ArgumentException($"ERROR: Invalid HTML tag name: {selector}");
                    //הוספת סלקטור חדש כבן של הסלקטור הנוכחי ועידכון הסלקטור הנוכחי להצביע עליו
                    //כדי ליצור היררכיה
                    Selector newSelector = new Selector();
                    currentSelector.Child = newSelector;
                    newSelector.Parent = currentSelector;
                    currentSelector = newSelector;

                }
                currentSelector.Parent.Child = null;
            }

            return rootSelector;
        }
    }
}
