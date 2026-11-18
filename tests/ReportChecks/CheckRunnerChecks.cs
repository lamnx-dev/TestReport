internal static class CheckRunnerChecks
{
    public static void Run()
    {
        static void Check(bool condition, string message)
        {
            if (!condition) throw new Exception(message);
            Console.WriteLine("PASS Runner " + message);
        }

        var directory = Path.Combine(AppContext.BaseDirectory, "runner-checks-" + Guid.NewGuid().ToString("N"));
        var logPath = Path.Combine(directory, "failure.log");
        using var error = new StringWriter();
        try
        {
            Check(CheckRunner.Run(() => { }, error, logPath) == 0 && error.ToString() == "" && !Directory.Exists(directory),
                "successful checks return zero without writing errors or logs");

            var exitCode = CheckRunner.Run(ThrowFromCheck, error, logPath);
            Check(exitCode == 1, "failed checks return nonzero instead of escaping the runner");
            var diagnostic = error.ToString();
            Check(diagnostic.Contains("InvalidOperationException: Intentional runner test failure") && diagnostic.Contains(nameof(ThrowFromCheck)),
                "console preserves the exception type, message and original stack");
            var log = File.ReadAllText(logPath);
            Check(log.Contains("Intentional runner test failure") && log.Contains(nameof(ThrowFromCheck)) && diagnostic.Contains(logPath),
                "log preserves failure details and its location is reported");

            error.GetStringBuilder().Clear();
            Check(CheckRunner.Run(ThrowFromCheck, error, directory) == 1 && error.ToString().Contains("Could not save the error log") &&
                error.ToString().Contains("Intentional runner test failure"), "unwritable log destination still returns failure and reports the original exception");
        }
        finally
        {
            if (File.Exists(logPath)) File.Delete(logPath);
            if (Directory.Exists(directory)) Directory.Delete(directory);
        }
    }

    private static void ThrowFromCheck() => throw new InvalidOperationException("Intentional runner test failure");
}
