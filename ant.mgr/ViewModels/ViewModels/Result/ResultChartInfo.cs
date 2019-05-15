//-----------------------------------------------------------------------
// <copyright file="ResultChartInfo.cs" company="Company">
// Copyright (C) Company. All Rights Reserved.
// </copyright>
// <author>nainaigu</author>
// <summary></summary>
//-----------------------------------------------------------------------
namespace ViewModels.Result
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// 图表 结果返回
    /// </summary>
    public class ResultChartInfo<T> where T :ResultChatBase, new()
    {
        #region Fields
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public List<T> DayData { get; set; }

        public List<T> WeekData { get; set; }

        public List<T> MonthData { get; set; }

        public List<T> QuarterData { get; set; }

        public List<T> YearData { get; set; }

        public Decimal Count { get; set; }
        #endregion

        #region Properties
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ResultChartInfo class.
        /// </summary>
        public ResultChartInfo(DateTime startTime, DateTime endTime, List<T> dayData, List<string> groupList)
        {
            this.DayData = dayData ?? new List<T>();
            this.StartTime = startTime;
            this.EndTime = endTime;
            FillDayData(startTime, endTime, groupList);
            FillWeekData();
            FillMonthData();
            FillQuarterData();
            FillYearData();
            this.Count = DayData.Select(r => r.Value).Sum();
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        private void FillDayData(DateTime startTime, DateTime endTime, List<string> groupList)
        {
            List<string> dayDataTimeRange = DayData.Select(r => r.Name).Distinct().ToList();
            while (startTime <= endTime && groupList.Count > 0)
            {
                var startTimeStr = startTime.ToString("yyyyMMdd");
                if (!dayDataTimeRange.Contains(startTimeStr))//如果数据库不存在该天数据，按组生成默认数据
                {
                    foreach (var item in groupList)
                    {
                        T currentChartCsm = new T
                        {
                            Name = startTimeStr,
                            GroupName = item,
                            Value = 0
                        };
                        DayData.Add(currentChartCsm);
                    }
                }
                else
                {
                    foreach (var item in groupList)//如果数据库存在该天数据在确认是否每一组数据都存在
                    {
                        var count = DayData.Where(r => r.Name.Equals(startTimeStr) && r.GroupName.Equals(item)).ToList().Count;
                        if (count < 1)
                        {
                            T currentChartCsm = new T
                            {
                                Name = startTimeStr,
                                GroupName = item,
                                Value = 0
                            };
                            DayData.Add(currentChartCsm);
                        }

                    }


                }
                startTime = startTime.AddDays(1);
            }
            this.DayData = this.DayData.OrderBy(r => r.Name).ToList();
        }

        private void FillWeekData()
        {
            this.WeekData = (from item in DayData
                             let se = getWeekStartAndEnd(item.Name)
                             let groupName = item.GroupName
                             group item by new { se, groupName }
                                 into g
                             select new T()
                             {
                                 Name = g.Key.se,
                                 GroupName = g.Key.groupName,
                                 Value = g.Select(r => r.Value).Sum()
                             }).ToList();
        }

        private void FillMonthData()
        {
            this.MonthData = (from item in DayData
                              let m = getMonthStr(item.Name)
                              let groupName = item.GroupName
                              group item by new { m, groupName }
                                  into g
                              select new T()
                              {
                                  Name = g.Key.m,
                                  GroupName = g.Key.groupName,
                                  Value = g.Select(r => r.Value).Sum()
                              }).ToList();
        }

        private void FillQuarterData()
        {
            this.QuarterData = (from item in DayData
                                let q = getQuarterStr(item.Name)
                                let groupName = item.GroupName
                                group item by new { y = q, groupName }
                                    into g
                                select new T()
                                {
                                    Name = g.Key.y,
                                    GroupName = g.Key.groupName,
                                    Value = g.Select(r => r.Value).Sum()
                                }).ToList();
        }
        private void FillYearData()
        {
            this.YearData = (from item in DayData
                             let y = getYearStr(item.Name)
                             let groupName = item.GroupName
                             group item by new { y, groupName }
                                 into g
                             select new T()
                             {
                                 Name = g.Key.y,
                                 GroupName = g.Key.groupName,
                                 Value = g.Select(r => r.Value).Sum()
                             }).ToList();
        }

        private string getMonthStr(string daytimeStr)
        {
            DateTime daytime = DateTime.ParseExact(daytimeStr, "yyyyMMdd", null);
            return String.Concat(daytime.Year.ToString(), "年", daytime.Month.ToString(), "月");
        }

        private string getQuarterStr(string daytimeStr)
        {
            DateTime daytime = DateTime.ParseExact(daytimeStr, "yyyyMMdd", null);
            DateTime startQuarter = daytime.AddMonths(0 - (daytime.Month - 1) % 3).AddDays(1 - daytime.Day);  //本季度初  
            DateTime endQuarter = startQuarter.AddMonths(3).AddDays(-1);  //本季度末 
            return string.Concat(startQuarter.ToString("yyyyMMdd"), "-", endQuarter.ToString("yyyyMMdd"));
        }
        private string getYearStr(string daytimeStr)
        {
            DateTime daytime = DateTime.ParseExact(daytimeStr, "yyyyMMdd", null);
            return String.Concat(daytime.Year.ToString(), "年");
        }
        private string getWeekStartAndEnd(string daytimeStr)
        {
            DateTime daytime = DateTime.ParseExact(daytimeStr, "yyyyMMdd", null);
            var dayNum = Convert.ToInt32(daytime.DayOfWeek.ToString("d"));
            var firstDayOfWeek = daytime.AddDays(-dayNum + 1);
            var lastDayOfWeek = daytime.AddDays(7 - dayNum);
            return string.Concat(firstDayOfWeek.ToString("yyyyMMdd"), "-", lastDayOfWeek.ToString("yyyyMMdd"));
        }
        #endregion

        #region Interfaces Methods
        #endregion
    }
}