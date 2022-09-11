using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmdMapMaid.Core.Models.Emm;
/// <summary>
/// エフェクト材質整列オプション
/// </summary>
/// <param name="EnableOverwrite">生成EMMを上書きして保存するか</param>
/// <param name="CreateBackupIfOverwrite">上書き保存する時、古いファイルをバックアップするか</param>
/// <param name="GenerationDirectory">生成/バックアップファイルの保存先ディレクトリ(nullで元EMMと同じディレクトリ)</param>
public record EmmMaterialOrderOptions(bool EnableOverwrite = true, bool CreateBackupIfOverwrite = true, string? GenerationDirectory = null);
