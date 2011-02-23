using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Mono.Options;

namespace ParallelHttpGet
{
    class Program
    {
        static void Main(string[] args)
        {
            var help = false;
            string file = null;
            string threads = null;
            int threadCount = 5;

            #region console parameter handling
            var optionSet = new OptionSet()
                .Add("?|help|h", "Prints out the options.", option => help = option != null)
            .Add("file|f", "The file containing the url's to call. REQUIRED.", option => file = option)
            .Add("threads|t", "The number of threads to use. 5 By default.", option => threads = option);

            try
            {
                optionSet.Parse(args);
            }
            catch (OptionException)
            {
                ShowHelp("Error - usage is:", optionSet);
            }

            if (help)
            {
                const string usageMessage = "ParallelHttpGet.exe /f[ile] VALUE /t[hreads] VALUE ";
                ShowHelp(usageMessage, optionSet);
            }

            if (string.IsNullOrEmpty(file))
            {
                ShowHelp("Error: You must specify the url file (/f).", optionSet);
            }
            if(!string.IsNullOrEmpty(threads))
            {
                int.TryParse(threads, out threadCount);
            }
            #endregion

            var urlLines = new List<string>();

            if (File.Exists(file))
            {
                StreamReader srFile = null;
                try
                {
                    string line = null;
                    srFile = new StreamReader(file);
                    while ((line = srFile.ReadLine()) != null)
                    {
                        urlLines.Add(line);
                    }
                }
                finally
                {
                    if (srFile != null)
                        srFile.Close();
                }
            } 

            var workers = new ThreadFactory(threadCount, urlLines.Count, (min, max, text) =>
            {

                for (var i = min; i <= max; i++)
                {
                    var request = (HttpWebRequest) WebRequest.Create("http://www.mayosoftware.com");
                    var response = (HttpWebResponse) request.GetResponse();
                    Thread.Sleep(1);
                }
            });

            workers.StartWorking();
            Console.Out.WriteLine("Completed. Press any key to continue...");
            Console.ReadKey(true);
        }

        public static void ShowHelp(string message, OptionSet optionSet)
        {
            Console.Error.WriteLine(message);
            optionSet.WriteOptionDescriptions(Console.Error);
            Environment.Exit(-1);
        }
    }
}