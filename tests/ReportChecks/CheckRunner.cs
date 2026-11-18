internal static class CheckRunner
{
    // Keep failed checks as failures without allowing an unhandled exception to invoke Windows Error Reporting.
    public static int Run(Action checks, TextWriter? error = null, string? logPath = null)
    {
        error ??= Console.Error;
        try
        {
            checks();
            return 0;
        }
        catch (Exception exception)
        {
            error.WriteLine("FAIL Report checks failed:");
            error.WriteLine(exception);

            logPath ??= Path.Combine(AppContext.BaseDirectory, "logs", "ReportChecks.log");
            try
            {
                logPath = Path.GetFullPath(logPath);
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.WriteAllText(logPath, $"{DateTimeOffset.UtcNow:O}{Environment.NewLine}{exception}{Environment.NewLine}");
                error.WriteLine($"Details saved to: {logPath}");
            }
            catch (Exception logError)
            {
                error.WriteLine($"Could not save the error log: {logError.Message}");
            }

            return 1;
        }
    }
}
