﻿namespace DAL.Data.Services;

public class ImageService : IImageService
{
    private string directoryName;
    private readonly string imagesDir;
    private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".tmp" };

    public ImageService(string directoryName)
    {
        this.directoryName = directoryName;

        string basePath = AppContext.BaseDirectory;
        imagesDir = Path.Combine(basePath, directoryName);

        if (!Directory.Exists(imagesDir))
        {
            Directory.CreateDirectory(imagesDir);
        }
    }

    public string SaveImage(string sourceImagePath)
    {
        if (string.IsNullOrWhiteSpace(sourceImagePath) || !File.Exists(sourceImagePath))
        {
            throw new ArgumentException("Файл зображення не знайдено");
        }

        string extension = Path.GetExtension(sourceImagePath).ToLower();
        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Непідтримуваний формат зображення");
        }
       
        // створюємо для локального збереження унікальну назву через використання 128 бітового Globally Unique Identifie
        string fileName = Guid.NewGuid() + extension;
        string destinationPath = Path.Combine(imagesDir, fileName); // робимо фінальний шлях до нашого файлу через діректорію

        File.Copy(sourceImagePath, destinationPath); // копіюємо наше зображення у внутрішній репозиторій

        // повертаємо відносний шлях
        return Path.Combine(directoryName, fileName);
    }

    public byte[] LoadImage(string relativePath)
    {
        string fullPath = Path.Combine(AppContext.BaseDirectory, relativePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("Зображення не знайдено");
        }

        return File.ReadAllBytes(fullPath);
    }
}
