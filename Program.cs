//           $$\   $$\  $$$$$$\  $$\   $$\ $$\     $$\ $$$$$$$$\ $$\   $$\         $$\    $$\ $$\   $$\ $$\   $$\  $$$$$$\        
//           $$$\  $$ |$$  __$$\ $$ |  $$ |\$$\   $$  |$$  _____|$$$\  $$ |        $$ |   $$ |$$ |  $$ |$$$\  $$ |$$  __$$\      $$$$\  $$$$  \
//           $$$$\ $$ |$$ /  \__|$$ |  $$ | \$$\ $$  / $$ |      $$$$\ $$ |        $$ |   $$ |$$ |  $$ |$$$$\ $$ |$$ /  \__|    $$$$$$$$$$$$$  |
//           $$ $$\$$ |$$ |$$$$\ $$ |  $$ |  \$$$$  /  $$$$$\    $$ $$\$$ |        $$$$$$$$$ |$$ |  $$ |$$ $$\$$ |$$ |$$$$\     $$$$$$$$$$$$$ /
//           $$ \$$$$ |$$ |\_$$ |$$ |  $$ |   \$$  /   $$  __|   $$ \$$$$ |        $$  ___$$ |$$ |  $$ |$$ \$$$$ |$$ |\_$$ |      $$$$$$$$$  /
//           $$ |\$$$ |$$ |  $$ |$$ |  $$ |    $$ |    $$ |      $$ |\$$$ |        $$ |   $$ |$$ |  $$ |$$ |\$$$ |$$ |  $$ |       $$$$$$  /
//           $$ | \$$ |\$$$$$$  |\$$$$$$  |    $$ |    $$$$$$$$\ $$ | \$$ |        $$ |   $$ |\$$$$$$  |$$ | \$$ |\$$$$$$  |         $$  /
//           \__|  \__| \______/  \______/     \__|    \________|\__|  \__|        \__|   \__| \______/ \__|  \__| \______/          \_/


/*
có một số điều muốn nói, đây chỉ là file code, tại chưa biết push hết code lên, tại chưa quen với git lắm
code ở dưới thì khởi tạo 1 project console là chạy được r đó
trước khi xài thì phải cài selenium nhé @@ tui code gà lắm, đừng gạch đá nha
*/
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
class Program
{
    public static string UserId;
    public static string Password;
    public static string LoginPageUrl = "https://www.facebook.com/login";
    public static string TargetPostUrl;
    public static string CommentFilePath = @"Comment"; // tự tạo file txt đi nha
    public static string AccountFilePath = @"Account.txt"; // tự tạo
    public static string PostLinkFilePath = @"PostLink.txt"; // tự tạo
    public static void VietnameseInConsole()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
    }
    public static void NavigateToURL(IWebDriver driver, string url)
    {
        Console.WriteLine("Đang tới " + url);
        driver.Navigate().GoToUrl(url);
    }
    
    public static void LoginFacebook(IWebDriver driver, string UserId, string Password)
    {
        IWebElement UserIdTextBox = driver.FindElement(By.XPath("//*[@id=\"email\"]"));
        IWebElement PasswordTextBox = driver.FindElement(By.XPath("//*[@id=\"pass\"]"));
        IWebElement ConfirmButton = driver.FindElement(By.XPath("//*[@id=\"loginbutton\"]"));
        Console.WriteLine("Tài khoản: " + UserId);
        UserIdTextBox.SendKeys(UserId);
        Thread.Sleep(3000);
        Console.WriteLine("Mật khẩu: " + Password);
        PasswordTextBox.SendKeys(Password);
        Thread.Sleep(3000);
        Console.WriteLine("Nhấn nút Login..");
        ConfirmButton.Click();
        Thread.Sleep(5000);
    }
    // Đọc comment trong file text rồi đưa vào list CommentList
    public static void GetCommentsFromTextFile(List<string> CommentList)
    {
        if (!File.Exists(CommentFilePath))
        {
            Console.WriteLine("Không tìm thấy File " + CommentFilePath);
            return;
        }
        Console.WriteLine(String.Concat(new object[]
        {
            "Đang đọc từng dòng trong ",
            CommentFilePath
        }));
        foreach (string comment in File.ReadLines(CommentFilePath))
        {
            CommentList.Add(comment);
            Console.WriteLine("Đã thêm " + comment + " vào list");
        }
        Console.WriteLine("Đã đọc xong file Comment");
   }
    public static void BeginComment(IWebDriver driver, List<string> CommentList)
    {
        var commentBox = WaitUntilVisible(driver, By.CssSelector("div[aria-placeholder]"));
        Console.WriteLine("Đang nhấn vào ô comment");
        Thread.Sleep(1000);
        try
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].style.border='2px solid red'", commentBox);
            Thread.Sleep(5000);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].focus()", commentBox);
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click()", commentBox);
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            Console.WriteLine("có lỗi xảy ra " + ex);
            driver.Quit();
        }
        

        string randomComment = CommentList[new Random().Next(CommentList.Count)];
        commentBox.SendKeys(randomComment);
        Thread.Sleep(1000);

        commentBox.SendKeys(OpenQA.Selenium.Keys.Enter);

        Console.WriteLine("Comment thành công: " + randomComment);
        Thread.Sleep(3000);
    }

    public static IWebElement WaitUntilVisible(IWebDriver driver, By by, int timeoutSeconds = 10)
    {
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
        return wait.Until(ExpectedConditions.ElementIsVisible(by));
    }
    
    public static bool InitAccount()
    {
        if(!File.Exists(AccountFilePath))
        {
            Console.WriteLine("Không tìm thấy đường dẫn tới file account: " + AccountFilePath);
            return false;
        }
        string[] account;
        Console.WriteLine("Đang đọc account từ " + AccountFilePath);
        string[] AccountFile = File.ReadAllLines(AccountFilePath);
        if(AccountFile.Length <= 0)
        {
            Console.WriteLine("Chưa nhập tài khoản mật khẩu vào file");
            return false;
        }
        foreach (var acc in AccountFile)
        {
            account = acc.Split('|');
            UserId = account[0];
            Password = account[1];
            break;
        } 
        return true;     
    }

    public static bool InitPostLink()
    {
        if (!File.Exists(PostLinkFilePath))
        {
            Console.WriteLine("Không tìm thấy đường dẫn tới file PostLink: " + PostLinkFilePath);
            return false;
        }
        TargetPostUrl = File.ReadAllText(PostLinkFilePath);
        if(TargetPostUrl.Length <= 0)
        {
            Console.WriteLine("Chưa có gì trong PostLink.txt");
            return false;
        }

        return true;
    }
    public static void InitData()
    {
        if(!InitAccount() || !InitPostLink())
        {
            Console.WriteLine("Có lỗi xảy ra");
        }
        else
        {
            Console.WriteLine("Khởi tạo thành công dữ liệu");
        }

    }
    
    public static void Main()
    {
        VietnameseInConsole();
        InitData();
        List<string> CommentList = new List<string>();
        GetCommentsFromTextFile (CommentList);
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--disable-notifications");
        options.AddArgument("--start-maximized");
        IWebDriver driver = new ChromeDriver(options);
        NavigateToURL(driver, LoginPageUrl);
        Console.WriteLine("Đợi load trang...");
        Thread.Sleep(6000);
        LoginFacebook(driver, UserId, Password);
        Console.WriteLine("Đăng Nhập thành công");
        NavigateToURL (driver, TargetPostUrl);
        Thread.Sleep(4000);
        BeginComment(driver, CommentList);
    }
}
