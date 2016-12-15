using System;
using System.Collections.Generic;
using System.Linq;

namespace PreludeSharp
{
    public interface IAction { void Invoke(); }
    public interface IAction<in T1> { void Invoke(T1 a); }
    public interface IAction<in T1, in T2> { void Invoke(T1 a, T2 b); }
    public interface IAction<in T1, in T2, in T3> { void Invoke(T1 a, T2 b, T3 c); }
    public interface IAction<in T1, in T2, in T3, in T4> { void Invoke(T1 a, T2 b, T3 c, T4 d); }
    public interface IAction<in T1, in T2, in T3, in T4, in T5> { void Invoke(T1 a, T2 b, T3 c, T4 d, T5 e); }

    public interface IFunc<out TR> { TR Invoke(); }
    public interface IFunc<in T1, out TR> { TR Invoke(T1 a); }
    public interface IFunc<in T1, in T2, out TR> { TR Invoke(T1 a, T2 b); }
    public interface IFunc<in T1, in T2, in T3, out TR> { TR Invoke(T1 a, T2 b, T3 c); }

    public class Map<TA, TB> : IFunc<IFunc<TA, TB>, IEnumerable<TA>, IEnumerable<TB>>
    {
        public IEnumerable<TB> Invoke(IFunc<TA, TB> a, IEnumerable<TA> b)
        {
            foreach (var item in b)
            {
                yield return a.Invoke(item);
            }
        }
    }


    public class ZipWith<TA, TB, TC> : IFunc<IFunc<TA, TB, TC>, IEnumerable<TA>, IEnumerable<TB>, IEnumerable<TC>>
    {
        public IEnumerable<TC> Invoke(IFunc<TA, TB, TC> a, IEnumerable<TA> b, IEnumerable<TB> c)
        {
            return b.Zip(c, (bb, cc) => a.Invoke(bb, cc));
        }
    }

    public class TupleCreation<TA, TB> : IFunc<TA, TB, Tuple<TA, TB>>
    {
        public Tuple<TA, TB> Invoke(TA a, TB b)
        {
            return Tuple.Create(a, b);
        }
    }

    public class Zip<TA, TB> : IFunc<IEnumerable<TA>, IEnumerable<TB>, IEnumerable<Tuple<TA, TB>>>
    {
        public IEnumerable<Tuple<TA, TB>> Invoke(IEnumerable<TA> b, IEnumerable<TB> c)
        {
            return new ZipWith<TA, TB, Tuple<TA, TB>>().Invoke(new TupleCreation<TA, TB>(), b, c);
        }
    }

    public class Foldr<TA, TB> : IFunc<IFunc<TA, TB, TB>, TB, IEnumerable<TA>, TB>
    {
        public TB Invoke(IFunc<TA, TB, TB> a, TB b, IEnumerable<TA> c)
        {
            var stack = new Stack<TA>(c);

            var accum = b;

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                accum = a.Invoke(current, accum);
            }

            return accum;
        }
    }

    public class Foldl<TA, TB> : IFunc<IFunc<TB, TA, TB>, TB, IEnumerable<TA>, TB>
    {
        public TB Invoke(IFunc<TB, TA, TB> a, TB b, IEnumerable<TA> c)
        {
            var stack = new Stack<TA>(c);

            var accum = b;

            foreach (var item in c)
            {
                accum = a.Invoke(accum, item);
            }

            return accum;
        }
    }

    public class PlusPlus<TA> : IFunc<IEnumerable<TA>, IEnumerable<TA>, IEnumerable<TA>>
    {
        public IEnumerable<TA> Invoke(IEnumerable<TA> a, IEnumerable<TA> b)
        {
            foreach (var item in a)
            {
                yield return item;
            }

            foreach (var item in b)
            {
                yield return item;
            }
        }
    }

    public class Concat<TA> : IFunc<IEnumerable<IEnumerable<TA>>, IEnumerable<TA>>
    {
        public IEnumerable<TA> Invoke(IEnumerable<IEnumerable<TA>> a)
        {
            return new Foldr<IEnumerable<TA>, IEnumerable<TA>>().Invoke(
                new PlusPlus<TA>(), Enumerable.Empty<TA>(), a);
        }
    }

    public class ReverseOperation<TA> : IFunc<IEnumerable<TA>, TA, IEnumerable<TA>>
    {
        public IEnumerable<TA> Invoke(IEnumerable<TA> a, TA b)
        {
            yield return b;

            foreach (var item in a)
            {
                yield return item;
            }
        }
    }

    public class Reverse<TA> : IFunc<IEnumerable<TA>, IEnumerable<TA>>
    {
        public IEnumerable<TA> Invoke(IEnumerable<TA> a)
        {
            return new Foldl<TA, IEnumerable<TA>>().Invoke(
                new ReverseOperation<TA>(), Enumerable.Empty<TA>(), a);
        }
    }
}
