using QR_Code_final.ReedSolomon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QR_Code_final.Classes
{
    /// <summary>
    /// Classe permettant de généré la liste de bit d'information a rentrer dans le QR code
    /// </summary>
    internal class QRCode
    {
        #region Attributs
        private static Dictionary<char, int> caraValeur = new Dictionary<char, int>();
        private static Dictionary<int, char> valeurCara = new Dictionary<int, char>();
        private string texteEntree;
        private byte version;
        private int nbOctetsFin = 0;
        private List<byte> dataBit = new List<byte>();
        private List<byte> dataOctet = null;
        private List<byte> dataCorrecteur = new List<byte>();
        #endregion

        #region Constructeurs
        public QRCode(string texteEntree)
        {
            LectureCara();
            this.texteEntree = texteEntree;

            ChoixVersion();
            InitData(TransformationTxtToInt(texteEntree));
        }
        #endregion
        public static Dictionary<int, char> ValeurCara
        { get { return valeurCara; } }

        public List<byte> DataBit
        { get { return dataBit; } }

        public List<byte> DataOctet
        { get { return dataOctet; } }

        public List<byte> DataCorrecteur
        { get { return dataCorrecteur; } }

        public Byte Version
        { get { return version; } }

        #region Méthodes
        /// <summary>
        /// Uniquement pour Level : L
        /// </summary>
        /// <returns></returns>
        private int ChoixVersion()
        {
            int nbEC = 0;
            switch (texteEntree.Length)
            {
                case <= 25: nbEC = 7; nbOctetsFin = 19; version = 1; break;    //V1
                case <= 47: nbEC = 10; nbOctetsFin = 34; version = 2; break;   //V2
                case <= 77: nbEC = 15; nbOctetsFin = 55; version = 3; break;   //V3
                case <= 114: nbEC = 20; nbOctetsFin = 80; version = 4; break;  //V4
                case <= 154: nbEC = 26; nbOctetsFin = 108; version = 5; break;  //V5
            }
            return nbEC;
        }

        /// <summary>
        /// Lit les caractères disponibles dans le fichier "CaraAutoriserFichier.csv" et leur valeur équivalente, et les ajoute aux Dictionary caraValeur et valeurCara.
        /// </summary>
        private static void LectureCara()
        {
            //Ici ne parcoure le fichier que si le fichier n'a jamais été lue
            //par contre si le fichier a déjà été lue une autre fois pour n'importe quel
            //QR code comme caraValeur et valeurCara sont static alors il n'y a pas besoin de 
            //réinsérrer les valeurs a l'intèrieur a chaque fois.
            if (caraValeur.Count != 0)
                return;
            StreamReader lectureFichier = new StreamReader("CaraAutoriserFichier.csv");
            string[] donnees = new string[2];
            while (lectureFichier.Peek() > 0)
            {
                donnees = lectureFichier.ReadLine().Split(";");
                caraValeur.Add(Convert.ToChar(donnees[0]), Convert.ToInt32(donnees[1]));
                valeurCara.Add(Convert.ToInt32(donnees[1]), Convert.ToChar(donnees[0]));
            }
        }

        /// <summary>
        /// Permet de transformé chaque caractère du texte en entrée en ça valeur dans le Dictionary caraValeur.
        /// </summary>
        /// <param name="texteEntree">le texte que l'on veut transformé en QR code</param>
        /// <returns></returns>
        public int[] TransformationTxtToInt(string texteEntree)
        {
            string texteUpper = texteEntree.ToUpper();
            List<int> retour = new List<int>();

            foreach (char car in texteUpper)
            {
                retour.Add(caraValeur[car]);
            }
            return retour.ToArray();
        }

        /// <summary>
        /// Permet de généré les listes de bit :
        /// dataCorrecteur qui contient les bits de correction d'érreur et 
        /// dataBit qui contient le texte transformée en binaire.
        /// </summary>
        /// <param name="caraValAscii">les valeur entière des caractère du texte</param>
        #region Data
        public void InitData(int[] caraValAscii)
        {
            Entete();
            for (int i = 0; i < caraValAscii.Length; i++)
            {
                if (caraValAscii.Length - i >= 2)
                {
                    EncodageCara(caraValAscii[i], caraValAscii[i + 1]);
                    i++;
                }
                else
                    EncodageCara(caraValAscii[i]);
            }
            CompletionData();
            dataOctet = ConvertionBitAOctet(dataBit);

            byte[] correctionErreur = ReedSolomonAlgorithm.Encode(dataOctet.ToArray(), ChoixVersion(), ErrorCorrectionCodeType.QRCode);
            foreach (byte val in correctionErreur)
            {
                dataOctet.Add(val);
                dataCorrecteur.AddRange(ConvertionOctetABit(val));
            }
            //Ici on ajoute les bits manquant pour les version entre 2 et 6 car sinon ils manques 
            //des bits au QRcode
            if (texteEntree.Length > 25)
            {
                dataCorrecteur.AddRange([0, 0, 0, 0, 0, 0, 0]);
            }
        }

        /// <summary>
        /// Les données qui sont au tout début du QR code (le mode et la taille du texte en entrée)
        /// </summary>
        private void Entete()
        {
            //Mode AlphaNumérique
            dataBit.AddRange([0, 0, 1, 0]);

            //taille du texte en entrée
            byte[] bitTaille = new byte[9];
            int tailleTexte = texteEntree.Length;
            for (int i = 8; i >= 0; i--)
            {
                bitTaille[i] = (byte)(tailleTexte % 2);
                tailleTexte /= 2;
            }
            dataBit.AddRange(bitTaille);
        }
        /// <summary>
        /// Permet de transfomé les valeur des caractère en base 45
        /// </summary>
        /// <param name="cara1">représente les "dizaines"</param>
        /// <param name="cara2">représente les "unités"</param>
        private void EncodageCara(int cara1, int cara2)
        {
            int cara = cara1 * 45 + cara2;

            Stack<byte> tempData = new Stack<byte>();

            while (cara > 0)
            {
                tempData.Push((byte)(cara % 2));
                cara /= 2;
            }

            while (tempData.Count < 11)
            {
                tempData.Push(0);
            }
            dataBit.AddRange(tempData);
        }
        /// <summary>
        /// Permet de transfomé la valeur du caractère en base 45 on utilise cette méthode que lorsque le nombre de caractère est impair
        /// </summary>
        /// <param name="cara1">représente les "dizaines"</param>
        /// <param name="cara2">représente les "unités"</param>
        private void EncodageCara(int cara)
        {
            Stack<byte> tempData = new Stack<byte>();

            while (cara > 0)
            {
                tempData.Push((byte)(cara % 2));
                cara /= 2;
            }

            while (tempData.Count < 6)
            {
                tempData.Push(0);
            }
            dataBit.AddRange(tempData);
        }
        /// <summary>
        /// Une fois que le texte a été transformé en bit il faut ajouté les bit de completion pour avoir le bon nombre de bit pour la taille du QR code
        /// </summary>
        private void CompletionData()
        {
            if (nbOctetsFin * 8 - 4 > dataBit.Count)
                dataBit.AddRange([0, 0, 0, 0]);

            while (dataBit.Count % 8 != 0)
            {
                dataBit.Add(0);
            }

            int nbOctet = dataBit.Count / 8;

            for (int i = 0; i + nbOctet < nbOctetsFin; i++)
            {
                if (i % 2 == 0)
                    // 11101100 : 236
                    dataBit.AddRange([1, 1, 1, 0, 1, 1, 0, 0]);
                else
                    // 00010001 : 17
                    dataBit.AddRange([0, 0, 0, 1, 0, 0, 0, 1]);
            }
        }

        /// <summary>
        /// Permet de transformé une liste de bit en une liste d'octet
        /// </summary>
        /// <returns></returns>
        public static List<byte> ConvertionBitAOctet(List<byte> bit)
        {
            List<byte> dataOctet = new List<byte>();
            for (int i = 0; i < bit.Count / 8; i++)
            {
                dataOctet.Add(
                    (byte)
                    (bit[i * 8] << 7 |
                    bit[i * 8 + 1] << 6 |
                    bit[i * 8 + 2] << 5 |
                    bit[i * 8 + 3] << 4 |
                    bit[i * 8 + 4] << 3 |
                    bit[i * 8 + 5] << 2 |
                    bit[i * 8 + 6] << 1 |
                    bit[i * 8 + 7]));
            }
            return dataOctet;
        }

        /// <summary>
        /// Permet de convertir un octet en ces 8 composantes binaire.
        /// </summary>
        /// <param name="val">l'octet a décomposer</param>
        /// <returns>Composantes binaire sous forme de List<byte></returns>
        public List<byte> ConvertionOctetABit(byte val)
        {
            List<byte> composantesBin = new List<byte>();
            int valModifier = val;
            for (int i = 7; i >= 0; i--)
            {
                if (valModifier / (int)Math.Pow(2, i) == 1)
                {
                    composantesBin.Add(1);
                    valModifier -= (int)Math.Pow(2, i);
                }
                else
                    composantesBin.Add(0);
            }
            return composantesBin;
        }
        #endregion
        #endregion
    }
}
