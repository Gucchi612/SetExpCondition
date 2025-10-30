using CommunityToolkit.Mvvm.ComponentModel;

namespace SetExpCondition.Models;

public partial class ConditionModel : ObservableObject
{
    [ObservableProperty]
    string name = string.Empty;

    [ObservableProperty]
    string factor1 = string.Empty;

    [ObservableProperty]
    string factor2 = string.Empty;

    [ObservableProperty]
    bool isSelected;
}