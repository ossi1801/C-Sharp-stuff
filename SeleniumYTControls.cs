using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

internal class Program
{
    
    private static void Main(string[] args)
    {

        var options = new ChromeOptions();
        //string profilePath = string.Format(@"{0}\Google\Chrome\User Data\", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        string profilePath = string.Format(@"{0}\User Data\", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
        string p = string.Format(@"user-data-dir={0}", profilePath);
        options.AddArgument("--no-sandbox");
        //options.AddArgument("--headless");     
        //options.AddArgument("--disable-gpu"); // applicable to windows os only
        //options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument(p);
        options.AddArgument("profile-directory=Profile 2");

        var webDriver = new ChromeDriver(options);
        string baseURL = "youtubeplaylisthere";
        webDriver.Url = baseURL;
        webDriver.Manage().Window.Minimize(); //minimize
    
        Console.WriteLine("Started" + webDriver.Title);
        webDriver.FindElement(By.TagName("Body")).SendKeys("k");
        Thread.Sleep(10000);       
        Next(ref webDriver);
        Thread.Sleep(5000);
        Previous(ref webDriver);   
    }
    private static void Next(ref ChromeDriver?  webDriver)
    {
        Console.WriteLine("Next song");
        webDriver.FindElement(By.TagName("Body")).SendKeys(Keys.Shift + "N");
       // WebDriverExtensions.FindElement(webDriver, By.CssSelector("a.ytp-next-button.ytp-button"), 10).Click();
    }
    private static void Previous(ref ChromeDriver? webDriver)
    {
        Console.WriteLine("Previous song");
        webDriver.FindElement(By.TagName("Body")).SendKeys(Keys.Shift + "P");
        webDriver.FindElement(By.TagName("Body")).SendKeys(Keys.Shift + "P");
       //WebDriverExtensions.FindElement(webDriver, By.CssSelector("a.ytp-prev-button.ytp-button"), 10).Click();
    }

}
