using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Storage;

namespace SetExpCondition.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string folderPath = string.Empty;

    [ObservableProperty]
    private string displayText = "保存先パス:";

    public ObservableCollection<string> Options1 { get; } =
        new(["条件1", "条件2", "条件3", "条件4"]);

    public ObservableCollection<string> Options2 { get; } =
        new(["条件1", "条件2", "条件3", "条件4"]);

    [RelayCommand]
    private void ConfirmFolder()
    {
        if (!string.IsNullOrWhiteSpace(FolderPath))
            DisplayText = $"保存先パス: {FolderPath}";
        else
            DisplayText = "パスが入力されていません。";
    }

    [RelayCommand]
    private async Task SelectCondition(string condition)
    {
        await App.Current.MainPage.DisplayAlert("Condition", $"{condition} がクリックされました。", "OK");
    }

    [RelayCommand]
    private async Task SaveCSV()
    {
        await App.Current.MainPage.DisplayAlert("Save CSV", "CSVが保存されました。", "OK");
    }

    [RelayCommand]
    private async Task OnPickerSelected(string selectedItem)
    {
        if (!string.IsNullOrWhiteSpace(selectedItem))
            await App.Current.MainPage.DisplayAlert("選択された項目", $"選択: {selectedItem}", "OK");
    }
}
