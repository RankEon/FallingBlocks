using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Data;

namespace FallingBlocks
{
    class Score : INotifyPropertyChanged
    {
        private string _GameScore;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// GameScore -property, containing the game score.
        /// </summary>
        public String GameScore
        {
            get
            {
                return _GameScore;
            }
            set
            {
                _GameScore = value;
                OnPropertyChanged("GameScore");
            }
        }

        /// <summary>
        /// Handles property change and triggers PropertyChanged event
        /// (implements INotifyPropertyChanged).
        /// </summary>
        /// <param name="property"></param>
        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        
    }
}
