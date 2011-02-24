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
using System.ComponentModel;

namespace Protomeme
{
	/// <summary>
	/// Interaction logic for FlashCardPrintPage.xaml
	/// </summary>
	public partial class FlashCardPrintPage : UserControl,
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



		#region PrintInfo (INotifyPropertyChanged Property)
		private PageInfo _PrintInfo;
		public PageInfo PrintInfo
		{
			get { return this._PrintInfo; }
			set
			{
				if (value == _PrintInfo)
					return;

				this._PrintInfo = value;
				this.OnPropertyChanged("PrintInfo");
			}
		}
		#endregion
		public class PageInfo: INotifyPropertyChanged
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
			#region StartIndex (INotifyPropertyChanged Property)
			private int _StartIndex;
			public int StartIndex
			{
				get { return this._StartIndex; }
				set
				{
					if (value == _StartIndex)
						return;

					this._StartIndex = value;
					this.OnPropertyChanged("StartIndex");
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
			#region CardsPerPage (INotifyPropertyChanged Property)
			private int _CardsPerPage;
			public int CardsPerPage
			{
				get { return this._CardsPerPage; }
				set
				{
					if (value == _CardsPerPage)
						return;

					this._CardsPerPage = value;
					this.OnPropertyChanged("CardsPerPage");
				}
			}
			#endregion
			#region Direction (INotifyPropertyChanged Property)
			private FlowDirection _Direction;
			public FlowDirection Direction
			{
				get { return this._Direction; }
				set
				{
					if (value == _Direction)
						return;

					this._Direction = value;
					this.OnPropertyChanged("Direction");
				}
			}
			#endregion
			#region SourceImages (INotifyPropertyChanged Property)
			private IList<FlashCardMakerViewModel.SourceImage> _SourceImages;
			public IList<FlashCardMakerViewModel.SourceImage> SourceImages
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

			#region BorderThickness (INotifyPropertyChanged Property)
			private Thickness _BorderThickness;
			public Thickness BorderThickness
			{
				get { return this._BorderThickness; }
				set
				{
					if (value == _BorderThickness)
						return;

					this._BorderThickness = value;
					this.OnPropertyChanged("BorderThickness");
				}
			}
			#endregion
			#region BorderBrush (INotifyPropertyChanged Property)
			private Brush _BorderBrush;
			public Brush BorderBrush
			{
				get { return this._BorderBrush; }
				set
				{
					if (value == _BorderBrush)
						return;

					this._BorderBrush = value;
					this.OnPropertyChanged("BorderBrush");
				}
			}
			#endregion
			public PageInfo()
			{

			}

			public void Build()
			{
				this.CardsOnPage = new System.Collections.ObjectModel.ObservableCollection<CardViewModel>();
				for(int i = 0; i < this.CardsPerPage; i++)
				{
					var pos = i+this.StartIndex;
					if (pos >= this.SourceImages.Count)
						break;
					var si = this.SourceImages[pos];

					var tr = si.TaggedRegions.ToList().Find(
						m => {return m.Tag == this.Tag;});
					var url = "missing_image.png";
					if (tr == null)
						continue;
					url = tr.ImageUrl;
					
					this.CardsOnPage.Add(new CardViewModel()
					{
						ImageUrl = url,
						Header = tr.Identifier
					});
				}
			}

			public class CardViewModel : INotifyPropertyChanged
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
				#region Header (INotifyPropertyChanged Property)
				private string _Header;
				public string Header
				{
					get { return this._Header; }
					set
					{
						if (value == _Header)
							return;

						this._Header = value;
						this.OnPropertyChanged("Header");
					}
				}
				#endregion
			}

			#region CardsOnPage (INotifyPropertyChanged Property)
			private System.Collections.ObjectModel.ObservableCollection<CardViewModel> _CardsOnPage;
			public System.Collections.ObjectModel.ObservableCollection<CardViewModel> CardsOnPage
			{
				get { return this._CardsOnPage; }
				set
				{
					if (value == _CardsOnPage)
						return;

					this._CardsOnPage = value;
					this.OnPropertyChanged("CardsOnPage");
				}
			}
			#endregion
		}

		public FlashCardPrintPage()
		{
			InitializeComponent();
		}
	}
}
