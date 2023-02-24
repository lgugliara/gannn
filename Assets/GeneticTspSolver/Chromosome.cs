﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneticTspSolver
{
    public class Chromosome<T> : IComparable
    {
        public Population<T> Parent { get; set; }
        public int Id { get; private set; }

        public Fitness<T> Fitness { get; set; } = new Fitness<T>();

        public ArraySegment<T> Values;
        public ArraySegment<Gene<T>> Genes;

        public int GenesCount => Values.Count;

        public static Chromosome<T> Adam { get; set; }

        public Chromosome(Population<T> parent, int id, int genes_count)
        {
            Parent = parent;
            Id = id;
            Fitness = new Fitness<T>();

            Values = new ArraySegment<T>(parent.AllValues, genes_count * id, genes_count);
            Genes = new ArraySegment<Gene<T>>(parent.AllGenes, genes_count * id, genes_count);

            Chromosome<T>.Activate(this, Genes);
        }

        public static void Activate(Chromosome<T> chromosome, ArraySegment<Gene<T>> genes)
        {
            Parallel.ForEach(
                genes,
                (g, s, i) =>
                {
                    if (g == null)
                        genes[(int)i] = new Gene<T>(chromosome, (int)i, chromosome.Values[(int)i]);
                    else
                        genes[(int)i].Value = chromosome.Values[(int)i];
                }
            );
        }

        public override string ToString() => Genes.Select(g => g.ToString()).Aggregate((i, j) => i + ';' + j);

        public static Chromosome<T> From(Chromosome<T> from, int id) => new Chromosome<T>(from.Parent, id, from.GenesCount);
        public static void Copy(Chromosome<T> from, Chromosome<T> to) => Chromosome < T >.Activate(from, to.Genes);

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Chromosome<T> other = obj as Chromosome<T>;
            if (other != null)
                return Fitness.Value.CompareTo(other.Fitness.Value);
            else
                throw new ArgumentException("Object is not a Chromosome");
        }
    }
}
