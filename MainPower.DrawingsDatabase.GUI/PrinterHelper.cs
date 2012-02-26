using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Windows.Controls;

namespace MainPower.DrawingsDatabase.GUI
{
    public static class PrinterHelper
    {
        public const double A4Width = 210 / 25.4 * 96;
        public const double A4Height= 297 / 25.4 * 96;

        public const double A3Width = 297 / 25.4 * 96;
        public const double A3Height = 420 / 25.4 * 96;


        private static string _previewWindowXaml =
            @"<Window
                xmlns                 ='http://schemas.microsoft.com/netfx/2007/xaml/presentation'
                xmlns:x               ='http://schemas.microsoft.com/winfx/2006/xaml'
                Title                 ='Print Preview - @@TITLE'
                Height                ='200'
                Width                 ='300'
                WindowStartupLocation ='CenterOwner'>
                <DocumentViewer Name='dv1'/>
             </Window>";


        public static void DoPreview(FlowDocument flowDoc, string title, double pageWidth, double pageHeight)
        {
            string fileName = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName();
            DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDoc).DocumentPaginator;

            paginator.PageSize = new Size(pageWidth, pageHeight);
            flowDoc.PagePadding = new Thickness(25);
            flowDoc.ColumnWidth = double.PositiveInfinity;

            try
            {
                // write the XPS document
                using (XpsDocument doc = new XpsDocument(fileName, FileAccess.ReadWrite))
                {
                    XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
                    writer.Write(paginator);
                }

                // Read the XPS document into a dynamically generated
                // preview Window 
                using (XpsDocument doc = new XpsDocument(fileName, FileAccess.Read))
                {
                    FixedDocumentSequence fds = doc.GetFixedDocumentSequence();

                    string s = _previewWindowXaml;
                    s = s.Replace("@@TITLE", title.Replace("'", "&apos;"));

                    using (var reader = new System.Xml.XmlTextReader(new StringReader(s)))
                    {
                        Window preview = System.Windows.Markup.XamlReader.Load(reader) as Window;

                        DocumentViewer dv1 = LogicalTreeHelper.FindLogicalNode(preview, "dv1") as DocumentViewer;
                        dv1.Document = fds as IDocumentPaginatorSource;

                        preview.ShowDialog();
                    }
                }
            }
            finally
            {
                if (File.Exists(fileName))
                {
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static void DoThePrint(System.Windows.Documents.FlowDocument document)
        {
            // Clone the source document's content into a new FlowDocument.
            // This is because the pagination for the printer needs to be
            // done differently than the pagination for the displayed page.
            // We print the copy, rather that the original FlowDocument.
            System.IO.MemoryStream s = new System.IO.MemoryStream();
            TextRange source = new TextRange(document.ContentStart, document.ContentEnd);
            source.Save(s, DataFormats.Xaml);
            FlowDocument copy = new FlowDocument();
            TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
            dest.Load(s, DataFormats.Xaml);

            // Create a XpsDocumentWriter object, implicitly opening a Windows common print dialog,
            // and allowing the user to select a printer.

            // get information about the dimensions of the seleted printer+media.
            System.Printing.PrintDocumentImageableArea ia = null;
            System.Windows.Xps.XpsDocumentWriter docWriter = System.Printing.PrintQueue.CreateXpsDocumentWriter(ref ia);

            if (docWriter != null && ia != null)
            {
                DocumentPaginator paginator = ((IDocumentPaginatorSource)copy).DocumentPaginator;

                // Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
                paginator.PageSize = new Size(ia.MediaSizeWidth, ia.MediaSizeHeight);
                Thickness t = new Thickness(72);  // copy.PagePadding;
                copy.PagePadding = new Thickness(
                                 Math.Max((double)ia.OriginWidth, t.Left),
                                   Math.Max((double)ia.OriginHeight, t.Top),
                                   Math.Max((double)(ia.MediaSizeWidth - (ia.OriginWidth + ia.ExtentWidth)), t.Right),
                                   Math.Max((double)(ia.MediaSizeHeight - (ia.OriginHeight + ia.ExtentHeight)), t.Bottom));

                copy.ColumnWidth = double.PositiveInfinity;
                //copy.PageWidth = 528; // allow the page to be the natural with of the output device

                // Send content to the printer.
                //docWriter.Write(paginator);
                //DoPreview(paginator, "Search Results");
            }

        }
    }
}
