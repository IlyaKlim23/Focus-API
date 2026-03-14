using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;

namespace Focus.Application.Services;

public class DailyNoteService(IDailyNoteRepository repository, INlpAnalyzer nlpAnalyzer) : IDailyNoteService
{
    public async Task<DailyNoteDto?> GetByDateAsync(Guid userId, DateOnly date, CancellationToken ct = default)
    {
        var note = await repository.GetByUserAndDateAsync(userId, date, ct);
        return note == null ? null : MapToDto(note);
    }

    public async Task<DailyNoteDto> CreateOrUpdateAsync(Guid userId, DateOnly date, CreateDailyNoteRequest request, CancellationToken ct = default)
    {
        var existing = await repository.GetByUserAndDateAsync(userId, date, ct);
        var analysis = await nlpAnalyzer.AnalyzeAsync(request.Content, ct);
        var factorsJson = analysis.ExtractedFactors.Count > 0
            ? string.Join(",", analysis.ExtractedFactors)
            : null;

        if (existing != null)
        {
            existing.Content = request.Content;
            existing.MoodScore = request.MoodScore;
            existing.EnergyLevel = request.EnergyLevel;
            existing.ExtractedFactors = factorsJson;
            await repository.UpdateAsync(existing, ct);
            return MapToDto(existing);
        }

        var note = new DailyNote
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Date = date,
            Content = request.Content,
            MoodScore = request.MoodScore,
            EnergyLevel = request.EnergyLevel,
            ExtractedFactors = factorsJson,
            CreatedAt = DateTime.UtcNow
        };
        await repository.AddAsync(note, ct);
        return MapToDto(note);
    }

    private static DailyNoteDto MapToDto(DailyNote n) => new(
        n.Id, n.Date, n.Content, n.MoodScore, n.EnergyLevel, n.ExtractedFactors, n.CreatedAt);
}
