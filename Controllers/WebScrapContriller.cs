using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Net;
using System.IO;

namespace WebScrapper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebScrapContriller : ControllerBase
    {
        private readonly ILogger<WebScrapContriller> _logger;

        public WebScrapContriller(ILogger<WebScrapContriller> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "DownloadImages")]
        public IActionResult Download(WebScrapRequest request)
        {
            var dlDir = request.DownloadDir.TrimEnd('\\').TrimEnd('/');
            if (!Directory.Exists(dlDir))
            {
                return BadRequest();
            }

            var options = new FirefoxOptions();
            var driver = new FirefoxDriver(options);

            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                driver.Navigate().GoToUrl(request.Url);

                int counter = 0;
                var target = driver.FindElement(By.ClassName("img-list"));
                var images = target.FindElements(By.TagName("img"));
                foreach (var img in images)
                {
                    counter += 1;
                    var _screenShot = (img as ITakesScreenshot)?.GetScreenshot();
                    _screenShot?.SaveAsFile(@$"{dlDir}\image_" + counter + $".jpg", ScreenshotImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                driver?.Dispose();
            }

            return Ok();
        }
    }
}