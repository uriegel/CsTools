using CsTools.Extensions;

namespace CsTools.Functional;

public static class Tree
{
    public static IEnumerable<TResult> FlattenTree<T, TState, TResult>(this IEnumerable<T> items, 
            Func<T, TState?, (IEnumerable<T>, TState?)> resolver, Func<T, TState?, TResult> selector, Func<T, TState?, bool> isSubtree, 
            CancellationToken? cancellation = null, Func<TState?, TState?, TState?>? addSubState = null, TState? state = null)
                where TState : class
        => items
            .Aggregate(
                new List<TResult>(), 
                (infos, item) =>  cancellation?.IsCancellationRequested != true
                                    ? isSubtree(item, state)
                                        ? infos.SideEffect(l => l.AddRange(resolver(item, state).Flatten(resolver, selector, isSubtree, cancellation, state, addSubState)))
                                        : infos.SideEffect(l => l.Add(selector(item, state)))
                                    : new List<TResult>());

    static IEnumerable<TResult> Flatten<T, TState, TResult>(this (IEnumerable<T> subItems, TState? state) items, Func<T, TState?, (IEnumerable<T>, TState?)> resolver, 
            Func<T, TState?, TResult> selector, Func<T, TState?, bool> isSubtree, CancellationToken? cancellation, 
                TState? state, Func<TState?, TState?, TState?>? addSubState)
                    where TState : class
        => items.subItems.FlattenTree(resolver, selector, isSubtree, cancellation, addSubState, addSubState != null ? addSubState(state, items.state) : null);
}