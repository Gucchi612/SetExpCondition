using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Maui;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel; // MainThread
using SetExpCondition.ViewModels;

namespace SetExpCondition.Views;

public partial class MainPage : ContentPage
{
    MainViewModel? vm;

    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        vm = BindingContext as MainViewModel;

        if (vm != null)
        {
            // ViewModel の CsvSaved イベントを購読して、保存成功時にモーダルを表示
            vm.CsvSaved += Vm_CsvSaved;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (vm != null)
        {
            vm.CsvSaved -= Vm_CsvSaved;
            vm = null;
        }
    }

    // 保存成功通知を受け取りモーダル表示（UI スレッドで実行）
    void Vm_CsvSaved(object? sender, string? path)
    {
        var displayPath = string.IsNullOrEmpty(path) ? "(ファイルパス無し)" : path;
        // UI スレッドで DisplayAlert を実行
        MainThread.InvokeOnMainThreadAsync(() => DisplayAlert("Completed Saving!", $"CSV を保存しました:\n{displayPath}", "OK"));
    }

    // 保存先確定ボタンのハンドラ：Windows のときは CSV ファイルを選択し、
    // 選択ファイルのあるフォルダを ViewModel.FolderPath に設定して ConfirmFolderCommand を呼ぶ。
    // キャンセル（未選択）は result が null で処理されないため、そのまま何もしない。
    async void OnConfirmFolderClicked(object? sender, EventArgs e)
    {
        if (BindingContext is not MainViewModel vm) return;

        // Windows のみ CSV ファイルを選択してファイル名を表示
        if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
        {
            try
            {
                var csvFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".csv" } },
                    { DevicePlatform.Android, new[] { "text/csv" } },
                    { DevicePlatform.iOS, new[] { "public.comma-separated-values-text" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.comma-separated-values-text" } }
                });

                var pickOptions = new PickOptions
                {
                    PickerTitle = "CSV ファイルを選択してください（キャンセルで未選択）",
                    FileTypes = csvFileType
                };

                var result = await FilePicker.Default.PickAsync(pickOptions);
                if (result != null)
                {
                    var full = result.FullPath ?? result.FileName;
                    var folder = Path.GetDirectoryName(full) ?? full;
                    vm.FolderPath = folder;

                    // ファイル名（拡張子 .csv を含む）を ViewModel に格納
                    vm.SelectedCsvFileName = Path.GetFileName(full);

                    if (vm.ConfirmFolderCommand.CanExecute(null))
                        vm.ConfirmFolderCommand.Execute(null);
                }
                else
                {
                    // キャンセル（未選択）した場合は FileName をクリア
                    vm.SelectedCsvFileName = string.Empty;
                }
            }
            catch (Exception)
            {
                vm.SelectedCsvFileName = string.Empty;
            }
        }
        else
        {
            vm.SelectedCsvFileName = string.Empty;
            if (vm.ConfirmFolderCommand.CanExecute(null))
                vm.ConfirmFolderCommand.Execute(null);
        }
    }

    void OnClearSelectionClicked(object? sender, EventArgs e)
    {
        if (BindingContext is not MainViewModel vm) return;

        vm.FolderPath = string.Empty;
        vm.SelectedCsvFileName = string.Empty;

        if (vm.ConfirmFolderCommand.CanExecute(null))
            vm.ConfirmFolderCommand.Execute(null);
    }
}
