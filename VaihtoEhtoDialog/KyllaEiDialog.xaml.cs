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

namespace VaihtoEhtoDialog
{
    /// <summary>s
    /// OK - Peruutatyyppinen-dialog, jolla käyttäjältä kysytään varmistus että hän haluaa tuhota valitut tilitapahtumat
    /// </summary>
    public partial class KyllaEiDialog : Window
    {

        /// <summary>
        /// Konstruktori dialogille
        /// </summary>
        public KyllaEiDialog()
        {
            VaihtoEhtoDialog.Properties.Resources.Culture = new CultureInfo(VaihtoEhtoDialog.Properties.Settings.Default.Kieli);
            InitializeComponent();
            HaeKenttiinNimet();
        }


        /// <summary>
        /// Haetaan kenttiin kielen mukaan oikeat tekstit
        /// </summary>
        private void HaeKenttiinNimet()
        {
            TextBlockTeksti.Text = Properties.Resources.TextBlockKysymys;
            BtnOK.Content = Properties.Resources.BtnOK;
            BtnPeruuta.Content = Properties.Resources.BtnPeruuta;
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
}
