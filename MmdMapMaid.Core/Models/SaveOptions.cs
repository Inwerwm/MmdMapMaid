namespace MmdMapMaid.Core.Models;

/// <summary>
/// 保存設定
/// </summary>
/// <param name="EnableOverwrite">生成ファイルを上書きして保存するか</param>
/// <param name="CreateBackupIfOverwrite">上書き保存する時、古いファイルをバックアップするか</param>
/// <param name="GenerationDirectory">生成/バックアップファイルの保存先ディレクトリ(nullで元ファイルと同じディレクトリ)</param>
public record SaveOptions(bool EnableOverwrite = true, bool CreateBackupIfOverwrite = true, string? GenerationDirectory = null)
{
    /// <summary>
    /// 設定に応じた保存/バックアップファイルパスを取得
    /// </summary>
    /// <param name="path">元になるパス</param>
    /// <returns>保存用パスとバックアップ用パス</returns>
    public (string SavePath, string BackupPath) GetPath(string path)
    {
        var otherDir = GenerationDirectory ?? Path.GetDirectoryName(path) ?? "";
        var otherPath = Path.Combine(otherDir, $"{Path.GetFileNameWithoutExtension(path)}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss-ff}{Path.GetExtension(path)}");

        return EnableOverwrite ? (path, otherPath) : (otherDir, path);
    }

    /// <summary>
    /// 設定に応じた保存/バックアップファイルパスを取得するとともに有効ならバックアップも作成する
    /// </summary>
    /// <param name="path">元になるパス</param>
    /// <returns>保存用パスとバックアップ用パス</returns>
    public (string SavePath, string BackupPath) CreatePathAndBackupIfEnable(string path)
    {
        var (savePath, backupPath) = GetPath(path);

        if (EnableOverwrite && CreateBackupIfOverwrite)
        {
            File.Copy(path, backupPath, true);
        }

        return (savePath, backupPath);
    }

    public string? GetOtherPath(string savePath, string backupPath)
    {
        return !EnableOverwrite ? savePath
            : CreateBackupIfOverwrite ? backupPath 
            : null;
    }
}
