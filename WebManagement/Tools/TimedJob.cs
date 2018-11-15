using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WBPlatform.Logging;
using WBPlatform.StaticClasses;

namespace WBPlatform.WebManagement.Tools
{
    public static class TimedJob
    {
        private static void AddToJobList(Job job)
        {
            L.I("Adding New Job to the collection: " + job.JobName);
            JobCollection.Add(job);
        }

        public static void AddToJobList(string JobName, Func<bool> factory, TimeSpan sleepTime) => AddToJobList(new Job(JobName, factory, sleepTime));
        public static void AddToJobList(string JobName, Func<bool> factory, int Hour, int Minute, int Seconds) => AddToJobList(JobName, factory, new TimeSpan(Hour, Minute, Seconds));
        public static void AddToJobList(string JobName, Func<bool> factory, int Minute, int Seconds) => AddToJobList(JobName, factory, 0, Minute, Seconds);
        public static void AddToJobList(string JobName, Func<bool> factory, int Seconds) => AddToJobList(JobName, factory, 0, 0, Seconds);

        public static void StartJobWatcher() => JobWatcher.Start();
        public static bool Status => JobWatcher.IsAlive;
        public static int JobCount => JobCollection.Count;

        private static ConcurrentBag<Job> JobCollection { get; } = new ConcurrentBag<Job>();
        private static Thread JobWatcher { get; } = new Thread(DoJobs);
        private static void DoJobs()
        {
            while (true)
            {
                foreach (var item in JobCollection)
                {
                    if (item.NeedStart)
                    {
                        L.I("Starting a job: " + item.JobName);
                        new Thread(new ThreadStart(item.DoJob)).Start();
                    }
                }
                Thread.Sleep(200);
            }
        }

        private class Job
        {
            public Job(string Name, Func<bool> factory, TimeSpan sleepTime)
            {
                Factory = factory ?? throw new ArgumentNullException(nameof(factory));
                SleepTime = sleepTime;
                JobName = Name;
            }
            public string JobName { get; }
            public bool IsRunning { get; private set; }
            public Func<bool> Factory { get; set; }
            public TimeSpan SleepTime { get; set; }
            public DateTime LastStartTime { get; private set; }
            public bool IsLastProcessSuccessful { get; private set; }

            public void DoJob()
            {
                IsRunning = true;
                LastStartTime = DateTime.Now;
                try
                {
                    IsLastProcessSuccessful = Factory();
                }
                catch (System.Exception ex)
                {
                    ex.LogException();
                    IsLastProcessSuccessful = false;
                }
                //IsLastProcessSuccessful = Factory();
                IsRunning = false;
            }
            public bool NeedStart => !IsRunning && DateTime.Now.Subtract(LastStartTime) >= SleepTime;
        }
    }
}
