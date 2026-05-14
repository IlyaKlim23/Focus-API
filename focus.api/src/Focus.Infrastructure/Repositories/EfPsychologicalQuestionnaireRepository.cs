using Focus.Database;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Focus.Infrastructure.Repositories;

public class EfPsychologicalQuestionnaireRepository(FocusDbContext db) : IPsychologicalQuestionnaireRepository
{
    public async Task<IReadOnlyList<PsychologicalQuestionnaire>> GetActiveAsync(CancellationToken ct = default) =>
        await db.PsychologicalQuestionnaires.Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(ct);

    public async Task<IReadOnlyList<PsychologicalQuestionnaireQuestion>> GetQuestionsAsync(Guid questionnaireId, CancellationToken ct = default) =>
        await db.PsychologicalQuestionnaireQuestions
            .Where(x => x.QuestionnaireId == questionnaireId)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(ct);

    public Task<UserQuestionnaireSchedule?> GetUserScheduleAsync(Guid userId, Guid questionnaireId, CancellationToken ct = default) =>
        db.UserQuestionnaireSchedules.FirstOrDefaultAsync(x => x.UserId == userId && x.QuestionnaireId == questionnaireId, ct);

    public async Task<IReadOnlyList<UserQuestionnaireSchedule>> GetUserSchedulesAsync(Guid userId, CancellationToken ct = default) =>
        await db.UserQuestionnaireSchedules.Where(x => x.UserId == userId).ToListAsync(ct);

    public async Task<UserQuestionnaireSchedule> UpsertUserScheduleAsync(UserQuestionnaireSchedule schedule, CancellationToken ct = default)
    {
        var existing = await GetUserScheduleAsync(schedule.UserId, schedule.QuestionnaireId, ct);
        if (existing == null)
        {
            db.UserQuestionnaireSchedules.Add(schedule);
        }
        else
        {
            existing.Cadence = schedule.Cadence;
            existing.NextDueAtUtc = schedule.NextDueAtUtc;
            existing.IsEnabled = schedule.IsEnabled;
            existing.UpdatedAt = schedule.UpdatedAt;
        }

        await db.SaveChangesAsync(ct);
        return existing ?? schedule;
    }

    public async Task AddResponseAsync(QuestionnaireResponse response, IReadOnlyList<QuestionnaireResponseItem> items, CancellationToken ct = default)
    {
        db.QuestionnaireResponses.Add(response);
        db.QuestionnaireResponseItems.AddRange(items);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<QuestionnaireResponse>> GetResponsesAsync(Guid userId, Guid questionnaireId, int take, CancellationToken ct = default) =>
        await db.QuestionnaireResponses
            .Where(x => x.UserId == userId && x.QuestionnaireId == questionnaireId)
            .OrderByDescending(x => x.SubmittedAtUtc)
            .Take(take)
            .ToListAsync(ct);
}
