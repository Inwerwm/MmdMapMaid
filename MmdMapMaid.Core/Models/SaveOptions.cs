namespace MmdMapMaid.Core.Models;

/// <summary>
/// 保存設定
/// </summary>
/// <param name="EnableOverwrite">生成ファイルを上書きして保存するか</param>
/// <param name="EnableBackup">上書き保存する時、古いファイルをバックアップするか</param>
/// <param name="GenerationDirectory">生成/バックアップファイルの保存先ディレクトリ(nullで元ファイルと同じディレクトリ)</param>
public record SaveOptions(bool EnableOverwrite = true, bool EnableBackup = true, string? GenerationDirectory = null)
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

        if (EnableOverwrite && EnableBackup && File.Exists(path))
        {
            File.Copy(path, backupPath, true);
        }

        return (savePath, backupPath);
    }

    /// <summary>
    /// 元とは異なるパスの方を取得する
    /// </summary>
    /// <param name="savePath">保存パス</param>
    /// <param name="backupPath">バックアップパス</param>
    /// <returns>元になったパスではない方</returns>
    public string? GetOtherPath(string savePath, string backupPath)
    {
        return !EnableOverwrite ? savePath
            : EnableBackup ? backupPath 
            : null;
    }

    /// <summary>
    /// 設定に応じてバックアップ付き保存を行い、新規に作られた方のファイルパスを返す
    /// </summary>
    /// <param name="path">保存したいパス</param>
    /// <param name="save">保存処理</param>
    /// <returns>新しくできたファイルの方のパス</returns>
    public string? SaveWithBackupAndReturnCreatedPath(string path, Action<string> save)
    {
        var (savePath, backupPath) = CreatePathAndBackupIfEnable(path);
        save(savePath);

        return GetOtherPath(savePath, backupPath);
    }
}
