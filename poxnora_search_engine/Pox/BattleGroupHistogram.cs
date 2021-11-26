using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine.Pox
{
    public class BattleGroupHistogramInt
    {
        public Dictionary<int, int> DataCount = new Dictionary<int, int>();
        public int RangeStart;
        public int RangeEnd;
        public int RangeStep;
        public List<int> RangeData = new List<int>();

        public void Clear()
        {
            DataCount.Clear();
        }


        public void Add(int key)
        {
            if (!DataCount.ContainsKey(key))
                DataCount.Add(key, 0);

            DataCount[key] += 1;
        }

        public void Remove(int key)
        {
            if(DataCount.ContainsKey(key))
            {
                DataCount[key] -= 1;
                if(DataCount[key] == 0)
                {
                    DataCount.Remove(key);
                }
            }
        }

        public void Update()
        {
            RangeData.Clear();

            // 0-
            RangeData.Add(0);
            // 0-5, 5-10, ...
            for (int i = RangeStart; i < RangeEnd; i+=RangeStep)
                RangeData.Add(0);
            // 30+
            RangeData.Add(0);


            foreach(var kv in DataCount)
            {
                int index = 1 + ((kv.Key - RangeStart) / RangeStep);
                if (index < 0)
                    index = 0;
                if (index >= RangeData.Count)
                    index = RangeData.Count - 1;

                RangeData[index] += kv.Value;
            }
        }

        public int GetColumnCount()
        {
            return RangeData.Count;
        }

        public string GetColumnLabel(int column)
        {
            if (RangeStep == 1)
            {
                if (column <= 0)
                    return string.Format("{0}-", RangeStart);
                else if (column >= RangeData.Count - 1)
                    return string.Format("{0}+", RangeEnd);
                else
                    return string.Format("{0}", RangeStart + column - 1);
            }
            else
            {
                if (column <= 0)
                    return string.Format("{0}-", RangeStart);
                else if (column >= RangeData.Count - 1)
                    return string.Format("{0}+", RangeEnd);
                else
                    return string.Format("{0}-{1}", RangeStart + (column * RangeStep) - RangeStep, RangeStart + (column * RangeStep) - 1);
            }
        }

        public int GetColumnValue(int column)
        {
            return RangeData[column];
        }

        public int GetColumnMaxValue()
        {
            int imax = RangeData[0];
            for (int i = 1; i < RangeData.Count; i++)
                imax = Math.Max(imax, RangeData[i]);

            return imax;
        }

        public int GetColumnMinValue()
        {
            int imin = RangeData[0];
            for (int i = 1; i < RangeData.Count; i++)
                imin = Math.Min(imin, RangeData[i]);

            return imin;
        }

        public int GetKeyAverageValue()
        {
            int sum = 0;
            int count = 0;

            foreach (var kv in DataCount)
            {
                sum += kv.Key * kv.Value;
                count += 1;
            }

            if (count == 0)
                return 0;

            return sum / count;
        }

        public int GetKeySum()
        {
            int sum = 0;

            foreach (var kv in DataCount)
                sum += kv.Key * kv.Value;

            return sum;
        }

        public int GetKeyMaxValue()
        {
            int imax = int.MinValue;
            foreach (var kv in DataCount)
                imax = Math.Max(imax, kv.Key);

            return imax;
        }

        public int GetKeyMinValue()
        {
            int imin = int.MaxValue;
            foreach (var kv in DataCount)
                imin = Math.Min(imin, kv.Key);

            return imin;
        }

        public int GetKeyValue(int key)
        {
            if (!DataCount.ContainsKey(key))
                return 0;

            return DataCount[key];
        }

        public List<int> GetValues()
        {
            List<int> ret = new List<int>();
            foreach(var kv in DataCount)
            {
                for(int i = 0; i < kv.Value; i++)
                {
                    ret.Add(kv.Key);
                }
            }

            ret.Sort();

            return ret;
        }
    }

    public struct HistogramSortValue<T>: IComparable<HistogramSortValue<T>>
    {
        public T key;
        public int value;

        public int CompareTo(HistogramSortValue<T> other)
        {
            return value.CompareTo(other.value);
        }
    }

    public class BattleGroupHistogram<T> where T: IComparable<T>, IEquatable<T>
    {
        public Dictionary<T, int> DataCount = new Dictionary<T, int>();

        public void Clear()
        {
            DataCount.Clear();
        }

        public void Add(T key)
        {
            if (!DataCount.ContainsKey(key))
                DataCount.Add(key, 0);

            DataCount[key] += 1;
        }

        public void Remove(T key)
        {
            if (DataCount.ContainsKey(key))
            {
                DataCount[key] -= 1;
                if (DataCount[key] == 0)
                {
                    DataCount.Remove(key);
                }
            }
        }

        public List<T> GetKeyList(bool sort_by_value = false)
        {
            if (!sort_by_value)
            {
                List<T> keys = DataCount.Keys.ToList();
                keys.Sort();

                return keys;
            }
            else
            {
                List<HistogramSortValue<T>> hsv_list = new List<HistogramSortValue<T>>();
                foreach(var kv in DataCount)
                {
                    hsv_list.Add(new HistogramSortValue<T>() { key = kv.Key, value = kv.Value });
                }
                hsv_list.Sort();

                List<T> ret = new List<T>();
                for(int i = hsv_list.Count-1; i>= 0; i--)
                {
                    ret.Add(hsv_list[i].key);
                }

                return ret;
            }
        }

        public int GetKeyValue(T key)
        {
            if (!DataCount.ContainsKey(key))
                return 0;

            return DataCount[key];
        }
    }
}
