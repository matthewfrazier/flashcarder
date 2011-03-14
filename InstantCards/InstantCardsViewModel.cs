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
using System.Windows.Media;
using System.Globalization;
using System.Runtime;
using System.Windows.Data;

namespace Protomeme
{
	public class InstantCardsViewModel : ViewModelBase
	{
		public InstantCardsViewModel()
		{
			this.ErrorCollector = new BoundErrorCollector();
			this.AllTags = new ObservableCollection<string>(
				new string[]{"red","blue","green"});

			this.PropertyChanged += new PropertyChangedEventHandler(FlashCardMakerViewModel_PropertyChanged);
		}

		public class TagViewModel : ViewModelBase
		{
		}

		public class TagViewModelCollection : ObservableCollection<TagViewModel>
		{
			
		}


		#region Persistence
		System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(FlashCardSession));
		#endregion
		#region Error Reporting
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
					StringBuilder message = new StringBuilder();
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
		#endregion

		public class TaggedRegion :
			SessionImageBase
		{
			public TaggedRegion()
			{
				this.PropertyChanged += new PropertyChangedEventHandler(TaggedRegion_PropertyChanged);
			}

			public TaggedRegion Clone()
			{
				var tr = new TaggedRegion()
				{
					Identifier = this.Identifier,
					Image = this.Image.Clone(),
					ImageUrl = this.ImageUrl,
					Region = this.Region,
					Tag = this.Tag
				};
				return tr;
			}

			void TaggedRegion_PropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				switch (e.PropertyName)
				{
					case "SourceImage":
					case "Region":
						{
							if (this.Region != null && this.SourceImage != null)
							{
								this.Image = this.SourceImage.CropImage(this.Region);
							}
						}
						break;
				}
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

			#region SourceImage (INotifyPropertyChanged Property)
			private SourceImage _SourceImage;
			[XmlIgnore]
			public SourceImage SourceImage
			{
				get { return this._SourceImage; }
				set
				{
					if (value == _SourceImage)
						return;

					this._SourceImage = value;
					this.OnPropertyChanged("SourceImage");
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
			#region Identifier (INotifyPropertyChanged Property)
			private string _Identifier;
			public string Identifier
			{
				get { return this._Identifier; }
				set
				{
					if (value == _Identifier)
						return;

					this._Identifier = value;
					this.OnPropertyChanged("Identifier");
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
		#region StatusMessage (INotifyPropertyChanged Property)
		private object _StatusMessage;
		public object StatusMessage
		{
			get { return this._StatusMessage; }
			set
			{
				if (value == _StatusMessage)
					return;

				this._StatusMessage = value;
				this.OnPropertyChanged("StatusMessage");
			}
		}
		#endregion

		#region AllTags (INotifyPropertyChanged Property)
		private ObservableCollection<string> _AllTags;
		public ObservableCollection<string> AllTags
		{
			get { return this._AllTags; }
			set
			{
				if (value == _AllTags)
					return;

				this._AllTags = value;
				this.OnPropertyChanged("AllTags");
			}
		}
		#endregion

		public class TaggedRegionCollection : ObservableCollection<TaggedRegion>
		{
			public TaggedRegion this[string tag]
			{
				get { return this.FirstOrDefault<TaggedRegion>(tr => tr.Tag == tag); }
			}
		}

		protected virtual TaggedRegionCollection
			GetDefaultTaggedRegions(SourceImage sourceImage)
		{
			var front = this.Session.FactoryTaggedRegion();
			front.Tag = "front";
			front.SourceImage = sourceImage;
			var back = this.Session.FactoryTaggedRegion();
			back.Tag = "back";
			back.SourceImage = sourceImage;
			return new TaggedRegionCollection()
			{
				front,back
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
						return;
					}

					if (this.SelectedSourceImage.TaggedRegions == null
						|| this.SelectedSourceImage.TaggedRegions.Count == 0)
					{
						this.SelectedSourceImage.TaggedRegions = this.GetDefaultTaggedRegions(
							this.SelectedSourceImage);
					}

					if (this.SelectedSourceImage.TaggedRegions.Count > 0)
						this.SelectedTaggedRegion = this.SelectedSourceImage.TaggedRegions[0];
					break;
				case "Session":
					break;
			}
		}

		public class FlashCardSession : INotifyPropertyChanged
		{
			public FlashCardSession()
			{
				this.SourceImages = new ObservableCollection<SourceImage>();
				this.SourceImages.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SourceImages_CollectionChanged);
			}

			public TaggedRegion FactoryTaggedRegion()
			{
				return new TaggedRegion()
				{
					Session = this
				};
			}

			void SourceImages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
			{
				switch (e.Action)
				{
					case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
						foreach (var item in e.NewItems)
						{
							var si = item as SourceImage;
							if (si == null)
								continue;
							si.Session = this;
						}
						break;
				}
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
		}

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

		#region public ICommand LoadSourceImagesFromFilesCommand
		public class LoadSourceImagesFromFilesFlashCardMakerViewModelCommand : ViewModelCommandBase<InstantCardsViewModel>
		{
			public LoadSourceImagesFromFilesFlashCardMakerViewModelCommand(InstantCardsViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				var paths = parameter as IEnumerable<string>;
				if (paths == null)
				{
					var ofd = new Microsoft.Win32.OpenFileDialog();
					ofd.Filter = "Images|*.png";
					ofd.Multiselect = true;
					var result = ofd.ShowDialog();
					if (result == null || !result.Value)
						return;
					paths = ofd.FileNames;
				}
				this.LoadSourceImagesFromFiles(paths);
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
			public override bool CanExecute(object parameter)
			{
				return base.CanExecute(parameter)
					&& this.ViewModel.Session != null;
			}
			public void LoadSourceImagesFromFiles(IEnumerable<string> paths)
			{
				this.Errors = new ObservableCollection<KeyValuePair<string, Exception>>();
				foreach (string path in paths)
				{
					try
					{
						var si = new SourceImage()
							{
								ImageUrl = path,
								Title = System.IO.Path.GetFileNameWithoutExtension(path),
								Session = this.ViewModel.Session,
								
							};
						si.TaggedRegions = this.ViewModel.GetDefaultTaggedRegions(si);
						this.ViewModel.Session.SourceImages.Add(
							si);
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
		public class ClearImagesFlashCardMakerViewModelCommand : ViewModelCommandBase<InstantCardsViewModel>
		{
			public ClearImagesFlashCardMakerViewModelCommand(InstantCardsViewModel viewModel)
				: base(viewModel)
			{
			}
			public override bool CanExecute(object parameter)
			{
				return base.CanExecute(parameter)
					&& this.ViewModel.Session != null
					&& this.ViewModel.Session.SourceImages != null
					&& this.ViewModel.Session.SourceImages.Count > 0;
			}
			public override void Execute(object parameter)
			{
				this.ViewModel.Session.SourceImages.Clear();
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
		public class SaveFlashCardMakerViewModelCommand : ViewModelCommandBase<InstantCardsViewModel>
		{
			public SaveFlashCardMakerViewModelCommand(InstantCardsViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				try
				{
					var path = parameter as string;

					if (String.IsNullOrEmpty(path) || System.IO.Directory.Exists(path))
					{

						var saveDialog = new Microsoft.Win32.SaveFileDialog();
						if (System.IO.Directory.Exists(path))
							saveDialog.InitialDirectory = path;
						saveDialog.Title = "Save Flash Card Session";
						saveDialog.Filter = @"Flash Card Session(*.flashcards.xml)|*.flashcards.xml";
						var result = saveDialog.ShowDialog();
						if (result == null || !result.Value)
							return;
						path = saveDialog.FileName;
					}
					this.ViewModel.StatusMessage = String.Format("Saving {0}", path);
					var session = this.ViewModel.Session;
					if (session == null)
					{
						session = new FlashCardSession();
					}
					session.SourceImages = this.ViewModel.Session.SourceImages;
					using (var xw = System.Xml.XmlWriter.Create(path))
					{
						this.ViewModel.xmlSerializer.Serialize(xw, session);
					}
					session.SessionPath = path;

					this.ViewModel.Session = session;

					this.ViewModel.StatusMessage = String.Format("Saved {0}", path);
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
		public class OpenSessionFlashCardMakerViewModelCommand : ViewModelCommandBase<InstantCardsViewModel>
		{
			public OpenSessionFlashCardMakerViewModelCommand(InstantCardsViewModel viewModel)
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
		public class ExportFlashCardMakerViewModelCommand : ViewModelCommandBase<InstantCardsViewModel>
		{
			public ExportFlashCardMakerViewModelCommand(InstantCardsViewModel viewModel)
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
							if (tr.Identifier == null)
								tr.Identifier = System.IO.Path.GetFileNameWithoutExtension(tr.ImageUrl);
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
		public class PrintFlashCardMakerViewModelCommand : ViewModelCommandBase<InstantCardsViewModel>
		{
			public PrintFlashCardMakerViewModelCommand(InstantCardsViewModel viewModel)
				: base(viewModel)
			{
			}

			public PageContent CreateFlashCardPage(
				List<SourceImage> images,
				int pagenum, int cardsPerPage, string tag, FlowDirection direction)
			{
				FixedPage fixedPage = new FixedPage();
				PageContent pageContent = new PageContent();
				//Set up the WPF Control to be printed
				FlashCardPrintPage page = new FlashCardPrintPage();
				page.PrintInfo = new FlashCardPrintPage.PageInfo()
				{
					SourceImages = images,
					CardsPerPage = cardsPerPage,
					StartIndex = pagenum * cardsPerPage,
					Tag = tag,
					Direction = direction,
					BorderThickness = new System.Windows.Thickness(2f),
					BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGray),
					ImageVerticalAlignment = VerticalAlignment.Top,
				};
				page.PrintInfo.Build();
				page.DataContext = page.PrintInfo;

				//Create first page of document
				fixedPage.Children.Add(page);
				((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);

				return pageContent;
			}

			public FixedDocument CreateFlashCardDocument(ICollection<SourceImage> sources)
			{
				var images = sources;
				if (images == null)
					images = this.ViewModel.Session.SourceImages;
				FixedDocument fixedDoc = new FixedDocument();
				int cardsPerPage = 8;
				//batch images into buckets of card size

				for (int pagenum = 0;
					pagenum <= images.Count / cardsPerPage;
					pagenum++)
				{
					var list = images.ToList();
					var frontPage = CreateFlashCardPage(
						list,
						pagenum, cardsPerPage, "front", FlowDirection.LeftToRight);
					fixedDoc.Pages.Add(frontPage);

					var backPage = CreateFlashCardPage(
						list,
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
					
					var selobjs = parameter as ICollection<object>;
					if (selobjs != null)
					{
						var doc = CreateFlashCardDocument(
							new ObservableCollection<SourceImage>(
							selobjs.Cast<SourceImage>()));
						this.ResultDocument = doc;
					}
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
		#region public ICommand PackageCommand
		public class PackageFlashCardMakerViewModelCommand : ViewModelCommandBase<InstantCardsViewModel>
		{
			public PackageFlashCardMakerViewModelCommand(InstantCardsViewModel viewModel)
				: base(viewModel)
			{
			}

			string CopyToRelativePath(string sourceUrl, string destdir)
			{
				var fn = System.IO.Path.GetFileName(sourceUrl);
				var destpath = System.IO.Path.Combine(
					destdir,
					fn);
				int count = 0;
				while (System.IO.File.Exists(destpath))
				{
					destpath = System.IO.Path.ChangeExtension(destpath,
						String.Format(
						"{0}{1}",
						count++,
						System.IO.Path.GetExtension(destpath)));
				}
				return destpath;
			}

			public override void Execute(object parameter)
			{
				var destpath = parameter as string;

				if (String.IsNullOrEmpty(destpath))
				{
					var saveDialog = new Microsoft.Win32.SaveFileDialog();
					if (System.IO.Directory.Exists(destpath))
						saveDialog.InitialDirectory = destpath;
					saveDialog.Title = "Package Flash Card Session and Images";
					saveDialog.Filter = @"Flash Card Session(*.flashcards.xml)|*.flashcards.xml";
					var result = saveDialog.ShowDialog();
					if (result == null || !result.Value)
						return;
					destpath = saveDialog.FileName;
				}
				var destdir = System.IO.Path.GetDirectoryName(destpath);

				//create a new session
				var newses = new FlashCardSession()
				{
					SessionPath = this.ViewModel.Session.SessionPath,
					Title = this.ViewModel.Session.Title,
					SourceImages = new ObservableCollection<SourceImage>()
				};

				//gather images at destpath, rename if needed, update session properties
				foreach (var si in this.ViewModel.Session.SourceImages)
				{
					try
					{
						var newpath = CopyToRelativePath(si.ImageUrl,
							destdir);
						System.IO.File.Copy(si.ImageUrl, newpath);
						var newsi = new SourceImage()
						{
							ImageUrl = newpath,
							Title = si.Title,
							TaggedRegions = new TaggedRegionCollection()
						};
						foreach (var tr in si.TaggedRegions)
						{
							try
							{
								var newtr = CopyToRelativePath(tr.ImageUrl,
									destdir);
								System.IO.File.Copy(tr.ImageUrl, newtr);
								var trinst = this.ViewModel.Session.FactoryTaggedRegion();
								trinst.ImageUrl = newtr;
								trinst.Region = tr.Region;
								trinst.Tag = tr.Tag;
								newsi.TaggedRegions.Add(trinst);
							}
							catch (Exception ex)
							{
								this.ViewModel.ErrorCollector.Add(new KeyValuePair<object, Exception>(
									tr.ImageUrl, ex));
							}
						}
						newses.SourceImages.Add(newsi);
					}
					catch (Exception ex)
					{
						this.ViewModel.ErrorCollector.Add(new KeyValuePair<object, Exception>(
							si.ImageUrl, ex));
					}
				}
				this.ViewModel.Session = newses;
				this.ViewModel.SaveCommand.Execute(destpath);
			}
		}
		PackageFlashCardMakerViewModelCommand _PackageCommand;
		public System.Windows.Input.ICommand PackageCommand
		{
			get
			{
				if (this._PackageCommand == null)
				{
					this._PackageCommand = new PackageFlashCardMakerViewModelCommand(this);
				}
				return this._PackageCommand;
			}
		}
		#endregion
		#region public ICommand NewSessionViewModelCommand
		public class NewSessionViewModelCommand : ViewModelCommandBase<InstantCardsViewModel>
		{
			public NewSessionViewModelCommand(InstantCardsViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				this.ViewModel.Session = new FlashCardSession()
				{
					SourceImages = new ObservableCollection<SourceImage>()
				};

			}
		}
		NewSessionViewModelCommand _NewSessionCommand;
		public System.Windows.Input.ICommand NewSessionCommand
		{
			get
			{
				if (this._NewSessionCommand == null)
				{
					this._NewSessionCommand = new NewSessionViewModelCommand(this);
				}
				return this._NewSessionCommand;
			}
		}
		#endregion
		#region public ICommand PasteImageFromClipboardViewModelCommand
		public class PasteImageFromClipboardViewModelCommand : ViewModelCommandBase<InstantCardsViewModel>
		{
			public PasteImageFromClipboardViewModelCommand(InstantCardsViewModel viewModel)
				: base(viewModel)
			{
			}
			public override bool CanExecute(object parameter)
			{
				return base.CanExecute(parameter);
			}
			public override void Execute(object parameter)
			{
				try
				{
					var image = (System.Windows.Interop.InteropBitmap)Clipboard.GetImage();
					if (image == null)
						return;
					this.ViewModel.Session.SourceImages.Add(
						new SourceImage()
						{
							Session = this.ViewModel.Session,
							Image = image.Clone()
						});
				}
				catch (Exception ex)
				{
					this.ViewModel.ErrorCollector.Add(new KeyValuePair<object, Exception>(
						"Clipboard", ex));
				}
			}
		}
		PasteImageFromClipboardViewModelCommand _PasteImageFromClipboardCommand;
		public System.Windows.Input.ICommand PasteImageFromClipboardCommand
		{
			get
			{
				if (this._PasteImageFromClipboardCommand == null)
				{
					this._PasteImageFromClipboardCommand = new PasteImageFromClipboardViewModelCommand(this);
				}
				return this._PasteImageFromClipboardCommand;
			}
		}
		#endregion
		#region public ICommand TagSelectedViewModelCommand
		public class TagSelectedViewModelCommand : ViewModelCommandBase<InstantCardsViewModel>
		{
			public TagSelectedViewModelCommand(InstantCardsViewModel viewModel)
				: base(viewModel)
			{
			}
			public override bool CanExecute(object parameter)
			{
				return base.CanExecute(parameter)
					&& parameter is ICollection<object>
					&& ((ICollection<object>)parameter).Count > 0;
			}
			public override void Execute(object parameter)
			{
				var selected = parameter as ICollection<object>;
				foreach (SourceImage si in selected)
				{
					//si.Tags += 
				}
			}
		}
		TagSelectedViewModelCommand _TagSelectedCommand;
		public System.Windows.Input.ICommand TagSelectedCommand
		{
			get
			{
				if (this._TagSelectedCommand == null)
				{
					this._TagSelectedCommand = new TagSelectedViewModelCommand(this);
				}
				return this._TagSelectedCommand;
			}
		}
		#endregion
		#region public ICommand DeletedSelectedSourceImagesViewModelCommand
		public class DeletedSelectedSourceImagesViewModelCommand : ViewModelCommandBase<InstantCardsViewModel>
		{
			public DeletedSelectedSourceImagesViewModelCommand(InstantCardsViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				var selobjs = parameter as ICollection<object>;
				if (selobjs != null)
				{
					foreach (SourceImage si in selobjs.ToList())
					{
						this.ViewModel.Session.SourceImages.Remove(si);
					}

				}
			}
		}
		DeletedSelectedSourceImagesViewModelCommand _DeletedSelectedSourceImagesCommand;
		public System.Windows.Input.ICommand DeletedSelectedSourceImagesCommand
		{
			get
			{
				if (this._DeletedSelectedSourceImagesCommand == null)
				{
					this._DeletedSelectedSourceImagesCommand = new DeletedSelectedSourceImagesViewModelCommand(this);
				}
				return this._DeletedSelectedSourceImagesCommand;
			}
		}
		#endregion


		#region public ICommand CloneSourceImageViewModelCommand
		public class CloneSourceImageViewModelCommand : ViewModelCommandBase<InstantCardsViewModel>
		{
			public CloneSourceImageViewModelCommand(InstantCardsViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				var si = parameter as SourceImage;
				var newsi = si.Clone();
				this.ViewModel.Session.SourceImages.Add(newsi);
			}
		}
		CloneSourceImageViewModelCommand _CloneSourceImageCommand;
		public System.Windows.Input.ICommand CloneSourceImageCommand
		{
			get
			{
				if (this._CloneSourceImageCommand == null)
				{
					this._CloneSourceImageCommand = new CloneSourceImageViewModelCommand(this);
				}
				return this._CloneSourceImageCommand;
			}
		}
		#endregion
		#region public ICommand StartQuizViewModelCommand
		public class StartQuizViewModelCommand : ViewModelCommandBase<InstantCardsViewModel>
		{
			public StartQuizViewModelCommand(InstantCardsViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				var qcol = new QuizViewModel.QuizCardCollection();
				foreach(var sc in this.ViewModel.Session.SourceImages)
				{
					qcol.Add(new QuizViewModel.QuizCard()
					{
						Id = sc.ImageUrl,
						SourceImage = sc,
						IsKnown = false,
					});
				}
				var qvm = new QuizViewModel()
				{
					AllCards = qcol,
				};

				var qw = new QuizListWindow();
				if (qvm.AllCards.Count > 0)
					qvm.CurrentCard = qvm.AllCards[0];
				qw.DataContext = qvm;
				qw.ShowActivated = true;
				qw.ShowInTaskbar = true;
				qw.Show();
			}
		}
		StartQuizViewModelCommand _StartQuizCommand;
		public System.Windows.Input.ICommand StartQuizCommand
		{
			get
			{
				if (this._StartQuizCommand == null)
				{
					this._StartQuizCommand = new StartQuizViewModelCommand(this);
				}
				return this._StartQuizCommand;
			}
		}
		#endregion


		public class SessionImageBase : INotifyPropertyChanged
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

			public SessionImageBase()
			{
				this.PropertyChanged += new PropertyChangedEventHandler(SessionImageBase_PropertyChanged);
			}

			#region Session (INotifyPropertyChanged Property)
			private FlashCardSession _Session;
			[XmlIgnore]
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
			#region LoadError (INotifyPropertyChanged Property)
			private Exception _LoadError;
			[XmlIgnore]
			public Exception LoadError
			{
				get { return this._LoadError; }
				set
				{
					if (value == _LoadError)
						return;

					this._LoadError = value;
					this.OnPropertyChanged("LoadError");
				}
			}
			#endregion

			public void SaveImage(string path)
			{
				var enc = new PngBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(this.Image));
				using (var fs = System.IO.File.Create(
					path))
				{
					enc.Save(fs);
				}
			}

			public CroppedBitmap CropImage(Int32Rect rect)
			{
				try
				{
					var cropped = new CroppedBitmap(
									this.Image,
									rect);
					return cropped;
				}
				catch (Exception)
				{
					return null;
				}
			}

			protected virtual void TryLoadImage(string path)
			{
				try
				{
					if (path == null)
						throw new ArgumentNullException("path");

					//if it's absolute and exists, just load it
					if (!System.IO.File.Exists(path) && this.Session != null)
					{
						//it might be local to the session
						string fn = System.IO.Path.GetFileName(this.ImageUrl);
						string dir = System.IO.Path.GetDirectoryName(this.Session.SessionPath);
						path = System.IO.Path.Combine(
							dir, fn);
					}
					this.Image = new BitmapImage(
						new Uri(path));
				}
				catch (Exception ex)
				{
					this.LoadError = ex;
				}
			}


			void SessionImageBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
			{
				switch (e.PropertyName)
				{
					case "Session":
					case "ImageUrl":
						if (this.Image == null && this.ImageUrl != null)
						{
							this.TryLoadImage(this.ImageUrl);
						}
						break;
				}
			}
		}

		/// <summary>
		/// An image that can be tagged with regions that represent parts of
		/// one or more flash cards
		/// </summary>
		public class SourceImage : SessionImageBase
		{
			public SourceImage()
			{
				this.TaggedRegions = new TaggedRegionCollection();
				this.TaggedRegions.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(TaggedRegions_CollectionChanged);
			}

			public SourceImage Clone()
			{
				var si = new SourceImage()
				{
					ImageUrl = this.ImageUrl,
					Image = this.Image.Clone(),
					Session = this.Session,
					Title = this.Title,
					Tags = this.Tags
				};
				foreach (var tr in this.TaggedRegions)
				{
					si.TaggedRegions.Add(tr.Clone());
				}
				return si;
			}

			void TaggedRegions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
			{
				switch (e.Action)
				{
					case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
						{
							foreach (TaggedRegion tr in e.NewItems)
							{
								tr.SourceImage = this;
							}
						}
						break;
				}
			}

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
			#region Tags (INotifyPropertyChanged Property)
			private string _Tags;
			public string Tags
			{
				get { return this._Tags; }
				set
				{
					if (value == _Tags)
						return;

					this._Tags = value;
					this.OnPropertyChanged("Tags");
				}
			}
			#endregion
			#region TaggedRegions (INotifyPropertyChanged Property)
			private TaggedRegionCollection _TaggedRegions;
			public TaggedRegionCollection TaggedRegions
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
