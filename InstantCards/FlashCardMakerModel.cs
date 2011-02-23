using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using System.Windows.Documents;

namespace Protomeme
{
	public class FlashCardMakerViewModel : INotifyPropertyChanged
	{

		public FlashCardMakerViewModel()
		{
			this.ErrorCollector = new BoundErrorCollector();

			this.PropertyChanged += new PropertyChangedEventHandler(FlashCardMakerViewModel_PropertyChanged);
		}

		System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(FlashCardSession));

		public class BoundErrorCollector :
			ObservableCollection<KeyValuePair<object, Exception>>,
			IErrorCollector,
			INotifyPropertyChanged
		{
			protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
			{
				base.OnCollectionChanged(e);
				var ha = this.Count > 0;
				if (ha)
				{
					var summary = String.Format("{0}: {1}", this[0].Key, this[0].Value.Message);
					if (this.Count > 1)
						summary += String.Format(" ({0} total errors)", this.Count);
					this.ShortErrorSummary = summary;
				}
				else
				{
					this.ShortErrorSummary = null;
				}
				this.HasErrors = ha;

			}
			#region INotifyPropertyChanged

			/// <summary>
			/// The PropertyChanged event is used by consuming code
			/// (like WPF's binding infrastructure) to detect when
			/// a value has changed.
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			/// <summary>
			/// Raise the PropertyChanged event for the 
			/// specified property.
			/// </summary>
			/// <param name="propertyName">
			/// A string representing the name of 
			/// the property that changed.</param>
			/// <remarks>
			/// Only raise the event if the value of the property 
			/// has changed from its previous value</remarks>
			protected void OnPropertyChanged(string propertyName)
			{
				// Validate the property name in debug builds
				VerifyProperty(propertyName);

				if (null != PropertyChanged)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}

			/// <summary>
			/// Verifies whether the current class provides a property with a given
			/// name. This method is only invoked in debug builds, and results in
			/// a runtime exception if the <see cref="OnPropertyChanged"/> method
			/// is being invoked with an invalid property name. This may happen if
			/// a property's name was changed but not the parameter of the property's
			/// invocation of <see cref="OnPropertyChanged"/>.
			/// </summary>
			/// <param name="propertyName">The name of the changed property.</param>
			[System.Diagnostics.Conditional("DEBUG")]
			private void VerifyProperty(string propertyName)
			{
				Type type = this.GetType();

				// Look for a *public* property with the specified name
				System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
				if (pi == null)
				{
					// There is no matching property - notify the developer
					string msg = "OnPropertyChanged was invoked with invalid " +
									"property name {0}. {0} is not a public " +
									"property of {1}.";
					msg = String.Format(msg, propertyName, type.FullName);
					System.Diagnostics.Debug.Fail(msg);
				}
			}

			#endregion
			#region public ICommand ResetCommand
			public class ResetBoundErrorCollectorCommand : BoundErrorCollectorCommandBase
			{
				public ResetBoundErrorCollectorCommand(BoundErrorCollector viewModel)
					: base(viewModel)
				{
				}
				public override void Execute(object parameter)
				{
					this.ViewModel.Clear();
				}
			}
			ResetBoundErrorCollectorCommand _ResetCommand;
			public System.Windows.Input.ICommand ResetCommand
			{
				get
				{
					if (this._ResetCommand == null)
					{
						this._ResetCommand = new ResetBoundErrorCollectorCommand(this);
					}
					return this._ResetCommand;
				}
			}
			#endregion
			#region public ICommand CopyToClipboardCommand
			public class CopyToClipboardBoundErrorCollectorCommand : BoundErrorCollectorCommandBase
			{
				public CopyToClipboardBoundErrorCollectorCommand(BoundErrorCollector viewModel)
					: base(viewModel)
				{
				}
				public override void Execute(object parameter)
				{
					StringBuilder message= new StringBuilder();
					foreach (var line in this.ViewModel)
					{
						message.AppendFormat("{0}: {1}\n",
							line.Key, line.Value);
					}
					Clipboard.SetText(message.ToString());
				}
			}
			CopyToClipboardBoundErrorCollectorCommand _CopyToClipboardCommand;
			public System.Windows.Input.ICommand CopyToClipboardCommand
			{
				get
				{
					if (this._CopyToClipboardCommand == null)
					{
						this._CopyToClipboardCommand = new CopyToClipboardBoundErrorCollectorCommand(this);
					}
					return this._CopyToClipboardCommand;
				}
			}
			#endregion


			#region BoundErrorCollector Command Base
			public abstract class BoundErrorCollectorCommandBase : System.Windows.Input.ICommand
			{
				public BoundErrorCollectorCommandBase(BoundErrorCollector viewModel)
				{
					this._viewModel = viewModel;
				}
				#region BoundErrorCollector ViewModel
				private BoundErrorCollector _viewModel;
				public virtual BoundErrorCollector ViewModel
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
			#region IErrorCollector Members

			#region HasErrors (INotifyPropertyChanged Property)
			private bool _HasErrors;
			public bool HasErrors
			{
				get { return this._HasErrors; }
				set
				{
					if (value == _HasErrors)
						return;

					this._HasErrors = value;
					this.OnPropertyChanged("HasErrors");
				}
			}
			#endregion
			#region ShortErrorSummary (INotifyPropertyChanged Property)
			private string _ShortErrorSummary;
			public string ShortErrorSummary
			{
				get { return this._ShortErrorSummary; }
				set
				{
					if (value == _ShortErrorSummary)
						return;

					this._ShortErrorSummary = value;
					this.OnPropertyChanged("ShortErrorSummary");
				}
			}
			#endregion
		

		
			#endregion
		}

		public class TaggedRegion :
			INotifyPropertyChanged
		{
			#region INotifyPropertyChanged

			/// <summary>
			/// The PropertyChanged event is used by consuming code
			/// (like WPF's binding infrastructure) to detect when
			/// a value has changed.
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			/// <summary>
			/// Raise the PropertyChanged event for the 
			/// specified property.
			/// </summary>
			/// <param name="propertyName">
			/// A string representing the name of 
			/// the property that changed.</param>
			/// <remarks>
			/// Only raise the event if the value of the property 
			/// has changed from its previous value</remarks>
			protected void OnPropertyChanged(string propertyName)
			{
				// Validate the property name in debug builds
				VerifyProperty(propertyName);

				if (null != PropertyChanged)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}

			/// <summary>
			/// Verifies whether the current class provides a property with a given
			/// name. This method is only invoked in debug builds, and results in
			/// a runtime exception if the <see cref="OnPropertyChanged"/> method
			/// is being invoked with an invalid property name. This may happen if
			/// a property's name was changed but not the parameter of the property's
			/// invocation of <see cref="OnPropertyChanged"/>.
			/// </summary>
			/// <param name="propertyName">The name of the changed property.</param>
			[System.Diagnostics.Conditional("DEBUG")]
			private void VerifyProperty(string propertyName)
			{
				Type type = this.GetType();

				// Look for a *public* property with the specified name
				System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
				if (pi == null)
				{
					// There is no matching property - notify the developer
					string msg = "OnPropertyChanged was invoked with invalid " +
									"property name {0}. {0} is not a public " +
									"property of {1}.";
					msg = String.Format(msg, propertyName, type.FullName);
					System.Diagnostics.Debug.Fail(msg);
				}
			}

			#endregion
			#region Tag (INotifyPropertyChanged Property)
			private string _Tag;
			public string Tag
			{
				get { return this._Tag; }
				set
				{
					if (value == _Tag)
						return;

					this._Tag = value;
					this.OnPropertyChanged("Tag");
				}
			}
			#endregion
			#region Region (INotifyPropertyChanged Property)
			private Int32Rect _Region;
			public Int32Rect Region
			{
				get { return this._Region; }
				set
				{
					if (value == _Region)
						return;

					this._Region = value;
					this.OnPropertyChanged("Region");
				}
			}
			#endregion
			#region ImageUrl (INotifyPropertyChanged Property)
			private string _ImageUrl;
			public string ImageUrl
			{
				get { return this._ImageUrl; }
				set
				{
					if (value == _ImageUrl)
						return;

					this._ImageUrl = value;
					this.OnPropertyChanged("ImageUrl");
				}
			}
			#endregion
			#region Image (INotifyPropertyChanged Property)
			private BitmapSource _Image;
			[XmlIgnore]
			public BitmapSource Image
			{
				get { return this._Image; }
				set
				{
					if (value == _Image)
						return;

					this._Image = value;
					this.OnPropertyChanged("Image");
				}
			}
			#endregion
		}


		#region ErrorCollector (INotifyPropertyChanged Property)
		private IErrorCollector _ErrorCollector;
		public IErrorCollector ErrorCollector
		{
			get { return this._ErrorCollector; }
			set
			{
				if (value == _ErrorCollector)
					return;

				this._ErrorCollector = value;
				this.OnPropertyChanged("ErrorCollector");
			}
		}
		#endregion

		protected virtual ObservableCollection<TaggedRegion>
			GetDefaultTaggedRegions()
		{
			return new ObservableCollection<TaggedRegion>()
			{
				new TaggedRegion()
				{
					Tag="front",
				},
				new TaggedRegion()
				{
					Tag="back"
				}
			};
		}

		void FlashCardMakerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "SelectedSourceImage":
					if (this.SelectedSourceImage == null)
					{
						this.SelectedTaggedRegion = null;
					}

					if (this.SelectedSourceImage.TaggedRegions == null
						|| this.SelectedSourceImage.TaggedRegions.Count == 0)
					{
						this.SelectedSourceImage.TaggedRegions = this.GetDefaultTaggedRegions();
					}

					if (this.SelectedSourceImage.TaggedRegions.Count > 0)
						this.SelectedTaggedRegion = this.SelectedSourceImage.TaggedRegions[0];
					break;
				case "Session":
					if (this.Session.SourceImages != null)
					{
						this.SourceImages = new ObservableCollection<SourceImage>(
							this.Session.SourceImages);
					}
					else
					{
						this.SourceImages = new ObservableCollection<SourceImage>();
					}
					break;
			}
		}

		public class FlashCardSession : INotifyPropertyChanged
		{
			#region INotifyPropertyChanged

			/// <summary>
			/// The PropertyChanged event is used by consuming code
			/// (like WPF's binding infrastructure) to detect when
			/// a value has changed.
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			/// <summary>
			/// Raise the PropertyChanged event for the 
			/// specified property.
			/// </summary>
			/// <param name="propertyName">
			/// A string representing the name of 
			/// the property that changed.</param>
			/// <remarks>
			/// Only raise the event if the value of the property 
			/// has changed from its previous value</remarks>
			protected void OnPropertyChanged(string propertyName)
			{
				// Validate the property name in debug builds
				VerifyProperty(propertyName);

				if (null != PropertyChanged)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}

			/// <summary>
			/// Verifies whether the current class provides a property with a given
			/// name. This method is only invoked in debug builds, and results in
			/// a runtime exception if the <see cref="OnPropertyChanged"/> method
			/// is being invoked with an invalid property name. This may happen if
			/// a property's name was changed but not the parameter of the property's
			/// invocation of <see cref="OnPropertyChanged"/>.
			/// </summary>
			/// <param name="propertyName">The name of the changed property.</param>
			[System.Diagnostics.Conditional("DEBUG")]
			private void VerifyProperty(string propertyName)
			{
				Type type = this.GetType();

				// Look for a *public* property with the specified name
				System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
				if (pi == null)
				{
					// There is no matching property - notify the developer
					string msg = "OnPropertyChanged was invoked with invalid " +
									"property name {0}. {0} is not a public " +
									"property of {1}.";
					msg = String.Format(msg, propertyName, type.FullName);
					System.Diagnostics.Debug.Fail(msg);
				}
			}

			#endregion

			#region Title (INotifyPropertyChanged Property)
			private string _Title;
			public string Title
			{
				get { return this._Title; }
				set
				{
					if (value == _Title)
						return;

					this._Title = value;
					this.OnPropertyChanged("Title");
				}
			}
			#endregion
			#region SessionPath (INotifyPropertyChanged Property)
			private string _SessionPath;
			public string SessionPath
			{
				get { return this._SessionPath; }
				set
				{
					if (value == _SessionPath)
						return;

					this._SessionPath = value;
					this.OnPropertyChanged("SessionPath");
				}
			}
			#endregion
			#region SourceImages (INotifyPropertyChanged Property)
			private List<SourceImage> _SourceImages;
			public List<SourceImage> SourceImages
			{
				get { return this._SourceImages; }
				set
				{
					if (value == _SourceImages)
						return;

					this._SourceImages = value;
					this.OnPropertyChanged("SourceImages");
				}
			}
			#endregion
		}

		#region INotifyPropertyChanged

		/// <summary>
		/// The PropertyChanged event is used by consuming code
		/// (like WPF's binding infrastructure) to detect when
		/// a value has changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raise the PropertyChanged event for the 
		/// specified property.
		/// </summary>
		/// <param name="propertyName">
		/// A string representing the name of 
		/// the property that changed.</param>
		/// <remarks>
		/// Only raise the event if the value of the property 
		/// has changed from its previous value</remarks>
		protected void OnPropertyChanged(string propertyName)
		{
			// Validate the property name in debug builds
			VerifyProperty(propertyName);

			if (null != PropertyChanged)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Verifies whether the current class provides a property with a given
		/// name. This method is only invoked in debug builds, and results in
		/// a runtime exception if the <see cref="OnPropertyChanged"/> method
		/// is being invoked with an invalid property name. This may happen if
		/// a property's name was changed but not the parameter of the property's
		/// invocation of <see cref="OnPropertyChanged"/>.
		/// </summary>
		/// <param name="propertyName">The name of the changed property.</param>
		[System.Diagnostics.Conditional("DEBUG")]
		private void VerifyProperty(string propertyName)
		{
			Type type = this.GetType();

			// Look for a *public* property with the specified name
			System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
			if (pi == null)
			{
				// There is no matching property - notify the developer
				string msg = "OnPropertyChanged was invoked with invalid " +
								"property name {0}. {0} is not a public " +
								"property of {1}.";
				msg = String.Format(msg, propertyName, type.FullName);
				System.Diagnostics.Debug.Fail(msg);
			}
		}

		#endregion

		#region Session (INotifyPropertyChanged Property)
		private FlashCardSession _Session;
		public FlashCardSession Session
		{
			get { return this._Session; }
			set
			{
				if (value == _Session)
					return;

				this._Session = value;
				this.OnPropertyChanged("Session");
			}
		}
		#endregion

		#region SourceImages (INotifyPropertyChanged Property)
		private ObservableCollection<SourceImage> _SourceImages;
		public ObservableCollection<SourceImage> SourceImages
		{
			get { return this._SourceImages; }
			set
			{
				if (value == _SourceImages)
					return;

				this._SourceImages = value;
				this.OnPropertyChanged("SourceImages");
			}
		}
		#endregion

		#region SelectedSourceImage (INotifyPropertyChanged Property)
		private SourceImage _SelectedSourceImage;
		public SourceImage SelectedSourceImage
		{
			get { return this._SelectedSourceImage; }
			set
			{
				if (value == _SelectedSourceImage)
					return;

				this._SelectedSourceImage = value;
				this.OnPropertyChanged("SelectedSourceImage");
			}
		}
		#endregion
		#region SelectedTaggedRegion (INotifyPropertyChanged Property)
		private TaggedRegion _SelectedTaggedRegion;
		public TaggedRegion SelectedTaggedRegion
		{
			get { return this._SelectedTaggedRegion; }
			set
			{
				if (value == _SelectedTaggedRegion)
					return;

				this._SelectedTaggedRegion = value;
				this.OnPropertyChanged("SelectedTaggedRegion");
			}
		}
		#endregion

		#region FlashCardMakerViewModel Command Base
		public abstract class FlashCardMakerViewModelCommandBase : System.Windows.Input.ICommand,
			INotifyPropertyChanged
		{
			#region INotifyPropertyChanged

			/// <summary>
			/// The PropertyChanged event is used by consuming code
			/// (like WPF's binding infrastructure) to detect when
			/// a value has changed.
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			/// <summary>
			/// Raise the PropertyChanged event for the 
			/// specified property.
			/// </summary>
			/// <param name="propertyName">
			/// A string representing the name of 
			/// the property that changed.</param>
			/// <remarks>
			/// Only raise the event if the value of the property 
			/// has changed from its previous value</remarks>
			protected void OnPropertyChanged(string propertyName)
			{
				// Validate the property name in debug builds
				VerifyProperty(propertyName);

				if (null != PropertyChanged)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}

			/// <summary>
			/// Verifies whether the current class provides a property with a given
			/// name. This method is only invoked in debug builds, and results in
			/// a runtime exception if the <see cref="OnPropertyChanged"/> method
			/// is being invoked with an invalid property name. This may happen if
			/// a property's name was changed but not the parameter of the property's
			/// invocation of <see cref="OnPropertyChanged"/>.
			/// </summary>
			/// <param name="propertyName">The name of the changed property.</param>
			[System.Diagnostics.Conditional("DEBUG")]
			private void VerifyProperty(string propertyName)
			{
				Type type = this.GetType();

				// Look for a *public* property with the specified name
				System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
				if (pi == null)
				{
					// There is no matching property - notify the developer
					string msg = "OnPropertyChanged was invoked with invalid " +
									"property name {0}. {0} is not a public " +
									"property of {1}.";
					msg = String.Format(msg, propertyName, type.FullName);
					System.Diagnostics.Debug.Fail(msg);
				}
			}

			#endregion

			#region Description (INotifyPropertyChanged Property)
			private string _Description;
			public string Description
			{
				get { return this._Description; }
				set
				{
					if (value == _Description)
						return;

					this._Description = value;
					this.OnPropertyChanged("Description");
				}
			}
			#endregion
			public FlashCardMakerViewModelCommandBase(FlashCardMakerViewModel viewModel)
			{
				this._viewModel = viewModel;
			}
			#region FlashCardMakerViewModel ViewModel
			private FlashCardMakerViewModel _viewModel;
			public virtual FlashCardMakerViewModel ViewModel
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
		#region public ICommand LoadSourceImagesFromFilesCommand
		public class LoadSourceImagesFromFilesFlashCardMakerViewModelCommand : FlashCardMakerViewModelCommandBase
		{
			public LoadSourceImagesFromFilesFlashCardMakerViewModelCommand(FlashCardMakerViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				var paths = parameter as IEnumerable<string>;
				if (paths == null)
					return;

				this.LoadSourceImagesFromFiles(paths);

				if (this.ViewModel.SourceImages == null)
					this.ViewModel.SourceImages = new ObservableCollection<SourceImage>();


			}

			#region Errors (INotifyPropertyChanged Property)
			private ObservableCollection<KeyValuePair<string, Exception>> _Errors;
			public ObservableCollection<KeyValuePair<string, Exception>> Errors
			{
				get { return this._Errors; }
				set
				{
					if (value == _Errors)
						return;

					this._Errors = value;
					this.OnPropertyChanged("Errors");
				}
			}
			#endregion

			public void LoadSourceImagesFromFiles(IEnumerable<string> paths)
			{
				this.Errors = new ObservableCollection<KeyValuePair<string, Exception>>();
				if (this.ViewModel.SourceImages == null)
					this.ViewModel.SourceImages = new ObservableCollection<SourceImage>();
				foreach (string path in paths)
				{
					try
					{
						this.ViewModel.SourceImages.Add(
							new SourceImage()
							{
								ImageUrl = path,
								Title = System.IO.Path.GetFileNameWithoutExtension(path)
							});
					}
					catch (Exception ex)
					{
						this.Errors.Add(new KeyValuePair<string, Exception>(
							path, ex));
					}
				}
			}

		}
		LoadSourceImagesFromFilesFlashCardMakerViewModelCommand _LoadSourceImagesFromFilesCommand;
		public System.Windows.Input.ICommand LoadSourceImagesFromFilesCommand
		{
			get
			{
				if (this._LoadSourceImagesFromFilesCommand == null)
				{
					this._LoadSourceImagesFromFilesCommand = new LoadSourceImagesFromFilesFlashCardMakerViewModelCommand(this);
				}
				return this._LoadSourceImagesFromFilesCommand;
			}
		}
		#endregion
		#region public ICommand ClearImagesCommand
		public class ClearImagesFlashCardMakerViewModelCommand : FlashCardMakerViewModelCommandBase
		{
			public ClearImagesFlashCardMakerViewModelCommand(FlashCardMakerViewModel viewModel)
				: base(viewModel)
			{
			}
			public override bool CanExecute(object parameter)
			{
				return base.CanExecute(parameter)
					&& this.ViewModel.SourceImages != null
					&& this.ViewModel.SourceImages.Count > 0;
			}
			public override void Execute(object parameter)
			{
				this.ViewModel.SourceImages.Clear();
			}
		}
		ClearImagesFlashCardMakerViewModelCommand _ClearImagesCommand;
		public System.Windows.Input.ICommand ClearImagesCommand
		{
			get
			{
				if (this._ClearImagesCommand == null)
				{
					this._ClearImagesCommand = new ClearImagesFlashCardMakerViewModelCommand(this);
				}
				return this._ClearImagesCommand;
			}
		}
		#endregion
		#region public ICommand SaveCommand
		public class SaveFlashCardMakerViewModelCommand : FlashCardMakerViewModelCommandBase
		{
			public SaveFlashCardMakerViewModelCommand(FlashCardMakerViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				try
				{
					var path = parameter as string;

					if (String.IsNullOrEmpty(path))
					{
						var saveDialog = new Microsoft.Win32.SaveFileDialog();
						saveDialog.Title = "Save Flash Card Session";
						saveDialog.Filter = @"Flash Card Session(*.flashcards.xml)|*.flashcards.xml";
						var result = saveDialog.ShowDialog();
						if (result == null || !result.Value)
							return;
						path = saveDialog.FileName;
					}
					this.Description = String.Format("Saving {0}", path);
					var session = this.ViewModel.Session;
					if (session == null)
					{
						session = new FlashCardSession();
					}
					session.SourceImages = this.ViewModel.SourceImages.ToList();
					using (var xw = System.Xml.XmlWriter.Create(path))
					{
						this.ViewModel.xmlSerializer.Serialize(xw, session);
					}
					session.SessionPath = path; 
					
					this.ViewModel.Session = session;

					this.Description = String.Format("Saved {0}", path);
				}
				catch (Exception ex)
				{
					this.ViewModel.ErrorCollector.Add(
						new KeyValuePair<object, Exception>(
							parameter, ex));
				}
			}
		}
		SaveFlashCardMakerViewModelCommand _SaveCommand;
		public System.Windows.Input.ICommand SaveCommand
		{
			get
			{
				if (this._SaveCommand == null)
				{
					this._SaveCommand = new SaveFlashCardMakerViewModelCommand(this);
				}
				return this._SaveCommand;
			}
		}
		#endregion
		#region public ICommand OpenSessionCommand
		public class OpenSessionFlashCardMakerViewModelCommand : FlashCardMakerViewModelCommandBase
		{
			public OpenSessionFlashCardMakerViewModelCommand(FlashCardMakerViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				var path = parameter as string;
				try
				{
					if (path == null)
					{
						var ofd = new Microsoft.Win32.OpenFileDialog();
						ofd.Filter = "Flash Cards|*.flashcards.xml";
						var result = ofd.ShowDialog();
						if (result == null || !result.Value)
							return;
						path = ofd.FileName;
					}
					using (var xw = System.Xml.XmlReader.Create(path))
					{
						this.ViewModel.Session = this.ViewModel.xmlSerializer.Deserialize(xw)
							as FlashCardSession;
					}
				}
				catch (Exception ex)
				{
					this.ViewModel.ErrorCollector.Add(new KeyValuePair<object, Exception>(
						path, ex));
				}
			}
		}
		OpenSessionFlashCardMakerViewModelCommand _OpenSessionCommand;
		public System.Windows.Input.ICommand OpenSessionCommand
		{
			get
			{
				if (this._OpenSessionCommand == null)
				{
					this._OpenSessionCommand = new OpenSessionFlashCardMakerViewModelCommand(this);
				}
				return this._OpenSessionCommand;
			}
		}
		#endregion
		#region public ICommand ExportCommand
		public class ExportFlashCardMakerViewModelCommand : FlashCardMakerViewModelCommandBase
		{
			public ExportFlashCardMakerViewModelCommand(FlashCardMakerViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				var path = parameter as string;
				try
				{
					if (String.IsNullOrEmpty(path))
					{
						var sfd = new Microsoft.Win32.SaveFileDialog();
						sfd.Title = "Export Flash Cards";
						sfd.Filter = @"HTML(*.htm)|*.htm";
						var result = sfd.ShowDialog();
						if (result == null || !result.Value)
							return;
						path = sfd.FileName;
					}
					foreach (var si in this.ViewModel.Session.SourceImages)
					{
						foreach (var tr in si.TaggedRegions)
						{
							if (tr.Region == Int32Rect.Empty)
								continue;
							//HACK: this will stomp multiple regions with the same tag
							var trpath =
								System.IO.Path.Combine(
									System.IO.Path.GetDirectoryName(path),
										String.Format("{0}-{1}.png",
										System.IO.Path.GetFileNameWithoutExtension(si.ImageUrl),
										tr.Tag));
							//load the image so we can explicitly crop it rather than using the in-memory version
							var image = new BitmapImage(new Uri(si.ImageUrl));
							//crop to a rounded representation of the rect
							var cropped = new CroppedBitmap(
								image,
								tr.Region);
							var enc = new PngBitmapEncoder();
							enc.Frames.Add(BitmapFrame.Create(cropped));
							try
							{
								using (var fs = System.IO.File.Create(
									trpath))
								{
									enc.Save(fs);
								}
								tr.ImageUrl = trpath;
							}
							catch (Exception fsex)
							{
								this.ViewModel.ErrorCollector.Add(new KeyValuePair<object, Exception>(
									trpath, fsex));
							}
						}
					}
				}
				catch (Exception ex)
				{
					this.ViewModel.ErrorCollector.Add(new KeyValuePair<object, Exception>(
						path, ex));
				}
			}
		}
		ExportFlashCardMakerViewModelCommand _ExportCommand;
		public System.Windows.Input.ICommand ExportCommand
		{
			get
			{
				if (this._ExportCommand == null)
				{
					this._ExportCommand = new ExportFlashCardMakerViewModelCommand(this);
				}
				return this._ExportCommand;
			}
		}
		#endregion
		#region public ICommand PrintCommand
		public class PrintFlashCardMakerViewModelCommand : FlashCardMakerViewModelCommandBase
		{
			public PrintFlashCardMakerViewModelCommand(FlashCardMakerViewModel viewModel)
				: base(viewModel)
			{
			}

			public PageContent CreateFlashCardPage(int pagenum, int cardsPerPage, string tag, FlowDirection direction)
			{
				FixedPage fixedPage = new FixedPage();
				PageContent pageContent = new PageContent();
				var images = this.ViewModel.SourceImages.ToList();
				//Set up the WPF Control to be printed
				FlashCardPrintPage page = new FlashCardPrintPage();
				page.PrintInfo = new FlashCardPrintPage.PageInfo()
				{
					SourceImages = images,
					CardsPerPage = cardsPerPage,
					StartIndex = pagenum * cardsPerPage,
					Tag = tag,
					Direction = direction,
					BorderThickness=new System.Windows.Thickness(2f),
					BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGray)
				};
				page.PrintInfo.Build();
				page.DataContext = page.PrintInfo;

				//Create first page of document
				fixedPage.Children.Add(page);
				((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);

				return pageContent;
			}

			public FixedDocument CreateFlashCardDocument()
			{
				FixedDocument fixedDoc = new FixedDocument();
				int cardsPerPage = 8;
				//batch images into buckets of card size

				for (int pagenum  = 0;
					pagenum < this.ViewModel.SourceImages.Count / cardsPerPage;
					pagenum ++)
				{
					var frontPage = CreateFlashCardPage(
						pagenum, cardsPerPage, "front", FlowDirection.LeftToRight);
					fixedDoc.Pages.Add(frontPage);

					var backPage = CreateFlashCardPage(
						pagenum, cardsPerPage, "back", FlowDirection.RightToLeft);
					fixedDoc.Pages.Add(backPage);
				}

				return fixedDoc;
			}

			#region ResultDocument (INotifyPropertyChanged Property)
			private FixedDocument _ResultDocument;
			public FixedDocument ResultDocument
			{
				get { return this._ResultDocument; }
				set
				{
					if (value == _ResultDocument)
						return;

					this._ResultDocument = value;
					this.OnPropertyChanged("ResultDocument");
				}
			}
			#endregion
			public override void Execute(object parameter)
			{
				try
				{
					var doc = CreateFlashCardDocument();
					this.ResultDocument = doc;
				}
				catch (Exception ex)
				{
					this.ViewModel.ErrorCollector.Add(new KeyValuePair<object, Exception>(
						null, ex));
				}
			}
		}
		PrintFlashCardMakerViewModelCommand _PrintCommand;
		public System.Windows.Input.ICommand PrintCommand
		{
			get
			{
				if (this._PrintCommand == null)
				{
					this._PrintCommand = new PrintFlashCardMakerViewModelCommand(this);
				}
				return this._PrintCommand;
			}
		}
		#endregion


		/// <summary>
		/// An image that can be tagged with regions that represent parts of
		/// one or more flash cards
		/// </summary>
		public class SourceImage : INotifyPropertyChanged
		{
			#region INotifyPropertyChanged

			/// <summary>
			/// The PropertyChanged event is used by consuming code
			/// (like WPF's binding infrastructure) to detect when
			/// a value has changed.
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;

			/// <summary>
			/// Raise the PropertyChanged event for the 
			/// specified property.
			/// </summary>
			/// <param name="propertyName">
			/// A string representing the name of 
			/// the property that changed.</param>
			/// <remarks>
			/// Only raise the event if the value of the property 
			/// has changed from its previous value</remarks>
			protected void OnPropertyChanged(string propertyName)
			{
				// Validate the property name in debug builds
				VerifyProperty(propertyName);

				if (null != PropertyChanged)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				}
			}

			/// <summary>
			/// Verifies whether the current class provides a property with a given
			/// name. This method is only invoked in debug builds, and results in
			/// a runtime exception if the <see cref="OnPropertyChanged"/> method
			/// is being invoked with an invalid property name. This may happen if
			/// a property's name was changed but not the parameter of the property's
			/// invocation of <see cref="OnPropertyChanged"/>.
			/// </summary>
			/// <param name="propertyName">The name of the changed property.</param>
			[System.Diagnostics.Conditional("DEBUG")]
			private void VerifyProperty(string propertyName)
			{
				Type type = this.GetType();

				// Look for a *public* property with the specified name
				System.Reflection.PropertyInfo pi = type.GetProperty(propertyName);
				if (pi == null)
				{
					// There is no matching property - notify the developer
					string msg = "OnPropertyChanged was invoked with invalid " +
									"property name {0}. {0} is not a public " +
									"property of {1}.";
					msg = String.Format(msg, propertyName, type.FullName);
					System.Diagnostics.Debug.Fail(msg);
				}
			}

			#endregion

			#region Title (INotifyPropertyChanged Property)
			private string _Title;
			public string Title
			{
				get { return this._Title; }
				set
				{
					if (value == _Title)
						return;

					this._Title = value;
					this.OnPropertyChanged("Title");
				}
			}
			#endregion

			#region ImageUrl (INotifyPropertyChanged Property)
			private string _ImageUrl;
			public string ImageUrl
			{
				get { return this._ImageUrl; }
				set
				{
					if (value == _ImageUrl)
						return;

					this._ImageUrl = value;
					this.OnPropertyChanged("ImageUrl");
				}
			}
			#endregion

			#region TaggedRegions (INotifyPropertyChanged Property)
			private ObservableCollection<TaggedRegion> _TaggedRegions;
			public ObservableCollection<TaggedRegion> TaggedRegions
			{
				get { return this._TaggedRegions; }
				set
				{
					if (value == _TaggedRegions)
						return;

					this._TaggedRegions = value;
					this.OnPropertyChanged("TaggedRegions");
				}
			}
			#endregion


		}
	}
}
