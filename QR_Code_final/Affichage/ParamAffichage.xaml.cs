using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QR_Code_final.Affichage
{
    /// <summary>
    /// Logique d'interaction pour ParamAffichage.xaml
    /// </summary>
    public partial class ParamAffichage : Page
    {
        public ParamAffichage()
        {
            InitializeComponent();
        }

        private void CliqueRun(object sender, RoutedEventArgs e)
        {
            if (MessagePlayer.Text.Length > 0)
            {
				new QRCodeAffichage(messagePlayer: MessagePlayer.Text);
				AffichageCentral.MessPlayer = MessagePlayer.Text;
                AffichageCentral.reset = true;
			}
		}
	}
}
