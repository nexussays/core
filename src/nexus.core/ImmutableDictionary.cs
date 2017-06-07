// Copyright Malachi Griffie
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using nexus.core.resharper;

namespace nexus.core
{
   /// <inheritdoc />
   public sealed class ImmutableDictionary<TKey, TValue> : IImmutableDictionary<TKey, TValue>
   {
      private readonly IDictionary<TKey, TValue> m_data;

      /// <summary>
      /// </summary>
      /// <param name="source">
      /// The underlying source for the immutable dictionary. Technically allowing you to make it mutable,
      /// but don't do that.
      /// </param>
      public ImmutableDictionary( IDictionary<TKey, TValue> source = null )
      {
         m_data = source ?? new Dictionary<TKey, TValue>();
      }

      /// <inheritdoc />
      public TValue this[ TKey key ] => m_data[key];

      /// <inheritdoc />
      public Int32 Count => m_data.Count;

      /// <inheritdoc />
      public IEnumerable<TKey> Keys => m_data.Keys;

      /// <inheritdoc />
      public IEnumerable<TValue> Values => m_data.Values;

      /// <inheritdoc />
      public Boolean ContainsKey( TKey key )
      {
         return m_data.ContainsKey( key );
      }

      /// <inheritdoc />
      public Option<TValue> Get( TKey key )
      {
         return m_data.TryGet( key );
      }

      /// <inheritdoc />
      public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
      {
         return m_data.GetEnumerator();
      }

      /// <inheritdoc />
      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }

   /// <summary>
   /// Extension methods for <see cref="IImmutableDictionary{TKey,TValue}" />
   /// </summary>
   [EditorBrowsable( EditorBrowsableState.Never )]
   public static class ImmutableDictionaryExtensions
   {
      /// <summary>
      /// Retrieve a key value pair from the dictionary
      /// </summary>
      public static KeyValuePair<TKey, TValue> GetPair
         <TKey, TValue>( [NotNull] this IImmutableDictionary<TKey, TValue> dict, [NotNull] TKey key )
      {
         Contract.Requires( dict != null );
         return Pair.Of( key, dict[key] );
      }

      /// <summary>
      /// Makethe given dictionary immutable
      /// </summary>
      public static ImmutableDictionary<Tk, Tv> ToImmutable<Tk, Tv>( this IDictionary<Tk, Tv> source )
      {
         return new ImmutableDictionary<Tk, Tv>( source );
      }
   }
}