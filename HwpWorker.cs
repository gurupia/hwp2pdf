using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.InteropServices;
using HwpObjectLib;

namespace hwp2pdf
{
    internal class HwpWorker : IDisposable
    {
        class WorkItem
        {
            public Func<HwpObject, object> Func;
            public object Result;
            public Exception Error;
            public ManualResetEventSlim Done = new ManualResetEventSlim(false);
        }

        Thread thread;
        BlockingCollection<WorkItem> queue = new BlockingCollection<WorkItem>();
        bool running = false;

        public HwpWorker()
        {
            thread = new Thread(Run);
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            running = true;
            thread.Start();
        }

        void Run()
        {
            HwpObject hwp = null;
            try
            {
                hwp = new HwpObject();
                while (running)
                {
                    WorkItem wi = null;
                    try
                    {
                        wi = queue.Take();
                    }
                    catch (InvalidOperationException)
                    {
                        break;
                    }
                    try
                    {
                        wi.Result = wi.Func(hwp);
                    }
                    catch (Exception ex)
                    {
                        wi.Error = ex;
                    }
                    finally
                    {
                        wi.Done.Set();
                    }
                }
            }
            catch
            {
                // ignore
            }
            finally
            {
                if (hwp != null)
                {
                    try
                    {
                        // call Quit safely
                        try { hwp.Quit(); } catch { }
                    }
                    finally
                    {
                        try
                        {
                            while (Marshal.ReleaseComObject(hwp) > 0) { }
                        }
                        catch { }
                    }
                }
            }
        }

        public T Invoke<T>(Func<HwpObject, T> func, int timeoutMs = Timeout.Infinite)
        {
            var wi = new WorkItem();
            wi.Func = (h) => func(h);
            queue.Add(wi);
            bool signaled = wi.Done.Wait(timeoutMs);
            if (!signaled) throw new TimeoutException("HwpWorker Invoke timed out");
            if (wi.Error != null) throw new InvalidOperationException("Worker error", wi.Error);
            return (T)wi.Result;
        }

        public void Invoke(Action<HwpObject> action, int timeoutMs = Timeout.Infinite)
        {
            Invoke<object>(h => { action(h); return null; }, timeoutMs);
        }

        public void Dispose()
        {
            running = false;
            try { queue.CompleteAdding(); } catch { }
            try { if (thread != null && thread.IsAlive) thread.Join(2000); } catch { }
            queue.Dispose();
        }
    }
}
