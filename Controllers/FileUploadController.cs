using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Lesson15._11.Data;
using Lesson15._11.Models;

namespace Lesson15._11.Controllers;
public class FileUploadController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
	[HttpGet]
	public IActionResult Downloads()
	{
		var currentDirectory = Directory.GetCurrentDirectory();
		var uploadsFolder = Path.Combine(currentDirectory, "UploadedFiles");
		if (!Directory.Exists(uploadsFolder))
		{
			return Content("ERROR! Directory does not exists");
		}
		var files = Directory.GetFiles(uploadsFolder).
		Select(file => new FileInfo(file)).ToList().
		Select(file => new
		{
			Name = file.Name,
			Size = $"{(file.Length / 1024f / 1024):F2} mb"
		}).ToList();
		return View(files);
	}
	public IActionResult Upload(IFormFile file, string password)
	{
		var files = HttpContext.Request.Form.Files.FirstOrDefault();
		if (file == null || file.Length == 0)
		{
			return Content("File no choosen");
		}
		var path = Directory.GetCurrentDirectory();
		var uploadsFolder = Path.Combine(path, "UploadedFiles");
		if (!Directory.Exists(uploadsFolder))
		{
			Directory.CreateDirectory(uploadsFolder);
		}
		var filepath = Path.Combine(uploadsFolder, file.FileName);
		using (var stream = new FileStream(filepath, FileMode.Create))
		{
			file.CopyTo(stream);
		}

		string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
		FileDataManager.AddFile(new FileData()
		{
			FileName = file.FileName,
			PasswordHash = passwordHash
		});
		return Content($"file <{file.FileName}> downloaded, {passwordHash}");
	}
	[HttpGet]
	public IActionResult DownloadFile(string fileName)
	{
		return View("DownloadFile", fileName);
	}
	[HttpPost]
	public IActionResult DownloadFile(string fileName, string password)
	{
		// var fileData = FileRepository.Files.FirstOrDefault(x => x.FileName == fileName);
		var fileData = FileDataManager.GetFile(fileName);
		var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
		var filepath = Path.Combine(uploadsFolder, fileName);
		if (fileData == null)
		{
			return Content("File does not exist");
		}
		if (BCrypt.Net.BCrypt.Verify(password, fileData.PasswordHash) == false)
		{
			return Content("Wrong Password!");
		}
		byte[] bytes = System.IO.File.ReadAllBytes(filepath);
		return File(bytes, "application/octet-stream", fileName);
	}
	public IActionResult DeleteFile(string fileName)
	{
		var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
		var filePath = Path.Combine(uploadsFolder, fileName);
		if (System.IO.File.Exists(filePath))
		{
			System.IO.File.Delete(filePath);
			return Content($"file <{fileName}> was deleted");
		}
		return Content($"ERROR! file <{fileName}> was NOT deleted");
	}
}