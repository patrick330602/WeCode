﻿using Developer_Hub_For_UWP.Presentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Developer_Hub_For_UWP.Pages
{
    public sealed partial class Page1 : Page
    {
        public Page1()
        {
            this.InitializeComponent();
            InitializeList();

            double L = gridView.ActualWidth;
            
        }

        private async void InitializeList()
        {
            Uri uri = new Uri("ms-appx:///Assets/Data/data.csv");
            var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            using (var storageStream = await storageFile.OpenReadAsync())
            {
                using (Stream stream = storageStream.AsStreamForRead())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        List<Item> Items = new List<Item>();
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(',');
                            string iconstring;
                            int p = int.Parse(values[0], System.Globalization.NumberStyles.HexNumber);
                            iconstring = ((char)p).ToString();
                            Items.Add(new Item { Name = values[1], Graph = iconstring });
                        }
                        gridView.ItemsSource = Items;
                    }
                }
            }
            
        }

        private async void gridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            Item output = e.ClickedItem as Item;
            AddToHistory(output);
            string selectInfo = output.Graph;
            var currentAV = ApplicationView.GetForCurrentView();
            var newAV = CoreApplication.CreateNewView();
            await newAV.Dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            async () =>
                            {
                                var newWindow = Window.Current;
                                var newAppView = ApplicationView.GetForCurrentView();
                                
                                newAppView.Title = selectInfo + loader.GetString("Detail");

                                var frame = new Frame();
                                frame.Navigate(typeof(Browser), selectInfo);
                                newWindow.Content = frame;
                                newWindow.Activate();

                                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(
                                    newAppView.Id,
                                    ViewSizePreference.UseMinimum,
                                    currentAV.Id,
                                    ViewSizePreference.UseMinimum);
                            });
           
        }
        private async void AddToHistory(Item data)
        {
            String ori = "";

            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await storageFolder.GetFileAsync("history_icon.log");
            StringBuilder result = new StringBuilder();
            using (var storageStream = await sampleFile.OpenReadAsync())
            {
                using (Stream Inputstream = storageStream.AsStreamForRead())
                {
                    using (StreamReader reader = new StreamReader(Inputstream))
                    {
                        ori = reader.ReadToEnd();
                        ori = ori +data.Name+"," +data.Graph+ Environment.NewLine;
                        reader.Dispose();
                    }
                    Inputstream.Dispose();
                }
                storageStream.Dispose();
            }
            var streamout = await sampleFile.OpenAsync(FileAccessMode.ReadWrite);
            using (var outputStream = streamout.GetOutputStreamAt(0))
            {
                using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                {
                    dataWriter.WriteString(ori);
                    await dataWriter.StoreAsync();
                    await outputStream.FlushAsync();
                }
            }
            streamout.Dispose();
        }

        private async void Inp_TextChanged(object sender, TextChangedEventArgs e)
        {
            string c = Inp.Text.ToLower();
            Uri uri = new Uri("ms-appx:///Assets/Data/data.csv");
            var storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
            using (var storageStream = await storageFile.OpenReadAsync())
            {
                using (Stream stream = storageStream.AsStreamForRead())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        List<Item> Items = new List<Item>();
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(',');
                            string iconstring;
                            int p = int.Parse(values[0], System.Globalization.NumberStyles.HexNumber);
                            iconstring = ((char)p).ToString();
                            if (values[1].ToLower().Contains(c))
                            {
                                Items.Add(new Item { Name = values[1], Graph = iconstring });
                            }
                        }
                        gridView.ItemsSource = Items;
                    }
                }
            }
        }

        private void StackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ItemsWrapGrid MyItemsPanel = (ItemsWrapGrid)gridView.ItemsPanelRoot;
            MyItemsPanel.ItemWidth = (e.NewSize.Width -10)/ 8;

        }
    }
   
}
