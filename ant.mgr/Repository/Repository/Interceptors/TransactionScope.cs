using System.Transactions;
using Autofac;
using Autofac.Annotation;
using Autofac.Aspect;
using Castle.DynamicProxy;

namespace Repository.Interceptors
{
    using System;
    using System.Threading.Tasks;


    /// <summary>
    /// 事物
    /// </summary>
    public class EnableTransactionScope : PointcutAttribute
    {
        /// <summary>
        /// 无参数返回拦截器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="invocation"></param>
        /// <param name="proceed"></param>
        /// <returns></returns>
        public override async Task InterceptAsync(IComponentContext context, IInvocation invocation, Func<IInvocation, Task> proceed)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await proceed(invocation);
                
                if(Transaction.Current.TransactionInformation.Status == TransactionStatus.Active)
                    scope.Complete();
            }
        }

        /// <summary>
        /// 有参数返回拦截器
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="context"></param>
        /// <param name="invocation"></param>
        /// <param name="proceed"></param>
        /// <returns></returns>
        public override async Task<TResult> InterceptAsync<TResult>(IComponentContext context, IInvocation invocation, Func<IInvocation, Task<TResult>> proceed)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var r = await proceed(invocation);

                if(Transaction.Current.TransactionInformation.Status == TransactionStatus.Active)
                    scope.Complete();

                return r;
            }
        }
    }
}