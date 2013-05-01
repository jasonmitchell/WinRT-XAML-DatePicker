using System;

namespace WinRTDatePicker
{
    public class SelectedDateChangedEventArgs : EventArgs
    {
        private readonly DateTime newDate;

        public SelectedDateChangedEventArgs(DateTime newDate)
        {
            this.newDate = newDate;
        }

        public DateTime NewDate
        {
            get { return newDate; }
        }
    }
}
