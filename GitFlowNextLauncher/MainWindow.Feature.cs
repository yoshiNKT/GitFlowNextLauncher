using GitFlowNextLauncher.Model;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows;

namespace GitFlowNextLauncher;

public partial class MainWindow
{
    private async void StartFeatureButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var featureName = NormalizeFeatureName(FeatureNameTextBox.Text);

        if (string.IsNullOrWhiteSpace(RepositoryPathTextBox.Text))
        {
            UpdateStatusText("リポジトリを開いてください。");
            // SE鳴らす
            SystemSounds.Asterisk.Play();

            return;
        }

        if (string.IsNullOrWhiteSpace(featureName))
        {
            UpdateStatusText("フィーチャー名を入力してください。");
            SystemSounds.Asterisk.Play();

            return;
        }

        var currentBranch =
            await GitManagement.GetCurrentBranchAsync(RepositoryPathTextBox.Text);

        if (currentBranch is null)
        {
            UpdateStatusText("現在のブランチを取得できませんでした。");
            SystemSounds.Asterisk.Play();
            return;
        }

        CurrentBranchTextBlock.Text = currentBranch;

        if (!IsTargetBranch(currentBranch))
        {
            UpdateStatusText("developブランチからフィーチャーを開始してください。");
            SystemSounds.Asterisk.Play();

            return;
        }

        StartFeatureButton.IsEnabled = false;

        try
        {
            var result = await GitFlowManagement.StartFeatureAsync(
                RepositoryPathTextBox.Text,
                featureName);

            if (result.ExitCode != 0)
            {
                UpdateStatusText($"フィーチャーの作成に失敗しました。\n{result.Output}");
                SystemSounds.Asterisk.Play();

                return;
            }

            var newCurrentBranch =
                await GitManagement.GetCurrentBranchAsync(RepositoryPathTextBox.Text);

            var updateText = $"feature/{featureName} を作成しました。";
            if (newCurrentBranch is null)
            {
                updateText += "ただし現在のブランチを取得できませんでした。";
            }

            CurrentBranchTextBlock.Text = newCurrentBranch ?? string.Empty;

            UpdateStatusText(updateText);
            SystemSounds.Asterisk.Play();

            FeatureNameTextBox.Clear();
        }
        catch (Exception ex)
        {
            UpdateStatusText($"エラーが発生しました。\n{ex.Message}");
            SystemSounds.Asterisk.Play();
        }
        finally
        {
            StartFeatureButton.IsEnabled = true;
        }
    }

    // Git使用禁止文字の置き換え
    private static string NormalizeFeatureName(string featureName)
    {
        featureName = featureName.Trim();

        // Gitブランチ名で使用できない文字を半角アンダースコアに置換
        // 全角はスペースのみ置き換え対象
        featureName = Regex.Replace(
            featureName,
            @"[\x00-\x20~^:?*\[\\　]",
    "_");

        return featureName;
    }
}
