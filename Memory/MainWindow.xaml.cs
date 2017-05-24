using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Memory.Annotations;

namespace Memory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public int DefaultGameTime = 20;
        public int BoardSize = 4;
        public int DifferentCardsCount;

        private ObservableCollection<MemoryCard> _memoryCards = new ObservableCollection<MemoryCard>();
        private MemoryCard _firstCard;
        private bool _gameStarted;
        private int _timeLeft = 20;

        public ObservableCollection<MemoryCard> MemoryCards
        {
            get { return _memoryCards; }
            set
            {
                if (Equals(value, _memoryCards)) return;
                _memoryCards = value;
                OnPropertyChanged();
            }
        }

        public MemoryCard FirstCard
        {
            get { return _firstCard; }
            set
            {
                if (Equals(value, _firstCard)) return;
                _firstCard = value;
                OnPropertyChanged();
            }
        }

        public bool GameStarted
        {
            get { return _gameStarted; }
            set
            {
                if (value == _gameStarted) return;
                _gameStarted = value;
                OnPropertyChanged();
            }
        }

        public int TimeLeft
        {
            get { return _timeLeft; }
            set
            {
                if (value == _timeLeft) return;
                _timeLeft = value;
                OnPropertyChanged();
            }
        }

        private DispatcherTimer gameTimer = new DispatcherTimer();
        private int _cardsGuessed;

        public int CardsGuessed
        {
            get { return _cardsGuessed; }
            set
            {
                if (value == _cardsGuessed) return;
                _cardsGuessed = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            DifferentCardsCount = BoardSize * BoardSize / 2;
            InitializeComponent();
            GenerateCards();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += (sender, args) =>
            {
                TimeLeft--;
                if (TimeLeft <= 0)
                    TimeUp();
            };
        }

        private void TimeUp()
        {
            MessageBoxResult result = MessageBox.Show(this, "You lost! Would you like to start again?", "Lost", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                GameStarted = false;
                GenerateCards();
                TimeLeft = DefaultGameTime;
                gameTimer.Stop();
            }
            else if (result == MessageBoxResult.No)
            {
                Close();
            }
        }

        private void GenerateCards()
        {
            CardsGuessed = 0;
            MemoryCards.Clear();
            List<MemoryCard> memoryCards = new List<MemoryCard>(16);
            for (int i = 1; i <= 8; i++)
            {
                memoryCards.Add(new MemoryCard(i));
                memoryCards.Add(new MemoryCard(i));
            }

            memoryCards.Shuffle();
            foreach (MemoryCard card in memoryCards)
                MemoryCards.Add(card);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CardOnClick(object sender, RoutedEventArgs e)
        {
            if (!GameStarted)
                return;

            Button button = sender as Button;
            MemoryCard card = button.DataContext as MemoryCard;

            card.Selected = !card.Selected;

            if (card.Selected)
            {
                if (FirstCard == null)
                {
                    FirstCard = card;
                    return;
                }

                if (FirstCard.Content == card.Content)
                {
                    FirstCard.Visible = false;
                    card.Visible = false;
                    CardsGuessed++;

                    if (CardsGuessed == DifferentCardsCount)
                        Win();
                }
                else
                {
                    FirstCard.Selected = false;
                    card.Selected = false;
                }
                FirstCard = null;
            }
            else
                FirstCard = null;
        }

        private void Win()
        {
            gameTimer.Stop();
            GameStarted = false;
            MessageBoxResult result = MessageBox.Show(this, "You won! Would you like to start again?", "Win", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                GenerateCards();
                TimeLeft = DefaultGameTime;
            }
            else if (result == MessageBoxResult.No)
            {
                Close();
            }
        }

        private void StartOnClick(object sender, RoutedEventArgs e)
        {
            GameStarted = !GameStarted;

            if (GameStarted)
                gameTimer.Start();
            else
                gameTimer.Stop();
        }

        private void ResetOnClick(object sender, RoutedEventArgs e)
        {
            GameStarted = false;
            GenerateCards();
            gameTimer.Stop();
            TimeLeft = DefaultGameTime;
            FirstCard = null;
        }
    }
}
