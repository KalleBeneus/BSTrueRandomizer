using System;
using System.Collections.Generic;
using System.Threading;
using BSTrueRandomizer.Exceptions;
using Microsoft.VisualBasic;

namespace BSTrueRandomizer.service
{
    internal class ItemRandomizerService
    {
        private readonly Random _random;

        public ItemRandomizerService(Random random)
        {
            _random = random;
        }

        public int GetRandomItemIndex(int availableNumberOfItems)
        {
            if (availableNumberOfItems <= 0)
            {
                throw new RandomizationException(
                    "No more items available for randomization. There may be a mismatch of available items and slots to randomize for");
            }

            return _random.Next(availableNumberOfItems);
        }

        public ICollection<int> GetRandomOpenSlotIndexes(int numberOfOpenSlots, int numberOfDesiredSlots)
        {
            if (numberOfOpenSlots < numberOfDesiredSlots)
            {
                throw new ArgumentException(
                    $"Number of open slots provided ({numberOfOpenSlots}) is less than number of items to randomize ({numberOfDesiredSlots})");
            }

            var openSlotIndexes = new SortedSet<int>();
            while (openSlotIndexes.Count < numberOfDesiredSlots)
            {
                openSlotIndexes.Add(_random.Next(numberOfOpenSlots));
            }

            return openSlotIndexes;
        }

        public IEnumerable<T> GetRandomEntriesFromList<T>(IList<T> sortTarget, int numberToTake)
        {
            List<T> result = new List<T>();
            int startIndex = sortTarget.Count - 1;
            for (int i = startIndex; i > startIndex - numberToTake; i--)
            {
                int swapIndex = _random.Next(i + 1);
                result.Add(sortTarget[swapIndex]);
                sortTarget[swapIndex] = sortTarget[i];
            }
            return result;
        }
    }
}