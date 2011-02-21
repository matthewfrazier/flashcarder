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

namespace Protomeme
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			
		}

		public Protomeme.FlashCardMakerViewModel Model { get; set; }

		#region Command Line Handler
		protected virtual void ExecuteCommandLine()
		{
			if (App.CommanLineArgs != null && App.CommanLineArgs.Length > 0)
			{
				string ses = null;
				string importdir = null;
				string importpattern = "*.png";

				var os = new NDesk.Options.OptionSet()
				{
					{"s|session=", v => { ses=v; }},
					{"d|importdir=", v => { importdir = v;}},
					{"p|importpattern=", v => {importpattern = v;}},
				};

				os.Parse(App.CommanLineArgs);

				if (importdir != null && importpattern != null)
				{
					var paths = System.IO.Directory.GetFiles(importdir, importpattern);

					this.Model.LoadSourceImagesFromFilesCommand.Execute(
					paths);
				}

				if (ses != null)
				{
					this.Model.OpenSessionCommand.Execute(ses);
				}
			}
		}
		#endregion

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.Model = new Protomeme.FlashCardMakerViewModel();
			this.DataContext = this.Model;
			this.ExecuteCommandLine();
		}
	}
}
