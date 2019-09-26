using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentClass
{   [Serializable]
    public class CycleItem: ISerializable
    {
        private Int16 _CycleDuration;

        private Int16 _OnTime;

        private Int16 _OffTime;
        
        public Int16 CycleDuration
        {
            get { return _CycleDuration; }
            set
            {
                if ((value > 600)||(value<(_OffTime+_OnTime)))
                {                    
                    throw new ArgumentOutOfRangeException("CycleDuration should be in a range from OnTime+OffTime to 600 s");                    
                }
                else _CycleDuration = value;
            }
        }

        public Int16 OnTime
        {
            get { return _OnTime; }
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException("OnTime could not be less then 1 s");
                if (value > 300) throw new ArgumentOutOfRangeException("OnTime could not be more then 300 s");
                _OnTime = value;
            }
        }

        public Int16 OffTime
        {
            get { return _OffTime; }
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException("OffTime could not be less then 1");
                if (value > 300) throw new ArgumentOutOfRangeException("OnTime could not be more then 300 s");
                _OffTime = value;
            }
        }
        public CycleItem()
        {
            OnTime          =   1;
            OffTime         =   1;
            CycleDuration   =   10;
        }
        public CycleItem (Int16 On, Int16 Off, Int16 Duration)
        {
            OnTime          =   On;
            OffTime         =   Off;
            CycleDuration   =   Duration;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("OnTime", OnTime);
            info.AddValue("OffTime", OffTime);
            info.AddValue("CycleDuration", CycleDuration);
        }
        public CycleItem(SerializationInfo info, StreamingContext context)
        {
            OnTime           =    (Int16)info.GetValue("OnTime", typeof(Int16));
            OffTime          =    (Int16)info.GetValue("OffTime", typeof(Int16));
            CycleDuration    =    (Int16)info.GetValue("CycleDuration", typeof(Int16));
        }
      
    }
}
