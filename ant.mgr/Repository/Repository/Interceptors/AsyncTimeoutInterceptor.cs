using Autofac.Annotation;
using Castle.DynamicProxy;

namespace Repository.Interceptors
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// 接口超时拦截器
    /// </summary>
    [Bean(typeof(AsyncInterceptor))]
    public class AsyncTimeoutInterceptor : AsyncInterceptor
    {

#if DEBUG
        private int timeout = 60;
#else
         private int timeout = 600;
#endif

        protected override async Task InterceptAsync(IInvocation invocation, Func<IInvocation, Task> proceed)
        {
            try
            {
                var task = proceed(invocation);
                await InterceptSyncWhenAny(task);
            }
            catch (TimeoutException ex)
            {
                throw new TimeoutException($"方法:[{invocation.Method.Name}]超时了请重试一下!", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("系统出错了", ex);
            }

        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, Func<IInvocation, Task<TResult>> proceed)
        {
            try
            {
                var task = proceed(invocation);
                return await InterceptAsyncWhenAny(task);
            }
            catch (TimeoutException ex)
            {
                throw new TimeoutException($"方法:[{invocation.Method.Name}]超时了请重试一下!", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("系统出错了", ex);
            }
        }

        #region Private
        async Task<TResult> InterceptAsyncWhenAny<TResult>(Task<TResult> tasks)
        {
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeout)).ContinueWith(_ => default(TResult));
            var completedTasks = await Task.WhenAny(tasks, timeoutTask);
            if (completedTasks != tasks)
            {
                throw new TimeoutException($"方法执行超时{timeout}秒了");
            }
            else
            {
                return await completedTasks;

            }

        }

        async Task InterceptSyncWhenAny(Task tasks)
        {
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeout));
            var completedTasks = await Task.WhenAny(tasks, timeoutTask);
            if (completedTasks != tasks)
            {
                throw new TimeoutException($"方法执行超时{timeout}秒了");
            }
            else
            {
                await completedTasks;

            }
        }
        #endregion
    }
}