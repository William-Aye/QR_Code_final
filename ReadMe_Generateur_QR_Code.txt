ReadMe : Générateur QR Code

Bonjour, Bonsoir

Notre Code sert à générer des QR Codes. L'affichage se fait via WPF.
Nous avons réalisé un générateur qui affiche en WPF le QR Code, par rapport au texte noté à droite.
Le premier QR Code généré est "VIDE" par défault.

Ce que notre générateur effectue :
- ECL : de type L uniquement
- Version de 1 à 5 comprise
- masque 0

Le générateur s'adapte suivant les limites des versions.
Le WPF limite le nombre de caractères tappables. Cependant, le code n'a pas de sécurité si on enlève la sécurité du WPF. Cela est dû au fait que nous aurions aimé faire plus de versions et que nous n'avons fixé qu'une constante limitante.

Concernons le code final, "QRCodeAffichage.xaml.cs" sert de Main puisque toutes les classes y sont appelées et que l'affichage est fait dedans. 
Dans un premier temps, nous envoyons le "messagePlayer" dans la classe QRCode afin que celui transforme le 'string' en 'List<byte>' de Bits utiles et de Bits d'Error Correction.

Nous avons séparé les 2 catégories de Bits afin de pouvoir identifier la Version nécessaire par rapport aux données à écrire. Le code identifie la version a 2 reprises dans "QRCode" pour le choix de la version et du nombre de ECL. Mais "QRMat" a besoin de savoir quelle version est utilisée afin de mettre les bons repères et parternes.

S'agissant de l'écriture du QR Code.
La matrice est définie à '2' partout. '2' est utilisé comme étant une case vide puisque les '0' et '1' sont des données.
Ainsi, nous allons écrire en remplaçant les '2' et tout ce qui est écrit est définitif.