namespace DAL.Entities;

public class AuctionLot
{
    private string title;
    private string description;
    private string relativeImagePath;
    private decimal startPrice;
    private DateTime? startTime;
    private DateTime? endTime;
    private int durationDays;

    public int Id { get; set; } // айді для бази даних

    public string Title
    {
        get => title;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length < 5 || value.Length > 200)
            {
                throw new ArgumentException("Назва лоту має містити 5-200 символів");
            }

            title = value;
        }
    }

    public string Description
    {
        get => description;
        set
        {
            if (value != null && value.Length > 5000)
            {
                throw new ArgumentException("Опис не може перевищувати 5000 символів");
            }

            description = string.IsNullOrWhiteSpace(value) ? "Без опису" : value;
        }
    }

    public string? RelativeImagePath
    {
        get => relativeImagePath;
        set
        { // базові перевірки на неіснуючий файл та шлях з недопустимими
            if (!string.IsNullOrWhiteSpace(value) && !File.Exists(value))
            {
                throw new FileNotFoundException("Файл зображення не знайдено за вказаним шляхом");
            }

            if (value.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                throw new ArgumentException("Шлях містить недопустимі символи");
            }

            relativeImagePath = value;
        }
    }

    public decimal StartPrice
    {
        get => startPrice;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(StartPrice), "Стартова ціна має бути > 0");
            }

            startPrice = value;
        }
    }

    public EnumLotStatuses Status { get; set; }


    public DateTime? StartTime
    {
        get => startTime;
        set
        {
            if (EndTime.HasValue && value.HasValue && value >= EndTime.Value)
            {
                throw new ArgumentException("Дата початку має бути раніше за дату завершення");
            }

            startTime = value;
        }
    }

    public int DurationDays // тривалість аукціону у днях
    {
        get => durationDays;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Тривалість повинна бути більшою за 0 днів");
            }

            durationDays = value;
        }
    }

    public DateTime? EndTime
    {
        get
        {
            if (endTime.HasValue)
            {
                return endTime.Value;
            }

            return StartTime.HasValue ? StartTime.Value.AddDays(DurationDays) : null;
        }
        set
        {
            if (StartTime.HasValue && value.HasValue && value <= StartTime.Value)
            {
                throw new ArgumentException("Дата завершення має бути пізніше за дату початку");
            }

            endTime = value;
        }
    }


    public int OwnerId { get; set; } // зовнішній ключ
    public RegisteredUser Owner { get; set; } // навігаційна властивість - власник лоту

    public int CategoryId { get; set; } // зовнішній ключ
    public Category Category { get; set; } // навігаційна властивість - категорія лоту

    public int? ManagerId { get; set; } // зовнішній ключ
    public Manager Manager { get; set; } // навігаційна властивість - менеджер лоту

    public List<Bid> Bids { get; set; } = new List<Bid>(); // 1 лот - багато ставок
}
