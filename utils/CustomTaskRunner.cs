using kun28.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace kun28.utils
{
    public class CustomTaskRunner
    {
        private ManualResetEventSlim _pauseEvent = new ManualResetEventSlim(true);
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public delegate Task TaskRunEvent();

        public TaskRunEvent TaskRun;

        public List<Func<Task<bool>>> taskList = new List<Func<Task<bool>>>();

        public async Task StartAsyncOperation()
        {
            try
            {
                for (int i = 0; i < taskList.Count; ++i)
                {
                    _pauseEvent.Wait(_cts.Token);
                    bool cnt = await taskList[i].Invoke();
                    if (_cts.IsCancellationRequested || !cnt) break;
                    await checkPause();
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        public async Task StartAsyncOperation(LinkTask task)
        {
            try
            {
                LinkTask taskRun = task;

                while (true)
                {
                    _pauseEvent.Wait(_cts.Token);
                    taskRun = await taskRun.run();
                    if (taskRun.fileName.Equals("tansuo_0")) break;
                    await checkPause();
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        public void Pause()
        {
            _pauseEvent.Reset();
        }

        public void Resume()
        {
            _pauseEvent.Set();
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        public async Task checkPause()
        {
            await Task.Run(() =>
            {
                try
                {
                    _pauseEvent.Wait(_cts.Token);
                }
                catch (OperationCanceledException)
                {

                }
            });
        }

        public bool checkCancel()
        {
            return _cts.IsCancellationRequested;
        }

        public async Task run()
        {
            await TaskRun?.Invoke();
        }

        public void release()
        {
            _pauseEvent.Dispose();
            _cts.Dispose();
        }

    }
}