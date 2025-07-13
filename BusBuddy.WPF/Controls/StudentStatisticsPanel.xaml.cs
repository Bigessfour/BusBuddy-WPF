using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BusBuddy.WPF.Controls
{
    public partial class StudentStatisticsPanel : UserControl
    {
        public StudentStatisticsPanel()
        {
            InitializeComponent();
            DataContext = this;
        }

        // Statistics properties with dependency property backing
        public static readonly DependencyProperty TotalStudentsProperty =
            DependencyProperty.Register("TotalStudents", typeof(int), typeof(StudentStatisticsPanel), new PropertyMetadata(0));

        public int TotalStudents
        {
            get { return (int)GetValue(TotalStudentsProperty); }
            set { SetValue(TotalStudentsProperty, value); }
        }

        public static readonly DependencyProperty ActiveStudentsProperty =
            DependencyProperty.Register("ActiveStudents", typeof(int), typeof(StudentStatisticsPanel), new PropertyMetadata(0));

        public int ActiveStudents
        {
            get { return (int)GetValue(ActiveStudentsProperty); }
            set { SetValue(ActiveStudentsProperty, value); }
        }

        public static readonly DependencyProperty InactiveStudentsProperty =
            DependencyProperty.Register("InactiveStudents", typeof(int), typeof(StudentStatisticsPanel), new PropertyMetadata(0));

        public int InactiveStudents
        {
            get { return (int)GetValue(InactiveStudentsProperty); }
            set { SetValue(InactiveStudentsProperty, value); }
        }

        public static readonly DependencyProperty StudentsWithRoutesProperty =
            DependencyProperty.Register("StudentsWithRoutes", typeof(int), typeof(StudentStatisticsPanel), new PropertyMetadata(0));

        public int StudentsWithRoutes
        {
            get { return (int)GetValue(StudentsWithRoutesProperty); }
            set { SetValue(StudentsWithRoutesProperty, value); }
        }

        public static readonly DependencyProperty StudentsWithoutRoutesProperty =
            DependencyProperty.Register("StudentsWithoutRoutes", typeof(int), typeof(StudentStatisticsPanel), new PropertyMetadata(0));

        public int StudentsWithoutRoutes
        {
            get { return (int)GetValue(StudentsWithoutRoutesProperty); }
            set { SetValue(StudentsWithoutRoutesProperty, value); }
        }

        // Chart data properties
        public static readonly DependencyProperty GradeDistributionProperty =
            DependencyProperty.Register("GradeDistribution", typeof(ObservableCollection<GradeData>),
                typeof(StudentStatisticsPanel), new PropertyMetadata(new ObservableCollection<GradeData>()));

        public ObservableCollection<GradeData> GradeDistribution
        {
            get { return (ObservableCollection<GradeData>)GetValue(GradeDistributionProperty); }
            set { SetValue(GradeDistributionProperty, value); }
        }

        public static readonly DependencyProperty StatusDistributionProperty =
            DependencyProperty.Register("StatusDistribution", typeof(ObservableCollection<StatusData>),
                typeof(StudentStatisticsPanel), new PropertyMetadata(new ObservableCollection<StatusData>()));

        public ObservableCollection<StatusData> StatusDistribution
        {
            get { return (ObservableCollection<StatusData>)GetValue(StatusDistributionProperty); }
            set { SetValue(StatusDistributionProperty, value); }
        }
    }

    // Data classes for the charts
    public class GradeData
    {
        public string Grade { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class StatusData
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
