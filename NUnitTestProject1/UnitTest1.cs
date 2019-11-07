using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace NUnitTestProject1
{
    public class Tests
    {

        IWebDriver Driver;
        WebDriverWait Wait;
        HashSet<string> VisitedArticles;

        readonly By RandomArticleLinkLocator = By.CssSelector("#n-randompage");
        readonly By FirstLinkLocator = By.XPath("( //div[@class='mw-parser-output']//a [not(ancestor::div[@class='NavContent'])] [not(ancestor::table)] [parent::p] [not(preceding-sibling::text()[1] = ', ')] [not(following-sibling::*[1][@lang])] [not(preceding-sibling::*[1][@lang][not(following-sibling::node()[1][not(contains(text(),')'))])])] [preceding-sibling::text()[1][substring(.,string-length(.)) = ' ']] [not(following-sibling::text()[1][contains(.,')') and not(contains(.,'('))])] )[1]");
        readonly By ArticleTitleLocator = By.CssSelector("h1#firstHeading");
        readonly string WikipediaUrl = "https://ru.wikipedia.org";
        readonly string PhilosophyPageUrl = "https://ru.wikipedia.org/wiki/%D0%A4%D0%B8%D0%BB%D0%BE%D1%81%D0%BE%D1%84%D0%B8%D1%8F";

        readonly string LogLine = "Шаг {0} – {1} - {2}";


        [SetUp]
        public void Setup()
        {
            Driver = new ChromeDriver("C:\\zAutomation");
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20));
            VisitedArticles = new HashSet<string>();

        }

        [Test]
        public void Test1()
        {
            Driver.Url = WikipediaUrl;

            Driver.FindElement(RandomArticleLinkLocator).Click();

            int StepNumber = 1;

            using (StreamWriter LogFile = new StreamWriter(@".\log.txt"))
            {
                while (Driver.Url != PhilosophyPageUrl)
                {
                    Thread.Sleep(500);
                    Wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                    try
                    {
                        Wait.Until(ExpectedConditions.ElementToBeClickable(FirstLinkLocator));
                    }
                    catch (NoSuchElementException e)
                    {
                        Assert.Fail("The page" + Driver.Url + "does not contain main article!");
                    }

                    LogFile.WriteLine(string.Format(LogLine, StepNumber, Driver.FindElement(ArticleTitleLocator).Text, Driver.Url));

                    if (VisitedArticles.Contains(Driver.Url))
                    {
                        Assert.Fail("Looping through the same articles! (" + Driver.Url + ")");
                    }

                    VisitedArticles.Add(Driver.Url);
                    StepNumber++;

                    Driver.FindElement(FirstLinkLocator).Click();

                }
            }

            Assert.Pass("Success!");
        }

        [TearDown]
        public void TearDown()
        {
            Driver.Close();
        }
    }
}