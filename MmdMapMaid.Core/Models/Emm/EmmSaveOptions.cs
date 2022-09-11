namespace MmdMapMaid.Core.Models.Emm;

/// <summary>
/// 保存設定
/// </summary>
/// <param name="EnableOverwrite">生成ファイルを上書きして保存するか</param>
/// <param name="CreateBackupIfOverwrite">上書き保存する時、古いファイルをバックアップするか</param>
/// <param name="GenerationDirectory">生成/バックアップファイルの保存先ディレクトリ(nullで元ファイルと同じディレクトリ)</param>
public record EmmSaveOptions(bool EnableOverwrite = true, bool CreateBackupIfOverwrite = true, string? GenerationDirectory = null);
