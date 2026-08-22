using System.Diagnostics;
using System.Text;

namespace GitFlowNextLauncher.Model;

public static class GitManagement
{
    const string NAME_GIT = "git";

    public static async Task<string?> GetCurrentBranchAsync(string repositoryPath)
    {
        var result = await RunGitAsync(
            repositoryPath,
            "branch --show-current");

        if (result.ExitCode != 0)
        {
            return null;
        }

        return result.Output.Trim();
    }

    public static async Task<bool> IsGitRepositoryAsync(string repositoryPath)
    {
        var result = await RunGitAsync(
            repositoryPath,
            "rev-parse --is-inside-work-tree");

        return result.ExitCode == 0 &&
               result.Output.Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<(int ExitCode, string Output)> RunGitAsync(
        string workingDirectory,
        string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = NAME_GIT,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
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
