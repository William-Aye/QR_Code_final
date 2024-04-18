using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;

namespace QR_Code_final
{
    internal class MyImage
    {
        private string image;
        private int tailleFichier;
        private int offset;
        private int headerInfo;
        private int largeur;
        private int hauteur;
        private int nbBitParCouleur;
        private Pixel[,] matPixel;

        public string Image { get { return image; } }
        public int TailleFichier { get { return tailleFichier; } }
        public int Offset { get { return offset; } }
        public int HeaderInfo { get { return headerInfo; } }
        public int Largeur { get { return largeur; } }
        public int Hauteur { get { return hauteur; } }
        public int NbBitParCouleur { get { return nbBitParCouleur; } }
        public Pixel[,] MatPixel { get { return matPixel; } set { matPixel = value; } }
        /// <summary>
        /// Permet de créée une MyImage a partir d'une matrice de pixel
        /// </summary>
        /// <param name="imageMat"></param>
        public MyImage(Pixel[,] imageMat)
        {
            image = "BM";
            offset = 54;
            headerInfo = 40;
            nbBitParCouleur = 24;
            matPixel = imageMat;
            largeur = imageMat.GetLength(1);
            hauteur = imageMat.GetLength(0);
            tailleFichier = offset + hauteur * largeur * 3;
        }
        /// <summary>
        /// Permet de créée une MyImage a partir d'une image BMP sauvegarder sous forme de fichier
        /// </summary>
        /// <param name="myFile"> nom du fichier. Attention a bien rajouter .bmp a la fin!!!</param>
        public MyImage(string myFile)
        {
            byte[] byteFile = System.IO.File.ReadAllBytes(myFile);
            image = "" + Convert.ToChar(byteFile[0]) + Convert.ToChar(byteFile[1]);
            tailleFichier = ConvertirEndianToInt(byteFile, 2);
            offset = ConvertirEndianToInt(byteFile, 10);
            headerInfo = ConvertirEndianToInt(byteFile, 14);
            largeur = ConvertirEndianToInt(byteFile, 18);
            hauteur = ConvertirEndianToInt(byteFile, 22);
            nbBitParCouleur = ConvertirEndianToInt2(byteFile, 28);
            matPixel = new Pixel[hauteur, largeur];
            int indic = offset;
            for (int i = 0; i < hauteur; i++)
                for (int j = 0; j < largeur; j++)
                {
                    matPixel[i, j] = new Pixel(byteFile[indic], byteFile[indic + 1], byteFile[indic + 2]);
                    indic += 3;
                }
        }

        /// <summary>
        /// Permet de transformé une MyImage en fichier BMP
        /// </summary>
        /// <param name="file">nom du fichier BMP ou l'image va être sauvegardé. Attention a bien rajouter .bmp !</param>
        public void FromImageToFile(string file)
        {
            byte[] byteFile = new byte[offset + matPixel.Length * 3];
            byteFile[0] = (byte)image[0];
            byteFile[1] = (byte)image[1];

            byte[] interByte = ConvertirIntToEndian(tailleFichier, 4);
            for (int i = 2; i < 6; i++)
            {
                byteFile[i] = interByte[i - 2];
            }
            interByte = ConvertirIntToEndian(offset, 4);
            for (int i = 10; i < 14; i++)
            {
                byteFile[i] = interByte[i - 10];
            }
            interByte = ConvertirIntToEndian(headerInfo, 4);
            for (int i = 14; i < 18; i++)
            {
                byteFile[i] = interByte[i - 14];
            }
            interByte = ConvertirIntToEndian(largeur, 4);
            for (int i = 18; i < 22; i++)
            {
                byteFile[i] = interByte[i - 18];
            }
            interByte = ConvertirIntToEndian(hauteur, 4);
            for (int i = 22; i < 26; i++)
            {
                byteFile[i] = interByte[i - 22];
            }
            interByte = ConvertirIntToEndian(nbBitParCouleur, 2);
            byteFile[28] = interByte[0];
            byteFile[29] = interByte[1];

            int indic = offset;
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    byteFile[indic] = matPixel[i, j].B;
                    byteFile[indic + 1] = matPixel[i, j].G;
                    byteFile[indic + 2] = matPixel[i, j].R;
                    indic += 3;
                }
            }
            File.WriteAllBytes(file, byteFile);
        }
        public int ConvertirEndianToInt(byte[] data, int startIndex)
        {
            int num = 0;
            for (int i = startIndex; i < startIndex + 4; i++)
            {
                num += (int)Math.Pow(256, (i - startIndex)) * data[i];
            }
            return num;
        }
        public int ConvertirEndianToInt2(byte[] data, int startIndex)
        {
            int num = 0;
            for (int i = startIndex; i < startIndex + 2; i++)
            {
                num += (int)Math.Pow(256, (i - startIndex)) * data[i];
            }
            return num;
        }
        public byte[] ConvertirIntToEndian(int val, int taille)
        {
            byte[] retour = new byte[taille];
            for (int i = 0; i < taille; i++, val /= 256)
            {
                retour[i] = (byte)(val % 256);
            }
            return retour;
        }
        public void AgrandirImage(int facteur)
        {
            hauteur *= facteur;
            largeur *= facteur;
            Pixel[,] matPixel = new Pixel[hauteur, largeur];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    matPixel[i, j] = this.matPixel[i / facteur, j / facteur];
                }
            }
            this.matPixel = matPixel;
        }
        public void ReduireImage(int facteur)
        {
            hauteur /= facteur;
            largeur /= facteur;
            Pixel[,] matArriv = new Pixel[hauteur, largeur];
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    matArriv[i, j] = matPixel[i * facteur, j * facteur];
                }
            }
            matPixel = matArriv;
        }
    }
}