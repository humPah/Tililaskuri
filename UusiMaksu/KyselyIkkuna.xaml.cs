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
using System.Globalization;
using Tilitapahtumat;

namespace UusiKysely
{
    /// <summary>
    /// 
    /// @author Matias Partanen
    /// @version 14.11.2012
    /// MaksuIkkuna-luokka, johon voi syöttää tietoja muokaten Tilitapahtuma-olion tietoja ja ottaa sen kiinni pääohjelmassa, yksin ei ole paljoa iloa
    /// @version 29.11.2012
    /// Muutettu luokkaa niin, että tukee myös muitakin kyselyitä kuin yhdenlaisia, samalla luokan nimeä muutettu KyselyIkkuna:ksi
    /// </summary>
    public partial class KyselyIkkuna : Window
    {

        private Object _Olio;
        public Object Olio
        {
            get { return _Olio; }
        }


        /// <summary>
        /// Konsturktori, luodaan uusi tilitapahtuma ja suoritetaan XAML-koodi
        /// Namespace muuttunut matkan varrella, mistä johtuu, että joutuu viittamaan vielä vanhaan tuossa kun ei osannut sitä samalla muuttaa..
        /// </summary>
        public KyselyIkkuna()
        {
            UusiMaksu.Properties.Resources.Culture = new CultureInfo(UusiMaksu.Properties.Settings.Default.Kieli);
            _Olio = new Tilitapahtuma();
            InitializeComponent();
            HaeKenttiinNimet();
        }
        

        /// <summary>
        /// Haetaan kenttiin kielen mukaan oikeat tekstit
        /// </summary>
        private void HaeKenttiinNimet()
        {
            LabelMaksu.Content = UusiMaksu.Properties.Resources.LabelTiedot;
            LabelKohde.Content = UusiMaksu.Properties.Resources.LabelKohde;            
            LabelPvm.Content = UusiMaksu.Properties.Resources.LabelPvm;
            LabelSumma.Content = UusiMaksu.Properties.Resources.LabelSumma;
            LabelSelitys.Content = UusiMaksu.Properties.Resources.LabelSelitys;            
            LabelTilinumero.Content = UusiMaksu.Properties.Resources.LabelTilinumero;
            BtnOK.Content = UusiMaksu.Properties.Resources.ButtonOK;
            BtnPeruuta.Content = UusiMaksu.Properties.Resources.ButtonPeruuta;
        }


