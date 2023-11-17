namespace Playground.Utils.Extensions;

public static class TaskExtensions
{
    public static async Task WaitAsync(this Task originalTask, CancellationToken cancellationToken)
    {
        if (originalTask.IsCompleted)
            return;

        using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var completedTask = await Task.WhenAny(originalTask, Task.Delay(Timeout.Infinite, tokenSource.Token));
        if (completedTask == originalTask)
        {
            tokenSource.Cancel(); // cancel the delay task

            await originalTask; // unwrap the result
        }

        throw new OperationCanceledException();
    }
}