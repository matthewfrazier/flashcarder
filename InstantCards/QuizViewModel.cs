using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace Protomeme
{
	public class QuizViewModel: INotifyPropertyChanged
	{
		public QuizViewModel()
		{
			this.CurrentRegionToShow = "front";
			this.PropertyChanged += new PropertyChangedEventHandler(QuizViewModel_PropertyChanged);
		}

		void QuizViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "CurrentRegionToShow":
				case "CurrentCard":
					try
					{
						this.CurrentCardImage = this.CurrentCard.SourceImage.TaggedRegions[this.CurrentRegionToShow].Image;
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debug.WriteLine(ex);
						this.CurrentCardImage = null;
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

		public class TestSession:INotifyPropertyChanged
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
		}

		public class QuizCardCollection : ObservableCollection<QuizCard>
		{
			public QuizCardCollection()
			{

			}
			public QuizCardCollection(IEnumerable<QuizCard> collection)
				: base(collection)
			{

			}
		}

		public class QuizCard: INotifyPropertyChanged
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
			#region Id (INotifyPropertyChanged Property)
			private string _Id;
			public string Id
			{
				get { return this._Id; }
				set
				{
					if (value == _Id)
						return;

					this._Id = value;
					this.OnPropertyChanged("Id");
				}
			}
			#endregion
			#region SourceImage (INotifyPropertyChanged Property)
			private InstantCardsViewModel.SourceImage _SourceImage;
			public InstantCardsViewModel.SourceImage SourceImage
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
			#region IsKnown (INotifyPropertyChanged Property)
			private bool _IsKnown;
			public bool IsKnown
			{
				get { return this._IsKnown; }
				set
				{
					if (value == _IsKnown)
						return;

					this._IsKnown = value;
					this.OnPropertyChanged("IsKnown");
				}
			}
			#endregion
		}


		#region CurrentCard (INotifyPropertyChanged Property)
		private QuizCard _CurrentCard;
		public QuizCard CurrentCard
		{
			get { return this._CurrentCard; }
			set
			{
				if (value == _CurrentCard)
					return;

				this._CurrentCard = value;
				this.OnPropertyChanged("CurrentCard");
			}
		}
		#endregion
		#region CurrentRegionToShow (INotifyPropertyChanged Property)
		private string _CurrentRegionToShow;
		public string CurrentRegionToShow
		{
			get { return this._CurrentRegionToShow; }
			set
			{
				if (value == _CurrentRegionToShow)
					return;

				this._CurrentRegionToShow = value;
				this.OnPropertyChanged("CurrentRegionToShow");
			}
		}
		#endregion
		#region CurrentCardImage (INotifyPropertyChanged Property)
		private ImageSource _CurrentCardImage;
		public ImageSource CurrentCardImage
		{
			get { return this._CurrentCardImage; }
			set
			{
				if (value == _CurrentCardImage)
					return;

				this._CurrentCardImage = value;
				this.OnPropertyChanged("CurrentCardImage");
			}
		}
		#endregion
		#region AllCards (INotifyPropertyChanged Property)
		private QuizCardCollection _AllCards;
		public QuizCardCollection AllCards
		{
			get { return this._AllCards; }
			set
			{
				if (value == _AllCards)
					return;

				this._AllCards = value;
				this.OnPropertyChanged("AllCards");
			}
		}
		#endregion
		#region public ICommand FlipCardViewModelCommand
		public class FlipCardViewModelCommand : ViewModelCommandBase<QuizViewModel>
		{
			public FlipCardViewModelCommand(QuizViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				if (this.ViewModel.CurrentRegionToShow == "front")
					this.ViewModel.CurrentRegionToShow = "back";
				else
					this.ViewModel.CurrentRegionToShow = "front";
			}
		}
		FlipCardViewModelCommand _FlipCardCommand;
		public System.Windows.Input.ICommand FlipCardCommand
		{
			get
			{
				if (this._FlipCardCommand == null)
				{
					this._FlipCardCommand = new FlipCardViewModelCommand(this);
				}
				return this._FlipCardCommand;
			}
		}
		#endregion
		#region public ICommand NextCardViewModelCommand
		public class NextCardViewModelCommand : ViewModelCommandBase<QuizViewModel>
		{
			public NextCardViewModelCommand(QuizViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				int index = this.ViewModel.AllCards.IndexOf(this.ViewModel.CurrentCard);
				if (index < this.ViewModel.AllCards.Count)
					index++;
				else
					index = 0;
				this.ViewModel.CurrentCard = this.ViewModel.AllCards[index];
			}
		}
		NextCardViewModelCommand _NextCardCommand;
		public System.Windows.Input.ICommand NextCardCommand
		{
			get
			{
				if (this._NextCardCommand == null)
				{
					this._NextCardCommand = new NextCardViewModelCommand(this);
				}
				return this._NextCardCommand;
			}
		}
		#endregion
		#region public ICommand PreviousCardViewModelCommand
		public class PreviousCardViewModelCommand : ViewModelCommandBase<QuizViewModel>
		{
			public PreviousCardViewModelCommand(QuizViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				throw new NotImplementedException();
			}
		}
		PreviousCardViewModelCommand _PreviousCardCommand;
		public System.Windows.Input.ICommand PreviousCardCommand
		{
			get
			{
				if (this._PreviousCardCommand == null)
				{
					this._PreviousCardCommand = new PreviousCardViewModelCommand(this);
				}
				return this._PreviousCardCommand;
			}
		}
		#endregion
		#region public ICommand ToggleKnownViewModelCommand
		public class ToggleKnownViewModelCommand : ViewModelCommandBase<QuizViewModel>
		{
			public ToggleKnownViewModelCommand(QuizViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				this.ViewModel.CurrentCard.IsKnown = !this.ViewModel.CurrentCard.IsKnown;
			}
		}
		ToggleKnownViewModelCommand _ToggleKnownCommand;
		public System.Windows.Input.ICommand ToggleKnownCommand
		{
			get
			{
				if (this._ToggleKnownCommand == null)
				{
					this._ToggleKnownCommand = new ToggleKnownViewModelCommand(this);
				}
				return this._ToggleKnownCommand;
			}
		}
		#endregion
		#region public ICommand ShuffleCardsViewModelCommand
		public class ShuffleCardsViewModelCommand : ViewModelCommandBase<QuizViewModel>
		{
			public ShuffleCardsViewModelCommand(QuizViewModel viewModel)
				: base(viewModel)
			{
			}
			public override void Execute(object parameter)
			{
				this.ViewModel.AllCards.ShuffleInPlace();
			}
		}
		ShuffleCardsViewModelCommand _ShuffleCardsCommand;
		public System.Windows.Input.ICommand ShuffleCardsCommand
		{
			get
			{
				if (this._ShuffleCardsCommand == null)
				{
					this._ShuffleCardsCommand = new ShuffleCardsViewModelCommand(this);
				}
				return this._ShuffleCardsCommand;
			}
		}
		#endregion
	}
}
