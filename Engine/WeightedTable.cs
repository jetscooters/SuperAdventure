using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class WeightedTable<T>
    {
        public List<TableItem<T>> Table;

        public WeightedTable()
        {
            Table = new List<TableItem<T>>();
        }

        public T GetRandomItemFromWeightedTable()
        {
            int sumOfWeights = Table.Sum(ti => ti.Weight);
            int randNum = (int)(RandomNumberGenerator.RandomDouble() * sumOfWeights) + 1;
            int accumulator = 0;

            for (int i = 0; i < Table.Count; i++)
            {
                accumulator += Table[i].Weight;
                if (randNum <= accumulator)
                {
                    return Table[i].Details;
                }
            }
            
            return default(T);
        }

        public void AddItem(T item, int weight)
        {
            Table.Add(new TableItem<T>(item, weight));
            SortTable();
        }

        private void SortTable()
        {
            //Sort table by weight
            Table.OrderBy(ti => ti.Weight);
        }
    }

    public class TableItem<T>
    {
        public T Details;
        public int Weight;

        public TableItem(T item, int weight)
        {
            Details = item;
            Weight = weight;
        }
    }
}
