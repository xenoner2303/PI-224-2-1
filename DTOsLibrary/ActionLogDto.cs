﻿namespace DTOsLibrary;

public class ActionLogDto
{
    public string ActionName { get; set; }
    public string Description { get; set; }
    public DateTime ActionTime { get; set; }

    public override string ToString() => $"{ActionName} {ActionTime.ToString("dd.MM.yyyy")} {Description} ";
}
