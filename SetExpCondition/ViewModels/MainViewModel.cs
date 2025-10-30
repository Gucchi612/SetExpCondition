using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SetExpCondition.Models;

namespace SetExpCondition.ViewModels;

public partial class MainViewModel : ObservableObject
{
    // Picker 用の要因水準
    public ObservableCollection<string> Options1 { get; } = new() { "Apright", "Walking-In-Place" };
    public ObservableCollection<string> Options2 { get; } = new() { "Slow", "Medium", "Fast" };

    // Pickers の選択（自動でプロパティ変更通知を出す）
    [ObservableProperty]
    string? selectedOption1;

    partial void OnSelectedOption1Changed(string? value)
    {
        UpdateSelectionFromPickers();
    }

    [ObservableProperty]
    string? selectedOption2;

    partial void OnSelectedOption2Changed(string? value)
    {
        UpdateSelectionFromPickers();
    }

    // 条件一覧（ボタン群)
    public ObservableCollection<ConditionModel> Conditions { get; } = new();

    // フォルダ / 表示テキスト
    [ObservableProperty]
    string? folderPath;

    [ObservableProperty]
    string displayText = "保存先パス:";

    // 選択された CSV ファイル名（拡張子込み）
    [ObservableProperty]
    string? selectedCsvFileName;


    public MainViewModel()
    {
        // 条件の直積（Options1 × Options2）を生成
        foreach (var f1 in Options1)
        {
            foreach (var f2 in Options2)
            {
                Conditions.Add(new ConditionModel
                {
                    Name = $"{f1}-{f2}",
                    Factor1 = f1,
                    Factor2 = f2,
                    IsSelected = false
                });
            }
        }

        // 初期選択（任意）
        if (Options1.Count > 0) SelectedOption1 = Options1[0];
        if (Options2.Count > 0) SelectedOption2 = Options2[0];
    }

    // ボタンから呼ばれるコマンド（ConditionModel を受け取る）
    [RelayCommand]
    void SelectCondition(ConditionModel model)
    {
        if (model is null) return;

        // Picker を更新（これにより Pickers -> Model の同期も動く）
        SelectedOption1 = model.Factor1;
        SelectedOption2 = model.Factor2;

        UpdateSelectedModel(model);
        //DisplayText = $"選択: {model.Name} (要因1={model.Factor1}, 要因2={model.Factor2})";
    }

    // 保存先確定コマンド
    [RelayCommand]
    void ConfirmFolder()
    {
        DisplayText = $"保存先CSV: {FolderPath}";
    }

    // CSV保存コマンド（実装を入れてください）
    [RelayCommand]
    void SaveCsv()
    {
        // CSV 保存処理をここに実装
    }

    void UpdateSelectionFromPickers()
    {
        ConditionModel? hit = null;
        foreach (var c in Conditions)
        {
            var match = c.Factor1 == SelectedOption1 && c.Factor2 == SelectedOption2;
            c.IsSelected = match;
            if (match) hit = c;
        }

        //if (hit != null)
            //DisplayText = $"選択: {hit.Name} (要因1={hit.Factor1}, 要因2={hit.Factor2})";
    }

    void UpdateSelectedModel(ConditionModel selected)
    {
        foreach (var c in Conditions)
            c.IsSelected = ReferenceEquals(c, selected);
    }
}
