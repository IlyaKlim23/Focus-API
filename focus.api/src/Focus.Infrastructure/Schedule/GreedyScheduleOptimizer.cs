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
        DateTime dayEnd)
    {
        var result = new List<ScheduledTask>();
        var sortedTasks = tasks.OrderByDescending(t => t.Priority).ThenBy(t => t.DueDate ?? DateTime.MaxValue).ToList();
        var usedSlots = new HashSet<DateTime>();

        foreach (var task in sortedTasks)
        {
            var duration = task.EstimatedMinutes > 0 ? task.EstimatedMinutes : 60;
            var bestSlot = FindBestSlot(productivityScores, dayStart, dayEnd, duration, usedSlots);
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
        HashSet<DateTime> usedSlots)
    {
        DateTime? best = null;
        double bestScore = -1;
        var current = new DateTime(dayStart.Year, dayStart.Month, dayStart.Day, dayStart.Hour, 0, 0, dayStart.Kind);

        while (current.AddMinutes(durationMinutes) <= dayEnd)
        {
            if (usedSlots.Contains(current)) { current = current.AddHours(1); continue; }
            var score = scores.GetValueOrDefault(current, 0.5);
            if (score > bestScore) { bestScore = score; best = current; }
            current = current.AddHours(1);
        }

        return best;
    }
}
