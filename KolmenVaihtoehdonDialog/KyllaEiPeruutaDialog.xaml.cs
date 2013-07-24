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

namespace KolmenVaihtoehdonDialog
{
    /// <summary>
    /// Kyllä/Ei/Peruuta-dialog, joka kysyy haluaako käyttäjä tallentaa muutokset ennen sulkemista ja laittaa buttonin jokaiselle vaihtoehdolle
    /// </summary>
    public partial class KyllaEiPeruutaDialog : Window
    {
        /// <summary>
        /// Booleanmuuttuja, jolla näkee jos käyttäjä painoi peruuta
        /// </summary>
        private bool _PainettiinkoPeruuta = false;
        public bool PainettiinkoPeruuta
        {
            get { return _PainettiinkoPeruuta; }
            set { _PainettiinkoPeruuta = value; }
        }


        /// <summary>
        /// Konstruktori dialogille
        /// </summary>
        public KyllaEiPeruutaDialog()
        {
            KolmenVaihtoehdonDialog.Properties.Resources.Culture = new CultureInfo(KolmenVaihtoehdonDialog.Properties.Settings.Default.Kieli);
            InitializeComponent();
            HaeKenttiinNimet();
        }      

        
        /// <summary>
        /// Haetaan kenttiin kielen mukaan oikeat tekstit
        /// </summary>
        private void HaeKenttiinNimet()
        {
            TextBlockTeksti.Text = Properties.Resources.TextBlockTeksti;
            BtnEi.Content = Properties.Resources.ButtonEi;
            BtnKylla.Content = Properties.Resources.ButtonKylla;
            BtnPeruuta.Content = Properties.Resources.ButtonPeruuta;
        }


        /// <summary>
        /// Painettiin kyllä, niin laitetaan dialogresultiksi true ja suljetaan dialog
        /// </summary>
        /// <param name="sender">Tapahtuman kutsuja</param>
        /// <param name="e">Tapahtuman parametrit, ei tarvita</param>
        private void BtnKylla_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }


        /// <summary>
        /// Painettiin ei, niin suljetaan ainoastaan dialog
        /// </summary>
        /// <param name="sender">Tapahtuman kutsuja</param>
        /// <param name="e">Tapahtuman parametrit, ei tarvita</param>
        private void BtnEi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// Painetaan peruuta, niin muutetaan boolean-muuttuja peruutanapin painallukselle true, laitetaan dialogresult true, ja suljetaan dialogi
        /// </summary>
        /// <param name="sender">Tapahtuman kutsuja</param>
        /// <param name="e">Tapahtuman parametrit, ei tarvita</param>
        private void BtnPeruuta_Click(object sender, RoutedEventArgs e)
        {            
            PainettiinkoPeruuta = true;
            this.DialogResult = true;
            this.Close();
        }
    }
}
