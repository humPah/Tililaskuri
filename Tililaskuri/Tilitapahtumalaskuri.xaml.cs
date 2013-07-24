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
using UusiKysely;
using MaksujenParseri;
using VaihtoEhtoDialog;
using TulostinApu;
using KolmenVaihtoehdonDialog;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.IO;


namespace Tililaskuri
{
    /// <summary>
    /// @author Matias Partanen
    /// @version 02.12.2012
    /// Tililaskuri-ohjelma, jossa voi lisäillä maksutapahtumia ja pitää kirjaa tuloistaan/menoistaan
    /// </summary>
    public partial class Tilitapahtumalaskuri : Window
    {
        /// Attribuutteina hieman propertyjä, joissa pidetään säiliötä tilitapahtumista 
        /// <summary>
        /// Lista kaikista tapahtumista, mitä käyttäjä on syöttänyt
        /// </summary>
        private ObservableCollection<Tilitapahtuma> _ListaTapahtumat;
        public ObservableCollection<Tilitapahtuma> ListaTapahtumat
        {
            get { return _ListaTapahtumat; }
            set { _ListaTapahtumat = value; }
        }
        /// <summary>
        /// Lista käyttäjän syöttämistä tilitapahtumista, mitkä auki olevalla hakuehdolla ovat näkyvillä
        /// </summary>
        private ObservableCollection<Tilitapahtuma> _ListaNayttoTapahtumat;
        public ObservableCollection<Tilitapahtuma> ListaNayttoTapahtumat
        {
            get { return _ListaNayttoTapahtumat; }
        }

        /// <summary>
        /// Konstruktori, joka pääasiassa kutsuu vain XAML-koodia luodessaan käyttöliittymää
        /// </summary>
        public Tilitapahtumalaskuri()
        {
            _ListaTapahtumat = new ObservableCollection<Tilitapahtuma>();
            _ListaNayttoTapahtumat = new ObservableCollection<Tilitapahtuma>();
            InitializeComponent();
            keskiDockPanel.Children.Remove(gridProgressBar);
            wrapPanelYla.Children.Clear();
            wrapPanelYla.Children.Add(ComboBoxHae);
            wrapPanelYla.Children.Add(TextBoxKohde);
            wrapPanelYla.Children.Add(BtnHae);
            wrapPanelYla.Children.Add(CheckBoxHaku);
            foreach (Tilitapahtuma t in _ListaTapahtumat)
            {
                _ListaNayttoTapahtumat.Add(t);
            }

            PaivitaTulotMenot();

            ///Lisätään Bindingit tietyille komennoille
            CommandBinding OpenCmdBinding = new CommandBinding(
                ApplicationCommands.Open,
                SuoritaAvaaKomento);
            this.CommandBindings.Add(OpenCmdBinding);

            CommandBinding SaveCmdBinding = new CommandBinding(
                ApplicationCommands.Save,
                SuoritaTallennaKomento);
            this.CommandBindings.Add(SaveCmdBinding);

            CommandBinding PrintCmdBinding = new CommandBinding(
                ApplicationCommands.Print,
                SuoritaTulostaKomento);
            this.CommandBindings.Add(PrintCmdBinding);

            CommandBinding CloseCmdBinding = new CommandBinding(
                ApplicationCommands.Close,
                SuoritaSuljeKomento);
            this.CommandBindings.Add(CloseCmdBinding);

            HaeKenttiinNimet();
            if (Tililaskuri.Properties.Settings.Default.Kieli.Equals("fi-FI"))
            {
                RadioButtonSuomi.IsChecked = true;
            }
            else
            {
                RadioButtonEnglanti.IsChecked = true;
            }
        }


