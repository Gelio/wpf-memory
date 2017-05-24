using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Memory.Annotations;

namespace Memory.Converters
{
    public class CardImage : INotifyPropertyChanged
    {
        private bool _expanded;
        private string _name;
        private string _file;
        private DateTime _date;

        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                if (value == _expanded) return;
                _expanded = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string File
        {
            get { return _file; }
            set
            {
                if (value == _file) return;
                _file = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (value.Equals(_date)) return;
                _date = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
