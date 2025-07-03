using Bus_Buddy.Data;
using Bus_Buddy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bus_Buddy.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly BusBuddyDbContext _context;

        public ScheduleService(BusBuddyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Activity>> GetAllSchedulesAsync()
        {
            return await _context.Activities
                .Include(a => a.Vehicle)
                .Include(a => a.Driver)
                .Include(a => a.Route)
                .OrderBy(a => a.ActivityDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<Activity> GetScheduleByIdAsync(int id)
        {
            return await _context.Activities
                .Include(a => a.Vehicle)
                .Include(a => a.Driver)
                .Include(a => a.Route)
                .FirstOrDefaultAsync(a => a.ActivityId == id) ?? throw new InvalidOperationException($"Activity with ID {id} not found.");
        }

        public async Task<List<Activity>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Activities
                .Include(a => a.Vehicle)
                .Include(a => a.Driver)
                .Include(a => a.Route)
                .Where(a => a.ActivityDate >= startDate && a.ActivityDate <= endDate)
                .OrderBy(a => a.ActivityDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<Activity> AddScheduleAsync(Activity schedule)
        {
            // Validate schedule conflict before adding
            if (await ValidateScheduleConflictAsync(schedule))
            {
                throw new InvalidOperationException("Schedule conflict detected. The vehicle or driver is already assigned during this time.");
            }

            _context.Activities.Add(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<Activity> UpdateScheduleAsync(Activity schedule)
        {
            // Validate schedule conflict before updating (excluding current schedule)
            var existingConflicts = await _context.Activities
                .Where(a => a.ActivityId != schedule.ActivityId &&
                           a.ActivityDate == schedule.ActivityDate &&
                           ((a.VehicleId == schedule.VehicleId) ||
                            (a.DriverId == schedule.DriverId)) &&
                           ((a.StartTime <= schedule.StartTime && a.EndTime >= schedule.StartTime) ||
                            (a.StartTime <= schedule.EndTime && a.EndTime >= schedule.EndTime) ||
                            (schedule.StartTime <= a.StartTime && schedule.EndTime >= a.EndTime)))
                .AnyAsync();

            if (existingConflicts)
            {
                throw new InvalidOperationException("Schedule conflict detected. The vehicle or driver is already assigned during this time.");
            }

            _context.Activities.Update(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _context.Activities.FindAsync(id);
            if (schedule == null)
                return false;

            _context.Activities.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Activity>> GetSchedulesByVehicleAsync(string vehicleId)
        {
            if (int.TryParse(vehicleId, out int parsedVehicleId))
            {
                return await _context.Activities
                    .Include(a => a.Vehicle)
                    .Include(a => a.Driver)
                    .Include(a => a.Route)
                    .Where(a => a.VehicleId == parsedVehicleId)
                    .OrderBy(a => a.ActivityDate)
                    .ThenBy(a => a.StartTime)
                    .ToListAsync();
            }
            return new List<Activity>();
        }

        public async Task<List<Activity>> GetSchedulesByDriverAsync(string driverName)
        {
            return await _context.Activities
                .Include(a => a.Vehicle)
                .Include(a => a.Driver)
                .Include(a => a.Route)
                .Where(a => a.Driver.DriverName == driverName)
                .OrderBy(a => a.ActivityDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<bool> ValidateScheduleConflictAsync(Activity schedule)
        {
            // Check for vehicle conflicts
            var vehicleConflict = await _context.Activities
                .Where(a => a.VehicleId == schedule.VehicleId &&
                           a.ActivityDate == schedule.ActivityDate &&
                           ((a.StartTime <= schedule.StartTime && a.EndTime >= schedule.StartTime) ||
                            (a.StartTime <= schedule.EndTime && a.EndTime >= schedule.EndTime) ||
                            (schedule.StartTime <= a.StartTime && schedule.EndTime >= a.EndTime)))
                .AnyAsync();

            if (vehicleConflict) return true;

            // Check for driver conflicts
            var driverConflict = await _context.Activities
                .Where(a => a.DriverId == schedule.DriverId &&
                           a.ActivityDate == schedule.ActivityDate &&
                           ((a.StartTime <= schedule.StartTime && a.EndTime >= schedule.StartTime) ||
                            (a.StartTime <= schedule.EndTime && a.EndTime >= schedule.EndTime) ||
                            (schedule.StartTime <= a.StartTime && schedule.EndTime >= a.EndTime)))
                .AnyAsync();

            if (driverConflict) return true;

            return false;
        }
    }
}
