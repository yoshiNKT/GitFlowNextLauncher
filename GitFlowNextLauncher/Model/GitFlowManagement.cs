using System.Diagnostics;

namespace GitFlowNextLauncher.Model;

public static class GitFlowManagement
{
    public static async Task<(int ExitCode, string Output)> StartFeatureAsync(
        string repositoryPath,
        string featureName)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = $"flow feature start \"{featureName}\"",
            WorkingDirectory = repositoryPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var output = await outputTask;
        var error = await errorTask;

        return (
            process.ExitCode,
            process.ExitCode == 0 ? output : error
        );
    }
}
