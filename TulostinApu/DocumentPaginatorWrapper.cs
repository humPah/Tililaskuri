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
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading;
using System.Printing;
using System.IO;

namespace TulostinApu
{
    /// <summary>
    /// Apuluokka, jota käytetään tulostamiseen flowdocumentilla
    /// </summary>
    public class DocumentPaginatorWrapper : DocumentPaginator
    {        
        Size m_PageSize;
        Size m_Margin;
        DocumentPaginator m_Paginator;
        Typeface m_Typeface;

        public DocumentPaginatorWrapper(DocumentPaginator paginator, Size pageSize, Size margin)
        {
            m_PageSize = pageSize;
            m_Margin = margin;
            m_Paginator = paginator;
            m_Paginator.PageSize = new Size(PageSize.Width - margin.Width * 2,
                                            PageSize.Height - margin.Height * 2);
            m_Typeface = new Typeface("Times New Roman");
        }

        public static void Main(string[] args)
        {
        }

        Rect Move(Rect rect)
        {
            //return rect;
            if (rect.IsEmpty)
            {
                return rect;
            }
            else
            {
                return new Rect(rect.Left + m_Margin.Width, rect.Top + m_Margin.Height,
                                rect.Width, rect.Height);
            }
        }


        /// <summary>
        /// Formatoidaan sivu näyttämään järkevältä, lisäten joka sivulle sivunumero sekä teksti Kohde - Päivämäärä - Summa
        /// </summary>
        /// <param name="pageNumber">sivunumero, joka on menossa</param>
        /// <returns>dokumentin sivu</returns>
        public override DocumentPage GetPage(int pageNumber)
        {
            DocumentPage page = m_Paginator.GetPage(pageNumber);

            ContainerVisual newpage = new ContainerVisual()
            {
                Transform = new TranslateTransform(m_Margin.Width, m_Margin.Height)
            };


            DrawingVisual title = new DrawingVisual();

            using (DrawingContext ctx = title.RenderOpen())
            {
                FormattedText text = new FormattedText("Kohde - Päivämäärä - Summa",
                    System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    m_Typeface, 18, Brushes.Black);
                FormattedText text2 = new FormattedText("" + (pageNumber + 1),
                    System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    m_Typeface, 18, Brushes.Black);
                ctx.DrawText(text, new Point(m_Margin.Width, -m_Margin.Height)); 
                ctx.DrawText(text2, new Point(m_PageSize.Width - text2.Width - m_Margin.Width, -m_Margin.Height));
            }

            DrawingVisual background = new DrawingVisual();

            newpage.Children.Add(background);

            ContainerVisual smallerPage = new ContainerVisual();

            smallerPage.Children.Add(page.Visual);

            newpage.Children.Add(smallerPage);

            newpage.Children.Add(title);

            return new DocumentPage(newpage);
        }

        public override bool IsPageCountValid
        {
            get
            {
                return m_Paginator.IsPageCountValid;
            }
        }

        public override int PageCount
        {
            get
            {
                return m_Paginator.PageCount;
            }
        }

        public override Size PageSize
        {
            get
            {
                return m_Paginator.PageSize;
            }

            set
            {
                m_Paginator.PageSize = value;
            }

        }
        public override IDocumentPaginatorSource Source
        {
            get
            {
                return m_Paginator.Source;
            }
        }
    }
}
