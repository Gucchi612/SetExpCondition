using CommunityToolkit.Maui.Storage;

namespace SetExpCondition;

public partial class MainPage : ContentPage
{
	// int count = 0;
	public List<string> Options { get; set; }

	public MainPage()
	{
		InitializeComponent();

		Options = new List<string>
		{
			"オプション1",
			"オプション2",
			"オプション3",
			"オプション4"
		};
		BindingContext = this;
	}

	// private void OnCounterClicked(object sender, EventArgs e)
	// {
	// 	count++;

	// 	if (count == 1)
	// 		CounterBtn.Text = $"Clicked {count} time";
	// 	else
	// 		CounterBtn.Text = $"Clicked {count} times";

	// 	SemanticScreenReader.Announce(CounterBtn.Text);
	// }


	private void OnSelectFolderClicked(object sender, EventArgs e)
	{
		// try
		// {
		// 	// FolderPicker を試す
		// 	var folder = await FolderPicker.Default.PickAsync();
		// 	if (folder != null)
		// 	{
		// 		FolderPathLabel.Text = folder.Folder.Path;
		// 	}
		// 	else
		// 	{
		// 		FolderPathLabel.Text = "フォルダ選択がキャンセルされました";
		// 	}
		// }
		// catch (NotImplementedException)
		// {
		// 	// プラットフォームで未実装の場合の通知
		// 	await DisplayAlert("未対応", "このプラットフォームではフォルダ選択がサポートされていません。", "OK");
		// }
		// catch (Exception ex)
		// {
		// 	// その他のエラー
		// 	await DisplayAlert("エラー", ex.Message, "OK");
		// }

		// テキストボックスの内容を取得してラベルに表示
		string enteredText = FolderPathEntry.Text;
		if (!string.IsNullOrWhiteSpace(enteredText))
		{
			DisplayLabel.Text = $"保存先パス: {enteredText}";
		}
		else
		{
			DisplayLabel.Text = "パスが入力されていません。";
		}
	}

	private void OnSaveClicked(object sender, EventArgs e)
	{
		// 保存処理を実装
		// DisplayLabel.Text = "設定が保存されました。";
	}

	private async void OnConditionClicked(object sender, EventArgs e)
	{
		if (sender is ConditionButton button)   // sender を ConditionButton 型にキャスト(これで正しく動作)
		{
			await DisplayAlert("Condition Button is Clicked!", $"{button.Text} がクリックされました。", "OK");
		}
	}

	private async void OnSaveCSVClicked(object sender, EventArgs e)
	{
		// CSV保存処理を実装
		await DisplayAlert("Save CSV!", "CSVが保存されました。", "OK");
	}
	
	private void OnPickerSelectedIndexChanged(object sender, EventArgs e)
    {
        if (OptionsPicker.SelectedIndex != -1)
        {
            string selectedOption = OptionsPicker.Items[OptionsPicker.SelectedIndex];
            DisplayAlert("選択された項目", $"選択された項目: {selectedOption}", "OK");
        }
    }
}

