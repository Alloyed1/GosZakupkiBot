using System.Drawing.Design;
using OpenQA.Selenium;

namespace GosZakupkiBot
{
    public class BrowsersModel
    {
        public IWebDriver Browser { get; set; }
        public bool IsFree { get; set; } = true;
    }
}