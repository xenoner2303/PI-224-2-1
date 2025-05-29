using System.ComponentModel;

public class AuctionLotModel : INotifyPropertyChanged
{
    private int _id;
    private string _title;
    private string _description;
    private decimal _startPrice;
    private decimal _bidStep;
    private int _categoryId;
    private DateTime _endDate;
    private string _status;

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

    public decimal BidStep
    {
        get => _bidStep;
        set { _bidStep = value; OnPropertyChanged(nameof(BidStep)); }
    }

    public int CategoryId
    {
        get => _categoryId;
        set { _categoryId = value; OnPropertyChanged(nameof(CategoryId)); }
    }

    public DateTime EndDate
    {
        get => _endDate;
        set { _endDate = value; OnPropertyChanged(nameof(EndDate)); }
    }

    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(nameof(Status)); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}