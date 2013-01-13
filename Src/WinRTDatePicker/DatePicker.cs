using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace WinRTDatePicker
{
    public sealed class DatePicker : Control
    {
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(DatePicker), new PropertyMetadata(default(DateTime)));
        public static readonly DependencyProperty DayOptionFormatProperty = DependencyProperty.Register("DayOptionFormat", typeof(string), typeof(DatePicker), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty MonthOptionFormatProperty = DependencyProperty.Register("MonthOptionFormat", typeof(string), typeof(DatePicker), new PropertyMetadata(default(string)));

        private readonly ObservableCollection<string> daysInMonth = new ObservableCollection<string>();
        private readonly ObservableCollection<int> yearsInRange = new ObservableCollection<int>();

        public DatePicker()
        {
            DefaultStyleKey = typeof(DatePicker);

            SelectedDate = DateTime.Today;
            DayOptionFormat = "dd dddd";
            MonthOptionFormat = "MMMM";

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            for (int i = 1; i <= DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month); i++)
            {
                DateTime date = new DateTime(SelectedDate.Year, SelectedDate.Month, i);
                daysInMonth.Add(date.ToString(DayOptionFormat));
            }

            for (int i = 1; i <= 12; i++)
            {
                DateTime monthStart = new DateTime(SelectedDate.Year, i, 1);
                MonthOptions.Items.Add(monthStart.ToString(MonthOptionFormat));
            }

            int minYear = SelectedDate.Year - 10;
            int maxYear = SelectedDate.Year + 10;
            for (int i = minYear; i <= maxYear; i++)
            {
                yearsInRange.Add(i);
            }

            CreateBindings();

            DayOptions.SelectionChanged += DayOptionsOnSelectionChanged;
            MonthOptions.SelectionChanged += MonthOptionsOnSelectionChanged;
            YearOptions.SelectionChanged += YearOptionsOnSelectionChanged;
        }

        private void CreateBindings()
        {
            Binding dayOptionsBinding = new Binding { Source = daysInMonth, Mode = BindingMode.OneWay };
            DayOptions.SetBinding(ItemsControl.ItemsSourceProperty, dayOptionsBinding);

            Binding yearOptionsBinding = new Binding { Source = yearsInRange, Mode = BindingMode.OneWay };
            YearOptions.SetBinding(ItemsControl.ItemsSourceProperty, yearOptionsBinding);

            DayOptions.SelectedIndex = SelectedDate.Day - 1;
            MonthOptions.SelectedIndex = SelectedDate.Month - 1;
            YearOptions.SelectedItem = SelectedDate.Year;
        }

        private void UpdateSelectedDate()
        {
            int year = (int)YearOptions.SelectedValue;
            int month = MonthOptions.SelectedIndex + 1;
            int day = DayOptions.SelectedIndex + 1;

            int maxDaysInMonth = DateTime.DaysInMonth(year, month);
            if (day > maxDaysInMonth)
            {
                day = maxDaysInMonth;
                DayOptions.SelectedIndex = maxDaysInMonth - 1;
            }

            if (month == 0)
                month = 1;

            if (day == 0)
                day = 1;

            SelectedDate = new DateTime(year, month, day);
        }

        private void UpdateDayOptions()
        {
            int selectedDayIndex = DayOptions.SelectedIndex;
            int month = MonthOptions.SelectedIndex + 1;

            daysInMonth.Clear();

            for (int i = 1; i <= DateTime.DaysInMonth(SelectedDate.Year, month); i++)
            {
                DateTime date = new DateTime(SelectedDate.Year, month, i);
                daysInMonth.Add(date.ToString(DayOptionFormat));
            }

            DayOptions.SelectedIndex = selectedDayIndex;
        }

        private void DayOptionsOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            UpdateSelectedDate();
        }

        private void MonthOptionsOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            UpdateSelectedDate();
            UpdateDayOptions();
        }

        private void YearOptionsOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            UpdateSelectedDate();
            UpdateDayOptions();
        }

        public DateTime SelectedDate
        {
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        public string DayOptionFormat
        {
            get { return (string)GetValue(DayOptionFormatProperty); }
            set { SetValue(DayOptionFormatProperty, value); }
        }

        public string MonthOptionFormat
        {
            get { return (string)GetValue(MonthOptionFormatProperty); }
            set { SetValue(MonthOptionFormatProperty, value); }
        }

        private ComboBox DayOptions
        {
            get { return (ComboBox)GetTemplateChild("DayOptions"); }
        }

        private ComboBox MonthOptions
        {
            get { return (ComboBox)GetTemplateChild("MonthOptions"); }
        }

        private ComboBox YearOptions
        {
            get { return (ComboBox)GetTemplateChild("YearOptions"); }
        }
    }
}
