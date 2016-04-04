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
        private bool mIsPOSDetectionActivated;
        private bool mIsStemmingActivated;
        private bool mIsWordNetActivated;
        private List<Word> mOrderedWordList;
        private static ReferringManager instance;

        public event PropertyChangedEventHandler PropertyChanged;

        protected ReferringManager() { }

        public string OriginalText { get; set; }
        public string ReferredText { get; set; }
        public double ReferringCoefficient { get; set; }
        public bool IsReferringCompete { get; set; }

        public static ReferringManager Instance
        {
            get { return instance ?? (instance = new ReferringManager()); }
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