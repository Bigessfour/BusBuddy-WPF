using BusBuddy.Core.Models;
using System;
using System.Collections.Generic;

namespace BusBuddy.Core.Services.Interfaces
{
    public interface IBusBuddyScheduleDataProvider
    {
        bool IsDirty { get; set; }
        List<IScheduleAppointment> GetScheduleForDay(DateTime selectedDate);
        List<IScheduleAppointment> GetSchedule(DateTime start, DateTime end);
        IScheduleAppointment NewScheduleAppointment();
        void AddItem(IScheduleAppointment appointment);
        void RemoveItem(IScheduleAppointment appointment);
        void CommitChanges();
    }
}
