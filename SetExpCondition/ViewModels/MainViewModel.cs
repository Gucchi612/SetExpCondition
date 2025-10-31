using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    // CSV 保存完了を通知するイベント（View が購読してモーダル表示する）
    public event EventHandler<string?>? CsvSaved;


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

    // CSV保存コマンド：一行目に要因名、二行目に水準名を書き込む
    [RelayCommand]
    public async Task SaveCsv()
    {
        try
        {
            // 出力先パス決定
            string? outPath = null;

            if (!string.IsNullOrWhiteSpace(SelectedCsvFileName) && !string.IsNullOrWhiteSpace(FolderPath))
            {
                outPath = Path.Combine(FolderPath!, SelectedCsvFileName!);
            }
            else if (!string.IsNullOrWhiteSpace(FolderPath))
            {
                // FolderPath がファイル名を含む可能性を考慮
                if (Path.HasExtension(FolderPath) && Path.GetExtension(FolderPath).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    outPath = FolderPath;
                }
                else
                {
                    // ディレクトリと見なしてデフォルト名を付与
                    outPath = Path.Combine(FolderPath, "experiment.csv");
                }
            }

            if (string.IsNullOrWhiteSpace(outPath))
            {
                DisplayText = "保存先が指定されていません。";
                return;
            }

            // ディレクトリ作成（必要なら）
            var dir = Path.GetDirectoryName(outPath) ?? "";
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // ヘッダー（要因名）と2行目（水準名）を作成
            // 要因名は UI のラベルと合わせて固定文字列にしています
            var factorNames = new[] { "Condition1_PL", "Condition2_VS" };
            var levelNames = new[] { SelectedOption1 ?? string.Empty, SelectedOption2 ?? string.Empty };

            string EscapeCsv(string s)
            {
                if (s is null) return "";
                if (s.Contains('"') || s.Contains(',') || s.Contains('\n') || s.Contains('\r'))
                    return "\"" + s.Replace("\"", "\"\"") + "\"";
                return s;
            }

            var headerLine = string.Join(",", factorNames.Select(EscapeCsv));
            var levelLine = string.Join(",", levelNames.Select(EscapeCsv));

            var sb = new StringBuilder();
            sb.AppendLine(headerLine);
            sb.AppendLine(levelLine);

            await File.WriteAllTextAsync(outPath, sb.ToString(), Encoding.UTF8);

            // 成功表示とファイル名保持
            SelectedCsvFileName = Path.GetFileName(outPath);

            // View に保存完了を通知（モーダル表示用）
            CsvSaved?.Invoke(this, outPath);
        }
        catch (Exception ex)
        {
            DisplayText = $"CSV保存に失敗しました: {ex.Message}";
        }
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
    }

    void UpdateSelectedModel(ConditionModel selected)
    {
        foreach (var c in Conditions)
            c.IsSelected = ReferenceEquals(c, selected);
    }
}
