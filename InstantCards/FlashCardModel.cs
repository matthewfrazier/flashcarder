using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Protomeme
{
	public class FlashCardModel: INotifyPropertyChanged
	{
		public interface IAnswerValidator
		{
		}

		public class MatchAnswerValidator: IAnswerValidator
		{
		}

		public class Question: INotifyPropertyChanged
		{
			#region INotifyPropertyChanged values

			public event PropertyChangedEventHandler PropertyChanged;

			protected void RaisePropertyChanged(string propertyName)
			{
				var handler = PropertyChanged;
				if (handler != null)
				{
					handler(this, new PropertyChangedEventArgs(propertyName));
				}
			}
			#endregion

			#region AnswerValidator (INotifyPropertyChanged Property)
			private IAnswerValidator _answerValidator;

			public IAnswerValidator AnswerValidator
			{
				get { return _answerValidator; }
				set
				{
					_answerValidator = value;
					RaisePropertyChanged("AnswerValidator");
				}
			}
			#endregion

		}

		public class FlashCard : Question
		{
		}

		#region INotifyPropertyChanged values

		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion

		#region QuestionList (INotifyPropertyChanged Property)
		private List<Question> _questionList;

		public List<Question> QuestionList
		{
			get { return _questionList; }
			set
			{
				_questionList = value;
				RaisePropertyChanged("QuestionList");
			}
		}
		#endregion

	}
}
