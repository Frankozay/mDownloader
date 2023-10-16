using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace mDownloader.Views
{
    public static class DataGridExtensions
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItems",
                typeof(IList),
                typeof(DataGridExtensions),
                new PropertyMetadata(null, OnSelectedItemsChanged));

        public static void SetSelectedItems(DataGrid element, IList value)
        {
            element.SetValue(SelectedItemsProperty, value);
        }

        public static IList GetSelectedItems(DataGrid element)
        {
            return (IList)element.GetValue(SelectedItemsProperty);
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                dataGrid.SelectionChanged -= DataGrid_SelectionChanged;

                if (e.NewValue != null)
                {
                    dataGrid.SelectionChanged += DataGrid_SelectionChanged;
                }
            }
        }

        private static void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                var selectedItems = GetSelectedItems(dataGrid);
                selectedItems.Clear();

                if (dataGrid.SelectedItems != null)
                {
                    foreach (var item in dataGrid.SelectedItems)
                        selectedItems.Add(item);
                }
            }
        }
    }
}
