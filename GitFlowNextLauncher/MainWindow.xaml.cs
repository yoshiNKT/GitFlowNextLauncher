using GitFlowNextLauncher.Model;
using System.IO;
using System.Media;
using System.Windows;

namespace GitFlowNextLauncher;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const string TargetBranch = "develop";

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        AppSettings.Load();

        if (string.IsNullOrWhiteSpace(AppSettings.LastRepositoryPath))
            return;

        if (!Directory.Exists(AppSettings.LastRepositoryPath))
            return;

        await OpenRepositoryAsync(AppSettings.LastRepositoryPath);
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(RepositoryPathTextBox.Text))
        {
            AppSettings.LastRepositoryPath = RepositoryPathTextBox.Text;
        }

        AppSettings.Save();
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Link;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }

        e.Handled = true;
    }

    private async void Window_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            return;

        var paths = (string[])e.Data.GetData(DataFormats.FileDrop);

        if (paths.Length != 1)
        {
            UpdateStatusText("リポジトリフォルダを1つだけドロップしてください。");
            SystemSounds.Asterisk.Play();

            return;
        }

        var repositoryPath = paths[0];

        if (!Directory.Exists(repositoryPath))
        {
            UpdateStatusText("フォルダをドロップしてください。");
            SystemSounds.Asterisk.Play();

            return;
        }

        await OpenRepositoryAsync(repositoryPath);
    }

    private async void Window_Activated(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(RepositoryPathTextBox.Text))
            return;

        var currentBranch =
            await GitManagement.GetCurrentBranchAsync(
                RepositoryPathTextBox.Text);

        CurrentBranchTextBlock.Text = currentBranch ?? string.Empty;

        UpdateFeatureAvailability(currentBranch);
    }



    private async Task OpenRepositoryAsync(string repositoryPath)
    {
        if (!await GitManagement.IsGitRepositoryAsync(repositoryPath))
        {
            UpdateStatusText("選択したフォルダはGitリポジトリではありません。");
            SystemSounds.Asterisk.Play();

            return;
        }

        RepositoryPathTextBox.Text = repositoryPath;

        var parentDirectory = Directory.GetParent(repositoryPath);

        if (parentDirectory is not null)
        {
            AppSettings.InitialDirPath = parentDirectory.FullName;
        }

        var branch = await GitManagement.GetCurrentBranchAsync(repositoryPath);

        if (branch is null)
        {
            CurrentBranchTextBlock.Text = string.Empty;
            UpdateStatusText("現在のブランチを取得できませんでした。");
            SystemSounds.Asterisk.Play();

            return;
        }

        CurrentBranchTextBlock.Text = branch ?? string.Empty;

        if(UpdateFeatureAvailability(branch))
        {
            UpdateStatusText("リポジトリを開きました。");
        }
        else
        {
            UpdateStatusText("リポジトリを開きましたが、developブランチが選択されていません。");
            SystemSounds.Asterisk.Play();
        }
    }

    // フィーチャー開始ボタンのEnabled切り替え
    private bool UpdateFeatureAvailability(string? branch)
    {
        var isTargetBranch = IsTargetBranch(branch);

        StartFeatureButton.IsEnabled = isTargetBranch;

        return isTargetBranch;
    }

    // ブランチのdevelop判定
    private bool IsTargetBranch(string? branch)
    {
        return string.Equals(
            branch,
            TargetBranch,
            StringComparison.OrdinalIgnoreCase);
    }

    // ステータスメッセージ更新
    private void UpdateStatusText(string text)
    {
        var time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        StatusTextBlock.Text = $"[{time}] {text}";
    }
}