using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_LP.Infrastructure
{
    public static class ConcurrentService
    {
        private readonly static object lockRS = new object();

        public static void ExecuteSafe(Action toDo)
        {
            lock (lockRS)
            {
                toDo();
            }
        }
    }
}