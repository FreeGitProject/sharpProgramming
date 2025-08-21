namespace TestPoly
{
    internal class Program
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
        static void Main(string[] args)
        {
            DerivedClass B = new DerivedClass();
            B.DoWork();  // Calls the new method.//DrivedClass DoWork
            Console.WriteLine();
            BaseClass A = B;
            A.DoWork();  // Also calls the new method.//BaseClass DoWork
        }
    }
}
