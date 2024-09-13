﻿using NTDLS.Semaphore;

namespace TestHarness
{
    internal class TestOptimisticCriticalResource
    {
        private readonly OptimisticCriticalResource<List<string>> _listOfObjects = new();
        private readonly List<Thread> _threads = new();

        private const int _threadsToCreate = 10;
        private const int _objectsPerIteration = 10000;

        public double Execute()
        {
            Console.WriteLine("[OptimisticCriticalResource] {");
            DateTime startTime = DateTime.UtcNow;

            //Create test threads:
            for (int i = 0; i < _threadsToCreate; i++)
                _threads.Add(new Thread(ThreadProc));

            _threads.ForEach((t) => t.Start()); //Start all the threads.
            _threads.ForEach((t) => t.Join()); //Wait on all threads to exit.

            Console.WriteLine($"\tObjects: {_listOfObjects.Read(o => o.Count):n0}");
            double duration = (DateTime.UtcNow - startTime).TotalMilliseconds;
            Console.WriteLine($"\tDuration: {duration:n0}");
            Console.WriteLine("}");

            return duration;
        }

        private void ThreadProc()
        {
            _listOfObjects.Read((o) =>
            {
                foreach (var item in o)
                {
                    if (item.StartsWith(Guid.NewGuid().ToString().Substring(0, 2)))
                    {
                        //Just doing random work to make the iterator take more time.
                    }
                }
            });

            _listOfObjects.Write((o) =>
            {
                //Removing items will break the above iterator in other threads.
                o.RemoveAll(o => o.StartsWith(Guid.NewGuid().ToString().Substring(0, 2)));
            });

            _listOfObjects.Write((o) =>
            {
                //Adding items will also break the above iterator in other threads.
                for (int i = 0; i < _objectsPerIteration; i++)
                {
                    var val = Guid.NewGuid().ToString().Substring(0, 4);

                    o.Add(val);
                }
            });
        }
    }
}
