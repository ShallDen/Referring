using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Referring.Core
{
    public class ReferringManager : INotifyPropertyChanged 
    {
        private string mOriginalText;
        private string mReferredText;
        private bool mIsPOSDetectionActivated;
        private bool mIsStemmingActivated;
        private bool mIsWordNetActivated;
        private bool mIsWordCutActivated;
        private List<Word> mOrderedWordList;
        private int mOriginalTextSentenceCount;
        private int mReferredTextSentenceCount;

        private static ReferringManager instance;

        public event PropertyChangedEventHandler PropertyChanged;

        protected ReferringManager() { }

        public double ReferringCoefficient { get; set; }
        public double ProgressPercentageCurrent { get; set; }
        public int WordCutLength { get; set; }
        public bool IsReferringRunning { get; set; }
        public bool IsReferringCompete { get; set; }
        public string WordNetDirectory { get; set; }

        public string AutoEssayPath
        {
            get { return FileManager.FileDirectory + "Summary_" + FileManager.FileName; }
        }

        public static ReferringManager Instance
        {
            get { return instance ?? (instance = new ReferringManager()); }
        }

        public string OriginalText
        {
            get { return mOriginalText; }
            set
            {
                if (mOriginalText != value)
                {
                    mOriginalText = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("OriginalText"));
                }
            }
        }

        public string ReferredText
        {
            get { return mReferredText; }
            set
            {
                if (mReferredText != value)
                {
                    mReferredText = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("ReferredText"));
                }
            }
        }

        public bool IsPOSDetectionActivated
        {
            get { return mIsPOSDetectionActivated; }
            set
            {
                if (mIsPOSDetectionActivated != value)
                {
                    mIsPOSDetectionActivated = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsPOSDetectionActivated"));
                }
            }
        }

        public bool IsStemmingActivated
        {
            get { return mIsStemmingActivated; }
            set
            {
                if (mIsStemmingActivated != value)
                {
                    mIsStemmingActivated = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsStemmingActivated"));
                }
            }
        }

        public bool IsWordNetActivated
        {
            get { return mIsWordNetActivated; }
            set
            {
                if (mIsWordNetActivated != value)
                {
                    mIsWordNetActivated = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsWordNetActivated"));
                }
            }
        }

        public bool IsWordCutActivated
        {
            get { return mIsWordCutActivated; }
            set
            {
                if (mIsWordCutActivated != value)
                {
                    mIsWordCutActivated = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsWordCutActivated"));
                }
            }
        }

        public List<Word> OrderedWordList
        {
            get { return mOrderedWordList; }
            set
            {
                if (mOrderedWordList != value)
                {
                    mOrderedWordList = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("OrderedWordList"));
                }
            }
        }

        public int OriginalTextSentenceCount
        {
            get { return mOriginalTextSentenceCount; }
            set
            {
                if (mOriginalTextSentenceCount != value)
                {
                    mOriginalTextSentenceCount = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("OriginalTextSentenceCount"));
                }
            }
        }

        public int ReferredTextSentenceCount
        {
            get { return mReferredTextSentenceCount; }
            set
            {
                if (mReferredTextSentenceCount != value)
                {
                    mReferredTextSentenceCount = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("ReferredTextSentenceCount"));
                }
            }
        }
    }
}

//private bool mIsStemmingIfNoSynsetsFoundActivated;

//public bool IsStemmingIfNoSynsetsFoundActivated
//{
//    get { return mIsStemmingIfNoSynsetsFoundActivated; }
//    set
//    {
//        if (mIsStemmingIfNoSynsetsFoundActivated != value)
//        {
//            mIsStemmingIfNoSynsetsFoundActivated = value;
//            if (PropertyChanged != null)
//                PropertyChanged(this, new PropertyChangedEventArgs("IsStemmingIfNoSynsetsFoundActivated"));
//        }
//    }
//}