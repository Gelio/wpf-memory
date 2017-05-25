using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using Memory.Annotations;
using Memory.Converters;

namespace Memory
{
    public class MemoryCard : INotifyPropertyChanged
    {
        private int _content;
        private bool _selected;
        private bool _visible = true;
        private CardImage _cardImage;
        private bool _animationVisible = false;
        private Duration _animationDuration = new Duration(TimeSpan.FromMilliseconds(1000));
        private bool _endGameAnimation;

        public int Content
        {
            get { return _content; }
            set
            {
                if (value == _content) return;
                _content = value;
                OnPropertyChanged();
            }
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (value == _selected) return;
                _selected = value;
                OnPropertyChanged();
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (value == _visible) return;
                _visible = value;
                OnPropertyChanged();
            }
        }

        public CardImage CardImage
        {
            get { return _cardImage; }
            set
            {
                if (Equals(value, _cardImage)) return;
                _cardImage = value;
                OnPropertyChanged();
            }
        }

        public bool AnimationVisible
        {
            get { return _animationVisible; }
            set
            {
                if (value == _animationVisible) return;
                _animationVisible = value;
                OnPropertyChanged();
            }
        }

        public bool EndGameAnimation
        {
            get { return _endGameAnimation; }
            set
            {
                if (value == _endGameAnimation) return;
                _endGameAnimation = value;
                OnPropertyChanged();
            }
        }

        public Duration AnimationDuration
        {
            get { return _animationDuration; }
            set
            {
                if (value.Equals(_animationDuration)) return;
                _animationDuration = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MemoryCard(int content, CardImage cardImage)
        {
            Content = content;
            CardImage = cardImage;
        }
    }
}
