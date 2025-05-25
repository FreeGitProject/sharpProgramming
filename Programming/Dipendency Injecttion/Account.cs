namespace Programming.Dipendency_Injecttion
{
    /// <summary>
    /// this is example of tight couping 
    /// </summary>
    class SavingAccount
    {
        public void  PrintDetail()
        {
            Console.WriteLine("Saving detail");
        }
    }
    class CurrentAccount
    {
        public void PrintDetail( )
        { Console.WriteLine("current detail"); }
    }
    internal class Account
    {
        SavingAccount sa = new SavingAccount();
        CurrentAccount ca = new CurrentAccount();

        public void PrintDetail() { 
            sa.PrintDetail();
            ca.PrintDetail();   
            }
    }
}
