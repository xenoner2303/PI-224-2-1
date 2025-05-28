using DTOsLibrary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Presentation.UIHelpers.SubControls
{
    /// <summary>
    /// Interaction logic for LotDemonstrationControl.xaml
    /// </summary>
    public partial class LotDemonstrationControl : UserControl
    {
        private AuctionLotDto auctionLotDto;

        public LotDemonstrationControl(AuctionLotDto auctionLotDto)
        {
            this.auctionLotDto = auctionLotDto;
            FillControlParts();

            InitializeComponent();
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Клік по лоту");
        }

        private void FillControlParts()
        {
            // заповнення контролеру, поки нічого до додавання веб апі та дто дял передавання через юрл зображень
        }
    }
}
