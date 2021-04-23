using System;
using System.Collections.Generic;
using System.Threading;

namespace ReaderWriterLock
{
    public class RwLock : IRwLock
    {
        private long changers = 0;
        private long writerBit = 1L << (sizeof(long) * 8 - 2);
        
        public void ReadLocked(Action action)
        {
            if (Interlocked.Increment(ref changers) >= writerBit)
                while (changers >= writerBit) {}
            
            action();
            
            Interlocked.Decrement(ref changers);
        }

        public void WriteLocked(Action action)
        {
            while (Interlocked.CompareExchange(ref changers, writerBit, 0) != 0) {}
            
            action();
            
            Interlocked.Add(ref changers, -writerBit);
        }
    }
}