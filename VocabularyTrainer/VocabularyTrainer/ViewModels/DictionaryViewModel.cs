﻿
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VocabularyTrainer.Data;
using VocabularyTrainer.Views;
using Xamarin.Forms;


namespace VocabularyTrainer.ViewModels
{
	public class DictionaryViewModel : BaseViewModel
	{
		public ObservableCollection<Word> Words { get; } = new ObservableCollection<Word>();
		public Command AddCommand { get; }
		public Command DeleteCommand { get; private set; }
		public Command EditCommand { get; }
		bool isMenuItemEnabled = false;
		public bool IsMenuItemEnabled
		{
			get { return isMenuItemEnabled; }
			set
			{
				isMenuItemEnabled = value;
				DeleteCommand.ChangeCanExecute();
			}
		}
		public Word SelectedWord
		{
			get => GetValue<Word>();
			set
			{
				SetValue(value);
				DeleteCommand.ChangeCanExecute();
				EditCommand.ChangeCanExecute();
			}
		}

		public DictionaryViewModel()
		{
			Title = "Dictionary";

			DeleteCommand = new Command(DeleteCommandAction, () => SelectedWord != null);
			EditCommand = new Command(EditCommandAction, () => SelectedWord != null);
			AddCommand = new Command(AddCommandAction);

			Task.Run(() => ReloadWordsAsync());
			App.DB.WordsUpdated += DB_WordsUpdated;
		}

		public void DownloadAction()
		{
			string jsonString = JsonConvert.SerializeObject(Words, Formatting.Indented);
		}

		private void DB_WordsUpdated(object sender, System.EventArgs e)
		{
			Task.Run(() => ReloadWordsAsync());
		}

		private async Task ReloadWordsAsync()
		{
			Words.Clear();
			var words = await App.DB.GetWordsAsync().ConfigureAwait(false);
			foreach (var word in words)
				Words.Add(word);
			OnPropertyChanged(nameof(Words));
		}

		public async void DeleteCommandAction()
		{
			await App.DB.DeleteWordAsync(SelectedWord.ID);
			Words.Remove(SelectedWord);
		}

		public async void EditCommandAction()
		{
			await Shell.Current.Navigation.PushAsync(new NewItemPage(SelectedWord));
		}

		public async void AddCommandAction()
		{
			await Shell.Current.GoToAsync(nameof(NewItemPage));

		}

		public void OnAppearing()
		{
		}
	}
}