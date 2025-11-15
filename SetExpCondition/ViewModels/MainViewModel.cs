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
    public ObservableCollection<string> Options1 { get; } = new() { "Standing", "Walking-In-Place" };
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

    // 試行回数（1以上）
    [ObservableProperty]
    int trialCount = 1;

    // 参加者 ID と Name（MainPage の Entry にバインド）
    [ObservableProperty]
    string? participantId;

    [ObservableProperty]
    string? participantName;

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

    // 試行回数を増やす（＋）
    [RelayCommand]
    void IncreaseTrialCount()
    {
        TrialCount++;
    }

    // 試行回数を減らす（－、1未満にはしない）
    [RelayCommand]
    void DecreaseTrialCount()
    {
        if (TrialCount > 1)
            TrialCount--;
    }

    // 試行回数をリセット（1 に戻す）
    [RelayCommand]
    void ResetTrialCount()
    {
        TrialCount = 1;
    }

    // CSV保存コマンド：先頭列に id,name を追加して書き込む
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

            // ヘッダー（先頭に id,name を追加）と値列を作成
            var headerColumns = new[] { "id", "name", "Trials", "PhysicalLocomotionMethod", "Virtualself-MovementSpeed" };
            var valueColumns = new[] {
                ParticipantId ?? string.Empty,
                ParticipantName ?? string.Empty,
                TrialCount.ToString(),
                SelectedOption1 ?? string.Empty,
                SelectedOption2 ?? string.Empty
            };

            string EscapeCsv(string s)
            {
                if (s is null) return "";
                if (s.Contains('"') || s.Contains(',') || s.Contains('\n') || s.Contains('\r'))
                    return "\"" + s.Replace("\"", "\"\"") + "\"";
                return s;
            }

            var headerLine = string.Join(",", headerColumns.Select(EscapeCsv));
            var valueLine = string.Join(",", valueColumns.Select(EscapeCsv));

            var sb = new StringBuilder();
            sb.AppendLine(headerLine);
            sb.AppendLine(valueLine);

            await File.WriteAllTextAsync(outPath, sb.ToString(), Encoding.UTF8);

            // 成功表示とファイル名保持
            SelectedCsvFileName = Path.GetFileName(outPath);

            // DisplayText の二行目以降に CSV 出力内容を表示する
            DisplayText = $"CSV を保存しました: {outPath}\n[{headerLine}] = [{valueLine}]";

            // 必要なら View に通知する処理（既存のイベント等）を呼ぶ
            CsvSaved?.Invoke(this, outPath);
        }
        catch (Exception ex)
        {
            DisplayText = $"CSV保存に失敗しました: {ex.Message}";
        }
    }

    // CSV 保存完了を通知するためのイベント（既存コードと互換）
    public event EventHandler<string?>? CsvSaved;

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
