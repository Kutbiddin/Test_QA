using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessWatcher
{
    internal delegate void DelegateMessage(string text);

    public class Watcher
    {
        private Process[] processes;
        private Process process;
        private string processName;
        
        private byte aliveTime;
        private byte checkTime;
        private StreamWriter logStream;
        private DelegateMessage writer;
        

        public Watcher(string processName = "notepad", string aliveTime = "5", string checkTime = "1")
        {
            this.processName = processName;

            bool convertResult1 = Byte.TryParse(aliveTime, out this.aliveTime);
            bool convertResult2 = Byte.TryParse(checkTime, out this.checkTime);

            if (convertResult1 && convertResult2)
            {
                this.InitStream();
                writer.Invoke("Successfully started");
                this.Watch();
            }
            else
            {
                writer.Invoke("Entered data is incorrect.");
            }
        }

        private void WriteWithFile(string text)
        {
            Console.WriteLine("{0:t}\t{1}", DateTime.Now, text);
            this.logStream.WriteLine("{0}\t{1}", DateTime.Now, text);
        }

        private void WriteWithoutFile(string text)
        {
            Console.WriteLine("{0:t}\t{1}", DateTime.Now, text);
        }

        private void InitStream(string way = "log.txt")
        {
            try
            {
                this.logStream = new StreamWriter(way);
            }
            catch (IOException ex)
            {
                writer = this.WriteWithoutFile;
                writer.Invoke(String.Format("Failed to create log file: {0}", ex.Message));
            }
            finally
            {
                writer = this.WriteWithFile;
                writer.Invoke("Log file created.");
            }
        }

        private void Watch()
        {
            try
            {
                processes = Process.GetProcessesByName(this.processName);
                if (processes.Length == 0)
                    writer.Invoke("Process not found.");
                byte watchingMinutes = 0;

                while (processes.Length != 0)
                {
                    if (watchingMinutes >= this.aliveTime)
                    {
                        this.Kill();
                        writer.Invoke("Process terminated forcibly.");
                        break;
                    }

                    Thread.Sleep(60000 * this.checkTime);
                    watchingMinutes++;
                    writer.Invoke(String.Format("The process has been running from {0} to {5} minutes.",
                        watchingMinutes, this.aliveTime));
                    processes = Process.GetProcessesByName(this.processName);
                }
            }
            catch (Exception ex)
            {
                writer.Invoke(String.Format("Observation Exception: {0}", ex.Message));
            }
            finally
            {
                writer.Invoke("Observation completed");
            }
        }

        private void Kill()
        {
            foreach (Process item in this.processes)
            {
                item.Kill();
            }
        }
    }
}