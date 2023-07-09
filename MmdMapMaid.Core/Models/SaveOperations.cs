namespace MmdMapMaid.Core.Models;

/// <summary>
/// ファイルの保存とバックアップ操作を行うクラスです。
/// </summary>
/// <param name="EnableOverwrite">生成ファイルを上書きして保存するか</param>
/// <param name="EnableBackup">上書き保存する時、古いファイルをバックアップするか</param>
/// <param name="GenerationDirectory">生成/バックアップファイルの保存先ディレクトリ(nullで元ファイルと同じディレクトリ)</param>
public record SaveOperations(bool EnableOverwrite = true, bool EnableBackup = true, string? GenerationDirectory = null, DateTime? Timestamp = null)
{
    /// <summary>
    /// 保存用パスとバックアップ用パスを決定します。
    /// </summary>
    /// <param name="path">元のファイルパス</param>
    /// <returns>保存用のパスとバックアップ用のパス</returns>
    private (string SavePath, string BackupPath) DeterminePaths(string path)
    {
        var otherDir = GenerationDirectory ?? Path.GetDirectoryName(path) ?? "";
        var otherPath = Path.Combine(otherDir, $"{Path.GetFileNameWithoutExtension(path)}_{(Timestamp ?? DateTime.Now):yyyy-MM-dd_HH-mm-ss-ff}{Path.GetExtension(path)}");

        return EnableOverwrite ? (path, otherPath) : (otherPath, path);
    }

    /// <summary>
    /// バックアップが有効であれば、保存用パスとバックアップ用パスを準備し、バックアップを作成します。
    /// </summary>
    /// <param name="path">元のファイルパス</param>
    /// <returns>保存用のパスとバックアップ用のパス</returns>
    private (string SavePath, string BackupPath) PrepareBackupIfEnabled(string path)
    {
        var (savePath, backupPath) = DeterminePaths(path);

        if (EnableOverwrite && EnableBackup && File.Exists(path))
        {
            File.Copy(path, backupPath, true);
        }

        return (savePath, backupPath);
    }

    /// <summary>
    /// 新しく作成されたファイルのパスを取得します。
    /// </summary>
    /// <param name="savePath">保存用のパス</param>
    /// <param name="backupPath">バックアップ用のパス</param>
    /// <returns>新たに作成されたファイルのパス</returns>
    private string? GetNewlyCreatedPath(string savePath, string backupPath)
    {
        return !EnableOverwrite ? savePath
            : EnableBackup ? backupPath 
            : null;
    }

    /// <summary>
    /// 設定に従ってファイルを保存し、必要であればバックアップを作成し、新たに作成されたファイルのパスを返します。
    /// </summary>
    /// <param name="path">保存したいファイルのパス</param>
    /// <param name="save">保存を行うためのアクション</param>
    /// <returns>新たに作成されたファイルのパス</returns>
    public string? SaveAndBackupFile(string path, Action<string> save)
    {
        var (savePath, backupPath) = PrepareBackupIfEnabled(path);
        save(savePath);

        return GetNewlyCreatedPath(savePath, backupPath);
    }

    /// <summary>
    /// ファイル名にサフィックスを追加します。
    /// </summary>
    /// <param name="fullPath">完全なファイルパス</param>
    /// <param name="suffix">追加するサフィックス</param>
    /// <returns>サフィックスが追加された新しいファイル名</returns>
    public static string AppendSuffixToFilename(string fullPath, string suffix) =>
        Path.Combine(Path.GetDirectoryName(fullPath) ?? ".",  Path.GetFileNameWithoutExtension(fullPath) + suffix + Path.GetExtension(fullPath));
}
