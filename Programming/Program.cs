using Programming;
using Programming.Abstract;
using Programming.Dipendency_Injecttion;
using Programming.Encapsulation;
using System.Collections;
using System.Runtime.InteropServices;

namespace HelloWorld
{
    public class BaseClass
    {
        public virtual void DoWork()
        {
            Console.WriteLine("BaseClass DoWork");
        }
        public virtual int WorkProperty
        {
            get { return 0; }
        }
    }
    public class DerivedClass : BaseClass
    {
       
        public override void DoWork() 
        {
            Console.WriteLine("DrivedClass DoWork");
        }
        public override int WorkProperty
        {
            
            get { return 0; }
        }
        
    }
    class Program
    {
        public static void Main(string[] args)
        {
            DerivedClass B = new DerivedClass();
            B.DoWork();  // Calls the new method.

            BaseClass A = B;
            A.DoWork();  // Also calls the new method.

        }
    }
}
//due to add new project not use console app