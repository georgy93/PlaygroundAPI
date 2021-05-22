namespace Playground.Utils.Extensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class TaskExtensions
    {
        public static async Task WaitAsync(this Task originalTask, CancellationToken cancellationToken)
        {
            if (originalTask.IsCompleted)
                return;

            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var completedTask = await Task.WhenAny(originalTask, Task.Delay(Timeout.Infinite, tokenSource.Token));
            if (completedTask != originalTask)
                throw new OperationCanceledException();

            tokenSource.Cancel();

            // unwrap the result
            await originalTask;
        }
    }
}