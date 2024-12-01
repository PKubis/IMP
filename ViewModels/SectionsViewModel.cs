using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Linq;
using IMP.Models;
using IMP.Services;
using System.Collections.Generic;

namespace IMP.ViewModels
{
    public class SectionsViewModel : BaseViewModel
    {
        public SectionsViewModel() { } // Domyślny konstruktor dla XAML

        public SectionsViewModel(string userId)
        {
            _userId = userId;
            _firebaseService = new RealtimeDatabaseService();

            ToggleDayCommand = new Command<string>(ToggleDay);
            AddSectionCommand = new Command(async () => await AddSection());

            LoadSections();
        }

        private readonly string _userId;
        private readonly RealtimeDatabaseService _firebaseService;

        public ObservableCollection<Section> Sections { get; set; } = new ObservableCollection<Section>();
        public Dictionary<string, string> DayColors { get; set; } = new Dictionary<string, string>
        {
            { "pn", "LightGray" },
            { "wt", "LightGray" },
            { "śr", "LightGray" },
            { "cz", "LightGray" },
            { "pt", "LightGray" },
            { "sb", "LightGray" },
            { "nd", "LightGray" }
        };

        public ICommand ToggleDayCommand { get; }
        public ICommand AddSectionCommand { get; }

        private string _sectionName;
        public string SectionName
        {
            get => _sectionName;
            set => SetProperty(ref _sectionName, value);
        }

        private string _startTime;
        public string StartTime
        {
            get => _startTime;
            set => SetProperty(ref _startTime, value);
        }

        private string _duration;
        public string Duration
        {
            get => _duration;
            set
            {
                if (int.TryParse(value, out int result))
                {
                    _duration = result.ToString();
                    SetProperty(ref _duration, _duration);
                }
            }
        }

        private List<string> _selectedDays = new List<string>();

        private void ToggleDay(string day)
        {
            if (_selectedDays.Contains(day))
            {
                _selectedDays.Remove(day);
                DayColors[day] = "LightGray"; // Dzień odznaczony
            }
            else
            {
                _selectedDays.Add(day);
                DayColors[day] = "Teal"; // Dzień zaznaczony
            }
            OnPropertyChanged(nameof(DayColors));
        }

        private async void LoadSections()
        {
            var sections = await _firebaseService.GetSectionsAsync(_userId);
            foreach (var section in sections)
            {
                Sections.Add(section);
            }
        }

        private async Task AddSection()
        {
            if (string.IsNullOrWhiteSpace(SectionName) || string.IsNullOrWhiteSpace(StartTime) || string.IsNullOrWhiteSpace(Duration))
                return;

            var newSection = new Section
            {
                Id = Guid.NewGuid().ToString(),
                Name = SectionName,
                StartTime = StartTime,
                Duration = int.Parse(Duration),
                SelectedDays = string.Join(", ", _selectedDays) // Zapisz wybrane dni
            };

            await _firebaseService.SaveSectionAsync(_userId, newSection);
            Sections.Add(newSection);

            // Resetuj pola po dodaniu sekcji
            SectionName = string.Empty;
            StartTime = string.Empty;
            Duration = string.Empty;
            _selectedDays.Clear();
            foreach (var day in DayColors.Keys.ToList())
                DayColors[day] = "LightGray";
            OnPropertyChanged(nameof(DayColors));
        }
    }
}
