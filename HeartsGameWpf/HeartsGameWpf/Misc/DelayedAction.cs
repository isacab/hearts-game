using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace HeartsGameWpf.Misc
{
    class DelayedAction
    {
        private CancellationTokenSource source = new CancellationTokenSource();

        public DelayedAction(Action action = null, int delay = 0)
        {
            this.Delay = delay;
            this.Action = action;
        }

        public int Delay { get; set; } // in milliseconds
        public Action Action { get; set; }
        public bool IsFinished { get; set; }
        public bool IsExecuting { get; set; }

        public void Cancel()
        {
            if (source != null && !IsFinished)
                source.Cancel();
        }

        public async void Execute()
        {
            if (Action == null)
                throw new NullReferenceException("Action is null");

            if (IsExecuting)
                throw new InvalidOperationException("Already execution");

            if (IsFinished)
                throw new InvalidOperationException("Action is finished");

            IsExecuting = true;

            CancellationToken token = source.Token;

            try
            {
                await Task.Delay(Delay, token);
            }
            catch (TaskCanceledException) { }
            finally
            {
                if(!token.IsCancellationRequested)
                {
                    Action();
                }
                IsExecuting = false;
                IsFinished = true;
            }
        }
    }
}
