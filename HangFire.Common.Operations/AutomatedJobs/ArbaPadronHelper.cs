using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SharedModel.Models.General;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangFire.Common.Operations.AutomatedJobs
{
    public class ArbaPadronHelper
    {

        public DataTable GenerateRows(string inputFile)
        {
            if (!System.IO.File.Exists(inputFile))
            {
                throw new Exception("File was not found: " + inputFile);
            }

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ProcessDate", typeof(long));
            dataTable.Columns.Add("Reg");
            dataTable.Columns.Add("ToDate");
            dataTable.Columns.Add("Cuit", typeof(long));
            dataTable.Columns.Add("BussinessName");
            dataTable.Columns.Add("Letter");
            dataTable.Columns.Add("Active", typeof(short));

            ConcurrentBag<RegisteredEntityArba> concurrentBag = new ConcurrentBag<RegisteredEntityArba>();
            Parallel.ForEach(System.IO.File.ReadLines(inputFile, Encoding.Default), (line, _, lineNumber) =>
            {
                if (line.Length == 64)
                {
                    concurrentBag.Add(new RegisteredEntityArba
                    {
                        Cuit = long.Parse(line.Substring(2, 11)),
                        BussinessName = line.Substring(13, 44),
                        Letter = line.Substring(57, 1),
                        ToDate = line.Substring(58, 6)
                    });
                }
            });

            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            while (concurrentBag.Count > 0)
            {
                if (concurrentBag.TryTake(out RegisteredEntityArba entity))
                {
                    dataTable.Rows.Add(
                        unixTimestamp,
                        entity.Reg,
                        entity.ToDate,
                        entity.Cuit,
                        entity.BussinessName,
                        entity.Letter,
                        entity.Active
                    );
                }
            }
            return dataTable;
        }

        public string LoginAndDownloadFile(string arbaLoginURL, string loginUser, string loginPassword, string downloadsFolder)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("download.default_directory", Path.GetDirectoryName(downloadsFolder + "\\"));
            chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
            chromeOptions.AddUserProfilePreference("safebrowsing.enabled", false);
            //chromeOptions.AddUserProfilePreference("intl.accept_languages", "nl");
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
            chromeOptions.AddArgument("--start-maximized");
            IWebDriver Driver = new ChromeDriver(@"C:\ChromeDriver", chromeOptions);
            Driver.Manage().Window.Maximize();

            Driver.Navigate().GoToUrl(arbaLoginURL);
            ((IJavaScriptExecutor)Driver).ExecuteScript("window.resizeTo(1024, 768);");

            string[] loginParts = loginUser.Split('-');

            IWebElement inputPrefijo = Driver.FindElement(By.Id("prefijo"));
            inputPrefijo.SendKeys(loginParts[0]);
            IWebElement inputCuil = Driver.FindElement(By.Id("dni"));
            inputCuil.SendKeys(loginParts[1]);
            IWebElement inputSufijo = Driver.FindElement(By.Id("sufijo"));
            inputSufijo.SendKeys(loginParts[2]);
            IWebElement inputPassword = Driver.FindElement(By.Id("password"));
            inputPassword.SendKeys(loginPassword);

            Driver.FindElement(By.CssSelector("#frmDatos.f-m .botones input[type=submit]")).Submit();

            // wait 2 seconds
            System.Threading.Thread.Sleep(2000);
            // Selecciona Rol 
            Driver.FindElement(By.Id("seleccionar")).Click();

            System.Threading.Thread.Sleep(2000);
            // Item menu "Regimen Intermediarios"
            Driver.FindElement(By.CssSelector(".navAplicaciones #cmenu > li:nth-child(3) > a")).Click();
            // Item menu "Padron Intermediarios"
            Driver.FindElement(By.CssSelector(".navAplicaciones #cmenu > li:nth-child(3) li a")).Click();

            System.Threading.Thread.Sleep(2000);

            string FileName = Driver.FindElement(By.CssSelector("table.subtituloColumna tbody tr:nth-child(2) td:nth-child(1)")).Text.ToString();
            ((IJavaScriptExecutor)Driver).ExecuteScript("javascript:descargar('" + FileName + "')");

            //Driver.FindElement(By.CssSelector("table.subtituloColumna tbody tr:nth-child(2) a.linkDestacado")).Click();
            // wait 20 seconds for file to finish downloading
            System.Threading.Thread.Sleep(60000 * 5);

            // Close browser and ends session
            Driver.Dispose();

            return FileName;
        }
    }
}
