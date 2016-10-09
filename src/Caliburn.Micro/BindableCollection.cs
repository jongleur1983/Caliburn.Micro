namespace Caliburn.Micro {
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// A base collection class that supports automatic UI thread marshalling.
    /// </summary>
    /// <typeparam name="T">The type of elements contained in the collection.</typeparam>
    public class BindableCollection<T> : ObservableCollection<T>, IObservableCollection<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref = "Caliburn.Micro.BindableCollection&lt;T&gt;" /> class.
        /// </summary>
        public BindableCollection()
        {
            IsNotifying = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref = "Caliburn.Micro.BindableCollection&lt;T&gt;" /> class.
        /// </summary>
        /// <param name = "collection">The collection from which the elements are copied.</param>
        public BindableCollection(IEnumerable<T> collection)
            : base(collection)
        {
            IsNotifying = true;
        }

        /// <summary>
        /// Enables/Disables property change notification.
        /// </summary>
        public bool IsNotifying { get; set; }

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        public virtual void NotifyOfPropertyChange(string propertyName) {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises a change notification indicating that all bindings should be refreshed.
        /// </summary>
        public void Refresh() {
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        
        /// <summary>
        /// Raises the <see cref = "E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged" /> event with the provided arguments.
        /// </summary>
        /// <param name = "e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
            if (IsNotifying) {
                Execute.OnUIThread(() => base.OnCollectionChanged(e));
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event with the provided arguments.
        /// </summary>
        /// <param name = "e">The event data to report in the event.</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
            if (IsNotifying) {
                Execute.OnUIThread(() => base.OnPropertyChanged(e));
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name = "items">The items.</param>
        public virtual void AddRange(IEnumerable<T> items) {
            bool anyItemAdded = false;
            var index = Count;
            foreach (var item in items)
            {
                this.InsertItem(index, item);
                index++;
                anyItemAdded = true;
            }

            if (anyItemAdded)
            {
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <param name = "items">The items.</param>
        public virtual void RemoveRange(IEnumerable<T> items) {
            var anyItemRemoved = false;
            foreach (var item in items) {
                var index = IndexOf(item);
                if (index >= 0) {
                    RemoveItem(index);
                    anyItemRemoved = true;
                }
            }

            if (anyItemRemoved) {
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
    }
}
