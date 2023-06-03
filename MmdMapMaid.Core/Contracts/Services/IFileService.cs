using System.Text.Json;

namespace MmdMapMaid.Core.Contracts.Services;

public interface IFileService
{
    T? Read<T>(string folderPath, string fileName, JsonSerializerOptions? options = null);

    void Save<T>(string folderPath, string fileName, T content, JsonSerializerOptions? options = null);

    void Delete(string folderPath, string fileName);
}
