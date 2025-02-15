namespace CsTools.Applicative;

public static class Applicative
{
    public static Action Apply<T>(this Action<T> f, T t)
        => () => f(t);
    public static Action<T2> Apply<T1, T2>(this Action<T1, T2> f, T1 t1)
        => t2 => f(t1, t2);
    public static Action<T3> Apply<T1, T2, T3>(this Action<T1, T2, T3> f, T1 t1, T2 t2)
        => t3 => f(t1, t2, t3);
    public static Action Apply<T1, T2, T3>(this Action<T1, T2, T3> f, T1 t1, T2 t2, T3 t3)
        => () => f(t1, t2, t3);
    public static Action Apply<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> f, T1 t1, T2 t2, T3 t3, T4 t4)
        => () => f(t1, t2, t3, t4);
    public static Action Apply<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> f, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        => () => f(t1, t2, t3, t4, t5);
    public static Action Apply<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> f, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        => () => f(t1, t2, t3, t4, t5, t6);


    public static Func<R> Apply<T, R>(this Func<T, R> f, T t)
        => () => f(t);
    public static Func<R> Apply<T1, T2, R>(this Func<T1, T2, R> f, T1 t1, T2 t2)
        => () => f(t1, t2);
    public static Func<R> Apply<T1, T2, T3, R>(this Func<T1, T2, T3, R> f, T1 t1, T2 t2, T3 t3)
        => () => f(t1, t2, t3);
    public static Func<R> Apply<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> f, T1 t1, T2 t2, T3 t3, T4 t4)
        => () => f(t1, t2, t3, t4);
    public static Func<R> Apply<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> f, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        => () => f(t1, t2, t3, t4, t5);

    public static Func<T2, R> Apply<T1, T2, R>(this Func<T1, T2, R> f, T1 t1)
        => t2 => f(t1, t2);
    public static Func<T3, R> Apply<T1, T2, T3, R>(this Func<T1, T2, T3, R> f, T1 t1, T2 t2)
        => t3 => f(t1, t2, t3);
    public static Func<T4, R> Apply<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> f, T1 t1, T2 t2, T3 t3)
        => t4 => f(t1, t2, t3, t4);
    public static Func<T5, R> Apply<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> f, T1 t1, T2 t2, T3 t3, T4 t4)
        => t5 => f(t1, t2, t3, t4, t5);

    public static Func<T2, T3, R> Apply<T1, T2, T3, R>(this Func<T1, T2, T3, R> f, T1 t1)
        => (t2, t3) => f(t1, t2, t3);
    public static Func<T3, T4, R> Apply<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> f, T1 t1, T2 t2)
        => (t3, t4) => f(t1, t2, t3, t4);
    public static Func<T4, T5, R> Apply<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> f, T1 t1, T2 t2, T3 t3)
        => (t4, t5) => f(t1, t2, t3, t4, t5);
    public static Func<T5, T6, R> Apply<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> f, T1 t1, T2 t2, T3 t3, T4 t4)
        => (t5, t6) => f(t1, t2, t3, t4, t5, t6);

    public static Func<T2, T3, T4, R> Apply<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> f, T1 t1)
        => (t2, t3, t4) => f(t1, t2, t3, t4);
    public static Func<T3, T4, T5, R> Apply<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> f, T1 t1, T2 t2)
        => (t3, t4, t5) => f(t1, t2, t3, t4, t5);
    public static Func<T4, T5, T6, R> Apply<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> f, T1 t1, T2 t2, T3 t3)
        => (t4, t5, t6) => f(t1, t2, t3, t4, t5, t6);

    public static Func<T2, T3, T4, T5, R> Apply<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> f, T1 t1)
        => (t2, t3, t4, t5) => f(t1, t2, t3, t4, t5);
    public static Func<T3, T4, T5, T6, R> Apply<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> f, T1 t1, T2 t2)
        => (t3, t4, t5, t6) => f(t1, t2, t3, t4, t5, t6);


    public static Func<T2, T3, T4, T5, T6, R> Apply<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> f, T1 t1)
        => (t2, t3, t4, t5, t6) => f(t1, t2, t3, t4, t5, t6);
}