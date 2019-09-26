using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentClass
{
    class Program
    {
        public static void OnCycleItemsChanged(Object sender, EventArgs args)
        {
            Console.WriteLine("Changes were done");
        }
        static void Main(string[] args)
        {
            CycleItem TestItemOne = new CycleItem();
            CycleItem TestItemTwo = new CycleItem(301, 10, 600);            
            Cycle Experiment = new Cycle();
            
            Experiment.CycleItems.Add(TestItemOne);
            Experiment.CycleItems.AddRange(new SycleItemList<CycleItem> { TestItemTwo, TestItemOne });
            Console.WriteLine($"Created:\t{Experiment.TimeCreated}\nChanged:\t{Experiment.TimeChanged}");
            foreach (var item in Experiment.CycleItems)
            {
                Console.WriteLine($"{item.OnTime}\t{item.OffTime}\t{item.CycleDuration}");
            }

            if (Experiment.Save("SaveTest"))
            {
                Console.WriteLine("Succesfully saved");
            }
            Cycle ExperimentTwo = Cycle.Load("SaveTest.exprmnt");
            

            foreach (var item in ExperimentTwo.CycleItems)
            {
                Console.WriteLine($"{item.OnTime}\t{item.OffTime}\t{item.CycleDuration}");
            }
            Console.ReadKey();
        }
        
    }
}
