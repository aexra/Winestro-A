using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Winestro_A.Helpers;

namespace Winestro_A.Models;
public class MainPageDataContainer : INotifyPropertyChanged
{
    private string connectionState = "fix me";
    private string guildsConnected = "0";
    private string playersActive = "0";
    private string currentTime = TimeHelper.NowS();
    private string runTime = "0";
    private string runBtnText = "Fix me";
    private string runBtnColor = "#ffffff";

    public string RunBtnText 
    { 
        get => runBtnText;
        set
        {
            if (value !=  runBtnText)
            {
                runBtnText = value;
                NotifyPropertyChanged();
            }
        }
    }
    public string RunBtnColor
    {
        get => runBtnColor;
        set
        {
            if (value != runBtnColor)
            {
                runBtnColor = value;
                NotifyPropertyChanged();
            }
        }
    }
    public string ConnectionState
    {
        get => connectionState;
        set 
        {
            if (value != connectionState)
            {
                connectionState = value;
                NotifyPropertyChanged();
            }
        }
    }
    public string GuildsConnected
    {
        get => guildsConnected;
        set
        {
            if (value != guildsConnected)
            {
                guildsConnected = value;
                NotifyPropertyChanged();
            }
        }
    }
    public string PlayersActive
    {
        get => playersActive;
        set
        {
            if (value != playersActive)
            {
                playersActive = value;
                NotifyPropertyChanged();
            }
        }
    }
    public string CurrentTime
    {
        get => currentTime;
        set
        {
            if (value != currentTime)
            {
                currentTime = value;
                NotifyPropertyChanged();
            }
        }
    }
    public string RunTime
    {
        get => runTime;
        set
        {
            if (value != runTime)
            {
                runTime = value;
                NotifyPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