        /// <summary>
        /// Konstruktori, joka ottaa parametrina annetun olion, reflektion avulla tutkii sen propertyt ja luo jokaiselle kysely-TextBoxUusiin Propertyn muuttujan nimellä
        /// Bindaa samalla TextBoxUusiit Propertyihin, joten kun käyttäjä painaa OK ja sulkee TextBoxUusiin saa käyttäjältä kysytyt tiedot talteen
        /// Huom ottaa luokan kaikki julkiset Propertyt käyttöönsä, ei toimi mikäli luokassa on esim. joitakin read-only propertyjä 
        /// </summary>
        /// <param name="obj">Luokan olio, jota halutaan käyttää</param>
        /// <param name="otsikkoTeksti">Yläotsikko, jonka ikkunalle haluaa antaa</param>
        public KyselyIkkuna(Object obj, String otsikkoTeksti)
        {
            InitializeComponent();
            this.Title = otsikkoTeksti;
            gridPaaIkkuna.RowDefinitions.Clear();

            _Olio = obj;
            int gridRivi = 0;
            RowDefinition rd;
            gridPaaIkkuna.Children.Clear();
            Object[] b = _Olio.GetType().GetProperties(); // Propertyt taulukkoon

            foreach (Object o in b)
            {
                rd = new RowDefinition();
                rd.Height = GridLength.Auto;    //Luodaan gridille uusi rivimäärittely, jotta rivittyvät fiksusti
                gridPaaIkkuna.RowDefinitions.Add(rd);

                Label label = new Label();
                char[] separator = new char[] { ' ' };
                string[] kentat = o.ToString().Split(separator, StringSplitOptions.None); // Yksittäisen Propertyn Luokka kentat[0], Propertyn muuttujan nimi kentat[1]
                label.Content = kentat[1];
                gridPaaIkkuna.Children.Add(label); //Luodaan label ja laitetaan gridiin
                Grid.SetRow(label, gridRivi);

                TextBox TextBoxUusi = new TextBox();
                TextBoxUusi.Margin = new System.Windows.Thickness(90, 0, 0, 0);
                TextBoxUusi.HorizontalAlignment = HorizontalAlignment.Center; // Luodaan TextBoxUusi ja laitetaan gridiin
                TextBoxUusi.Width = 180;
                gridPaaIkkuna.Children.Add(TextBoxUusi);
                Grid.SetRow(TextBoxUusi, gridRivi);
                gridRivi++;

                Binding Bindaus = new Binding("Olio." + kentat[1]);
                Bindaus.Source = this;
                Bindaus.Mode = BindingMode.TwoWay;  // Bindataan kyseinen TextBoxUusi annettuun Propertyyn
                Bindaus.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;

                //Omat converterit jos haluaa, niin pitää määritellä tässä
                if (kentat[1].Equals("Päivämäärä")) Bindaus.Converter = new DateTimeToStringConverter();
                //loppu
                TextBoxUusi.SetBinding(TextBox.TextProperty, Bindaus);
            }

            rd = new RowDefinition();
            rd.Height = GridLength.Auto;
            gridPaaIkkuna.RowDefinitions.Add(rd);
            Grid.SetRow(BtnOK, gridRivi);  //Laitetaan OK- ja peruutanapit paikoilleen
            Grid.SetRow(BtnPeruuta, gridRivi);
            gridPaaIkkuna.Children.Add(BtnOK);
            gridPaaIkkuna.Children.Add(BtnPeruuta);
        }

        /// <summary>
        /// Laitetaan dialogResultiksi true ja palataan pääohjelmaan
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
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
    }


    /// <summary>
    /// ValueConverter, jolla muutetaan DateTime merkkijonoksi muotoa DD.MM.YYYY ja toisinpäin
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class DateTimeToStringConverter : IValueConverter
    {
        /// <summary>
        /// Muutetaan DateTime String-luokan merkkijonoksi muotoa päivä.kuukausi.vuosi
        /// </summary>
        /// <param name="value">Arvo, jota tutkitaan</param>
        /// <param name="targetType">kohdetyyppi, ei tarvita</param>
        /// <param name="parameter">ei tarvita</param>
        /// <param name="culture">Järjestelmän asetuksia, ei tarvita</param>
        /// <returns>merkkijono päivämäärästä</returns>
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            DateTime dt = new DateTime();
            dt = (DateTime)value;
            return dt.Date.ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// Muutetaan merkkijono DateTime-luokan olioksi mikäli mahdollista
        /// </summary>
        /// <param name="value">Arvo, jota tutkitaan</param>
        /// <param name="targetType">Kohdetyyppi, ei tarvita</param>
        /// <param name="parameter">ei tarvita</param>
        /// <param name="culture">Järjestelmän asetuksia, ei tarvita</param>
        /// <returns>Päivämäärä DateTime-luokkana</returns>
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            string tekstikentta = (string)value;
            int paiva, kuukausi, vuosi;
            char[] separator = new char[] { '.', ':', '\\', '/' };
            string[] kentat = tekstikentta.Split(separator, StringSplitOptions.None);
            try
            {
                paiva = System.Convert.ToInt16(kentat[0]);
                kuukausi = System.Convert.ToInt16(kentat[1]);
                vuosi = System.Convert.ToInt16(kentat[2]);
            }
            catch (FormatException)
            {
                return new DateTime(1, 1, 1);
            }
            catch (IndexOutOfRangeException)
            {
                return new DateTime(1, 1, 1);
            }

            DateTime dt = new DateTime(vuosi, kuukausi, paiva);
            return dt;
        }
    }
}
