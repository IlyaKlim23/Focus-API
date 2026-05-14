using Focus.Domain.Interfaces;

namespace Focus.Infrastructure.Schedule;

/// <summary>
/// Жадный алгоритм: высокоприоритетные задачи в слоты с максимальной продуктивностью.
/// </summary>
public class GreedyScheduleOptimizer : IScheduleOptimizer
{
    public IReadOnlyList<ScheduledTask> Optimize(
        IReadOnlyList<TaskInput> tasks,
        IReadOnlyDictionary<DateTime, double> productivityScores,
        DateTime dayStart,
        DateTime dayEnd,
        IReadOnlyList<DailyUnavailableWindow>? unavailableWindows = null)
    {
        var result = new List<ScheduledTask>();
        var sortedTasks = tasks.OrderByDescending(t => t.Priority).ThenBy(t => t.DueDate ?? DateTime.MaxValue).ToList();
        var usedSlots = new HashSet<DateTime>();

        foreach (var task in sortedTasks)
        {
            var duration = task.EstimatedMinutes > 0 ? task.EstimatedMinutes : 60;
            var bestSlot = FindBestSlot(
                productivityScores,
                dayStart,
                dayEnd,
                duration,
                task.DueDate,
                usedSlots,
                unavailableWindows);
            if (bestSlot.HasValue)
            {
                result.Add(new ScheduledTask(task.Id, bestSlot.Value, duration));
                for (var i = 0; i < duration; i += 60)
                    usedSlots.Add(bestSlot.Value.AddMinutes(i));
            }
        }

        return result;
    }

    private static DateTime? FindBestSlot(
        IReadOnlyDictionary<DateTime, double> scores,
        DateTime dayStart,
        DateTime dayEnd,
        int durationMinutes,
        DateTime? dueDate,
        HashSet<DateTime> usedSlots,
        IReadOnlyList<DailyUnavailableWindow>? unavailableWindows)
    {
        DateTime? best = null;
        double bestScore = -1;
        var current = new DateTime(dayStart.Year, dayStart.Month, dayStart.Day, dayStart.Hour, 0, 0, dayStart.Kind);

        while (current.AddMinutes(durationMinutes) <= dayEnd)
        {
            if (usedSlots.Contains(current)) { current = current.AddHours(1); continue; }
            if (!IsWindowAvailable(current, durationMinutes, unavailableWindows)) { current = current.AddHours(1); continue; }
            if (dueDate.HasValue && current.AddMinutes(durationMinutes) > dueDate.Value) { current = current.AddHours(1); continue; }
            var score = scores.GetValueOrDefault(current, 0.5);
            if (score > bestScore) { bestScore = score; best = current; }
            current = current.AddHours(1);
        }

        return best;
    }

    private static bool IsWindowAvailable(
        DateTime slotStart,
        int durationMinutes,
        IReadOnlyList<DailyUnavailableWindow>? unavailableWindows)
    {
        if (unavailableWindows == null || unavailableWindows.Count == 0) return true;

        // Недоступные окна задаются пользователем в локальном времени
        var localSlotStart = slotStart.Kind == DateTimeKind.Utc ? slotStart.ToLocalTime() : slotStart;
        var localSlotEnd = localSlotStart.AddMinutes(durationMinutes);
        foreach (var window in unavailableWindows)
        {
            var from = localSlotStart.Date.AddMinutes(window.FromMinute);
            var to = localSlotStart.Date.AddMinutes(window.ToMinute);
            if (window.FromMinute > window.ToMinute)
            {
                // Окно через полночь: например 22:00-06:00
                // Проверяем две версии окна:
                // 1) текущий день: [22:00 today, 06:00 tomorrow)
                // 2) предыдущий день: [22:00 yesterday, 06:00 today)
                var overnightTo = to.AddDays(1);
                if (localSlotStart < overnightTo && localSlotEnd > from)
                    return false;

                var prevFrom = from.AddDays(-1);
                var prevTo = to;
                if (localSlotStart < prevTo && localSlotEnd > prevFrom)
                    return false;
            }
            else
            {
                if (localSlotStart < to && localSlotEnd > from)
                    return false;
            }
        }

        return true;
    }
}
