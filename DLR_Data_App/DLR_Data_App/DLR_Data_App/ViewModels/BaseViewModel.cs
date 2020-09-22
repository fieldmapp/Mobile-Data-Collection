using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DLR_Data_App.ViewModels
{
    /// <summary>
    /// Base class which handles the title and changing properties
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public string TitleString = string.Empty;
        public string Title
        {
            get => TitleString;
            set => SetProperty(ref TitleString, value);
        }
        
        /// <summary>
        /// Sets a property and calls the PropertyChanged event
        /// </summary>
        /// <typeparam name="T">Type of both value and backing store</typeparam>
        /// <param name="backingStore">Reference to the variable whose data should be changing</param>
        /// <param name="value">New value</param>
        /// <param name="propertyName">Name of the changing property</param>
        /// <param name="onChanged">Action which is called when the backing store is not allready containing the new value</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// EventHandler for changed property
        /// </summary>
        /// <param name="propertyName">Name of the changed property. Is automaticaly set using the <see cref="CallerMemberNameAttribute"/>.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;

            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
