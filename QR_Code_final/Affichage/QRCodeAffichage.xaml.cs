using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QR_Code_final.Classes;

namespace QR_Code_final.Affichage
{
    /// <summary>
    /// Logique d'interaction pour QRCodeAffichage.xaml
    /// </summary>
    public partial class QRCodeAffichage : Page
    {
		//En fait on affiche que 1 fois sur deux le texte du QR code
		//parcequ'on appelle 2 fois le constructeur pour pouvoir afficher le 
		//QR code en WPF avec notre méthode.
		private static bool affichageDecodage = true;
		public QRCodeAffichage(string messagePlayer)
        {
			SuppGrille();
            InitializeComponent();

			QRCode DataQR = new QRCode(messagePlayer);
            DataQR.DataBit.AddRange(DataQR.DataCorrecteur);
            QRMat QR = new QRMat(DataQR.DataBit.ToArray(), DataQR.Version); GenererGrille(QR.Module);
			if (affichageDecodage)
			{
				affichageDecodage = false;
				MessageBox.Show(QR.Decodage("QRActuelle.bmp"));
			}
			else
				affichageDecodage = true;
        }
		/// <summary>
		/// Généré une grille en WPF pour afficher la matrice de bit mit en entrée
		/// </summary>
		/// <param name="grille"></param>
		private void GenererGrille(byte[,] grille)
		{
			for (int i = 0; i < grille.GetLength(0) + 4; i++)
			{
				RowDefinition row = new RowDefinition();
				ColumnDefinition column = new ColumnDefinition();
				GridQR.RowDefinitions.Add(row);
				GridQR.ColumnDefinitions.Add(column);
			}

			for (int i = 2; i < grille.GetLength(0)+2; i++)
				for (int j = 2; j < grille.GetLength(1)+2; j++)
				{
					Button QRGrille = new Button();
					QRGrille.Name = $"Bit{i}_{j}";
					QRGrille.BorderBrush = (grille[i-2, j-2] == 1) ? Brushes.Black : Brushes.White;
                    QRGrille.Background = (grille[i-2, j-2] == 1) ? Brushes.Black : Brushes.White;
					Grid.SetRow(QRGrille, i);
					Grid.SetColumn(QRGrille, j);
					GridQR.Children.Add(QRGrille);
				}
		}
		/// <summary>
		/// Suppresion de la grille pour pouvoir recréée une autre grille plus tard.
		/// On fait celà car toutes les grilles n'ont pas forcément la même taille.
		/// </summary>
		private void SuppGrille()
		{
			if (GridQR != null)
			{
				GridQR.RowDefinitions.Clear();
				GridQR.ColumnDefinitions.Clear();
			}
		}
	}
}
