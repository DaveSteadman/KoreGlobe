// <fileheader>

using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace KoreCommon;


public abstract class KoreBackgroundTaskRunner<T> where T : class
{
    private KoreThreadSafeValue<T> resultValue = new KoreThreadSafeValue<T>();
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public void Start()
    {
        var cancellationToken = cancellationTokenSource.Token;
        Task.Run(async () => await PerformBackgroundWork(cancellationToken), cancellationToken)
            .ContinueWith(task =>
            {
                if (task.IsCanceled) return;
                if (task.IsFaulted) return;

                resultValue.Value = task.Result;
                FinalizeWithResult(task.Result);
            }, TaskScheduler.Default);
    }

    public void Cancel()
    {
        cancellationTokenSource.Cancel();
    }

    protected abstract Task<T> PerformBackgroundWork(CancellationToken cancellationToken);

    protected abstract void FinalizeWithResult(T result);

    protected T? Result => resultValue.Value;

    protected bool IsResultAvailable => resultValue.IsSet;
}
