using System;
using System.Collections.Generic;
using System.Threading;

namespace ReaderWriterLock
{
    public class RwLock : IRwLock
    {
        private int readersCount;
        private int writersCount;
        private readonly object readWriteLocker = new object();
        
        public void ReadLocked(Action action)
        {
            lock (readWriteLocker)
            {
                while (writersCount > 0)
                    Monitor.Wait(readWriteLocker);
                Interlocked.Increment(ref readersCount);
            }
            action();
            PassIfZero(Interlocked.Decrement(ref readersCount));
        }

        public void WriteLocked(Action action)
        {
            lock (readWriteLocker)
            {
                while (readersCount + writersCount > 0)
                    Monitor.Wait(readWriteLocker);
                Interlocked.Increment(ref writersCount);
            }
            action();
            PassIfZero(Interlocked.Decrement(ref writersCount));
        }

        private void PassIfZero(int value)
        {
            if (value != 0) return;
            lock (readWriteLocker)
                Monitor.PulseAll(readWriteLocker);
        }
    }
}