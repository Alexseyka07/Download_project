using System;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using Lesson15._11.Models;

namespace Lesson15._11.Data;

public static class FileDataManager
{
	private static string filePath = Path.Combine(Directory.GetCurrentDirectory(), "fileData.json");
	public static List<FileData> GetAllFiles()
	{
		if(!File.Exists(filePath))
		{
			return new List<FileData>();
		}
		var json = File.ReadAllText(filePath);
		return JsonSerializer.Deserialize<List<FileData>>(json) ?? new List<FileData>();
	}
	public static void AddFile(FileData fileData)
	{
		var files = GetAllFiles();
		files.Add(fileData);
		SaveAllFiles(files);
	}
	private static void SaveAllFiles(List<FileData> files)
	{
		var json = JsonSerializer.Serialize(files, new JsonSerializerOptions {WriteIndented = true});
		File.WriteAllText(filePath, json);
	}
	public static FileData? GetFile(string filename)
	{
		return GetAllFiles().FirstOrDefault(x => x.FileName == filename);
	}
}
