using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tilitapahtumat;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MaksujenParseri
{

    /// <summary>
    /// MaksujenParseri-luokka, johon voi pastea Osuuspankista tietoja   
    /// @version 4.12.2012
    /// @author Matias Partanen
    /// </summary>
    public partial class MaksuParseri : Window
    {
        /// <summary>
        /// Lista tapahtumista, jotka yritetään parsea tekstikentästä, jonka käyttäjä antaa
        /// </summary>
        private List<Tilitapahtuma> _ListaTapahtumat = new List<Tilitapahtuma>();
        public List<Tilitapahtuma> ListaTapahtumat
        {
            get { return _ListaTapahtumat; }
        }


        /// <summary>
        /// Konstruktori MaksuParserille
        /// </summary>
        public MaksuParseri()
        {
            MaksujenParseri.Properties.Resources.Culture = new CultureInfo(MaksujenParseri.Properties.Settings.Default.Kieli);
            InitializeComponent();
            HaeKenttiinNimet();
        }


        /// <summary>   
        /// Haetaan tekstikenttiin oikeat tiedot riippuen siitä mikä kieli on valittu
        /// </summary>
        private void HaeKenttiinNimet()
        {
            MenuMuokkaa.Header = "_" + Properties.Resources.MenuMuokkaa;
            MenuItemKopioi.Header = "_" + Properties.Resources.MenuItemKopioi;
            MenuItemLeikkaa.Header = "_" + Properties.Resources.MenuItemLeikkaa;
            MenuItemLiita.Header = "_" + Properties.Resources.MenuItemLiita;

            LabelMaksu.Content = Properties.Resources.LabelKopioiTeksti;
            BtnOK.Content = Properties.Resources.ButtonOK;
            BtnPeruuta.Content = Properties.Resources.ButtonPeruuta;
        }


        /// <summary>
        /// Laitetaan dialogResultiksi true ja palataan pääohjelmaan
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            ParseTapahtumat();
            this.Close();
        }


        /// <summary>
        /// Palataan pääohjelmaan dialogResultina false(oletusarvo)
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void BtnPeruuta_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// Parsetaan Osuuspankin tiedoista yksittäisiä maksuja. Tiedot joita voi parsea löytyvät kohdasta Tilit -> Verkkotiliote, ja siitä tietyn välin maksut
        /// Ei mikään erityisen hyvä komponentti, joten toimii vain tiettyjen maksujen kanssa. Omalla kuitenkin parsesi iloisesti 1.1.2012 - 30.11.2012-väliset maksut, että jotenkin toimii
        /// Poikkeuksia voi olla, sillä ei kaikenlaisia maksuja omasta historiasta löydy(esim tilisiirrot ulkomaille heittäis poikkeuksen varmaankin)
        /// Yksi rivi, josta koitetaan parsea pitäisi näyttää suunnilleen:
        /// 3.11.2012	3.11.2012	-44	106	TILISIIRTO	TNNET OY	FI29 1045 3000 1397 57 /NDEAFIHH	00000 00396 73001 60188	    20121103/593497/JC0742	
        /// </summary>
        private void ParseTapahtumat()
        {
            string[] separator = new string[] { "\r\n" };
            string[] kentatTaulukko = TextBoxSyote.Text.Split(separator, StringSplitOptions.None);
            try
            {
                foreach (string maksu in kentatTaulukko)
                {
                    if (maksu.Equals("")) return;
                    Tilitapahtuma t = new Tilitapahtuma();
                    string[] maksuTaulu = maksu.Trim().Split(' ');
                    t.Päivämäärä = DateTime.Parse(maksuTaulu[0]);
                    t.Summa = Convert.ToDouble(maksuTaulu[2]);
                    t.Selitys = maksuTaulu[4];

                    string uusi = "";
                    for (int i = 5; i < maksuTaulu.Length; i++)
                    {
                        uusi = uusi + maksuTaulu[i] + " ";
                    }

                    string[] stringSeparators = new string[] { "Viesti:" };
                    string[] uusiTaulu = uusi.Split(stringSeparators, StringSplitOptions.None);
                    if (uusiTaulu.Length > 1)
                    {
                        t.Kohde = uusiTaulu[0];
                    }
                    else
                    {
                        stringSeparators = new string[] { "FI" };
                        uusiTaulu = uusi.Split(stringSeparators, StringSplitOptions.None);
                        if (uusiTaulu.Length > 1)
                        {
                            t.Kohde = uusiTaulu[0];
                            t.Tilinumero = "FI" + uusiTaulu[1];
                        }
                    }

                    if(t.Selitys.ToLower().Equals("automaattinosto")) t.Kohde = "Automaatti";
                    if(t.Selitys.ToLower().Equals("palkka")) t.Kohde = "Palkka";

                    ListaTapahtumat.Add(t);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Ongelma maksujen kääntämisessä");
            }
        }
    
    }
}
