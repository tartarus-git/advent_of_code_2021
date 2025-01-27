﻿using System.Reflection;
using AdventOfCode;
using Pos = AdventOfCode.GenericPosition3D<int>;

namespace aoc
{
    public class Day19
    {
        // Beacon Scanner: Map cloud of points onto other clouds

        static List<List<Pos>> ReadData(string file)
        {
            var ret = new List<List<Pos>>();
            var list = new List<Pos>();
            foreach (var s in File.ReadAllLines(ReadInput.GetPath(Day, file)))
            {
                var v = s.Split(',');
                if (s.Contains(','))
                    list.Add(new Pos(int.Parse(v[0]), int.Parse(v[1]), int.Parse(v[2])));
                else
                {
                    if (list.Count > 0)
                        ret.Add(list);
                    list = new List<Pos>();
                }
            }
            if (list.Count > 0)
                ret.Add(list);
            return ret;
        }
        static Pos RotateX90(Pos p) => new(p.x, -p.z, p.y);
        static Pos FlipX(Pos p) => new(-p.x, p.y, -p.z);
        static Pos Y2X(Pos p) => new(p.y, -p.x, p.z);
        static Pos Z2X(Pos p) => new(p.z, p.y, -p.x);
        static List<List<Pos>> Permutations(List<Pos> pos)
        {
            var ret = new List<List<Pos>>();
            ret.Add(pos);
            ret.Add(pos.Select(a => Y2X(a)).ToList());
            ret.Add(pos.Select(a => Z2X(a)).ToList());
            foreach (var l in new List<List<Pos>>(ret))
                ret.Add(l.Select(a => FlipX(a)).ToList());
            foreach (var l in new List<List<Pos>>(ret))
            {
                ret.Add(l.Select(a => RotateX90(a)).ToList());
                ret.Add(ret.Last().Select(a => RotateX90(a)).ToList());
                ret.Add(ret.Last().Select(a => RotateX90(a)).ToList());
            }
            return ret;
        }

        static HashSet<Pos> beacons = new HashSet<Pos>();
        static HashSet<Pos> scanners = new HashSet<Pos>();
        static string fileUsed = "";
        static void CreateWorld(string file)
        {
            fileUsed = file;
            var w = ReadData(file);
            beacons = w[0].ToHashSet();
            scanners.Add(new Pos());
            var todo = w.Skip(1).Select(a => Permutations(a)).ToList();
            while (todo.Count > 0)
            {
                var scannerPerms = todo[0];
                todo.RemoveAt(0);
                bool found = false;
                foreach (var perm in scannerPerms)
                {
                    var deltas = new Dictionary<Pos, int>();
                    foreach (var sp in perm)
                        foreach (var wp in beacons)
                            deltas.Inc(wp - sp, 1);
                    (Pos d, int n) = deltas.ToList().OrderBy(a => a.Value).Last();
                    if (n >= 12)
                    {
                        beacons.UnionWith(perm.Select(a => a + d));
                        scanners.Add(d);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    todo.Add(scannerPerms);
            }
        }
        public static (int n, int dist) Part(string file)
        {
            if (beacons.Count == 0 || file != fileUsed)
                CreateWorld(file);
            int max = scanners.SelectMany(w => scanners, (a, b) => a.ManhattanDistance(b)).Max();
            return (beacons.Count, max);
        }

        public static Object PartA(string file) => Part(file).n;
        public static Object PartB(string file) => Part(file).dist;

        static void Main() => Aoc.Execute(Day, PartA, PartB);
        static string Day => Aoc.Day(MethodBase.GetCurrentMethod()!);
    }
}
