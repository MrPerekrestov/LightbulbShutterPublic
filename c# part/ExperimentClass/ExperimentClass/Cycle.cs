using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentClass
{
    [Serializable]
   public class SycleItemList<CycleItem>:List<CycleItem>
    {
        public new void Add(CycleItem item)
        {
            base.Add(item);
          //  OnCycleItemsChange();
        }
        public event EventHandler CycleItemsChanged;
        private void OnCycleItemsChange()
        {
            CycleItemsChanged?.Invoke(this, new EventArgs { });
        }        
    }
    [Serializable]
   public class Cycle: IEnumerable<CycleItem>,ISerializable
    {
        private DateTime    _TimeCreated;
        private DateTime    _TimeChanged;
        private Boolean     _ChangesDone;
        private Int16       _CycleNumber;
        private SycleItemList<CycleItem> _CycleItems;

        public Int16 CycleNumber
        {
            get { return _CycleNumber; }
            set
            {
                if ((value < 1) || (value > 100)) throw new ArgumentOutOfRangeException("CycleNumber should be in a range from 1 to 100");
                _CycleNumber = value;
            }
        }

        public DateTime TimeCreated
        {
            get { return _TimeCreated; }
        }

        public DateTime TimeChanged
        {
            get { return _TimeChanged; }
        }

        public Boolean ChangesDone
        {
            set
            {
                _ChangesDone = value;
                _TimeChanged = DateTime.Now;
            }
            get { return _ChangesDone; }
        }

        public SycleItemList<CycleItem> CycleItems
        {
            get { return _CycleItems; }
            set { _CycleItems = value; }
        }

        public IEnumerator<CycleItem> GetEnumerator()
        {
            return _CycleItems.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        

        public Cycle()
        {
            _TimeCreated = DateTime.Now;
            _TimeChanged = DateTime.Now;
            _ChangesDone = false;
            CycleItems = new SycleItemList<CycleItem>();
        }
        public Cycle(SycleItemList<CycleItem> ListOfCycleItems)
        {
            _TimeCreated = DateTime.Now;
            _TimeChanged = DateTime.Now;
            _ChangesDone = false;
            CycleItems = ListOfCycleItems;
        }
        public Boolean Save(string FileName)
        {
            try
            {
                using (FileStream fs = new FileStream(FileName, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    this.ChangesDone = false;
                    bf.Serialize(fs, this);
                }
                return true;
            }
            catch 
            {
                return false;
                throw new Exception("Cycle save error");
            }
           
        }

        public static Cycle Load(string FileName)
        {
            try
            {
                using (FileStream fs = new FileStream(FileName, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return (Cycle)bf.Deserialize(fs);                    
                }
            }
            catch
            {
                throw new Exception("Cycle load error");
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("TimeCreated", TimeCreated);
            info.AddValue("TimeChanged", TimeChanged);
            info.AddValue("ChangesDone", ChangesDone);
            info.AddValue("CycleItems", CycleItems);
            info.AddValue("CycleNumber", CycleNumber);
        }
        public Cycle(SerializationInfo info, StreamingContext context)
        {
            _TimeCreated = (DateTime)info.GetValue("TimeCreated", typeof(DateTime));
            _TimeChanged = (DateTime)info.GetValue("TimeChanged", typeof(DateTime));
            _ChangesDone = (Boolean)info.GetValue("ChangesDone",typeof(Boolean));
            CycleItems = (SycleItemList<CycleItem>)info.GetValue("CycleItems", typeof(SycleItemList<CycleItem>));
            _CycleNumber = (Int16)info.GetValue("CycleNumber", typeof(Int16));
        }
        
        public event EventHandler CycleItemsChange;
        public virtual void OnCycleItemsChange()
        {
            CycleItemsChange?.Invoke(this, new EventArgs { });
        }

    }
}
