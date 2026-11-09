using TestReport.Api.Domain.Models;

namespace TestReport.Api.Application.Interfaces;

public interface IMultiColumnPrintSettingsStore
{
    MultiColumnPrintSettings Get();
    MultiColumnPrintSettings Save(MultiColumnPrintSettings settings);
    MultiColumnPrintSettings Reset();
}
