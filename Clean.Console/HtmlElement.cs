using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Console
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<string> Classes { get; set; } = new List<string>();
        public string InnerHtml { get; set; } = "";
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();


        public override string ToString()
        {
            string s = "";
            if (Name != null) s += "Name: " + Name;
            if (Id != null) s += " Id: " + Id;
            if (Classes.Count > 0)
            {
                s += " Classes: ";
                foreach (var c in Classes)
                    s += c + " ";
            }
            return s;
        }
        //הפונקציה מחזירה את כל צאצאי האלמנט
        //IEnumerable מסוג:כדי שיחזיר בעת קריאה
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                HtmlElement currentElement = queue.Dequeue();
                // "החזרת" האלמנט הנוכחי מהפונקציה.
                yield return currentElement;

                // לולאה על כל ילדי האלמנט.
                foreach (HtmlElement child in currentElement.Children)
                {
                    // הוספת כל ילד לתור.
                    queue.Enqueue(child);

                }

            }
        }
        //הפונקציה מחזירה את כל האבות האלמנט
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this.Parent;

            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }
        //פונקצית מעטפת לצורך חיפוש הselector  המבוקש
        //מעבר על כל ילד ועפלת חיפוש על כל צאצאיו
        public IEnumerable<HtmlElement> FindBySelector( Selector selector)
        {

            // יצירת אוסף לאחסון תוצאות החיפוש
            HashSet<HtmlElement> resultsSet = new HashSet<HtmlElement>();
            
            foreach (HtmlElement child in Descendants())
            {
                FindElementsBySelector(child, selector, resultsSet);

            }
            // החזרת תוצאות החיפוש
            return resultsSet;
        }
        //הפונקציה אשר מחפשת את הselector המבוקש עבור כל אלמנט 
        // על עץ האלמנטים ועץ הסלקטור
        private static void FindElementsBySelector(HtmlElement element, Selector selector, HashSet<HtmlElement> results)
        {
            // בדיקת תנאי עצירה - הגענו לסוף הסלקטור
            if (selector == null)
            {
                // הוספת האלמנט הנוכחי לתוצאות
                results.Add(element);
                return;
            }

            // סינון צאצאים לפי שם התג
            if (!string.IsNullOrEmpty(selector.TagName))
            {
                foreach (HtmlElement child in element.Descendants().Where(e => e.Name == selector.TagName))
                {
                    // הפעלת הפונקציה ריקורסיבית על הצאצאים המתאימים
                    FindElementsBySelector(child, selector.Child, results);
                }
            }

            // סינון צאצאים לפי מזהה
            if (!string.IsNullOrEmpty(selector.Id))
            {
                if (element.Id == selector.Id)
                {
                    // הפעלת הפונקציה ריקורסיבית על הצאצאים המתאימים
                    FindElementsBySelector(element, selector.Child, results);
                }
            }

            // סינון צאצאים לפי מחלקות
            if (selector.Classes.Count > 0)
            {
                bool allClassesMatch = true;
                foreach (string className in selector.Classes)
                {
                    if (!element.Classes.Contains(className))
                    {
                        allClassesMatch = false;
                        break;
                    }
                }

                if (allClassesMatch)
                {
                    // הפעלת הפונקציה ריקורסיבית על הצאצאים המתאימים
                    FindElementsBySelector(element, selector.Child, results);
                }
            }
        }


    }
}

