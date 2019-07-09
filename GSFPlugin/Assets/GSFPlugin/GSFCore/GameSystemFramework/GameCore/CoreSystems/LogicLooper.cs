using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameSystem.GameCore
{
    public class LogicLooper
    {
        private bool running;
        private bool isClosed;
        private Task loopTask;

        public float TargetFPS = 30f;

        public delegate void OnUpdatedEventHandler(TimeSpan deltaTime);
        public delegate void OnCloseEventHandler();

        public OnUpdatedEventHandler OnUpdated;
        public OnCloseEventHandler OnClose;

        public LogicLooper(float fps = 30f)
        {
            TargetFPS = fps;
        }

        public void Start()
        {
            loopTask = Task.Run(Loop);
        }

        public void Stop()
        {
            running = false;
        }

        private void Loop()
        {
            DateTime curr_time = DateTime.UtcNow;
            DateTime last_time = curr_time;
            running = true;
            while (running)
            {
                curr_time = DateTime.UtcNow;
                TimeSpan deltaTime;
                // caculate time span between current and last time
                if ((deltaTime = curr_time - last_time).TotalMilliseconds > 0)
                {
                    OnUpdated.Invoke(deltaTime);
                }
                // correct time into fps
                float TargetSecond = 1f / TargetFPS;
                int delayTime = (int)(TargetSecond - deltaTime.TotalSeconds) * 1000;
                // force release thread 5 ms
                if (delayTime > 5)
                    Thread.Sleep(delayTime);
                else
                    Thread.Sleep(5);
                last_time = curr_time;
            }
            if (isClosed)
                Task.Run(CloseSafely);
        }

        public void Close()
        {
            if (isClosed)
                return;
            if (!running)
                CloseSafely();
            running = false;    // set loop stopped
            isClosed = true;
            loopTask.Wait();
        }

        private void CloseSafely()
        {
            OnClose.Invoke();
        }
    }
}