using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Controllers
{
    public class gitpullController : Controller
    {
        //
        // GET: /Git/

        public ActionResult Index()
        {
            ProcessStartInfo processinfo = new ProcessStartInfo("cmd.exe");
            processinfo.LoadUserProfile = true;
            processinfo.UseShellExecute = false;
            processinfo.CreateNoWindow = true;
            
            processinfo.RedirectStandardOutput = true;
            processinfo.RedirectStandardInput = true;
            processinfo.RedirectStandardError = true;
            processinfo.WorkingDirectory = @"D:\Documents\Work\jeff\kontinuity\KontinuityCRM\";

            var process = new Process();
            process.StartInfo = processinfo;

            var sb = new System.Text.StringBuilder();

            process.OutputDataReceived += (s, e) => 
            { 
                sb.AppendLine(e.Data);
            };

            bool processStarted = process.Start();
           
            process.BeginOutputReadLine();

            process.StandardInput.WriteLine("dir");
            process.StandardInput.WriteLine("Exit");
            
            process.WaitForExit();
            process.Close();

            var link = string.Format("<br><a href='{0}'>deploy</a>", Url.Action("deploy"));
            return Content(sb.Replace(System.Environment.NewLine, "<br>").ToString() +  link);
        }

        public ActionResult deploy() 
        {
           
            string path = "D:\\batch\\vazquez\\kcrmdeploy.bat";
            ProcessStartInfo process = new ProcessStartInfo();
            process.FileName = @"C:\Windows\System32\cmd.exe";
            process.LoadUserProfile = true;
          
            process.UseShellExecute = false;
            process.CreateNoWindow = true;
            process.RedirectStandardOutput = true;
            process.RedirectStandardInput = true;
            process.RedirectStandardError = true;
            process.WorkingDirectory = @"D:\";
            Process batchProcess = Process.Start(process);

                      
            // sending input // reading output
            StreamReader stream = System.IO.File.OpenText(path);
            StreamReader outstream = batchProcess.StandardOutput;
            StreamWriter instream = batchProcess.StandardInput;

            while (stream.Peek() != -1)
            {
                instream.WriteLine(stream.ReadLine());
            }
            stream.Close();
            instream.WriteLine("Exit");
            instream.Close();

            string output = outstream.ReadToEnd().Trim();
            outstream.Close();

            batchProcess.WaitForExit();
            batchProcess.Close();
          
            return Content(output.Replace(System.Environment.NewLine, "<br>"));
        }


        public ActionResult test()
        {
            var psi = new ProcessStartInfo("cmd.exe")
            {
                WorkingDirectory = Environment.CurrentDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
               
            };

            var sb = new System.Text.StringBuilder();

            using (var process = new Process { StartInfo = psi })
            {
                // delegate for writing the process output to the response output
                Action<Object, DataReceivedEventArgs> dataReceived = ((sender, e) =>
                {
                    if (e.Data != null) // sometimes a random event is received with null data, not sure why - I prefer to leave it out
                    {
                        Response.Write(e.Data);
                        Response.Write(Environment.NewLine);
                        Response.Flush();

                        sb.AppendLine(e.Data);
                    }
                });

                process.OutputDataReceived += new DataReceivedEventHandler(dataReceived);
                process.ErrorDataReceived += new DataReceivedEventHandler(dataReceived);

                // use text/plain so line breaks and any other whitespace formatting is preserved
                Response.ContentType = "text/plain";

                // start the process and start reading the standard and error outputs
                process.Start();

                //process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                process.StandardInput.WriteLine("dir");
                process.StandardInput.WriteLine("exit");

                // wait for the process to exit
                process.WaitForExit();

                // an exit code other than 0 generally means an error
                //if (process.ExitCode != 0)
                //{
                //    Response.StatusCode = 500;
                //}
            }

            Response.End();

            return Content(sb.ToString());
        }

    }
}
