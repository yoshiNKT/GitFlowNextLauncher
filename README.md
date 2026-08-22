# GitFlowNextLauncher

A lightweight GUI client for Git Flow feature branch creation using [git-flow-next](https://github.com/git-tower/git-flow-next).

GitFlowNextLauncher is a small WPF application designed to provide a simple GUI for starting Git Flow feature branches without relying on SourceTree's Git Flow integration.

## Features

- Open a Git repository
  - Folder selection dialog
  - Drag and drop
- Automatically restore the last opened repository
- Remember the initial directory for the repository selection dialog
- Display the current branch
- Automatically refresh the current branch when the application becomes active
- Start a Git Flow feature branch from the `develop` branch
- Display Git / git-flow-next errors in the application
- Supports branch names containing Japanese characters

## Requirements

- Windows
- Git
- [git-flow-next](https://github.com/git-tower/git-flow-next)

## Installing git-flow-next

GitFlowNextLauncher uses `git-flow-next` through the Git command line.

### Using WinGet

The easiest way to install `git-flow-next` on Windows is with WinGet.

Open PowerShell or Command Prompt and run:

```powershell
winget install GitTower.GitFlowNext
```

After installation, verify that it is available:

```powershell
git flow version
```

Example:

```
git-flow-next version 1.2.0
```

You can also verify the Git version with:

```
git --version
```

If `git flow version` works correctly, GitFlowNextLauncher can use `git-flow-next`.

## Usage
### 1. Open a repository

Launch GitFlowNextLauncher and select a Git repository using リポジトリを開く.

You can also drag and drop a repository folder into the application window.

The application checks whether the selected folder is a Git repository and displays the current branch.

### 2. Start a feature branch

Feature branches can be started only when the current branch is `develop`.

Enter the feature name without the `feature/` prefix.

For example:

```
ImageCacheManager
```

Click フィーチャー開始 to create:

```
feature/ImageCacheManager
```

The underlying command is:

```
git flow feature start "ImageCacheManager"
```

After the feature branch is created, the current branch displayed by the application is updated automatically.

### 3. Switching branches outside the application

GitFlowNextLauncher checks the current branch again when the application window becomes active.

For example:

```
GitFlowNextLauncher
        ↓
Visual Studio / SourceTree
        ↓
Switch to develop
        ↓
GitFlowNextLauncher
```

When returning to GitFlowNextLauncher, the application refreshes the current branch and updates the Feature button accordingly.

### Git Flow workflow

GitFlowNextLauncher currently focuses only on starting feature branches.

The intended workflow is:

```
develop
   │
   └── feature/example
            │
            ├── Push to GitHub
            │
            └── Merge into develop
```

Feature branch merging and deletion are handled through the existing GitHub workflow and are intentionally not included in GitFlowNextLauncher.

### Configuration

GitFlowNextLauncher stores its settings as JSON under the user's local application data directory.

The following settings are stored:

* Last opened repository
* Initial directory used by the repository selection dialog

### Documentation

Detailed specifications and the development history are available in:

* [Specification and Development History (Japanese)](docs/specification.md)

### Development

GitFlowNextLauncher is implemented as a WPF application.

The application intentionally avoids additional NuGet dependencies and uses standard .NET APIs for Git process execution and settings persistence.

Git operations are executed through `git.exe`, while Git Flow feature operations are executed through `git-flow-next`.

### License

[MIT License](LICENSE)
