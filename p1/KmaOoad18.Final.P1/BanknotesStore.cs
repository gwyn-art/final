using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.FSharp.Collections;

namespace KmaOoad18.Final.P1
{
    public class BanknotesStore
    {
      private Dictionary<int, int> Store;
      private List<PossibleOut> OutStore;
      private long goal = 0;
      public BanknotesStore()
      {
        Store = new Dictionary<int, int>();
        OutStore = new List<PossibleOut>();
      }

      public BanknotesStore Add(params (int banknote, int qty)[] cash)
      {
        
        foreach (var banknote in cash)
        {
            int availableAmount = 0;
            if (Store.TryGetValue(banknote.Item1, out availableAmount)) {
              Store.Add(banknote.Item1, availableAmount + banknote.Item2);
            } else {
              Store.Add(banknote.Item1, banknote.Item2);
            }
        }

        return this;
      }

      public Dictionary<int, int>.KeyCollection GetBanknotesType()
      {
        return Store.Keys;
      }

      public long getAmount()
      {
        long result = 0;

        foreach (int banknote in Store.Keys)
        {
          int amount = 0;
          Store.TryGetValue(banknote, out amount);
          result += banknote * amount;   
        }

        return result;
      }

      public BanknotesStore Withdraw(long amount)
      {
        goal = amount;
        List<int> suitableBanknotes = new List<int>(Store.Keys);

        // suitableBanknotes = suitableBanknotes.Where(x => x > amount).ToList();
        // suitableBanknotes.Sort((x, y) => x - y);
        
        Change(new List<int>(), suitableBanknotes, 0, 0, amount);
        
        if(OutStore.Count == 0) {
          OutStore = new List<PossibleOut> ();
          return this;
        }

        Out();
        OutStore = new List<PossibleOut> ();

        return this;
      }

      private void Change(List<int> notes, List<int> amounts, int highest, long sum, long goal)
        {
            if (OutStore.Count > 0) {
              return;
            }
            
            if (sum == goal)
            {
                OutStore.Add(new PossibleOut(notes, amounts));
                return;
            }

            if (sum > goal)
            {
                return;
            }

            foreach (int value in amounts)
            {
                int count = notes.Count(x => x == value);
                int avaliable = 0;
                Store.TryGetValue(value, out avaliable);

                if (count >= avaliable) {
                  continue;
                }

                if (value >= highest)
                {
                  List<int> copy = new List<int>(notes);
                  copy.Add(value);
                  Change(copy, amounts, value, sum + value, goal);
                }
            }
        }

      private bool Out () {
        bool result = false;
        
        foreach (var banknotes in  OutStore[0].Store) {
          int avaliable = 0;
          Store.TryGetValue(banknotes.Key, out avaliable);
          if (avaliable == banknotes.Value) {
            Store.Remove(banknotes.Key);
          } else {
            Store.Remove(banknotes.Key);
            Store.Add(banknotes.Key, avaliable - banknotes.Value);
          }
        }

        return result;
      }

      private class PossibleOut
      {
        public Dictionary<int, int> Store;

        public PossibleOut (List<int> notes, List<int> amounts)
        {
          Store = new Dictionary<int, int>();

          foreach (int amount in amounts)
          {
              int count = notes.Count(value => value == amount);
              
              if (count > 0) {
                Store.TryAdd(amount, count);
              }
          }
        }
      }
    }
}