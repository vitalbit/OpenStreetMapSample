﻿using MapSampleCommon;
using System.Linq;

namespace OpenStreetMapSample
{
    public partial class MainPage : ContentPage
    {
        readonly IEnumerable<ISample> allSamples;
        Func<object?, EventArgs, bool>? clicker;

        public MainPage()
        {
            InitializeComponent();

            // nullable warning workaround"
            var test = this.listView ?? throw new InvalidOperationException();

            allSamples = AllSamples.GetSamples() ?? new List<ISample>();

            var categories = allSamples.Select(s => s.Category).Distinct().OrderBy(c => c);
            picker!.ItemsSource = categories.ToList<string>();
            picker.SelectedIndexChanged += PickerSelectedIndexChanged;
            picker.SelectedItem = "Forms";
        }

        private void FillListWithSamples()
        {
            var selectedCategory = picker.SelectedItem?.ToString() ?? "";
            listView.ItemsSource = allSamples.Where(s => s.Category == selectedCategory).Select(x => x.Name);
        }

        private void PickerSelectedIndexChanged(object? sender, EventArgs e)
        {
            FillListWithSamples();
        }

        private void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }

            var sampleName = e.SelectedItem.ToString();
            var sample = allSamples.FirstOrDefault(x => x.Name == sampleName);

            clicker = null;
            if (sample is IFormsSample formsSample)
                clicker = formsSample.OnClick;

            if (sample != null)
                (Application.Current?.MainPage as NavigationPage)?.PushAsync(new MapPage(sample.Setup, clicker));

            listView.SelectedItem = null;
        }
    }

}
