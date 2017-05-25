using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Memory.Annotations;
using Memory.Converters;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace Memory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public int DefaultGameTime = 60;
        public int BoardSize = 4;
        public int DifferentCardsCount;
        public int CardFlipDelay = 500;
        public int EndGameFirstCardDuration = 1500;
        public double EndGameAnimationReductionRatio = 0.9;

        private ObservableCollection<MemoryCard> _memoryCards = new ObservableCollection<MemoryCard>();
        private MemoryCard _firstCard;
        private bool _gameStarted;
        private int _timeLeft;
        private readonly DispatcherTimer _gameTimer = new DispatcherTimer();
        private int _cardsGuessed;
        private ObservableCollection<CardImage> _cardImages = new ObservableCollection<CardImage>();
        private MemoryCard _secondCard;
        private bool _endGameAnimation;
        private bool _resetEnabled = true;

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

        public MemoryCard SecondCard
        {
            get { return _secondCard; }
            set
            {
                if (Equals(value, _secondCard)) return;
                _secondCard = value;
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

        public ObservableCollection<CardImage> CardImages
        {
            get { return _cardImages; }
            set
            {
                if (Equals(value, _cardImages)) return;
                _cardImages = value;
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

        public bool ResetEnabled
        {
            get { return _resetEnabled; }
            set
            {
                if (value == _resetEnabled) return;
                _resetEnabled = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            DifferentCardsCount = BoardSize * BoardSize / 2;
            TimeLeft = DefaultGameTime;
            InitializeComponent();
            PopulateCardImages();
            GenerateCards();

            _gameTimer.Interval = TimeSpan.FromSeconds(1);
            _gameTimer.Tick += (sender, args) =>
            {
                TimeLeft--;
                if (TimeLeft <= 0)
                    TimeUp();
            };
        }

        private void TimeUp()
        {
            _gameTimer.Stop();
            GameStarted = false;

            MessageBoxResult result = MessageBox.Show(this, "You lost! Would you like to start again?", "Lost", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                GenerateCards();
                TimeLeft = DefaultGameTime;
            }
            else if (result == MessageBoxResult.No)
                RunEndGameAnimation();
        }

        private void GenerateCards()
        {
            CardsGuessed = 0;
            MemoryCards.Clear();
            List<MemoryCard> memoryCards = new List<MemoryCard>(16);
            for (int i = 1; i <= 8; i++)
            {
                CardImage cardImage = CardImages[i - 1];
                memoryCards.Add(new MemoryCard(i, cardImage));
                memoryCards.Add(new MemoryCard(i, cardImage));
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

            if (FirstCard != null && SecondCard != null)
                return;

            Button button = sender as Button;
            if (button == null)
                return;

            MemoryCard card = button.DataContext as MemoryCard;
            if (card == null)
                return;

            card.Selected = true;

            Grid cardGrid = GetGridFromControlByItem(card);
            PrepareOpacityStoryboard(1, TimeOfFlipBackSlider.Value, cardGrid).Begin();

            if (FirstCard == null)
            {
                FirstCard = card;
                return;
            }

            SecondCard = card;
            OnBothCardsSelected();
        }

        private async void OnBothCardsSelected()
        {
            // Wait for second card to show up
            await Task.Delay(CardFlipDelay);
            // Wait for both cards to stay up (slider can be changed in the meantime, therefore I need to read it again)
            await Task.Delay((int)TimeOfFlipBackSlider.Value);

            Grid firstCardGrid = GetGridFromControlByItem(FirstCard);
            Grid secondCardGrid = GetGridFromControlByItem(SecondCard);

            PrepareOpacityStoryboard(0, TimeOfFlipBackSlider.Value, firstCardGrid).Begin();
            PrepareOpacityStoryboard(0, TimeOfFlipBackSlider.Value, secondCardGrid).Begin();

            await Task.Delay((int) TimeOfFlipBackSlider.Value);
            FirstCard.Selected = SecondCard.Selected = false;

            if (FirstCard.Content == SecondCard.Content)
            {
                FirstCard.Visible = SecondCard.Visible = false;
                FirstCard = SecondCard = null;

                CardsGuessed++;
                if (CardsGuessed == DifferentCardsCount)
                    Win();
            }
            else
            {
                FirstCard = SecondCard = null;
            }
        }

        private void Win()
        {
            _gameTimer.Stop();
            GameStarted = false;
            MessageBoxResult result = MessageBox.Show(this, "You won! Would you like to start again?", "Win", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                GenerateCards();
                TimeLeft = DefaultGameTime;
            }
            else if (result == MessageBoxResult.No)
                RunEndGameAnimation();
        }

        private async void RunEndGameAnimation()
        {
            EndGameAnimation = true;
            ResetEnabled = false;
            foreach (MemoryCard card in MemoryCards)
                card.Visible = false;
            double cardDuration = EndGameFirstCardDuration;

            for (int i = 0; i < DifferentCardsCount * 2; i++)
            {
                Grid innerGrid = GetGridFromControlByIndex(i);
                PrepareOpacityStoryboard(0.5, cardDuration, innerGrid).Begin();

                MemoryCards[i].Visible = true;
                MemoryCards[i].Selected = true;
                await Task.Delay((int)cardDuration);
                cardDuration *= EndGameAnimationReductionRatio;
            }

            ResetEnabled = true;
        }

        private Storyboard PrepareOpacityStoryboard(double to, double miliseconds, Grid grid)
        {
            Storyboard sb = new Storyboard();

            DoubleAnimation animation = new DoubleAnimation(to, new Duration(TimeSpan.FromMilliseconds(miliseconds)));
            Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.OpacityProperty));
            Storyboard.SetTarget(animation, grid);

            sb.Children.Add(animation);

            return sb;
        }

        private Grid GetGridFromControlByIndex(int index)
        {
            var border = VisualTreeHelper.GetChild(CardsItemControl.ItemContainerGenerator.ContainerFromIndex(index), 0);
            var outerGrid = VisualTreeHelper.GetChild(border, 0);
            Grid innerGrid = VisualTreeHelper.GetChild(outerGrid, 1) as Grid;

            return innerGrid;
        }

        private Grid GetGridFromControlByItem(MemoryCard card)
        {
            var border = VisualTreeHelper.GetChild(CardsItemControl.ItemContainerGenerator.ContainerFromItem(card), 0);
            var outerGrid = VisualTreeHelper.GetChild(border, 0);
            Grid innerGrid = VisualTreeHelper.GetChild(outerGrid, 1) as Grid;

            return innerGrid;
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            GameStarted = !GameStarted;

            if (GameStarted)
                _gameTimer.Start();
            else
                _gameTimer.Stop();
        }

        private void OnResetClick(object sender, RoutedEventArgs e)
        {
            GameStarted = false;
            GenerateCards();
            _gameTimer.Stop();
            TimeLeft = DefaultGameTime;
            FirstCard = null;
            EndGameAnimation = false;
        }

        private void OnCollapseChecked(object sender, RoutedEventArgs e)
        {
            foreach (CardImage cardImage in CardImages)
                cardImage.Expanded = true;
        }

        private void PopulateCardImages()
        {
            string imagesDirectory = "Images";
            CardImages.Clear();
            for (int i = 1; i <= DifferentCardsCount; i++)
            {
                string fileName = Path.GetFullPath(Path.Combine(imagesDirectory, +i + ".jpg"));
                if (!File.Exists(fileName))
                {
                    MessageBox.Show(this, "Cannot open file " + fileName, "Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Close();
                }


                CardImages.Add(new CardImage() { FilePath = fileName, Name = "name" + i, Date = File.GetCreationTime(fileName) });
            }
        }

        private void OnImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (image == null)
                return;

            CardImage cardImage = image.DataContext as CardImage;

            if (cardImage == null)
                return;

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            bool? result = dialog.ShowDialog(this);
            if (!result.Value)
                return;

            cardImage.FilePath = dialog.FileName;
        }

        private void OnCollapseUnchecked(object sender, RoutedEventArgs e)
        {
            foreach (CardImage cardImage in CardImages)
                cardImage.Expanded = false;
        }
    }
}
