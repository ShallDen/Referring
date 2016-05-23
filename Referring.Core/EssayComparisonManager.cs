using System.ComponentModel;

namespace Referring.Core
{
    public enum ComparisonType
    {
        FullSentences,
        MainWordFulness,
        MainWordAccuracyWithError,
        MainWordAccuracyWithSignificanceKoefficient
    }

    public class EssayComparisonManager : INotifyPropertyChanged
    {
        private double mEssayComparisonPercentage;
        private bool mIsUseCurrentEssayAsFirstFile;

        private static EssayComparisonManager instance;

        public event PropertyChangedEventHandler PropertyChanged;

        public static EssayComparisonManager Instance
        {
            get { return instance ?? (instance = new EssayComparisonManager()); }
        }

        public ComparisonType ComparisonType { get; set; }
        public bool IsComparisonCompete { get; set; }
        public string FisrtEssay { get; set; }
        public string FisrtEssayPath { get; set; }
        public string SecondEssay { get; set; }
        public string SecondEssayPath { get; set; }
        public double ProgressPercentageCurrent { get; set; }

        public bool IsUseCurrentEssayAsFirstFile
        {
            get { return mIsUseCurrentEssayAsFirstFile; }
            set
            {
                if (mIsUseCurrentEssayAsFirstFile != value)
                {
                    mIsUseCurrentEssayAsFirstFile = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsUseCurrentEssayAsFirstFile"));
                }
            }
        }

        public double EssayComparisonPercentage
        {
            get { return mEssayComparisonPercentage; }
            set
            {
                if (mEssayComparisonPercentage != value)
                {
                    mEssayComparisonPercentage = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("EssayComparisonPercentage"));
                }
            }
        }        
    }
}
