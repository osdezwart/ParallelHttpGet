using System;
using System.Collections.Generic;
using System.Threading;

namespace ParallelHttpGet
{
    public class ThreadFactory
    {
        readonly int _workersCount;
        private readonly List<Thread> _threads = new List<Thread>();

        public ThreadFactory(int threadCount, int workCount, Action<int, int, string> action)
        {
            _workersCount = threadCount;

            int totalWorkLoad = workCount;
            int workLoad = totalWorkLoad / _workersCount;
            int extraLoad = totalWorkLoad % _workersCount;

            for (int i = 0; i < _workersCount; i++)
            {
                int min, max;
                if (i < (_workersCount - 1))
                {
                    min = (i * workLoad);
                    max = ((i * workLoad) + workLoad - 1);
                }
                else
                {
                    min = (i * workLoad);
                    max = (i * workLoad) + (workLoad - 1 + extraLoad);
                }
                string name = "Working Thread#" + i;

                var worker = new Thread(() => action(min, max, name)) {Name = name};
                _threads.Add(worker);
            }
        }

        public void StartWorking()
        {
            foreach (var thread in _threads)
            {
                thread.Start();
            }

            foreach (var thread in _threads)
            {
                thread.Join();
            }
        }
    }
}