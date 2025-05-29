using DAL.Entities;

namespace BLL.EntityBLLModels;

    public int Id
    {
        get => _id;
        set { _id = value; OnPropertyChanged(nameof(Id)); }
    }

    public string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(nameof(Title)); }
    }

    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(nameof(Description)); }
    }

    public decimal StartPrice
    {
        get => _startPrice;
        set { _startPrice = value; OnPropertyChanged(nameof(StartPrice)); }
    }

    public byte[]? ImageBytes { get; set; } // зберігаємо зображення як необов'язковий масив байтів рядок для передачі через API

    public DateTime CreatedAt { get; set; }

    public DateTime? StartTime { get; set; }
    public int DurationDays { get; set; }
    public DateTime? EndTime { get; set; }

    public int CategoryId { get; set; }
    public int? ManagerId { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}