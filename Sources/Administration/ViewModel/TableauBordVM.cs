using Administration.Data;
using Administration.Data.Context;
using Administration.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Administration.ViewModel
{
    /// <summary>
    /// ViewModel pour le tableau de bord, gérant les données affichées dans l'interface utilisateur.
    /// </summary>
    public partial class TableauBordVM : ObservableObject
    {
        private readonly AdministrationContext _dbContext;

        [ObservableProperty]
        private string dateDuJour;

        [ObservableProperty]
        private string heureActuelle;

        [ObservableProperty]
        private int placesOccupees;

        [ObservableProperty]
        private int placesDisponibles;

        [ObservableProperty]
        private SeriesCollection etatStationnementSeries;

        [ObservableProperty]
        private SeriesCollection revenusSeries = new SeriesCollection();

        [ObservableProperty]
        private List<string> joursLabels = new List<string>();

        public Func<double, string> YFormatter { get; private set; } = value => value.ToString("C");

        [ObservableProperty]
        private FiltreType _filtreActif = FiltreType.Tous; // par défaut

        /// <summary>
        /// Indique si le filtre "Tous" est actif.
        /// </summary>
        public bool FiltreTous
        {
            get => FiltreActif == FiltreType.Tous;
            set
            {
                if (value) FiltreActif = FiltreType.Tous;
                ChargerGraphiqueRevenus();
            }
        }


        /// <summary>
        /// Indique si le filtre "Tickets" est actif.
        /// </summary>
        public bool FiltreTickets
        {
            get => FiltreActif == FiltreType.Tickets;
            set
            {
                if (value) FiltreActif = FiltreType.Tickets;
                ChargerGraphiqueRevenus();
            }
        }

        /// <summary>
        /// Indique si le filtre "Abonnements" est actif.
        /// </summary>
        public bool FiltreAbonnements
        {
            get => FiltreActif == FiltreType.Abonnements;
            set
            {
                if (value) FiltreActif = FiltreType.Abonnements;
                ChargerGraphiqueRevenus();
            }
        }

        [ObservableProperty]
        private bool afficherDiagramme = true;  // Par défaut on montre le graphique

        [ObservableProperty]
        private string messageAlerte;  // Pour le message d'alerte

        public IRelayCommand RefreshCommand { get; }

        /// <summary>
        /// Constructeur du ViewModel.
        /// Initialise le contexte de base de données et charge les données initiales.
        /// </summary>
        public TableauBordVM()
        {
            AdministrationContextFactory factory = new AdministrationContextFactory();
            _dbContext = factory.CreateDbContext(new string[0]);

            DateDuJour = DateTime.Now.ToString("yyyy/MM/dd");
            HeureActuelle = DateTime.Now.ToString("HH:mm");

            RefreshCommand = new RelayCommand(ChargerDonnees);

            ChargerDonnees();
        }


        /// <summary>
        /// Charge les données du tableau de bord (date, heure, état du stationnement, graphique des revenus).
        /// </summary>
        private void ChargerDonnees()
        {
            DateDuJour = DateTime.Now.ToString("yyyy/MM/dd");
            HeureActuelle = DateTime.Now.ToString("HH:mm");

            ChargerEtatStationnement();
            ChargerGraphiqueRevenus();
        }

        /// <summary>
        /// Charge l'état actuel du stationnement (places occupées et disponibles).
        /// </summary>
        private void ChargerEtatStationnement()
        {
            try
            {
                //la capacité max de la configuration la plus récente
                var derniereConfig = _dbContext.Configurations
                    .OrderByDescending(c => c.DateModification)
                    .FirstOrDefault();

                if (derniereConfig == null || derniereConfig.CapaciteMax <= 0)
                {
                    // Aucun paramètre de capacité trouvé, on informe l'utilisateur
                    AfficherDiagramme = false;  // Cache le diagramme
                    MessageAlerte = "⚠️ Aucune capacité maximale définie dans la configuration.\nVeuillez configurer la capacité dans la console de gestion.";

                    return;
                }

                int totalPlaces = derniereConfig.CapaciteMax;

                // Filtre les tickets non payés pour aujourd'hui seulement
                DateTime dateDuJour = DateTime.Today;

                var ticketsNonPayesAujourdHui = _dbContext.Tickets
                    .Where(t => !t.EstPaye && !t.EstConverti && t.TempsArrive.Date == dateDuJour)
                    .Count();

                PlacesOccupees = ticketsNonPayesAujourdHui;
                PlacesDisponibles = totalPlaces - ticketsNonPayesAujourdHui;

                EtatStationnementSeries = new SeriesCollection
            {
                new PieSeries { Title = "Occupées", Values = new ChartValues<double> { PlacesOccupees }, Fill = Brushes.Orange },
                new PieSeries { Title = "Disponibles", Values = new ChartValues<double> { PlacesDisponibles }, Fill = Brushes.LightGreen }
            };
            }
            catch (Exception ex)
            {
                // En cas d'erreur, affiche un message d'erreur et masque le diagramme
                AfficherDiagramme = false;
                MessageAlerte = Resource.ErrorUnexpected + $" : {ex.Message}";
            }
        }

        /// <summary>
        /// Charge le graphique des revenus en fonction du filtre actif (Tous, Tickets, Abonnements). (elle n'est pas de moi à 100% mais de ChatGPT
        /// </summary>
        private void ChargerGraphiqueRevenus()
        {
            try
            {
                DateTime aujourdHui = DateTime.Now.Date;
                DateTime ilYA7Jours = aujourdHui.AddDays(-7);

                var paiements = _dbContext.Paiements
                    .Where(p => p.DatePaiement.Date >= ilYA7Jours && p.DatePaiement.Date <= aujourdHui)
                    .ToList();

                switch (FiltreActif)
                {
                    case FiltreType.Tickets:
                        paiements = paiements.Where(p => !string.IsNullOrEmpty(p.TicketId)).ToList();
                        break;
                    case FiltreType.Abonnements:
                        paiements = paiements.Where(p => !string.IsNullOrEmpty(p.AbonnementId)).ToList();
                        break;
                    case FiltreType.Tous:
                    default:
                        break; // Ne filtre rien
                }

                // Initialisation et agrégation par jour
                var revenusParJour = Enumerable.Range(0, 7)
                    .Select(i => new
                    {
                        Date = ilYA7Jours.AddDays(i).ToString("dd/MM"),
                        Montant = paiements
                                    .Where(p => p.DatePaiement.Date == ilYA7Jours.AddDays(i))
                                    .Sum(p => p.Montant)
                    })
                    .ToDictionary(x => x.Date, x => x.Montant);

                // Mise à jour de la VM
                RevenusSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Revenus",
                    Values = new ChartValues<decimal>(revenusParJour.Values),
                    Fill = new SolidColorBrush(Color.FromRgb(63, 81, 181)) // Couleur bleue Material Design
                }
            };

                JoursLabels = revenusParJour.Keys.ToList();

                // Notifier
                OnPropertyChanged(nameof(RevenusSeries));
                OnPropertyChanged(nameof(JoursLabels));
            }
            catch (Exception ex)
            {
                // En cas d'erreur, affiche un message d'erreur et masque le diagramme
                AfficherDiagramme = false;
                MessageAlerte = Resource.ErrorUnexpected + $" : {ex.Message}";
            }
        }
    }

    public enum FiltreType
    {
        Tous,
        Tickets,
        Abonnements
    }
}
