﻿namespace DAL.Data.Services;

public interface IImageService
{
    string SaveImage(string sourceImagePath);
    byte[] LoadImage(string relativePath);
}
