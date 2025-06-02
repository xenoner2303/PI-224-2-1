using DTOsLibrary;
using UI.UIHelpers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UI.Subcontrols
{
    /// <summary>
    /// Interaction logic for LotDemonstrationControl.xaml
    /// </summary>
    public partial class LotDemonstrationControl : UserControl
    {
        public AuctionLotDto auctionLotDto;
        public event EventHandler? LotSelected;
        private bool isSelected = false;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;

                var chosenBrush = this.TryFindResource("ChosenColor") as Brush;
                var standardBrush = this.TryFindResource("StandartViewColor") as Brush;

                if(chosenBrush != null && standardBrush != null)
                {
                    MainBorder.Background = isSelected ? chosenBrush : standardBrush;
                }
            }
        }

        public LotDemonstrationControl(AuctionLotDto auctionLotDto)
        {
            InitializeComponent();

            this.auctionLotDto = auctionLotDto;
            FillControlParts();
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LotSelected?.Invoke(this, EventArgs.Empty);
        }

        private void FillControlParts()
        {
            if (auctionLotDto == null)
            {
                return;
            }

            LotNameTextBlock.Text = auctionLotDto.Title;
            PosterNameTextBlock.Text = auctionLotDto.Owner?.ToString() ?? "Невідомий";
            DescriptionTextBlock.Text = auctionLotDto.Description;
            LotStatusTextBlock.Text = auctionLotDto.Status.ToString();

            var maxBid = auctionLotDto.Bids?.OrderByDescending(b => b.Amount).FirstOrDefault(); // впорядковую колекцію від найбільшої ставки до найменшої та беру найбільшу
            BidAmountTextBlock.Text = maxBid != null ? maxBid.ToString() : $"Ставок ще немає, Стартова ціна: {auctionLotDto.StartPrice}";

            LotImage.Source = UILoadHelper.LoadImage(auctionLotDto);
        }
    }
}
