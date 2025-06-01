using DTOsLibrary;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Presentation.UIHelpers.SubControls
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
                MainBorder.Background = isSelected ? Brushes.LightBlue : (Brush)new BrushConverter().ConvertFrom("#FFF0F0F0");
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

            var maxBid = auctionLotDto.Bids?.OrderByDescending(b => b.Amount).FirstOrDefault(); // впорядковую колекцію від найбільшої ставки до найменшої та беру найбільшу
            BidAmountTextBlock.Text = maxBid != null ? maxBid.ToString() : "Ставок ще немає";

            LotImage.Source = UILoadHelper.LoadImage(auctionLotDto);
        }
    }
}
