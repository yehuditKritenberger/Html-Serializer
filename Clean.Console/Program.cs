// See https://aka.ms/new-console-template for more information


using Clean.Console;
using System.Collections.Generic;
using System.Text.RegularExpressions;

var html = await Load("https://forum.netfree.Link/category/1/%D7%94%D7%9B%D7%AB%D7%96%D7%95%D7%AA");

// הסרת רווחים מיותרים
html = new Regex("[\\r\\n\\t]").Replace(new Regex("\\s{2,}").Replace(html, ""), "");

// פיצול לפי תגיות
var htmlLines = Regex.Split(html, "<(.*?)>").Where(s => !string.IsNullOrEmpty(s)).ToList();

// יצירת אובייקט שורש
HtmlElement rootElement = CreateChild(htmlLines[1].Split(' ')[0], null, htmlLines[1]);
//בניית עץ אלמנטים
Serialize(rootElement, htmlLines.Skip(2).ToList());
//הדפסת עץ האלמנטים
PrintHtmlTree(rootElement, "");
//חיפוש אוביקטים עפ"י הסלקטור המבוקש
//הדפסת האובייקטים שנימצאו
Console.WriteLine("ul.nav.navbar-nav");
var list1 = rootElement.FindBySelector(Selector.ParseSelectorString("ul.nav.navbar-nav"));
foreach (var l in list1)
{
    Console.WriteLine(l + " ");
}
Console.WriteLine("span.visible-sm-inline");
var list2 = rootElement.FindBySelector(Selector.ParseSelectorString("span.visible-sm-inline"));
foreach (var l in list2)
{
    Console.WriteLine(l + " ");
}
Console.WriteLine("button.btn");
var list3 = rootElement.FindBySelector(Selector.ParseSelectorString("button.btn"));
foreach (var l in list3)
{
    Console.WriteLine(l + " ");
}


async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
//בניית עץ אלמנטים
static HtmlElement Serialize(HtmlElement rootElement, List<string> htmlLines)
{
    HtmlElement currentElement = rootElement;
   

    foreach (string line in htmlLines)
    {
        // שם התגית
        string tagName = line.Split(' ')[0];
        // טיפול בתגיות מיוחדות
        if (tagName == "/html")
        {
            break;
        }
        else if (line.StartsWith("/"))
        {
            currentElement = currentElement.Parent;
            continue;
        }
        // טיפול בטקסט פנימי
        if ( line.Length > 1&&! HtmlHelper.Instance.HtmlTags.Contains(tagName))
        {
            currentElement.InnerHtml += line;
            continue;
        }
      
          //  יצירת אובייקט חדש עבור תגית חדשה
          HtmlElement newElement = CreateChild(tagName, currentElement,line);
           currentElement.Children.Add(newElement);
       
        // טיפול בתגיות סוגרות
        //אם זה תגית לא סוגרת האובייקט הנוכחי הופך להיות אבא
        if (!HtmlHelper.Instance.HtmlOneTags.Contains(tagName) && !line.EndsWith("/"))
        {
            currentElement = newElement;
        }
    }
    return rootElement;
}
//יצירת אובייקט חדש והוספתו כילד לאובייקט האב
 static HtmlElement CreateChild(string tagName, HtmlElement currentElement, string line)
{
    HtmlElement child = new HtmlElement { Name = tagName, Parent = currentElement };
    //attributesמציאת  
    var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line);
    foreach ( var attr in attributes)
    {
        //הפרדת attr לname ולvalue
        string attributeName = attr.ToString().Split('=')[0];
        string attributeValue = attr.ToString().Split('=')[1].Replace("\"", "");
        //השמת נתוני האובייקט
        if (attributeName.ToLower() == "class")
            child.Classes.AddRange(attributeValue.Split(' '));
        else if (attributeName.ToLower() == "id")
            child.Id = attributeValue;
        else child.Attributes.Add(attributeName, attributeValue);
    }
    return child; 
}

static void PrintHtmlTree(HtmlElement element, string indentation)
{
    Console.WriteLine($"{indentation}{element}");
    foreach (var child in element.Children)
        PrintHtmlTree(child, indentation + "  ");
}