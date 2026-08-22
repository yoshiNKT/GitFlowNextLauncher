using GitFlowNextLauncher.Model;
using Microsoft.Win32;
using System.IO;
using System.Media;
using System.Windows;

namespace GitFlowNextLauncher;

public partial class MainWindow
{
    private async void OpenRepositoryButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Gitリポジトリを選択してください"
        };

        if (!string.IsNullOrWhiteSpace(AppSettings.InitialDirPath) &&
            Directory.Exists(AppSettings.InitialDirPath))
        {
            dialog.InitialDirectory = AppSettings.InitialDirPath;
        }

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        var repositoryPath = dialog.FolderName;

        await OpenRepositoryAsync(repositoryPath);
    }

    private void RepositoryPathTextBox_DragOver(
    object sender,
    DragEventArgs e)
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

    private async void RepositoryPathTextBox_Drop(
        object sender,
        DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            return;

        var paths = (string[])e.Data.GetData(DataFormats.FileDrop);

        if (paths.Length != 1 || !Directory.Exists(paths[0]))
        {
            UpdateStatusText("Gitリポジトリのフォルダをドロップしてください。");
            SystemSounds.Asterisk.Play();

            return;
        }

        await OpenRepositoryAsync(paths[0]);

        e.Handled = true;
    }
}
