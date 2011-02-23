using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Xps;
using System.Windows.Media;
using System.Printing;
using System.IO;
using System.IO.Packaging;

namespace Protomeme
{
	public static class PrintHelper
	{
		private static PageMediaSize A4PaperSize = new PageMediaSize(816, 1248);

		public static void PrintPreview(Window owner, FormData data)
		{
			using (MemoryStream xpsStream = new MemoryStream())
			{
				using (Package package = Package.Open(xpsStream, FileMode.Create, FileAccess.ReadWrite))
				{
					string packageUriString = "memorystream://data.xps";
					Uri packageUri = new Uri(packageUriString);

					PackageStore.AddPackage(packageUri, package);

					XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Maximum, packageUriString);
					XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
					Form visual = new Form(data);

					PrintTicket printTicket = new PrintTicket();
					printTicket.PageMediaSize = A4PaperSize;
					writer.Write(visual, printTicket);
					FixedDocumentSequence document = xpsDocument.GetFixedDocumentSequence();
					xpsDocument.Close();

					PrintPreviewWindow printPreviewWnd = new PrintPreviewWindow(document);
					printPreviewWnd.Owner = owner;
					printPreviewWnd.ShowDialog();
					PackageStore.RemovePackage(packageUri);
				}
			}
		}
	}
}
