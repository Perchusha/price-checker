using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace PriceChecker
{
    public class PriceCheckerService
    {
        public async Task<string> GetHtmlUsingSeleniumAsync(string url)
        {
            return await Task.Run(() =>
            {
                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;

                var options = new ChromeOptions();
                options.AddArgument("--headless");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--window-size=1920,1080");
                options.AddExcludedArgument("enable-automation");
                options.AddAdditionalOption("useAutomationExtension", false);
                options.AddArgument("--disable-blink-features=AutomationControlled");
                options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                    "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36");

                using (var driver = new ChromeDriver(service, options))
                {
                    driver.ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");
                    driver.Navigate().GoToUrl(url);
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    wait.Until(d => !d.Title.Contains("Just a moment") && !d.PageSource.Contains("Cloudflare"));
                    return driver.PageSource;
                }
            });
        }

        public async Task<decimal> GetPriceFromUrl(string url)
        {
            try
            {
                string html = await GetHtmlUsingSeleniumAsync(url);

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                if (url.ToLower().Contains("amazon"))
                {
                    var priceNode = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'a-price-whole')]");
                    if (priceNode != null)
                    {
                        string priceText = priceNode.InnerText.Trim();
                        string cleaned = new string(priceText.Where(ch => char.IsDigit(ch) || ch == '.' || ch == ',').ToArray());
                        cleaned = cleaned.Replace(',', '.');
                        if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                        {
                            return price;
                        }
                    }
                }
                else if (url.ToLower().Contains("nbsklep"))
                {
                    var priceNode = doc.DocumentNode.SelectSingleNode("//*[@data-qa-product_price]");
                    if (priceNode != null)
                    {
                        string priceAttr = priceNode.GetAttributeValue("data-qa-product_price", "");
                        if (!string.IsNullOrEmpty(priceAttr))
                        {
                            if (decimal.TryParse(priceAttr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                            {
                                return price;
                            }
                        }
                    }
                }
                else if (url.ToLower().Contains("ceneo"))
                {
                    var valueNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'product-top__price')]//span[@class='value']");
                    var pennyNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'product-top__price')]//span[@class='penny']");

                    if (valueNode != null && pennyNode != null)
                    {
                        string priceText = valueNode.InnerText.Trim() + pennyNode.InnerText.Trim();
                        string numericPart = new string(priceText.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
                        if (decimal.TryParse(numericPart, NumberStyles.Any, new CultureInfo("pl-PL"), out decimal price))
                        {
                            return price;
                        }
                    }
                }
                else if (url.ToLower().Contains("x-kom"))
                {
                    var priceNode = doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'parts__Price-sc-53da58c9-2')]");
                    if (priceNode != null)
                    {
                        string priceText = priceNode.InnerText.Trim();
                        string numericPart = new string(priceText.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());
                        if (decimal.TryParse(numericPart, NumberStyles.Any, new CultureInfo("pl-PL"), out decimal price))
                        {
                            return price;
                        }
                    }
                }
                else if (url.ToLower().Contains("morele.net"))
                {
                    var priceNode = doc.DocumentNode.SelectSingleNode("//div[@id='product_price']");
                    if (priceNode != null)
                    {
                        string dataPrice = priceNode.GetAttributeValue("data-price", "");
                        if (!string.IsNullOrEmpty(dataPrice))
                        {
                            if (decimal.TryParse(dataPrice, System.Globalization.NumberStyles.Any,
                                                 System.Globalization.CultureInfo.InvariantCulture, out decimal price))
                            {
                                return price;
                            }
                        }
                    }
                }
                else if (url.ToLower().Contains("yesstyle.com"))
                {
                    var priceNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'productDetailPage_priceContainer__8AIXw') and contains(@class, 'notranslate')]/span[contains(@class, 'productDetailPage_sellingPrice__s6PZu')]");
                    if (priceNode != null)
                    {
                        string priceText = priceNode.InnerText.Trim();
                        string numericPart = new string(priceText.Where(c => char.IsDigit(c) || c == ',' || c == '.').ToArray());

                        if (decimal.TryParse(numericPart, NumberStyles.Any, new CultureInfo("pl-PL"), out decimal price))
                        {
                            return price;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при получении цены: " + ex.Message);
            }
            return decimal.MaxValue;
        }
    }
}
