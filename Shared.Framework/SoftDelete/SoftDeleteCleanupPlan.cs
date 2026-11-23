namespace Shared.Framework.SoftDelete;

public sealed class SoftDeleteCleanupPlan
{
    public SoftDeleteCleanupPlan(IEnumerable<SoftDeleteCleanupStep> steps)
    {
        Steps = steps.ToList();
    }

    public IReadOnlyList<SoftDeleteCleanupStep> Steps { get; }
}
