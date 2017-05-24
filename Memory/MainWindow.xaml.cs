using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Memory.Annotations;

namespace Memory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<MemoryCard> _memoryCards = new ObservableCollection<MemoryCard>();

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

        public MainWindow()
        {
            InitializeComponent();
            generateCards();
        }

        private void generateCards()
        {
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

        private void CardOnMouseEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = new SolidColorBrush(Colors.DarkBlue);
        }

        private void CardOnMouseLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = new SolidColorBrush(Colors.AliceBlue);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CardOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            MemoryCard card = button.DataContext as MemoryCard;

            card.Selected = !card.Selected;
        }
    }
}
