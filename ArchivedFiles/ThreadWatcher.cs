using System;
using System.Threading;

namespace XRRemote {
    public class ThreadWatcher {
        public readonly Thread thread;
        public ThreadWatcher(Thread thread) { ;
            this.thread = thread;
        }
        ~ThreadWatcher() {
            //this.thread.Abort();
        }
        public void Start() {
            this.thread.Start();
        }
    }
}
