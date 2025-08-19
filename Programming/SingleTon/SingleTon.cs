using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programming.SingleTon
{
    internal class SingleTon
    {
        private static SingleTon Instance;

        private SingleTon()
        {
            
        }
        public static  SingleTon GetInstance()
        { 
            if(Instance is null)
            {
                Instance = new SingleTon();
            }
            return Instance;
        }

        public void Print()
        {
            Console.WriteLine("One Instance");
        }
    }

    class Program1
    {
        static void Main(string[] args)
        {
            SingleTon singleTon = SingleTon.GetInstance();
            singleTon.Print();
        }
    }
}
