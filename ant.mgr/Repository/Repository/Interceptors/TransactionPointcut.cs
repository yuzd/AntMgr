using System;
using System.Transactions;
using System.Threading.Tasks;
using Autofac.Annotation;
using NLog;


namespace Repository.Interceptors
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method)]
    public class EnableTransactionScope:Attribute
    {
        public EnableTransactionScope()
        {
            
        }

        public EnableTransactionScope(int scopeTimeout)
        {
            this.ScopeTimeout = scopeTimeout;
        }

        public EnableTransactionScope(TransactionScopeOption scopeType)
        {
            this.TransactionScopeOption = scopeType;
        }

        public EnableTransactionScope(int scopeTimeout,TransactionScopeOption scopeType)
        {
            this.ScopeTimeout = scopeTimeout;
            this.TransactionScopeOption = scopeType;
        }

        /// <summary>
        /// 单位是秒
        /// </summary>
        public int ScopeTimeout { get; set; }

        /// <summary>
        /// 类型
        /// 默认类型为：Required 当外层有事物则用该事物 没有的话就创建
        /// RequiresNew 的话是不管外层有没有事物都会创建一个新事物
        /// Suppress 不参与任何事务
        /// </summary>
        public TransactionScopeOption TransactionScopeOption { get; set; } = TransactionScopeOption.Required;
    }

    /// <summary>
    /// 事物切面，所有是Respository结尾的容器对象的方法上打了EnableTransactionScope的会走进事物  开头的方法都会走进这个切面
    /// </summary>
    [Pointcut(Class = "*Respository",AttributeType = typeof(EnableTransactionScope))]
    public class TransactionPointcut
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        /// <summary>
        /// 事物环绕拦截器
        /// </summary>
        /// <returns></returns>
        [Around]
        public async Task RunWithTransaction(AspectContext aspectContext,AspectDelegate next, EnableTransactionScope option)
        {
            logger.Debug($"start transactionScope on `{aspectContext.TargetMethod.DeclaringType?.FullName + "." + aspectContext.TargetMethod.Name}`");

            var timeOut = option.ScopeTimeout;
            if (timeOut < 1)
            {
                timeOut = 30 * 60;//默认设置半小时
            }
            using (var scope = new TransactionScope(option.TransactionScopeOption,TimeSpan.FromSeconds(timeOut), TransactionScopeAsyncFlowOption.Enabled))
            {
                await next(aspectContext);

                if (Transaction.Current.TransactionInformation.Status == TransactionStatus.Active)
                {
                    scope.Complete();
                    logger.Debug($"submit transactionScope on `{aspectContext.TargetMethod.DeclaringType?.FullName + "." + aspectContext.TargetMethod.Name}`");
                }
            }   
        }
    }
}