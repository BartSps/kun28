using kun28.Models;
using System.Collections.Generic;

namespace kun28.ViewModel
{
    class MainViewModel:INotify
    {
        public static short STOP = 0;
        public static short START = 1;
        public static short PAUSE = 2;

        //public int WIN_START_X;
        //public int WIN_START_Y;
        //public int WIN_WIDTH;
        //public int WIN_HEIGHT;

        private static int times;

        public int Times
        {
            get => times;
            set
            {
                times = value;
                OnPropertyChanged();
            }
        }

        private short status = 0;

        public short Status { 
            get => status;
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }
    }
}
