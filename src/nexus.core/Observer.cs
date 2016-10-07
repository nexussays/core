// Copyright Malachi Griffie
// 
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics.Contracts;

namespace nexus.core
{
   public static class Observer
   {
      public static IObserver<T> Create<T>( Action<T> onNext, Action onComplete = null, Action<Exception> onError = null )
      {
         return new FunctionObserver<T>( onNext, onComplete, onError );
      }

      public static IDisposable Subscribe<T>( this IObservable<T> observable, Action<T> onNext, Action onComplete = null,
                                              Action<Exception> onError = null )
      {
         Contract.Requires( observable != null );
         return observable.Subscribe( Create( onNext, onComplete, onError ) );
      }

      public static IDisposable SubscribeOnComplete<T>( this IObservable<T> observable, Action onComplete )
      {
         Contract.Requires( observable != null );
         return observable.Subscribe( Create<T>( null, onComplete, null ) );
      }

      public static IDisposable SubscribeOnError<T>( this IObservable<T> observable, Action<Exception> onError )
      {
         Contract.Requires( observable != null );
         return observable.Subscribe( Create<T>( null, null, onError ) );
      }

      public static IDisposable SubscribeOnNext<T>( this IObservable<T> observable, Action<T> onNext )
      {
         Contract.Requires( observable != null );
         return observable.Subscribe( Create( onNext, null, null ) );
      }

      private sealed class FunctionObserver<T> : IObserver<T>
      {
         private readonly Action m_onComplete;
         private readonly Action<Exception> m_onError;
         private readonly Action<T> m_onNext;

         public FunctionObserver( Action<T> onNext, Action onComplete = null, Action<Exception> onError = null )
         {
            m_onNext = onNext;
            m_onComplete = onComplete;
            m_onError = onError;
         }

         public void OnCompleted()
         {
            m_onComplete?.Invoke();
         }

         public void OnError( Exception error )
         {
            m_onError?.Invoke( error );
         }

         public void OnNext( T value )
         {
            m_onNext?.Invoke( value );
         }
      }
   }
}