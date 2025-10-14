using CommunityToolkit.Maui.Storage;

namespace SetExpCondition;

public partial class MainPage : ContentPage
{
	// int count = 0;

	public MainPage()
	{
		InitializeComponent();
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
			DisplayLabel.Text = $"入力されたパス: {enteredText}";
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
		await MainThread.InvokeOnMainThreadAsync(async () =>
		{
			if (sender is Button button)
			{
				await DisplayAlert("条件ボタン", $"{button.Text} がクリックされました。", "OK");
			}
		});
	}
}

