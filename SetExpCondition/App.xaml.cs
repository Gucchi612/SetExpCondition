using Microsoft.Maui;
using Microsoft.Maui.Controls;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;
#endif

namespace SetExpCondition;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();

#if WINDOWS
        // Windows の場合のみ、ウィンドウサイズを変更する処理を遅延実行
        Microsoft.Maui.Controls.Application.Current.Dispatcher.Dispatch(async () =>
        {
            await Task.Delay(200); // ウィンドウ生成が完了するまで少し待つ

            var window = App.Current.Windows.FirstOrDefault()?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            if (window is null) return;

            var hwnd = WindowNative.GetWindowHandle(window);
            var winId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(winId);

            // 初期サイズを指定（例：幅 1000px, 高さ 700px）
            appWindow.Resize(new SizeInt32(1000, 700));

            // ウィンドウを中央に表示する
            //var displayArea = DisplayArea.GetFromWindowId(winId, DisplayAreaFallback.Primary);
            //var centerPosition = new PointInt32(
            //    (displayArea.WorkArea.Width - 1200) / 2,
            //    (displayArea.WorkArea.Height - 800) / 2
            //);
            //appWindow.Move(centerPosition);
        });
#endif
    }
}
