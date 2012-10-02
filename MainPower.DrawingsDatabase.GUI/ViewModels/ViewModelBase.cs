using System;
using System.ComponentModel;
using System.Diagnostics;

namespace MainPower.DrawingsDatabase.Gui.ViewModels
{
    public class ViewModelBase :INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool ThrowOnInvalidPropertyName { get; set; }
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        public void VerifyPropertyName(string propertyName)
        {
            //Verify that the property name matches a real,
            //public instance property on this object
            //an empty property name is ok, used to refresh all properties
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
