using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;

namespace Clean.Console
{
    public class HtmlHelper
    {
        //יצירת מופע בודד מסוג המחלקה
        private readonly static HtmlHelper _instance = new HtmlHelper();
        //החזרת המופע לשימוש בנתוני אובייקט המחלקה
        public static HtmlHelper Instance => _instance;
        public string[] HtmlTags { get; set; }
        public string[] HtmlOneTags { get; set; }
       
        private HtmlHelper()
        {
            //ReadAllText פונקצית מערכת זו קורא לנו את הנתונים מן הקובץ
            //שינוי מאפיני הקובץ ל=>Copy always
            //המרה למערך  string[] JsonSerializer.Deserialize<string[]>
            HtmlTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("seed/HtmlTags.json"));
            HtmlOneTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("seed/HtmlOneTags.json"));
        }
    }
}
