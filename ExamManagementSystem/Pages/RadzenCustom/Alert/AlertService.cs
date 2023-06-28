using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace Radzen
{
    public class AlertService
    {
        public ObservableCollection<RadzenAlert> Alerts { get; private set; } = new ObservableCollection<RadzenAlert>();

        public void Alert(RadzenAlert message)
        {
            if (!Alerts.Contains(message))
            {
                Alerts.Add(message);
            }
        }
    }

    public class DataStore
    {
        public event DataChangedHandler DataChanged;

        public delegate Task DataChangedHandler();

        public void OnDataChanged()
        {
            DataChanged?.Invoke();
        }
    }
}