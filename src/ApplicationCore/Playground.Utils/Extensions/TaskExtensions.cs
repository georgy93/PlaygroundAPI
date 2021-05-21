namespace Playground.Utils.Extensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class TaskExtensions
    {
        public static async Task WaitAsync(this Task task, CancellationToken cancellationToken)
        {
            if (task.IsCompleted)
                return;

            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var completedTask = await Task.WhenAny(task, Task.Delay(Timeout.Infinite, tokenSource.Token));
            if (completedTask != task)
                throw new OperationCanceledException();

            tokenSource.Cancel();

            await task;
        }
    }
}