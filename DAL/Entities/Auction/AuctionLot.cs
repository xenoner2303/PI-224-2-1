namespace DAL.Entities;

public class AuctionLot
{
    private string title;
    private string description;
    private string relativeImagePath;
    private decimal startPrice;
    private DateTime endTime;
    private DateTime startime;

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

    public DateTime StartTime // час буде автоматично вводитися при підтвердженні лоту менеджером
    {
        get => startime;
        set
        {
            if (value > EndTime)
            {
                throw new ArgumentException("Не можна розпочати аукціон післявстановленого кінця торгів");
            }

            startime = value;
        }
    }

    public DateTime EndTime // час завершення аукціону, який буде вводити користувач
    {
        get => endTime;
        set
        {
            if (value < StartTime)
            {
                throw new ArgumentException("EndTime не може бути раніше CreatedAt");
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
