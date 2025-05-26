using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;

namespace TodoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [Route("GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)] + " Get"
        })
        .ToArray();
    }

    [HttpPost(Name = "PostWeatherForecast")]
    [Route("PostWeatherForecast")]
    public IEnumerable<WeatherForecast> Post()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)] + " Post"
        })
        .ToArray();
    }

    [HttpGet(Name = "PuppeteerSharpTest")]
    [Route("PuppeteerSharpTest")]
    public async Task<IActionResult> PuppeteerSharpTest()
    {
        var url = "http://localhost:5239/WeatherForecast/PostWeatherForecast";
        await new BrowserFetcher().DownloadAsync();
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions{ Headless = true });
        var page = await browser.NewPageAsync();
        await page.SetCookieAsync(new CookieParam { Name = "127-cookie", Value = "worst", Url =  url});

        await page.SetRequestInterceptionAsync(true);
        page.Request += async (sender, e) =>
        {
            var payload = new Payload { Method = HttpMethod.Post, PostData = "doggo" };
            await e.Request.ContinueAsync(payload);
        };

        await page.GoToAsync(url);
        await page.PdfAsync("./output/output.pdf");
        return this.Ok("Test succeeded");
    }

    [HttpGet(Name = "PuppeteerSharpCustomTest")]
    [Route("PuppeteerSharpCustomTest")]
    public async Task<IActionResult> PuppeteerSharpCustomTest()
    {
        var launcherArgs = new[]
        {
            "--disable-background-timer-throttling",
            "--disable-breakpad",
            "--disable-client-side-phishing-detection",
            "--disable-cloud-import",
            "--disable-default-apps",
            "--disable-dev-shm-usage",
            "--disable-extensions",
            "--disable-gesture-typing",
            "--disable-hang-monitor",
            "--disable-infobars",
            "--disable-notifications",
            "--disable-offer-store-unmasked-wallet-cards",
            "--disable-offer-upload-credit-cards",
            "--disable-popup-blocking",
            "--disable-print-preview",
            "--disable-prompt-on-repost",
            "--disable-setuid-sandbox",
            "--disable-speech-api",
            "--disable-sync",
            "--disable-tab-for-desktop-share",
            "--disable-translate",
            "--disable-voice-input",
            "--disable-wake-on-wifi",
            "--disk-cache-size=33554432",
            "--enable-async-dns",
            "--enable-simple-cache-backend",
            "--enable-tcp-fast-open",
            "--hide-scrollbars",
            "--ignore-gpu-blacklist",
            "--media-cache-size=33554432",
            "--metrics-recording-only",
            "--mute-audio",
            "--no-default-browser-check",
            "--no-first-run",
            "--no-pings",
            "--no-sandbox",
            "--no-zygote",
            "--password-store=basic",
            "--prerender-from-omnibox=disabled",
            "--use-gl=angle",
            "--use-angle=swiftshader",
            "--use-mock-keychain",
        };

        var executablePath = "D:\\TAMU_docs\\5th_and_beyond\\PuppeteerSharp\\TodoApi\\chrome\\win64-133.0.6943.126\\chrome-win64\\chrome.exe";

        using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            ExecutablePath = executablePath,
            AcceptInsecureCerts = true,
            Args = launcherArgs
        });

        using var page = await browser.NewPageAsync();

        var html = "<html><head></head><body>Hello World</body></html>";

        await page.SetContentAsync(html);

        var content = await page.GetContentAsync();

        var pdfContent = await page.PdfDataAsync();

        // await page.PdfAsync("./output/output1.pdf");
        return File(pdfContent, "application/pdf", "output.pdf");
        // return this.Ok("Test succeeded");
    }
}
