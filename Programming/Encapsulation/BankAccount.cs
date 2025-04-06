using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programming.Encapsulation
{
    internal class BankAccount
    {
        private decimal balance;
        public void Deposit(decimal amount)
        {
            if (amount > 0)
            {
                balance += amount;
            }
        }
        public void Withdraw(decimal amount)
        {
            if (amount > 0 && amount <= balance)
            {
                balance -= amount;
            }
        }
        public decimal GetBalance()
        {
            return balance;
        }
    }
}
