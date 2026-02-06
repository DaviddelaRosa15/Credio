using System.Runtime.ExceptionServices;

namespace Credio.Core.Application.Extensions;

public static class TaskExtensions
{
    public static async Task<TResult[]> WhenAll<TResult>(this IEnumerable<Task<TResult>> source)
    {
        Task<TResult[]> task = Task.WhenAll(source);

        try
        {
            return await task.ConfigureAwait(false);

        }
        catch
        {
            if (task.Exception is not null) ExceptionDispatchInfo.Capture(task.Exception).Throw();

            throw;
        }
    }
}