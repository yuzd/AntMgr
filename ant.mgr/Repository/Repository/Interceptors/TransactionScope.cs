using System.Transactions;
using Autofac.Aspect;
using System.Threading.Tasks;
using NLog;


namespace Repository.Interceptors
{

    /// <summary>
    /// 事物切面，所有是Respository结尾的容器对象的所有UseTransaction开头的方法都会走进这个切面
    /// </summary>
    [Pointcut(Class = "*Respository",Method = "UseTransaction*")]
    public class EnableTransactionScope
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        /// <summary>
        /// 事物拦截器
        /// </summary>
        /// <param name="aspectContext"></param>
        /// <returns></returns>
        [Around]
        public async Task RunWithTransaction(PointcutContext aspectContext)
        {
            logger.Debug($"start transactionScope on `{aspectContext.InvocationMethod.DeclaringType.FullName + "." + aspectContext.InvocationMethod.Name}`");
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await aspectContext.Proceed();

                if (Transaction.Current.TransactionInformation.Status == TransactionStatus.Active)
                {
                    scope.Complete();
                    logger.Debug($"submit transactionScope on `{aspectContext.InvocationMethod.DeclaringType.FullName + "." + aspectContext.InvocationMethod.Name}`");
                }
            }   
        }
    }
}