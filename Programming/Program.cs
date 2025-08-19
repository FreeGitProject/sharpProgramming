using Programming;
using Programming.Abstract;
using Programming.Dipendency_Injecttion;
using Programming.Encapsulation;
using System.Collections;
using System.Runtime.InteropServices;

namespace HelloWorld
{
    interface IEmplyee
    {
    public string id { get; set; }
        public string name { get; set; }
      //  private dateOfBirth;
   // private city;
string FullName(string name + "city");
    }
    public  class A
    {

        public  void Add()
        {

            Console.WriteLine("Hello A");
        }
        //public abstract void Add();

    }
    public class B : A
    {
        public virtual void Add()
        {
            Console.WriteLine("Hello B");
        }

    }
    public class C : B
    {

       // private static C _c = new C();
        // static C()
        //{
        //    Console.WriteLine("c ka static constructure");
        //}
        //private C()
        //{
        //        Console.WriteLine("Defaut constru");
        //}
        //public C( string g)
        //{
        //    Console.WriteLine(  "parameter");
        //}
        //public static C CreateInstance()
        //{
        //    return _c;
        //}
        public override void Add()
        {
            base.Add();
            Console.WriteLine("Hello C");
        }
        public int GetCustomerInfo(string customerId)

        {
            return 1;
            //Method implementation

        }

        public string GetCustomerInfo(string customerId,int no=0)
        {
            return "d";//string.Empty;
            //Method implementation 
        } 
    }
        class Program
    {
        static void Main(string[] args)
        {
         //   int a = 8;
           // B b1 = new A();
            A objA = new B();
            B objB = new B();
            C objectC = new C();
            // A objC = new C();
            //   C objC = new C();
            // B objB1 = new C();
            //  A a1 = new A();
            objA.Add();//a
            objB.Add();//b
          //  objC.Add();//b
           // objB1.Add();//c
            ArrayList aa = new ArrayList();
            aa.Add("dd");

           
            objectC.Add();

            //Console.Write(objC.GetCustomerInfo("23"));
            //Console.Write(objC.GetCustomerInfo("23",1));

            //var h = C.GetCustomerInfoh("jhf");
            //a1.Add();   //a
            // Console.WriteLine("Hello World!");
            //int[] nums = { 1, 2, 3, 4, 5, 6, 7 };
            //int k = 3;

            // RotateArray.RotateArrayMethod(nums,k);
            //   Solution.PrintArray();
            //Solution.TwoSum();
            //Solution.TwoSumuseDisnary();
            // Students obj = new Students();
            //var s= obj.SetGetName = "asd";
            // Console.WriteLine("Name is : " + s);
            //obj.Name = "";
            //obj.Age = -5;
            //obj.SetName("");
            // obj.GetName();
            //obj.SetAge(-225);
            //obj.GetAge();

            //BankAccount obj = new BankAccount();
            //Console.WriteLine("Intial Balance : " + obj.GetBalance());
            //obj.Deposit(23);
            //Console.WriteLine(obj.GetBalance());



            // Account a = new Account();  
            // a.PrintDetail();
            //CalulatorAbstract calulatorAbstract = new CalulatorAbstract();

        }
    }
}