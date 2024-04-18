using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace QR_Code_final.Classes
{
    internal class QRMat
    {
        private byte[,] module = null;
        const int i = -2;
        public byte[,] Module
        { get { return module; } }

        public QRMat(byte[] dataBit, byte version)
        {
            CreationMotif(version);
            Parcours(dataBit);
        }

        /// <summary>
        /// Créer une matrice avec tous les motifs obligatoires en fonction de la version
        /// l'initialisation a la valeur 2 pour toute la matrice montre les cases qui sont à définir
        /// </summary>
        /// <param name="version">
        /// Permet de savoir quels motifs mettre
        /// seuls les versions 1 et 2 sont codées
        /// </param>
        private void CreationMotif(byte version)
        {
            if (version != 0)
            {
                module = new byte[17 + 4 * version, 17 + 4 * version];

                for (int i = 0; i < module.GetLength(0); i++)
                    for (int j = 0; j < module.GetLength(1); j++)
                        module[i, j] = 2;

                CreationRepere(3, 3);
                CreationRepere(3, module.GetLength(1) - 4);
                CreationRepere(module.GetLength(0) - 4, 3);

                if (version > 1)
                    CreationRepereSec(module.GetLength(0) - 7, module.GetLength(1) - 7);

                CreationLignes();
                CreationInfo();
            }
        }

        /// <summary>
        /// Créer un repère sur la matrice
        /// de centre X et Y
        /// </summary>
        /// <param name="centreX">
        /// Le centre d'axe X du repère 
        /// </param>
        /// <param name="centreY">
        /// Le centre d'axe Y du repère
        /// </param>
        private void CreationRepere(int centreX, int centreY)
        {
            for (int i = -4; i < 5; i++)
                for (int j = -4; j < 5; j++)
                {
                    int coordX = i + centreX, coordY = j + centreY;

                    if (coordX >= 0
                        && coordY >= 0
                        && coordX < module.GetLength(0)
                        && coordY < module.GetLength(1))
                    {
                        if (i == -4 || i == 4 || j == -4 || j == 4)
                            module[coordX, coordY] = 0;
                        else if ((i == -2 || i == 2 || j == -2 || j == 2)
                            && i < 3 && i > -3 && j < 3 && j > -3)
                            module[coordX, coordY] = 0;
                        else
                            module[coordX, coordY] = 1;
                    }
                }
        }
        /// <summary>
        /// Créer un repère plus petit sur la matrice
        /// de centre X et Y
        /// </summary>
        /// <param name="centreX">
        /// Le centre d'axe X du repère 
        /// </param>
        /// <param name="centreY">
        /// Le centre d'axe Y du repère
        /// </param>
        private void CreationRepereSec(int centreX, int centreY)
        {
            for (int i = -2; i < 3; i++)
                for (int j = -2; j < 3; j++)
                {
                    int coordX = i + centreX, coordY = j + centreY;

                    if (coordX >= 0
                        && coordY >= 0
                        && coordX < module.GetLength(0)
                        && coordY < module.GetLength(1))
                    {
                        if (i == 0 && j == 0)
                            module[coordX, coordY] = 1;
                        else if (i == -2 || i == 2 || j == -2 || j == 2)
                            module[coordX, coordY] = 1;
                        else
                            module[coordX, coordY] = 0;
                    }
                }
        }
        /// <summary>
        /// Permet de Créer des Lignes sur le QRCode
        /// Ces lignes sont obligatoires dans tous les formats
        /// Elles possèdent une position fixe si on prend en référentiel 
        /// </summary>
        /// <param name="axe">
        /// 0 donne la création sur l'axe X
        /// 1 donne la création sur l'axe Y
        /// </param>
        private void CreationLignes()
        {
            bool cligno = false;
            for (int i = 0; i < module.GetLength(0); i++)
            {
                if (module[i, 6] == 2)
                {
                    if (cligno)
                    {
                        module[i, 6] = 0;
                        module[6, i] = 0;
                    }
                    else
                    {
                        module[i, 6] = 1;
                        module[6, i] = 1;
                    }
                }
                cligno = !cligno;
            }
        }

        /// <summary>
        /// Remplit les zones réservées à l'info du QR Code
        /// Nous avons un bitsInfo "fixé"a
        /// Nous en profitons également pour créer le carré noir
        /// </summary>
        private void CreationInfo()
        {
            int compt = 0;
            byte[] bitsInfo = { 1, 1, 1, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0 };
            for (int i = 0; i < module.GetLength(0); i++)
            {
                if (i < 9 && i != 6)
                {
                    if (module[i, 8] == 2)
                        module[i, 8] = bitsInfo[bitsInfo.Length - 1 - compt];
                    if (module[8, i] == 2)
                        module[8, i] = bitsInfo[compt];
                    compt++;
                }
                else if (i > module.GetLength(0) - 9)
                {
                    if (module[i, 8] == 2)
                        module[i, 8] = bitsInfo[bitsInfo.Length - 1 - compt + 1];
                    if (module[8, i] == 2)
                        module[8, i] = bitsInfo[compt - 1];
                    compt++;
                }

            }
            module[module.GetLength(0) - 8, 8] = 1;
        }
        /// <summary>
        /// Permet de transformée les données du QR code en une matrice de bit qui sera la matrice représentant le QR code final.
        /// </summary>
        /// <param name="dataBit">le texte transformée en bit de donnée</param>
        /// <param name="dataCorrec">les bit de correction d'érreur</param>
        private void Parcours(byte[] dataBit)
        {
            int compt = 0;
            bool diago = false;
            bool ascendant = true;
            int i = module.GetLength(0) - 1, j = module.GetLength(1) - 1;
            while (compt < dataBit.Length)
            {
                //Incrémentation si la case est modifiable
                if (module[i, j] == 2)
                {
                    //Ici on applique le masque 0
                    if ((i + j) % 2 == 0)
                        module[i, j] = (byte)(dataBit[compt] == 0 ? 1 : 0);
                    else
                        module[i, j] = dataBit[compt];

                    compt++;
                }

                //Déplacement du "curseur"
                if (diago)
                {
                    i = ascendant ? i - 1 : i + 1;    //ordre lecteur "true : false"
                    j++;
                }
                else
                {
                    j--;
                }
                diago = !diago;

                //Modification Ascention Descente
                if (i < 0)
                {
                    i = 0;
                    j -= 2;
                    ascendant = false;
                }
                else if (i > module.GetLength(0) - 1)
                {
                    i = module.GetLength(0) - 1;
                    j -= 2;
                    ascendant = true;
                }
                if (j == 6) j--;
                // La ligne de la Zone Info est occupée entièrement et n'est donc pas pris en compte dans le paterne de tracage
            }

            //Ici on transforme le QR code en fichier bmp
            Pixel[,] QRCodePixel = new Pixel[module.GetLength(0), module.GetLength(1)];
            for (int x = 0; x < QRCodePixel.GetLength(0); x++)
            {
                for (int y = 0; y < QRCodePixel.GetLength(1); y++)
                {
                    //Si la valeur est 1 on met en noir dans l'image sinon ont met en blanc
                    //On est obligé de faire une rotation parceque sinon le QR code n'est pas dans le
                    //bon sens
                    if (module[x, y] == 1)
                        QRCodePixel[QRCodePixel.GetLength(0) - y - 1, x] = new Pixel(0, 0, 0);
                    else
                        QRCodePixel[QRCodePixel.GetLength(0) - y - 1, x] = new Pixel(255, 255, 255);
                }
            }
            MyImage QRCodeImage = new MyImage(QRCodePixel);
            QRCodeImage.AgrandirImage(4);
            QRCodeImage.FromImageToFile("QRActuelle.bmp");
        }
        /// <summary>
        /// Permet de décoder le texte mit a l'intèrieur d'un QR code a partir d'un fichier bmp.
        /// </summary>
        /// <param name="QRCodeBMP">nom du fichier bmp d'ou sont tirée les données. Attention ne pas oublier de rajouter .bmp a la fin!!!</param>
        /// <returns></returns>
        public string Decodage(string QRCodeBMP)
        {
            MyImage QRCodeImage = new MyImage(QRCodeBMP);
            QRCodeImage.ReduireImage(4);
            byte[,] module = new byte[QRCodeImage.Hauteur, QRCodeImage.Largeur];
            for (int x = 0; x < QRCodeImage.Hauteur; x++)
            {
                for (int y = 0; y < QRCodeImage.Largeur; y++)
                {
                    if (QRCodeImage.MatPixel[x, y].B == 255
                        && QRCodeImage.MatPixel[x, y].G == 255
                        && QRCodeImage.MatPixel[x, y].R == 255)
                        module[y, QRCodeImage.Hauteur - x - 1] = 0;
                    else
                        module[y, QRCodeImage.Hauteur - x - 1] = 1;
                }
            }
            HashSet<string> bitANePasRecuperer = new HashSet<string>();
            CreationRepereANePasPrendre(bitANePasRecuperer, 3, 3);
            CreationRepereANePasPrendre(bitANePasRecuperer, 3, module.GetLength(1) - 4);
            CreationRepereANePasPrendre(bitANePasRecuperer, module.GetLength(0) - 4, 3);

            //Pour version 1 à 6 il y a toujours un seul repère
            if (module.GetLength(0) >= 25 && module.GetLength(0) <= 41)
            {
                CreationRepereSecANePasPrendre(bitANePasRecuperer, module.GetLength(0) - 7, module.GetLength(1) - 7);
            }

            //paterne de ligne
            bool cligno = false;
            for (int y = 0; y < module.GetLength(0); y++)
            {
                bitANePasRecuperer.Add(y + "," + 6);
                bitANePasRecuperer.Add(6 + "," + y);
                cligno = !cligno;
            }

            //Zone info
            for (int y = 0; y < module.GetLength(0); y++)
            {
                if (y < 9 && y != 6)
                {
                    bitANePasRecuperer.Add(y + "," + 8);
                    bitANePasRecuperer.Add(8 + "," + y);
                }
                else if (y > module.GetLength(0) - 9)
                {
                    bitANePasRecuperer.Add(y + "," + 8);
                    bitANePasRecuperer.Add(8 + "," + y);
                }
            }
            List<byte> bitRecuperer = new List<byte>();
            bool diago = false;
            bool ascendant = true;
            int i = module.GetLength(0) - 1, j = module.GetLength(1) - 1;
            while (j != 0 || i != module.GetLength(0) - 1)
            {
                if (!bitANePasRecuperer.Contains(i + "," + j))
                {
                    if ((i + j) % 2 == 0)
                        bitRecuperer.Add((byte)(module[i, j] == 0 ? 1 : 0));
                    else
                        bitRecuperer.Add(module[i, j]);
                }
                //Déplacement du "curseur"
                if (diago)
                {
                    i = ascendant ? i - 1 : i + 1;    //ordre lecteur "true : false"
                    j++;
                }
                else
                {
                    j--;
                }
                diago = !diago;

                //Modification Ascention Descente
                if (i < 0)
                {
                    i = 0;
                    j -= 2;
                    ascendant = false;
                }
                else if (i > module.GetLength(0) - 1)
                {
                    i = module.GetLength(0) - 1;
                    j -= 2;
                    ascendant = true;
                }
                if (j == 6) j--;
                // La ligne de la Zone Info est occupée entièrement et n'est donc pas pris en compte dans le paterne de tracage
            }

            int tailleTexte = 0;
            for (int x = 4; x <= 13; x++)
            {
                tailleTexte += (int)Math.Pow(2, (13 - x - 1)) * bitRecuperer[x];
            }

            //Ici on vérifie quel est la taille du texte et on enlève donc tout ce qui ne nous intéresse pas
            //donc tout ce qui n'est pas les caractère mit en entrée.
            bitRecuperer.RemoveRange(0, 13);
            int bitDonnees = (tailleTexte - (tailleTexte % 2)) * 11 / 2 + (tailleTexte % 2) * 6;
            bitRecuperer.RemoveRange(bitDonnees, bitRecuperer.Count - bitDonnees);
            List<int> caraNumList = new List<int>();
            int cara = 0;
            int comp = 10;
            StringBuilder texteQRcode = new StringBuilder();

            //Ici on met 6 * (tailleTexte % 2) comme ça on a pas besoin de mettre cette partie dans le if else
            for (int x = 0; x < bitRecuperer.Count - 6 * (tailleTexte % 2); x++)
            {
                if (comp == 0)
                {
                    cara += (int)Math.Pow(2, comp) * bitRecuperer[x];
                    caraNumList.Add(cara);
                    cara = 0;
                    comp = 10;
                }
                else
                {
                    cara += (int)Math.Pow(2, comp) * bitRecuperer[x];
                    comp--;
                }
            }

            if (tailleTexte % 2 == 1)
            {
                cara = 0;
                comp = 5;
                for (int x = bitRecuperer.Count - 6; x < bitRecuperer.Count; x++)
                {
                    if (comp == 0)
                    {
                        cara += (int)Math.Pow(2, comp) * bitRecuperer[x];
                        caraNumList.Add(cara);
                    }
                    else
                    {
                        cara += (int)Math.Pow(2, comp) * bitRecuperer[x];
                        comp--;
                    }
                }
                int car2;
                for (int x = 0; x <= caraNumList.Count - 2; x++)
                {
                    car2 = caraNumList[x] % 45;
                    texteQRcode.Append(QRCode.ValeurCara[(caraNumList[x] - car2) / 45] + "" + QRCode.ValeurCara[car2]);
                }
                texteQRcode.Append(QRCode.ValeurCara[caraNumList[caraNumList.Count - 1] % 45]);
            }
            else
            {
                int car2;
                for (int x = 0; x < caraNumList.Count; x++)
                {
                    car2 = caraNumList[x] % 45;
                    texteQRcode.Append(QRCode.ValeurCara[(caraNumList[x] - car2) / 45] + "" + QRCode.ValeurCara[car2]);
                }
            }
            return "Décodage image BMP : " + texteQRcode.ToString();
        }
        /// <summary>
        /// Créée un repère ou on ajoute au HashSet pour savoir les partie du QR code qu'il ne faut pas lire
        /// </summary>
        /// <param name="bitANePasRecuperer">le HashSet auquel on ajoute les bits a ne pas lire</param>
        /// <param name="centreX">
        /// Le centre d'axe X du repère 
        /// </param>
        /// <param name="centreY">
        /// Le centre d'axe Y du repère
        private void CreationRepereANePasPrendre(HashSet<string> bitANePasRecuperer, int centreX, int centreY)
        {
            for (int i = -4; i < 5; i++)
                for (int j = -4; j < 5; j++)
                {
                    int coordX = i + centreX, coordY = j + centreY;

                    if (coordX >= 0
                        && coordY >= 0
                        && coordX < module.GetLength(0)
                        && coordY < module.GetLength(1))
                    {
                        bitANePasRecuperer.Add(coordX + "," + coordY);
                    }
                }
        }
        /// <summary>
        /// Créée un repère plus petit que l'on ajoute au HashSet pour savoir les partie du QR code qu'il ne faut pas lire
        /// </summary>
        /// <param name="bitANePasRecuperer">le HashSet auquel on ajoute les bits a ne pas lire</param>
        /// <param name="centreX">
        /// Le centre d'axe X du repère 
        /// </param>
        /// <param name="centreY">
        /// Le centre d'axe Y du repère
        private void CreationRepereSecANePasPrendre(HashSet<string> bitANePasRecuperer, int centreX, int centreY)
        {
            for (int i = -2; i < 3; i++)
                for (int j = -2; j < 3; j++)
                {
                    int coordX = i + centreX, coordY = j + centreY;

                    if (coordX >= 0
                        && coordY >= 0
                        && coordX < module.GetLength(0)
                        && coordY < module.GetLength(1))
                    {
                        bitANePasRecuperer.Add(coordX + "," + coordY);
                    }
                }
        }
    }
}
