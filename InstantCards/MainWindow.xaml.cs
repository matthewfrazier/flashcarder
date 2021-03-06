﻿using System;
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

		#region public ICommand PrintCommand
		public class PrintMainWindowCommand : ViewModelCommandBase<MainWindow>
		{
			public PrintMainWindowCommand(MainWindow viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				this.ViewModel.Model.PrintCommand.Execute(parameter);
				var pc = this.ViewModel.Model.PrintCommand as InstantCardsViewModel.PrintFlashCardMakerViewModelCommand;
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

		public Protomeme.InstantCardsViewModel Model { get; set; }

		#region Command Line Handler
		protected virtual void ExecuteCommandLine()
		{
			if (App.CommanLineArgs != null && App.CommanLineArgs.Length > 0)
			{
				string ses = null;
				string importdir = null;
				string importpattern = "*.png";
				bool print = false;

				var os = new NDesk.Options.OptionSet()
				{
					{"s|session=", v => { ses=v; }},
					{"d|importdir=", v => { importdir = v;}},
					{"i|importpattern=", v => {importpattern = v;}},
					{"p|print",var=>{ print = true;}},
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

				if (print)
				{
					this.PrintCommand.Execute(null);
				}
			}
			else
			{
				this.Model.NewSessionCommand.Execute(null);
			}
		}
		#endregion

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.Model = new Protomeme.InstantCardsViewModel();
			this.DataContext = this.Model;
			this.ExecuteCommandLine();
		}
	}
}
