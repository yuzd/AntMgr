//-----------------------------------------------------------------------
// <copyright file="TaskHelper.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Async
{
    /// <summary>
    /// 
    /// </summary>
    public class AsyncHelper
    {
        #region Helpers

        public static Task GetActionTask(Action action, CancellationToken token)
        {
            var task = new Task(action, token);

            task.Start();

            return task;
        }

        public  static Task<T> GetTask<T>(Func<T> func, CancellationToken token)
        {
            var task = new Task<T>(func, token);

            task.Start();

            return task;
        }

        #endregion
    }
}