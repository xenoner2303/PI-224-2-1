namespace DAL.Entities;

public class AuctionLot
{
    private string title;
    private string description;
    private string imagePath;
    private decimal startPrice;
    private DateTime startTime;
    private DateTime endTime;

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

    public string ImagePath
    {
        get => imagePath;
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

            imagePath = value;
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
        get => startTime;
        set
        {
            if (value > EndTime)
            {
                throw new ArgumentException("Не можна розпочати аукціон післявстановленого кінця торгів");
            }

            startTime = value;
        }
    }

    public DateTime EndTime
    {
        get => endTime;
        set
        {
            if (value < StartTime)
            {
                // Автоматично ставимо +3 хвилини
                endTime = StartTime.AddMinutes(3);
            }
            else
            {
                endTime = value;
            }
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
