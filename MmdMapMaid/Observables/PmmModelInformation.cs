using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MmdMapMaid.Observables;

public partial class PmmModelInformation : ObservableRecipient
{
    [ObservableProperty]
    private int _index;
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private string _path;

    public PmmModelInformation(int index, string name, string path)
    {
        _index = index;
        _name = name;
        _path = path;
    }
}
