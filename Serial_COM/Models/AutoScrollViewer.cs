using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace AutoScrollViewer
{
    public static class AutoScrollViewer
    {
        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached(
                "AutoScroll",
                typeof(bool),
                typeof(AutoScrollViewer),
                new PropertyMetadata(false, OnAutoScrollChanged));

        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }

        /// <summary>
        /// // [OnAutoScrollChanged]
        /// // [자동 스크롤 내리기!]
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnAutoScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox listBox)
            {
                if ((bool)e.NewValue)
                {
                    if (listBox.Items.SourceCollection is INotifyCollectionChanged collection)
                    {
                        collection.CollectionChanged += (sender, args) =>
                        {
                            if (args.Action == NotifyCollectionChangedAction.Add && listBox.Items.Count > 0)
                            {
                                listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
                            }

                        };

                    }

                }

            }

        }

    }

}
