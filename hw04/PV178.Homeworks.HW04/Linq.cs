using System;
using System.Collections.Generic;
using System.Linq;

namespace PV178.Homeworks.HW04
{
    public static class Linq
    {
        /// <summary>
        /// Check if collection is subset of another collection.
        /// </summary>
        /// <typeparam name="T">Type of items in collections.</typeparam>
        /// <param name="subset">Subset to check.</param>
        /// <param name="superset">Superset to compare.</param>
        /// <returns>True if every item in subset is also in superset, false otherwise.</returns>
        public static bool IsSubsetOf<T>(this IEnumerable<T> subset, IEnumerable<T> superset)
        {
            return !subset.Except(superset).Any();
        }

        /// <summary>
        /// Identity function. return its input
        /// </summary>
        /// <typeparam name="T">Type of input.</typeparam>
        /// <param name="item">Input.</param>
        /// <returns>Passed input.</returns>
        public static T Id<T>(T item)
        {
            return item;
        }

        /// <summary>
        /// Inner join (equal to Join extension) without usage of join.
        /// Does not preserve order of Join!!
        /// 
        /// Correlates the elements of two sequences based on matching keys. The default 
        /// equality comparer is used to compare keys.
        /// (Copypasted from Join documentation)
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <typeparam name="TResult">The type of the result elements.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
        /// <returns>An IEnumerable that has elements of type TResult 
        /// that are obtained by performing an inner join on two sequences.</returns>
        public static IEnumerable<TResult> ZipJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector
        )
        {
            return outer.PrepareToJoin(inner, outerKeySelector, innerKeySelector)
                .Zip(
                    inner.PrepareToJoin(outer, innerKeySelector, outerKeySelector),
                    (outerGroup, innerGroup) => outerGroup
                        .SelectMany(
                            outerItem => Enumerable
                                .Repeat(outerItem, innerGroup.Count())
                                .Zip(innerGroup, resultSelector)
                        )
                ).SelectMany(Id);
        }

        /// <summary>
        /// Extract distinct keys from collection of elements.
        /// Filter null values
        /// </summary>
        /// <typeparam name="TKey">Type of key returned by the key selector function.</typeparam>
        /// <typeparam name="T">Type of elements in collection.</typeparam>
        /// <param name="collection">Collection to project.</param>
        /// <param name="collectionKeySelector">Function to extract key from element.</param>
        /// <returns>Collection of distinct keys (without null values) of input elements.</returns>
        public static IEnumerable<TKey> Project<TKey, T>(
            this IEnumerable<T> collection,
            Func<T, TKey> collectionKeySelector
        )
        {
            return collection.Select(collectionKeySelector).Where(key => key != null).Distinct();
        }

        /// <summary>
        /// Prepare collection to join.
        /// Filter out elements with null keys or keys not present in other collection, 
        /// then order and group by key.
        /// </summary>
        /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
        /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
        /// <returns>Collection prepared to join.</returns>
        private static IEnumerable<IGrouping<TKey, TOuter>> PrepareToJoin<TOuter, TInner, TKey>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector
        )
        {
            var innerKeys = new HashSet<TKey>(inner.Project(innerKeySelector));
            return outer
                .Where(outerItem => innerKeys.Contains(outerKeySelector(outerItem)))
                .OrderBy(outerKeySelector)
                .GroupBy(outerKeySelector);
        }
    }
}