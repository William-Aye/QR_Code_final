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
using System.Windows.Threading;
using QR_Code_final.Affichage;

namespace QR_Code_final
{
	/// <summary>
	/// Logique d'interaction pour AffichageCentral.xaml
	/// </summary>
	public partial class AffichageCentral : Page
	{
		public static bool reset;
		public static string MessPlayer = "vide";
		DispatcherTimer timer = new DispatcherTimer();


		public AffichageCentral()
		{
			InitializeComponent();

			this.QRCodeAffichage.Navigate(new QRCodeAffichage(MessPlayer));
			//this.QRCodeAffichage.Navigate(new Uri("Affichage/QRCodeAffichage.xaml", UriKind.Relative));
			this.ParamAffichage.Navigate(new Uri("Affichage/ParamAffichage.xaml", UriKind.Relative));

			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Tick += ResetQR;
			timer.Start();
		}

		void ResetQR(object sender, EventArgs e)
		{
			if (reset)
			{
				this.QRCodeAffichage.Navigate(new QRCodeAffichage(MessPlayer));
			}
			reset = false;
		}
	}
}
