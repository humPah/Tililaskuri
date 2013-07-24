using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tilitapahtumat
{    
    
    /// <summary>
    /// @author Matias Partanen
    /// @version 14.11.2012
    /// Yksittäinen tilitapahtuma-luokka, joka lähinnä säilöö tietoa itseensä
    /// </summary>
    public class Tilitapahtuma
    {
        /// <summary>
        /// Tarvittavat propertyt, joilla säilötään tiedot ja saadaan ne myös irti tarvittaessa
        /// </summary>
        private string _Kohde;
        public string Kohde
        {
            get { return _Kohde; }
            set { this._Kohde = value; }
        }

        private DateTime _Päivämäärä;
        public DateTime Päivämäärä
        {
              get { return _Päivämäärä; }
              set { this._Päivämäärä = value; }
        }

        private double _Summa;
        public double Summa
        {
            get { return _Summa; }
            set { this._Summa = value; }
        }

        private string _Selitys;
        public string Selitys
        {
            get { return _Selitys; }
            set { this._Selitys = value; }
        }

        private string _Tilinumero;
        public string Tilinumero
        {
            get { return _Tilinumero; }
            set { this._Tilinumero = value; }
        }
        
        /// <summary>
        /// Konstruktori, ei tee paljoa eikä tarvitsekaan
        /// </summary>
        public Tilitapahtuma()
        {
            Kohde = "";
            Selitys = "";
            Tilinumero = "";
            Summa = 0;
            Päivämäärä = DateTime.Today;
        }

    }
}
