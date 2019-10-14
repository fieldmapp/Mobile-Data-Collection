using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DLR_Data_App.ViewModels
{
  /**
   * Base class which handles the title and changing properties
   */
  public class BaseViewModel : INotifyPropertyChanged
  {
    public string TitleString = string.Empty;
    public string Title
    {
      get => TitleString;
      set => SetProperty(ref TitleString, value);
    }

    /**
     * Setting property
     * @param backingStore Old parameter
     * @param value New parameter
     * @param propertyName Name of property
     * @param onChanged EventHandler
     */
    protected bool SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName]string propertyName = "",
        Action onChanged = null)
    {
      if (EqualityComparer<T>.Default.Equals(backingStore, value))
        return false;

      backingStore = value;
      onChanged?.Invoke();
      OnPropertyChanged(propertyName);
      return true;
    }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    /**
     * EventHandler for changed property
     * @param propertyName Name of changed property
     */
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
      var changed = PropertyChanged;

      changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
  }
}
