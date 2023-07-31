namespace Leavetown.Client.Utilities
{
    public class Debouncer
    {
        private CancellationTokenSource? _cancelTokenSource;

        public async Task Debounce(Func<Task> func, int milliseconds = 500)
        {
            _cancelTokenSource?.Cancel();
            _cancelTokenSource?.Dispose();

            _cancelTokenSource = new CancellationTokenSource();

            await Task.Delay(milliseconds, _cancelTokenSource.Token);

            await func();
        }
    }
}
