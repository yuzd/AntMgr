using System.Transactions;
using Autofac.Aspect;
using System.Threading.Tasks;
using NLog;


namespace Repository.Interceptors
{

    /// <summary>
    /// 事物
    /// </summary>
    public class EnableTransactionScope : PointcutAttribute
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 事物拦截器
        /// </summary>
        /// <param name="aspectContext"></param>
        /// <param name="_next"></param>
        /// <returns></returns>
        public override async Task OnInvocation(AspectContext aspectContext, AspectDelegate _next)
        {
            logger.Debug($"start transactionScope on `{aspectContext.InvocationContext.TargetType.FullName + "." + aspectContext.InvocationContext.Method.Name}`");
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _next(aspectContext);

                if (Transaction.Current.TransactionInformation.Status == TransactionStatus.Active)
                {
                    scope.Complete();
                    logger.Debug($"submit transactionScope on `{aspectContext.InvocationContext.TargetType.FullName + "." + aspectContext.InvocationContext.Method.Name}`");
                }
            }
        }
    }
}