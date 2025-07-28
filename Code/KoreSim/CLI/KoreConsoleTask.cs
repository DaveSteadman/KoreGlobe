using System;
using System.Threading;
using System.Threading.Tasks;


namespace XX;

// KoreTask - A class representing a task that can be executed asynchronously and cancelled if needed
// Complete flag allows callto to check if the task is complete

public abstract class KoreConsoleTask
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    public bool TaskCompleted { get; private set; }

    protected KoreConsoleTask()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        TaskCompleted = false;
    }

    public void StartExecution()
    {
        Task.Run(async () => await ExecuteAsync(), _cancellationTokenSource.Token).ContinueWith(task =>
        {
            TaskCompleted = true; // Set the flag once the task is complete

            if (task.IsCanceled)
            {
                // Handle task cancellation here
            }
            else if (task.IsFaulted)
            {
                // Handle exceptions from the task, if any
            }
            else
            {
                // Handle successful completion or perform further actions if needed
            }
        }, TaskContinuationOptions.ExecuteSynchronously);
    }

    public void Cancel()
    {
        _cancellationTokenSource.Cancel();
    }

    public async Task ExecuteAsync()
    {
        try
        {
            // No need to call _cancellationTokenSource.Token.ThrowIfCancellationRequested() here if the next operation checks the token.
            await ExecuteCoreAsync(_cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            // Handle the cancellation (cleanup, logging, etc.)
        }
        catch (Exception)
        {
            // Log the exception here in a thread-safe manner
            throw; // Rethrow or handle the exception as needed
        }
        finally
        {
            TaskCompleted = true; // Ensure the flag is set even if an exception occurs
        }
    }

    protected abstract Task ExecuteCoreAsync(CancellationToken cancellationToken);
}
