using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace WinRTDatePicker
{
    [TemplatePart(Name = "_DayOptions", Type = typeof(ComboBox))]
    [TemplatePart(Name = "_MonthOptions", Type = typeof(ComboBox))]
    [TemplatePart(Name = "_YearOptions", Type = typeof(ComboBox))]
    public sealed class DatePicker : Control
    {
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(DatePicker), new PropertyMetadata(default(DateTime), SelectedDateChangedCallback));
        public static readonly DependencyProperty DayOptionFormatProperty = DependencyProperty.Register("DayOptionFormat", typeof(string), typeof(DatePicker), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty MonthOptionFormatProperty = DependencyProperty.Register("MonthOptionFormat", typeof(string), typeof(DatePicker), new PropertyMetadata(default(string)));

        private readonly ObservableCollection<string> daysInMonth = new ObservableCollection<string>();
        private readonly ObservableCollection<string> monthsInRange = new ObservableCollection<string>(); 
        private readonly ObservableCollection<int> yearsInRange = new ObservableCollection<int>();

        public DatePicker()
        {
            DefaultStyleKey = typeof(DatePicker);

            SelectedDate = DateTime.Today;
            DayOptionFormat = "dd dddd";
            MonthOptionFormat = "MMMM";
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            monthsInRange.Clear();
            for (int i = 1; i <= 12; i++)
            {
                DateTime monthStart = new DateTime(DateTime.Now.Year, i, 1);
                monthsInRange.Add(monthStart.ToString(MonthOptionFormat));
            }

            CreateBindings();
            SetSelectedDate(SelectedDate);

            DayOptions.SelectionChanged += DayOptionsOnSelectionChanged;
            MonthOptions.SelectionChanged += MonthOptionsOnSelectionChanged;
            YearOptions.SelectionChanged += YearOptionsOnSelectionChanged;
        }

        private void SetSelectedDate(DateTime newSelectedDate)
        {
            if (DayOptions != null && MonthOptions != null && YearOptions != null)
            {
                daysInMonth.Clear();
                yearsInRange.Clear();

                for (int i = 1; i <= DateTime.DaysInMonth(newSelectedDate.Year, newSelectedDate.Month); i++)
                {
                    DateTime date = new DateTime(newSelectedDate.Year, newSelectedDate.Month, i);
                    daysInMonth.Add(date.ToString(DayOptionFormat));
                }

                int minYear = newSelectedDate.Year - 10;
                int maxYear = newSelectedDate.Year + 10;
                for (int i = minYear; i <= maxYear; i++)
                {
                    yearsInRange.Add(i);
                }

                DayOptions.SelectedIndex = newSelectedDate.Day - 1;
                MonthOptions.SelectedIndex = newSelectedDate.Month - 1;
                YearOptions.SelectedItem = newSelectedDate.Year;
            }
        }

        private void CreateBindings()
        {
            Binding dayOptionsBinding = new Binding { Source = daysInMonth, Mode = BindingMode.OneWay };
            DayOptions.SetBinding(ItemsControl.ItemsSourceProperty, dayOptionsBinding);

            Binding monthOptionsBinding = new Binding { Source = monthsInRange, Mode = BindingMode.OneWay };
            MonthOptions.SetBinding(ItemsControl.ItemsSourceProperty, monthOptionsBinding);
            
            Binding yearOptionsBinding = new Binding { Source = yearsInRange, Mode = BindingMode.OneWay };
            YearOptions.SetBinding(ItemsControl.ItemsSourceProperty, yearOptionsBinding);
        }

        private void UpdateSelectedDateFromInputs()
        {
            if (YearOptions.SelectedIndex >= 0 && MonthOptions.SelectedIndex >= 0 && DayOptions.SelectedIndex >= 0)
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
            UpdateSelectedDateFromInputs();
        }

        private void MonthOptionsOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            UpdateSelectedDateFromInputs();
            UpdateDayOptions();
        }

        private void YearOptionsOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            UpdateSelectedDateFromInputs();
            UpdateDayOptions();
        }

        private static void SelectedDateChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DateTime oldValue = (DateTime)args.OldValue;
            DateTime newValue = (DateTime)args.NewValue;

            if (newValue != oldValue)
            {
                DatePicker datePicker = (DatePicker)obj;
                datePicker.SetSelectedDate(newValue);
            }
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
            get { return (ComboBox)GetTemplateChild("_DayOptions"); }
        }

        private ComboBox MonthOptions
        {
            get { return (ComboBox)GetTemplateChild("_MonthOptions"); }
        }

        private ComboBox YearOptions
        {
            get { return (ComboBox)GetTemplateChild("_YearOptions"); }
        }
    }
}
