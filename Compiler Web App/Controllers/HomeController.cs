using compiler_projecto.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.FormCollection;

namespace compiler_projecto.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static string text_content = "";
        static string dataFile = "";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(string text, string loadedfile, string state)
        {
            text_content = text;
            ViewBag.input = text;
            ViewBag.output = "";
            if (state == "parse")
            {
                //parse code.
                List<string> BigInput = new List<string>();
                string inputText = "";
                text_content += "\n";
                for (int i = 0; i < text_content.Length; i++)
                {
                    if (text_content[i] != '\n')
                    {
                        inputText += text_content[i];
                    }
                    else
                    {
                        inputText += '\n';
                        BigInput.Add(inputText);
                        inputText = "";
                    }
                }

                string ret = ScannerModel.Main(BigInput, false);

                ViewBag.output = ret;
                //ViewBag.input = "";
                return View("Index");
            }
            else if (state == "scan")
            {
                //scan code.
                List<string> BigInput = new List<string>();
                string inputText = "";
                text_content += "\n";
                string go = "";
                for (int i = 0; i < text_content.Length; i++)
                {
                    if (text_content[i] != '\n')
                    {
                        inputText += text_content[i];
                    }
                    else
                    {
                        inputText += '\n';
                        BigInput.Add(inputText);
                        inputText = "";
                    }
                }

                string ret = ScannerModel.Main(BigInput, true);

                ViewBag.output = ret;
                //ViewBag.input = "";
                //ViewBag.output = text_content;
                return View("Index");
            }
            else
            {
                //dataFile = text;
                ViewBag.output = loadedfile;
                return View("Index");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
