using Stx.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Stx.Collections.Concurrent;
using System.Threading;

namespace Stx.Net
{
    public class TaskScheduler
    {
        public ConcurrentList<ScheduledTask> Tasks { get; private set; } = new ConcurrentList<ScheduledTask>();

        public ScheduledTask Repeat(Action run, int interval)
        {
            ScheduledTask t = new ScheduledTask(run, 0, interval);

            Tasks.Add(t);

            return t;
        }

        public ScheduledTask RunLater(Action run, int time)
        {
            ScheduledTask t = new ScheduledTask(run, time);

            Tasks.Add(t);

            return t;
        }

        public ScheduledTask RepeatLater(Action run, int delay, int interval)
        {
            ScheduledTask t = new ScheduledTask(run, delay, interval);

            Tasks.Add(t);

            return t;
        }

        public bool StopRun(Action run)
        {
            ScheduledTask st = Tasks.FirstOrDefault((e) => e.Run == run);

            if (st == null)
                return false;

            st.Stop();

            return Tasks.Remove(st);
        }

        public bool StopRun(string taskID)
        {
            ScheduledTask st = Tasks.FirstOrDefault((e) => e.TaskID == taskID);

            if (st == null)
                return false;

            st.Stop();

            return Tasks.Remove(st);
        }

        public bool StopRun(ScheduledTask task)
            => StopRun(task.TaskID);

        public class ScheduledTask : ThreadSafeDataTransfer<ScheduledTask>
        {
            public int Delay { get; set; }
            public int? Interval { get; set; }
            public bool Loop
            {
                get
                {
                    return Interval != null;
                }
            }
            public string TaskID { get; }

            public Action Run { get; set; }

            internal Task task;

            private bool stopOnNext = true;

            public ScheduledTask(Action run, int delay)
            {
                Run = run;
                Delay = delay;
                Interval = null;
                TaskID = Guid.NewGuid().ToString();

                task = Running();
            }

            public ScheduledTask(Action run, int delay, int loopInterval)
            {
                Run = run;
                Delay = delay;
                Interval = loopInterval;
                TaskID = Guid.NewGuid().ToString();

                task = Running();
            }

            public void Stop()
            {
                stopOnNext = false;
            }

            private async Task Running()
            {
                if (Delay > 0)
                {
                    await Task.Delay(Delay);

                    if (stopOnNext)
                        Transfer(this);
                }

                if (Loop)
                {
                    while (stopOnNext)
                    {
                        await Task.Delay(Interval.Value);

                        Transfer(this);
                    }
                }
            }

            protected override void Received(ScheduledTask data)
            {
                data.Run?.Invoke();
            }
        }
    }
}