        /// <summary>
        /// Laitetaan kaikkiin kenttiin oikeat nimet riippuen siitä mikä kieli on valittu
        /// Auttaa, että kieltä voi vaihtaa lennosta
        /// </summary>
        private void HaeKenttiinNimet()
        {
            MenuTiedosto.Header = "_" + Properties.Resources.MenuTiedosto;
            MenuMuokkaa.Header = "_" + Properties.Resources.MenuMuokkaa;
            MenuApua.Header = "_" + Properties.Resources.MenuApua;

            MenuItemAvaa.Header = "_" + Properties.Resources.MenuItemAvaa;
            MenuItemTallenna.Header = "_" + Properties.Resources.MenuItemTallenna;
            MenuItemTulosta.Header = "_" + Properties.Resources.MenuItemTulosta;
            MenuItemSulje.Header = "_" + Properties.Resources.MenuItemSulje;
            MenuItemLisaaMaksu.Header = "_" + Properties.Resources.MenuLisääMaksu;
            MenuItemLisaaMaksuja.Header = "_" + Properties.Resources.MenuLisääMaksuja;
            MenuItemPoistaMaksuja.Header = "_" + Properties.Resources.MenuPoistaMaksuja;
            
            MenuItemKieli.Header = "_" + Properties.Resources.MenuItemKieli;
            MenuItemOhje.Header = "_" + Properties.Resources.MenuItemOhje;

            LabelMenotTeksti.Content = Properties.Resources.LabelMenot;
            LabelTulotTeksti.Content = Properties.Resources.LabelTulot;
            LabelYhteensaTeksti.Content = Properties.Resources.LabelYhteensa;
            LabelEdistymispalkki.Content = Properties.Resources.LabelEdistymisPalkki;

            CheckBoxPalkki.Content = Properties.Resources.CheckBoxEdistymispalkki;
            CheckBoxHaku.Content = Properties.Resources.CheckBoxHaeEhdolla;

            DataGridTilitapahtumat.Columns[0].Header = Properties.Resources.DataGridHeaderKohde;
            DataGridTilitapahtumat.Columns[1].Header = Properties.Resources.DataGridHeaderPvm;
            DataGridTilitapahtumat.Columns[2].Header = Properties.Resources.DataGridHeaderSumma;
            DataGridTilitapahtumat.Columns[3].Header = Properties.Resources.DataGridHeaderSelitys;
            DataGridTilitapahtumat.Columns[4].Header = Properties.Resources.DataGridHeaderTilinumero;

            CbItemKohde.Content = Properties.Resources.ComboBoxItemKohde;
            CbItemPvm.Content = Properties.Resources.ComboBoxItemPvm;
            CbItemSumma.Content = Properties.Resources.ComboBoxItemSumma;
            CbItemSelitys.Content = Properties.Resources.ComboBoxItemSelitys;
            CbItemTilinumero.Content = Properties.Resources.ComboBoxItemTilinumero;

            BtnHae.Content = Properties.Resources.ButtonHae;
            BtnEdistymispalkki.Content = Properties.Resources.ButtonEdistymisPalkki;

            DataGridMenuItemMaksu.Header = Properties.Resources.DataGridMenuLisääMaksu;
            DataGridMenuItemMaksuja.Header = Properties.Resources.DataGridMenuLisääMaksuja;
            DataGridMenuItemPoista.Header = Properties.Resources.DataGridMenuPoistaMaksuja;
        }


        /// <summary>
        /// Kysytään käyttäjältä haluaako hän tallentaa nykyiset auki olevat tilitapahtumat, jonka jälkeen avataan OpenFileDialog, jossa käyttäjä hakee haluamansa
        /// .las-päättyvän Tililaskuri-tiedoston, jonka jälkeen nykyinen lista tilitapahutmista tyhjennetään ja tiedostosta parsetaan tapahtumat kyseiseen listaan,
        /// päivittäen lopulta tapahtumat datagrid-elementtiin
        /// </summary>
        /// <param name="target">Tapahtuman kutsuja, tässä tapauksessa Tililaskuri</param>
        /// <param name="e">Tapahtuman eventit, ei tarvita</param>
        private void SuoritaAvaaKomento(object target, ExecutedRoutedEventArgs e)
        {

            KyllaEiPeruutaDialog dialog = new KyllaEiPeruutaDialog();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                if (dialog.PainettiinkoPeruuta == true) return;
                SuoritaTallennaKomento(target, null);
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = ".las";
            dlg.DefaultExt = ".las";
            dlg.Filter = "Tililaskuri-tiedostot (.las)|*.las";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                ListaTapahtumat.Clear();
                string filename = dlg.FileName;
                StreamReader reader = new StreamReader(filename);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Tilitapahtuma t = new Tilitapahtuma();
                    char[] separator = new char[] { '|' };
                    string[] kentat = line.Split(separator, StringSplitOptions.None);
                    t.Kohde = kentat[0];
                    t.Päivämäärä = DateTime.Parse(kentat[1]);
                    t.Summa = Convert.ToDouble(kentat[2]);
                    t.Selitys = kentat[3];
                    t.Tilinumero = kentat[4];
                    ListaTapahtumat.Add(t);
                }

