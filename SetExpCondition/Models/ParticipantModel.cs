using CommunityToolkit.Mvvm.ComponentModel;

namespace SetExpCondition.Models;

public partial class ParticipantModel : ObservableObject
{
    [ObservableProperty]
    int id;

    [ObservableProperty]
    string name = string.Empty;
}