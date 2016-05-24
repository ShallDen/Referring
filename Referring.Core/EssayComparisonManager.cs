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
        private string mFisrtEssayToolTipText;
        private string mSecondEssayToolTipText;
        private bool mIsUseCurrentEssayAsFirstFile;

        private static EssayComparisonManager instance;

        public event PropertyChangedEventHandler PropertyChanged;

        public static EssayComparisonManager Instance
        {
            get { return instance ?? (instance = new EssayComparisonManager()); }
        }

        public double AccuracyError { get; set; }
        public ComparisonType ComparisonType { get; set; }
        public string FisrtEssay { get; set; }
        public string FisrtEssayPath { get; set; }
        public bool IsComparisonCompete { get; set; }
        public double ProgressPercentageCurrent { get; set; }
        public string SecondEssay { get; set; }
        public string SecondEssayPath { get; set; }


        public string FisrtEssayToolTipText
        {
            get { return mFisrtEssayToolTipText; }
            set
            {
                if (mFisrtEssayToolTipText != value)
                {
                    mFisrtEssayToolTipText = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("FisrtEssayToolTipText"));
                }
            }
        }

        public string SecondEssayToolTipText
        {
            get { return mSecondEssayToolTipText; }
            set
            {
                if (mSecondEssayToolTipText != value)
                {
                    mSecondEssayToolTipText = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("SecondEssayToolTipText"));
                }
            }
        }

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
