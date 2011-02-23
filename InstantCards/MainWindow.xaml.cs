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


		#region MainWindow Command Base
		public abstract class MainWindowCommandBase : System.Windows.Input.ICommand
		{
			public MainWindowCommandBase(MainWindow viewModel)
			{
				this._viewModel = viewModel;
			}
			#region MainWindow ViewModel
			private MainWindow _viewModel;
			public virtual MainWindow ViewModel
			{
				protected get { return this._viewModel; }
				set { this._viewModel = value; }
			}
			#endregion

			#region ICommand Members
			public virtual bool CanExecute(object parameter)
			{
				return (this.ViewModel != null);
			}

			public virtual event EventHandler CanExecuteChanged;

			protected virtual void OnCanExecuteChanged()
			{
				var handler = this.CanExecuteChanged;
				if (handler == null)
					return;
				handler(this, EventArgs.Empty);
			}

			public abstract void Execute(object parameter);

			#endregion
		}
		#endregion
		#region public ICommand PrintCommand
		public class PrintMainWindowCommand : MainWindowCommandBase
		{
			public PrintMainWindowCommand(MainWindow viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				this.ViewModel.Model.PrintCommand.Execute(parameter);
				var pc = this.ViewModel.Model.PrintCommand as FlashCardMakerViewModel.PrintFlashCardMakerViewModelCommand;
				pc.Execute(null);
				PrintPreview pv = new PrintPreview();
				pv.DataContext = pc.ResultDocument;
				pv.ShowDialog();
				
			}
		}
		PrintMainWindowCommand _PrintCommand;
		public System.Windows.Input.ICommand PrintCommand
		{
			get
			{
				if (this._PrintCommand == null)
				{
					this._PrintCommand = new PrintMainWindowCommand(this);
				}
				return this._PrintCommand;
			}
		}
		#endregion


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
