using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Protomeme
{
	public class ProgressInfo
	{
		public string Title { get; set; }
		public long? CurrentCount { get; set; }
		public long? ExpectedCount { get; set; }
	}

	public interface IProgressSource: INotifyPropertyChanged
	{
		ProgressInfo CurrentProgress { get; }
		IEnumerable<IProgressSource> ProgressDetails { get; }
	}

	public interface IProgressSink
	{
	}

	public interface IRunnable
	{
		bool CanRun { get; }
		void Run(object state);
	}

	public interface IErrorCollector: 
		IList<KeyValuePair<object,Exception>>,
		INotifyPropertyChanged
	{
		bool HasErrors { get; }
		string ShortErrorSummary { get; }
	}


	/// <summary>
	/// Describes an object which encapsulates an operation which may need to be exected
	/// over time, in the background, in a deferred state, with progress, etc.
	/// </summary>
	public interface IModelCommand: IRunnable, IProgressSource, IProgressSink
	{
		event EventHandler RunStarted;
		
		/// <summary>
		/// Fired in any case the command completes, even in an error state
		/// </summary>
		event EventHandler RunCompleted;

		object Result { get; set; }
				
	}

	public abstract class ModelCommandBase :IModelCommand
	{
		public ModelCommandBase()
		{
			this.Settings = new ModelCommandSettings()
			{
				NotifyProgressOnStart = true,
				NotifyProgressOnFinish = true,
				NotifyProgress = true
			};
		}
		public class ModelCommandSettings
		{
			public bool NotifyProgressOnStart { get; set; }
			public bool NotifyProgressOnFinish { get; set; }
			public bool NotifyProgress { get; set; }
			
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



		#region Settings (INotifyPropertyChanged Property)
		private ModelCommandSettings _Settings;
		public ModelCommandSettings Settings
		{
			get { return this._Settings; }
			set
			{
				if (value == _Settings)
					return;

				this._Settings = value;
				this.OnPropertyChanged("Settings");
			}
		}
		#endregion
		
		#region IRunnable Members

		public bool CanRun
		{
			get { return true; }
		}

		public void Run(object state)
		{
		}

		#endregion

		#region IProgressSource Members


		#region CurrentProgress (INotifyPropertyChanged Property)
		private ProgressInfo _CurrentProgress;
		public ProgressInfo CurrentProgress
		{
			get { return this._CurrentProgress; }
			set
			{
				if (value == _CurrentProgress)
					return;

				this._CurrentProgress = value;
				this.OnPropertyChanged("CurrentProgress");
			}
		}
		#endregion

		#region ProgressDetails (INotifyPropertyChanged Property)
		private IEnumerable<IProgressSource> _ProgressDetails;
		public IEnumerable<IProgressSource> ProgressDetails
		{
			get { return this._ProgressDetails; }
			set
			{
				if (value == _ProgressDetails)
					return;

				this._ProgressDetails = value;
				this.OnPropertyChanged("ProgressDetails");
			}
		}
		#endregion

		#endregion

		#region Result (INotifyPropertyChanged Property)
		private object _Result;
		public object Result
		{
			get { return this._Result; }
			set
			{
				if (value == _Result)
					return;

				this._Result = value;
				this.OnPropertyChanged("Result");
			}
		}
		#endregion


		public event EventHandler RunStarted;
		protected virtual void OnRunStarted()
		{
			EventHandler handler = this.RunStarted;
			if (handler == null)
				return;
			handler(this, EventArgs.Empty);
		}


		public event EventHandler RunCompleted;
		protected virtual void OnRunCompleted()
		{
			EventHandler handler = this.RunCompleted;
			if (handler == null)
				return;
			handler(this, EventArgs.Empty);
		}
	}


	public abstract class ViewModelBase: INotifyPropertyChanged
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


	public abstract class ViewModelCommandBase<TViewModel> : System.Windows.Input.ICommand,
		System.ComponentModel.INotifyPropertyChanged
		where TViewModel:class
	{
		public ViewModelCommandBase(TViewModel viewModel)
		{
			this._ViewModel = viewModel;
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

		#region ViewModel (INotifyPropertyChanged Property)
		private TViewModel _ViewModel;
		public TViewModel ViewModel
		{
			get { return this._ViewModel; }
			set
			{
				if (value == _ViewModel)
					return;

				this._ViewModel = value;
				this.OnPropertyChanged("ViewModel");
			}
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

}
