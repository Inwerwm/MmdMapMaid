namespace Test;
internal static class TestData
{
    /// <summary>
    /// テストデータ用フォルダのパス
    /// </summary>
    public static string TestDataDirectory => "../../TestData/";
    /// <summary>
    /// 生成データ用フォルダ名
    /// </summary>
    public static string GeneratedDirectoryName => "Generated";
    /// <summary>
    /// 生成データ用フォルダのパス
    /// </summary>
    public static string GeneratedDirectory => Path.Combine(TestDataDirectory, GeneratedDirectoryName);

    /// <summary>
    /// テストデータ用フォルダ内のパスをファイル名から作成する
    /// </summary>
    /// <param name="filename">取得したいパスのファイル名</param>
    /// <returns>テストデータ用フォルダ内の指定ファイルへのパス</returns>
    public static string GetPath(string filename) => Path.Combine(TestDataDirectory, filename);

    /// <summary>
    /// 生成データ用フォルダ内のパスをファイル名から作成する
    /// </summary>
    /// <param name="filename">取得したいパスのファイル名</param>
    /// <returns>生成データ用フォルダ内の指定ファイルへのパス</returns>
    public static string GetGeneratedPath(string filename) => Path.Combine(GeneratedDirectory, filename);

    /// <summary>
    /// ファイル名と拡張子の間に文字列を挟んだものを作って返す
    /// </summary>
    /// <param name="filename">元ファイル名(ディレクトリ名は消える)</param>
    /// <param name="suffix">拡張子の前に挟む文字列</param>
    /// <returns>接尾辞追加後ファイル名</returns>
    public static string AppendSuffix(string filename, string suffix) => $"{Path.GetFileNameWithoutExtension(filename)}{suffix}{Path.GetExtension(filename)}";
}
