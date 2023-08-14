using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WF_CrawlDataUsingSeleniumChrome
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public async Task<List<string>> GetData()
        {
            
            string fullUrl = "https://investorlift.com/";
            List<string> programmerLinks = new List<string>();

            try
            {
                var options = new ChromeOptions()
                {
                    BinaryLocation = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe"
                };

                options.AddArguments("--proxy-server=" + "172.16.23.254" + ":" + "3128"); // sử dụng qua proxy
                options.AddArguments(new List<string>() { "headless", "disable-gpu" }); // không hiện trang chrome lên

                IWebDriver browser = new ChromeDriver(options);
                browser.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(5);
                browser.Navigate().GoToUrl(fullUrl);

                int a = -1;
                int b = 0;

                while (a < b)
                {
                    ((IJavaScriptExecutor)browser).ExecuteScript("window.scrollTo(0, document.body.scrollHeight - 150)");
                    a = b;
                    await Task.Delay(5).ContinueWith(t =>
                    {
                        //var webElements = browser.FindElements(By.ClassName("listing-compact-title"));
                        var webElements = browser.FindElements(By.ClassName("property-list-item"));
                        b = webElements.Count;
                    });
                }

                //var links = browser.FindElementsByXPath("//li[not(contains(@class, 'tocsection'))]/a[1]");
                var elements = browser.FindElements(By.ClassName("property-list-item"));
                int i = 1;
                foreach (var element in elements)
                {
                    var url = element.FindElement(By.ClassName("listing-item"));
                    var title = element.FindElement(By.ClassName("listing-compact-title"));
                    var address = element.FindElement(By.ClassName("second-line-address"));
                    var price = element.FindElement(By.ClassName("listing-price"));

                    programmerLinks.Add($"{i}: Url: {url.GetAttribute("href")}      Title: {title.Text}     Address: {address.Text}     Price: {price.Text}");
                    i++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetData ERR: {ex.ToString()}");
            }

            return programmerLinks;
        }

        private async void btnGet_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var datas = await GetData();
            Console.WriteLine($"Crawl completed: {datas.Count} item - {stopwatch.ElapsedMilliseconds}");
            stopwatch.Stop();
            if (datas != null)
            {
                lbData.DataSource = datas;
            }    
        }
    }
}