                HaeTapahtumat(this, null);
            }   
        }


        /// <summary>
        /// Avataan SaveFileDialog, jossa käyttäjä voi valita mihin tiedostoon hän haluaa tilitapahtumat tallentaa, jonka jälkeen suoritetaan tallentaminen tiedostoon
        /// </summary>
        /// <param name="target">Tapahtuman kutsuja, tässä tapauksessa Tililaskuri</param>
        /// <param name="e">Tapahtuman eventit, ei tarvita</param>
        private void SuoritaTallennaKomento(object target, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = ".las";
            dlg.DefaultExt = ".las";
            dlg.Filter = "Tililaskuri-tiedostot (.las)|*.las";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {               
                FileStream fs = (FileStream)dlg.OpenFile();
                StreamWriter writer = new StreamWriter(fs);
                foreach (Tilitapahtuma t in ListaTapahtumat)
                {                    
                    writer.Write(t.Kohde + "|");
                    writer.Write(t.Päivämäärä.Date.ToString("dd.MM.yyyy") + "|");
                    writer.Write(t.Summa + "|");
                    writer.Write(t.Selitys + "|");
                    writer.Write(t.Tilinumero + "|");
                    writer.WriteLine();
                }

                writer.Close();
            }

        }

        /// <summary>
        /// Avataan tulostusdialog, jossa käyttäjä voi valita millä tulostimella tulostus suoritetaan, jonka jälkeen tulostetaan jokaisen maksun kohdde, päivämäärä sekä summa paperille
        /// </summary>
        /// <param name="target">Tapahtuman kutsuja, tässä tapauksessa Tililaskuri</param>
        /// <param name="e">Tapahtuman eventit, ei tarvita</param>
        private void SuoritaTulostaKomento(object target, ExecutedRoutedEventArgs e)
        {
            PrintDialog dialog = new PrintDialog();
            Nullable<bool> result = dialog.ShowDialog();
            if (result == false) return;
            PrintDialog printDialog = new PrintDialog();
            FlowDocumentReader docreader = new FlowDocumentReader();
            FlowDocument flowDocument = new FlowDocument();

            foreach(Tilitapahtuma t in ListaNayttoTapahtumat)
            {
                Paragraph myParagraph = new Paragraph();
                myParagraph.Margin = new Thickness(4);
                myParagraph.FontSize = 12;                
                myParagraph.Inlines.Add(new Run(t.Kohde + "  -  " + t.Päivämäärä.Date.ToString("dd.MM.yyyy") + "  -  " + t.Summa));
                flowDocument.Blocks.Add(myParagraph);
            }


            IDocumentPaginatorSource doc = flowDocument;
            DocumentPaginatorWrapper paginator = new DocumentPaginatorWrapper(doc.DocumentPaginator, new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight), new Size(20, 20));

            printDialog.PrintDocument(paginator, this.Title);
            
        }



        /// <summary>
        /// Kysytään käyttäjältä haluaako hän tallentaa tilitapahtumat ennen sulkua Kyllä/Ei/Peruuta-dialogilla, jonka jälkeen suljetaan ohjelma mikäli ei painettu Peruuta
        /// </summary>
        /// <param name="target">Tapahtuman kutsuja, tässä tapauksessa Tililaskuri</param>
        /// <param name="e">Tapahtuman eventit, ei tarvita</param>
        private void SuoritaSuljeKomento(object target, ExecutedRoutedEventArgs e)
        {
            KyllaEiPeruutaDialog dialog = new KyllaEiPeruutaDialog();
            dialog.ShowDialog(); 
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                if (dialog.PainettiinkoPeruuta == true) return;
                SuoritaTallennaKomento(target, null);
            }
            this.Close();
        }




        /// <summary>
        /// Tyhjennetään ylhäällä oleva WrapPanel elementeistä ja pistetään Kohteelle tarvittavat combobox, haku-textboxit sekä button hakemiselle paneeliin
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void CbItemKohde_Selected(object sender, RoutedEventArgs e)
        {
            if (wrapPanelYla == null || BtnHae == null) return;
            wrapPanelYla.Children.Clear();
            wrapPanelYla.Children.Add(ComboBoxHae);
            wrapPanelYla.Children.Add(TextBoxKohde);
            wrapPanelYla.Children.Add(BtnHae);
            wrapPanelYla.Children.Add(CheckBoxHaku);
        }


        /// <summary>
        /// Tyhjennetään ylhäällä oleva WrapPanel elementeistä ja pistetään Päivämäärälle tarvittavat combobox, haku-textboxit sekä button hakemiselle paneeliin
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void CbItemPvm_Selected(object sender, RoutedEventArgs e)
        {
            wrapPanelYla.Children.Clear();
            wrapPanelYla.Children.Add(ComboBoxHae);
            wrapPanelYla.Children.Add(TextBoxPvm1);
            wrapPanelYla.Children.Add(TextBlockViiva);
            wrapPanelYla.Children.Add(TextBoxPvm2);
            wrapPanelYla.Children.Add(BtnHae);
            wrapPanelYla.Children.Add(CheckBoxHaku);
        }


        /// <summary>
        /// Tyhjennetään ylhäällä oleva WrapPanel elementeistä ja pistetään Summalle tarvittavat combobox, haku-textboxit sekä button hakemiselle paneeliin
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void CbItemSumma_Selected(object sender, RoutedEventArgs e)
        {
            wrapPanelYla.Children.Clear();
            wrapPanelYla.Children.Add(ComboBoxHae);
            wrapPanelYla.Children.Add(TextBoxNumber1);
            wrapPanelYla.Children.Add(TextBlockViiva);
            wrapPanelYla.Children.Add(TextBoxNumber2);
            wrapPanelYla.Children.Add(BtnHae);
            wrapPanelYla.Children.Add(CheckBoxHaku);
        }


        /// <summary>
        /// Tyhjennetään ylhäällä oleva WrapPanel elementeistä ja pistetään TapahtumanSelitykselle tarvittavat combobox, haku-textboxit sekä button hakemiselle paneeliin
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void CbItemSelitys_Selected(object sender, RoutedEventArgs e)
        {
            wrapPanelYla.Children.Clear();
            wrapPanelYla.Children.Add(ComboBoxHae);
            wrapPanelYla.Children.Add(TextBoxSelitys);
            wrapPanelYla.Children.Add(BtnHae);
            wrapPanelYla.Children.Add(CheckBoxHaku);
        }


        /// <summary>
        /// Tyhjennetään ylhäällä oleva WrapPanel elementeistä ja pistetään Tilinumerolle tarvittavat combobox, haku-textboxit sekä button hakemiselle paneeliin
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void CbItemTilinumero_Selected(object sender, RoutedEventArgs e)
        {
            wrapPanelYla.Children.Clear();
            wrapPanelYla.Children.Add(ComboBoxHae);
            wrapPanelYla.Children.Add(TextBoxTilinumero);
            wrapPanelYla.Children.Add(BtnHae);
            wrapPanelYla.Children.Add(CheckBoxHaku);
        }


        /// <summary>
        /// Poistetaan keskiDockPanelin elementit ja laitetaan ne tilalle niin, että gridProgressBar tulee näkyviin
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void CheckBoxPalkki_Checked(object sender, RoutedEventArgs e)
        {
            keskiDockPanel.Children.Remove(gridProgressBar);
            keskiDockPanel.Children.Remove(DataGridTilitapahtumat);
            keskiDockPanel.Children.Add(gridProgressBar);
            keskiDockPanel.Children.Add(DataGridTilitapahtumat);
        }

        /// <summary>
        /// Poistetaan keskiDockPanelista gridProgressBar näkyvistä
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void CheckBoxPalkki_Unchecked(object sender, RoutedEventArgs e)
        {
            keskiDockPanel.Children.Remove(gridProgressBar);
        }


        /// <summary>
        /// Suljetaan ohjelma
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void MenuItemLopeta_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// Luodaan uusi dialog, jossa kysytään tietoja uudelle maksulle. Mikäli käyttäjä painaa OK dialogissa, luodaan uusi maksu. Muussa tapauksessa ei tehdä mitään.
        /// Voisi tehdä "geneerisellä" konstruktorilla, mutta vaikeuttaa kielen vaihtoa, niin tehty parametrittomalla konstruktorilla
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void MenuLisaaMaksu_Click(object sender, RoutedEventArgs e)
        {
            KyselyIkkuna dialog = new KyselyIkkuna();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                _ListaTapahtumat.Add((Tilitapahtuma)dialog.Olio);
                HaeTapahtumat(this, null);
            }
        }


        /// <summary>
        /// Avataan MaksuParseri-dialog, johon käyttäjä voi syöttää tietoja, joka parseaa ne listaan, jonka jälkeen lisätään annetut Tilitapahtumat jo olemassa olevaan listaan
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void MenuLisaaMaksuja_Click(object sender, RoutedEventArgs e)
        {
            MaksuParseri dialog = new MaksuParseri();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                foreach (Tilitapahtuma t in dialog.ListaTapahtumat)
                {
                    ListaTapahtumat.Add(t);
                }

                HaeTapahtumat(sender, e);
            }
        }


        /// <summary>
        /// Kysytään käyttäjältä haluako hän varmasti poistaa valitut maksut, ja mikäli kyllä, poistetaan ne. Jos maksuja ei ole yhtään, niin ei tehdä mitään
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void MenuPoistaMaksuja_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridTilitapahtumat.SelectedItems.Count == 0) return;

            KyllaEiDialog dialog = new KyllaEiDialog();
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                List<Tilitapahtuma> valitutTapahtumat = new List<Tilitapahtuma>();

                foreach (Tilitapahtuma tapahtuma in DataGridTilitapahtumat.SelectedItems)
                {
                    valitutTapahtumat.Add(tapahtuma);
                }

                foreach (Tilitapahtuma tapahtuma in valitutTapahtumat)
                {
                    ListaTapahtumat.Remove(tapahtuma);
                }

                HaeTapahtumat(this, null);
            }
        }


        /// <summary>
        /// Haetaan hakuehdoilla tapahtumat ja listataan ne datagridin sisään
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void BtnHae_Click(object sender, RoutedEventArgs e)
        {
            HaeTapahtumat(sender, e);
        }


        /// <summary>
        /// Tyhjennetään näytöllä olevat tapahtumat ja haetaan hakuehdoilla uudet
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void HaeTapahtumat(object sender, RoutedEventArgs e)
        {
            ListaNayttoTapahtumat.Clear();
            foreach (Tilitapahtuma t in ListaTapahtumat)
            {
                ListaNayttoTapahtumat.Add(t);
            }


            if (CheckBoxHaku.IsChecked == true)
            {
                HaeYksittaisellaEhdolla(sender, e);
            }
            else
            {
                HaeKaikillaEhdoilla(sender, e);
            }



        }


        /// <summary>
        /// Haetaan yksittäisellä valitulla hakuehdolla, eli verrataan kaikki Tilitapahtumia laittaen kaikki tulokset näytettävien listalle, josta poistetaan ne jotka eivät täytä hakuehtoa
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void HaeYksittaisellaEhdolla(object sender, RoutedEventArgs e)
        {
            List<Tilitapahtuma> valitutTapahtumat = new List<Tilitapahtuma>();
            foreach (Tilitapahtuma t in ListaNayttoTapahtumat)
            {
                valitutTapahtumat.Add(t); // Ensiksi kaikki näytettävien listalle, josta poistetaan ne, jotka ei täytä ehtoa
            }

            if (CbItemKohde.IsSelected == true)
            {
                Regex regex = new Regex(".*" + TextBoxKohde.Text + ".*", RegexOptions.IgnoreCase);
                foreach (Tilitapahtuma t in valitutTapahtumat)
                {
                    Match match = regex.Match(t.Kohde);
                    if (match.Success == false) //Mikäli hakuehto ei vastaa tilitapahtuman vastaavaa, niin poistetaan listalta
                    {
                        ListaNayttoTapahtumat.Remove(t);
                    }
                }
            }

            if (CbItemPvm.IsSelected == true)
            {
                //Mikäli textboxit tyhjiä, laitetaan maximi/minimiarvot vertailukohdaksi, muuten koitetaan parsea käyttäjän syöte vertailtavaksi
                DateTime dt1;
                DateTime dt2;
                int paiva, kuukausi, vuosi, paiva2, kuukausi2, vuosi2;
                char[] separator = new char[] { '.', ':', '\\', '/' };
                string[] kentat;
                string[] kentat2;
                if (TextBoxPvm1.Text.Equals(""))
                {
                    kentat = "1.1.1".Split(separator, StringSplitOptions.None);
                }
                else
                {
                    kentat = TextBoxPvm1.Text.Split(separator, StringSplitOptions.None);
                }

                if (TextBoxPvm2.Text.Equals(""))
                {
                    kentat2 = "31.12.2099".Split(separator, StringSplitOptions.None);
                }
                else
                {
                    kentat2 = TextBoxPvm2.Text.Split(separator, StringSplitOptions.None);
                }
                try
                {
                    paiva = Convert.ToInt16(kentat[0]);
                    kuukausi = Convert.ToInt16(kentat[1]);
                    vuosi = Convert.ToInt16(kentat[2]);

                    paiva2 = Convert.ToInt16(kentat2[0]);
                    kuukausi2 = Convert.ToInt16(kentat2[1]);
                    vuosi2 = Convert.ToInt16(kentat2[2]);
                }
                catch (FormatException)
                {
                    return;
                }
                catch (IndexOutOfRangeException)
                {
                    return;
                }

                try
                {
                    dt1 = new DateTime(vuosi, kuukausi, paiva);
                    dt2 = new DateTime(vuosi2, kuukausi2, paiva2);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return;
                }


                foreach (Tilitapahtuma t in valitutTapahtumat)
                {
                    //Jos tilitapahtuman päivämäärä alle ensimmäisen tai yli toisen textboxin syötteen, niin poistetaan ne näkyvistä
                    if (dt1.CompareTo(t.Päivämäärä) > 0)
                    {
                        ListaNayttoTapahtumat.Remove(t);
                    }

                    if (dt2.CompareTo(t.Päivämäärä) < 0)
                    {
                        ListaNayttoTapahtumat.Remove(t);
                    }
                }

            }

            if (CbItemSumma.IsSelected == true)
            {
                double nro1;
                double nro2;

                try
                {
                    //Mikäli laatikot tyhjiä, laitetaan minimi- ja maksimiarvo vertailtaviin muuttujiin
                    if (TextBoxNumber1.Text.Equals("")) nro1 = Double.MinValue;
                    else nro1 = Convert.ToDouble(TextBoxNumber1.Text);

                    if (TextBoxNumber2.Text.Equals("")) nro2 = Double.MaxValue;
                    else nro2 = Convert.ToDouble(TextBoxNumber2.Text);
                }
                catch (FormatException)
                {
                    return;
                }

                foreach (Tilitapahtuma t in valitutTapahtumat)
                {
                    //Jos tapahtuman summa alle ensimmäisen textboxin syötteen tai yli toisen textboxin syötteen, niin poistetaan näytettävien listalta
                    if ((t.Summa < nro1) || (t.Summa > nro2))
                    {
                        ListaNayttoTapahtumat.Remove(t);
                    }
                }
            }

            if (CbItemSelitys.IsSelected == true)
            {
                Regex regex = new Regex(".*" + TextBoxSelitys.Text + ".*", RegexOptions.IgnoreCase);
                foreach (Tilitapahtuma t in valitutTapahtumat)
                {
                    Match match = regex.Match(t.Selitys);
                    if (match.Success == false) //Mikäli hakuehto ei vastaa tilitapahtuman vastaavaa, niin poistetaan listalta
                    {
                        _ListaNayttoTapahtumat.Remove(t); 
                    }
                }
            }

            if (CbItemTilinumero.IsSelected == true)
            {
                Regex regex = new Regex(".*" + TextBoxTilinumero.Text + ".*", RegexOptions.IgnoreCase);
                foreach (Tilitapahtuma t in valitutTapahtumat)
                {
                    Match match = regex.Match(t.Tilinumero);
                    if (match.Success == false)//Mikäli hakuehto ei vastaa tilitapahtuman vastaavaa, niin poistetaan listalta
                    {
                        _ListaNayttoTapahtumat.Remove(t);
                    }
                }
            }

            PaivitaTulotMenot();
        }


        /// <summary>
        /// Haetaan näytettävät tilitapahtumat kaikilla käyttäjän syöttämillä hakuehdoilla, mikäli ne eivät ole tyhjiä
        /// eli verrataan kaikki Tilitapahtumia laittaen ensin kaikki tulokset näytettävien listalle, josta poistetaan ne jotka eivät täytä hakuehtoja
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void HaeKaikillaEhdoilla(object sender, RoutedEventArgs e)
        {

            List<Tilitapahtuma> valitutTapahtumat = new List<Tilitapahtuma>();
            foreach (Tilitapahtuma t in ListaNayttoTapahtumat)
            {
                valitutTapahtumat.Add(t); // Ensiksi kaikki näytettävien listalle, josta poistetaan ne, jotka ei täytä ehtoja
            }


            if (TextBoxKohde.Text.Equals("") != true)
            {
                Regex regex = new Regex(".*" + TextBoxKohde.Text + ".*", RegexOptions.IgnoreCase);
                foreach (Tilitapahtuma t in valitutTapahtumat)
                {
                    Match match = regex.Match(t.Kohde);
                    if (match.Success == false)//Mikäli hakuehto ei vastaa tilitapahtuman vastaavaa, niin poistetaan listalta
                    {
                        ListaNayttoTapahtumat.Remove(t);
                    }
                }
            }

            if ((TextBoxPvm1.Text.Equals("") != true) || (TextBoxPvm2.Text.Equals("") != true))
            {
                DateTime dt1;
                DateTime dt2;
                int paiva, kuukausi, vuosi, paiva2, kuukausi2, vuosi2;
                string[] kentat, kentat2;
                char[] separator = new char[] { '.', ':', '\\', '/' };
                if (TextBoxPvm1.Text.Equals("")) //Mikäli textboxeista toinen on tyhjä, laitetaan maximi/minimiarvot vertailukohdaksi, muuten koitetaan parsea käyttäjän syöte vertailtavaksi
                {
                    kentat = "1.1.1".Split(separator, StringSplitOptions.None);
                }
                else
                {
                    kentat = TextBoxPvm1.Text.Split(separator, StringSplitOptions.None);
                }

                if (TextBoxPvm2.Text.Equals(""))
                {
                    kentat2 = "31.12.2099".Split(separator, StringSplitOptions.None);
                }
                else
                {
                    kentat2 = TextBoxPvm2.Text.Split(separator, StringSplitOptions.None);
                }

                try
                {
                    paiva = Convert.ToInt16(kentat[0]);
                    kuukausi = Convert.ToInt16(kentat[1]);
                    vuosi = Convert.ToInt16(kentat[2]);

                    paiva2 = Convert.ToInt16(kentat2[0]);
                    kuukausi2 = Convert.ToInt16(kentat2[1]);
                    vuosi2 = Convert.ToInt16(kentat2[2]);
                }
                catch (FormatException)
                {
                    return;
                }
                catch (IndexOutOfRangeException)
                {
                    return;
                }

                try
                {
                    dt1 = new DateTime(vuosi, kuukausi, paiva);
                    dt2 = new DateTime(vuosi2, kuukausi2, paiva2);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return;
                }


                foreach (Tilitapahtuma t in valitutTapahtumat)
                {
                    //Jos tilitapahtuman päivämäärä alle ensimmäisen tai yli toisen textboxin syötteen, niin poistetaan ne näkyvistä
                    if (dt1.CompareTo(t.Päivämäärä) > 0)
                    {
                        ListaNayttoTapahtumat.Remove(t);
                    }

                    if (dt2.CompareTo(t.Päivämäärä) < 0)
                    {
                        ListaNayttoTapahtumat.Remove(t);
                    }
                }

            }

            if ((TextBoxNumber1.Text.Equals("") != true) || (TextBoxNumber2.Text.Equals("") != true))
            {
                double nro1;
                double nro2;

                try
                {
                    //Mikäli laatikoista toinen on tyhjä, laitetaan minimi- ja maksimiarvo vertailtaviin muuttujiin
                    if (TextBoxNumber1.Text.Equals("")) nro1 = Double.MinValue;
                    else nro1 = Convert.ToDouble(TextBoxNumber1.Text);

                    if (TextBoxNumber2.Text.Equals("")) nro2 = Double.MaxValue;
                    else nro2 = Convert.ToDouble(TextBoxNumber2.Text);
                }
                catch (FormatException)
                {
                    return;
                }

                foreach (Tilitapahtuma t in valitutTapahtumat)
                {
                    if ((t.Summa < nro1) || (t.Summa > nro2)) //Jos tapahtuman summa alle ensimmäisen textboxin syötteen tai yli toisen textboxin syötteen, niin poistetaan näytettävien listalta
                    {
                        ListaNayttoTapahtumat.Remove(t);
                    }
                }
            }

            if (TextBoxSelitys.Text.Equals("") != true)
            {
                Regex regex = new Regex(".*" + TextBoxSelitys.Text + ".*", RegexOptions.IgnoreCase);
                foreach (Tilitapahtuma t in valitutTapahtumat)
                {
                    Match match = regex.Match(t.Selitys);
                    if (match.Success == false)//Mikäli hakuehto ei vastaa tilitapahtuman vastaavaa, niin poistetaan listalta
                    {
                        _ListaNayttoTapahtumat.Remove(t);
                    }
                }
            }

            if (TextBoxTilinumero.Text.Equals("") != true)
            {
                Regex regex = new Regex(".*" + TextBoxTilinumero.Text + ".*", RegexOptions.IgnoreCase);
                foreach (Tilitapahtuma t in valitutTapahtumat)
                {
                    Match match = regex.Match(t.Tilinumero);
                    if (match.Success == false)//Mikäli hakuehto ei vastaa tilitapahtuman vastaavaa, niin poistetaan listalta
                    {
                        _ListaNayttoTapahtumat.Remove(t);
                    }
                }
            }

            PaivitaTulotMenot();
        }


        /// <summary>
        /// Päivitetään tulot ja menot laskemalla ne näkyvillä olevista tilitapahtumista, ja päivitetään ne sitten oikeisiin kenttiin
        /// </summary>
        private void PaivitaTulotMenot()
        {
            double tulot = 0, menot = 0, yhteensa = 0;
            foreach (Tilitapahtuma t in ListaNayttoTapahtumat)
            {
                double summa = t.Summa;
                if (summa > 0)
                {
                    tulot = tulot + summa;                    
                }
                else
                {
                    menot = menot + summa;
                }
                yhteensa = yhteensa + summa;
            }

            LabelMenot.Content = String.Format("{0:0.00}", menot); ;
            LabelTulot.Content = String.Format("{0:0.00}", tulot);
            LabelYhteensa.Content = String.Format("{0:0.00}", yhteensa);      
            if (yhteensa >= 0)
            {
                LabelYhteensa.Foreground = Brushes.Green;
            }
            else
            {
                LabelYhteensa.Foreground = Brushes.Red;
            }


            double luku = 0;
            try
            {
                luku = Convert.ToDouble(TextBoxEdistymispalkki.Text);
                progressBarMenot.Maximum = luku;
            }
            catch (FormatException)
            {
                progressBarMenot.Maximum = 1000;
            }

            progressBarMenot.Value = Math.Abs(menot);
        }
        
        
        /// <summary>
        /// Mikäli yksittäiseen elementtiin tulee muutoksia, niin päivitetään summat tuloista ja menoista
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void DataGridTilitapahtumat_CurrentCellChanged(object sender, EventArgs e)
        {
            PaivitaTulotMenot();
        }


        /// <summary>
        /// Mikäli painetaan progressbarin nappia, niin päivitetään tulot ja menot
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void BtnProgressBar_Click(object sender, RoutedEventArgs e)
        {
            PaivitaTulotMenot();
        }


        /// <summary>
        /// Katsotaan onko ylhäällä olevien hakukentän textboxien päällä tapahtuneen napin painallus ollut enter, ja jos on, suoritettaan haku
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void TextBoxHaku_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                HaeTapahtumat(sender, null);
            }
        }


        /// <summary>
        /// Katsotaan progressbarin vieressä olevan textboxin päällä tapahtuneen napin painallus ollut enter, ja jos on, niin päivitetään tulot ja menot
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void TextBoxProgressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PaivitaTulotMenot();
            }
        }


        /// <summary>
        /// Skaalataan ikkunan koon muuttuessa progressbaarin koko ikkunan kokoon sopivammaksi
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void ikkuna_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            progressBarMenot.Height = this.Height-200;
        }


        /// <summary>
        /// Katsotaan onko datagridin päällä painettu nappi ollut delete, jos on, niin suoritetaan valittujen maksujen poisto
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void DataGridTilitapahtumat_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                MenuPoistaMaksuja_Click(sender, null);
            }
        }

        
        /// <summary>
        /// Kieli Suomi valittu, laitetaan tälle ja dialogeille kieleksi fi-FI ja päivitetään kenttiin oikeat nimet
        /// Järkevämpi tapa varmaan olisi tehdä yhteinen luokka jolla Resources, josta kaikki saisi ja tarvitsisi muuttaa vain yhtä, mutta toimii tälläkin tavalla
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void RadioButtonSuomi_Click(object sender, RoutedEventArgs e)
        {
            Tililaskuri.Properties.Resources.Culture = new CultureInfo("fi-FI");
            HaeKenttiinNimet();
            Tililaskuri.Properties.Settings.Default.Kieli = "fi-FI";
            Properties.Settings.Default.Save();
            KolmenVaihtoehdonDialog.Properties.Settings.Default.Kieli = "fi-FI";
            KolmenVaihtoehdonDialog.Properties.Settings.Default.Save();
            MaksujenParseri.Properties.Settings.Default.Kieli = "fi-FI";
            MaksujenParseri.Properties.Settings.Default.Save();
            VaihtoEhtoDialog.Properties.Settings.Default.Kieli = "fi-FI";
            VaihtoEhtoDialog.Properties.Settings.Default.Save();
            UusiMaksu.Properties.Settings.Default.Kieli = "fi-FI";
            UusiMaksu.Properties.Settings.Default.Save();
        }


        /// <summary>
        /// Kieli Suomi valittu, laitetaan tälle ja dialogeille kieleksi en-US ja päivitetään kenttiin oikeat nimet
        /// Järkevämpi tapa varmaan olisi tehdä yhteinen luokka jolla Resources, josta kaikki saisi ja tarvitsisi muuttaa vain yhtä, mutta toimii tälläkin tavalla
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void RadioButtonEnglanti_Click(object sender, RoutedEventArgs e)
        {
            Tililaskuri.Properties.Resources.Culture = new CultureInfo("en-US");
            HaeKenttiinNimet();
            Tililaskuri.Properties.Settings.Default.Kieli = "en-US";
            Properties.Settings.Default.Save();
            KolmenVaihtoehdonDialog.Properties.Settings.Default.Kieli = "en-US";
            KolmenVaihtoehdonDialog.Properties.Settings.Default.Save();
            MaksujenParseri.Properties.Settings.Default.Kieli = "en-US";
            MaksujenParseri.Properties.Settings.Default.Save();
            VaihtoEhtoDialog.Properties.Settings.Default.Kieli = "en-US";
            VaihtoEhtoDialog.Properties.Settings.Default.Save();
            UusiMaksu.Properties.Settings.Default.Kieli = "en-US";
            UusiMaksu.Properties.Settings.Default.Save();
        }


        /// <summary>
        /// Avataan WWW-selain ja sieltä ohje, sivut työn alla
        /// </summary>
        /// <param name="sender">Kutsuja joka kutsui eventtiä</param>
        /// <param name="e">Tapahtuman argumentit, ei tarvita</param>
        private void MenuItemOhje_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://users.jyu.fi/~maemanpa/gko/");
        }

    }


    /// <summary>
    /// ValidationRule-luokasta peritty luokka, jossa tarkistetaan onko summa numero ja palautetaan ValidResult tai InvalidResult riippuen tuloksesta
    /// </summary>
    public class RealNumberRule : ValidationRule
    {
        /// <summary>
        /// Katsotaan pystyykö annetun syötteen muuttamaan luvuksi, jos kyllä, niin palautetaan ValidResult
        /// </summary>
        /// <param name="value">arvo, jota tutkitaan</param>
        /// <param name="cultureInfo">järjestelmän asetuksia, ei tarvita tässä</param>
        /// <returns>ValidationResult onko syöte validi vai ei</returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double luku = 0;
            try
            {
                luku = Convert.ToDouble(value);
            }
            catch (FormatException)
            {
                return new ValidationResult(false, "Summa ei ole numero!");
            }
            return ValidationResult.ValidResult;

        }
    }


    /// <summary>
    /// ValidationRule-luokasta peritty luokka, jossa tarkistetaan onko annettu syöte oikea päivämäärä ja palautetaan ValidResult  mikäli on
    /// </summary>
    public class RealDateRule : ValidationRule
    {
        /// <summary>
        /// Tarkistetaan pystyykö syötteen muuttamaan päivämääräksi, palauttaen validresult mikäli pystyy
        /// </summary>
        /// <param name="value">arvo, jota tutkitaan</param>
        /// <param name="cultureInfo">järjestelmän asetuksia, ei tarvita</param>
        /// <returns> ValidationResult onko syöte validi vai ei</returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string tekstikentta = (string)value;            
            int paiva, kuukausi, vuosi;
            char[] separator = new char[] { '.', ':' };
            string[] kentat = tekstikentta.Split(separator, StringSplitOptions.None);
            try
            {
                paiva = System.Convert.ToInt16(kentat[0]);
                kuukausi = System.Convert.ToInt16(kentat[1]);
                vuosi = System.Convert.ToInt16(kentat[2]);
            }
            catch (FormatException)
            {
                return new ValidationResult(false, "Epäkelpo päivämäärä! Anna päivä, kuukausi sekä vuosi muodossa 12.10.2012");
            }
            catch (IndexOutOfRangeException)
            {
                return new ValidationResult(false, "Epäkelpo päivämäärä! Anna päivä, kuukausi sekä vuosi muodossa 12.10.2012");
            }
            catch (OverflowException)
            {
                return new ValidationResult(false, "Epäkelpo päivämäärä! Anna päivä, kuukausi sekä vuosi muodossa 12.10.2012");
            }

            try
            {
                DateTime dt = new DateTime(vuosi, kuukausi, paiva);
            }
            catch (ArgumentOutOfRangeException)
            {
                return new ValidationResult(false, "Epäkelpo päivämäärä! Anna päivä, kuukausi sekä vuosi muodossa 12.10.2012");
            }
            return ValidationResult.ValidResult;
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
