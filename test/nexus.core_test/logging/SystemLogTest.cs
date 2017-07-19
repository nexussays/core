﻿using System;
using System.Diagnostics.Contracts;
using nexus.core;
using nexus.core.logging;
using nexus.core.logging.sink;
using nexus.core.time;
using NUnit.Framework;

namespace nexus.core_test.logging
{
   [TestFixture]
   internal class SystemLogTest
   {
      private SystemLog m_log;

      [SetUp]
      public void Setup()
      {
         m_log = new SystemLog( new DefaultTimeProvider(), 50 );
      }

      [TestCase( LogLevel.Error )]
      [TestCase( LogLevel.Warn )]
      [TestCase( LogLevel.Info )]
      [TestCase( LogLevel.Trace )]
      public void after_writing_1_log_entry_a_newly_attached_sink_will_receive_that_entry( LogLevel level )
      {
         const Int32 count = 1;
         WriteStringsToLog( level, count );
         var handledEntries = 0;
         Action<ILogEntry> handler = e => { handledEntries++; };
         Contract.Requires<ArgumentNullException>( handler != null );
         m_log.AddSink( new ActionLogSink( handler ) );
         Assert.That( handledEntries, Is.EqualTo( count ) );
      }

      [TestCase( LogLevel.Error )]
      [TestCase( LogLevel.Warn )]
      [TestCase( LogLevel.Info )]
      [TestCase( LogLevel.Trace )]
      public void after_writing_1_log_entry_a_previously_attached_sink_will_receive_that_entry( LogLevel level )
      {
         const Int32 count = 1;
         var handledEntries = 0;
         Action<ILogEntry> handler = e => { handledEntries++; };
         Contract.Requires<ArgumentNullException>( handler != null );
         m_log.AddSink( new ActionLogSink( handler ) );
         WriteStringsToLog( level, count );
         Assert.That( handledEntries, Is.EqualTo( count ) );
      }

      [TestCase( LogLevel.Error )]
      [TestCase( LogLevel.Warn )]
      [TestCase( LogLevel.Info )]
      [TestCase( LogLevel.Trace )]
      public void after_writing_1_log_entry_a_newly_attached_sink_will_receive_the_proper_message( LogLevel level )
      {
         const Int32 count = 1;
         var writtenValue = WriteStringsToLog( level, count );
         var check = AddSinkCheckEntryMessages( writtenValue );
         Assert.That( check.Item2, Is.Null, "Expected log message \"{0}\", actual message: ".F( check.Item1 ) );
      }

      [TestCase( LogLevel.Error )]
      [TestCase( LogLevel.Warn )]
      [TestCase( LogLevel.Info )]
      [TestCase( LogLevel.Trace )]
      public void after_writing_1_log_entry_a_previously_attached_sink_will_receive_the_proper_message( LogLevel level )
      {
         const Int32 count = 1;
         String entryValue = null;
         Action<ILogEntry> handler = e => { entryValue = e.DebugMessage; };
         Contract.Requires<ArgumentNullException>( handler != null );
         m_log.AddSink( new ActionLogSink( handler ) );
         var writtenValue = WriteStringsToLog( level, count );
         Assert.That( entryValue, Is.EqualTo( writtenValue[0] ) );
      }

      [TestCase( LogLevel.Error )]
      [TestCase( LogLevel.Warn )]
      [TestCase( LogLevel.Info )]
      [TestCase( LogLevel.Trace )]
      public void
         after_writing_log_entries_to_fill_the_buffer_a_newly_attached_sink_will_receive_them_all_in_sequence_order(
            LogLevel level )
      {
         WriteStringsToLog( level, m_log.LogBufferSize );
         var seqCheck = AddSinkCheckSequence( 0 );
         Assert.That(
            seqCheck.Item1,
            Is.EqualTo( m_log.LogBufferSize ),
            "Expected sequence number {0} and got {1}".F( seqCheck.Item1, seqCheck.Item2 ) );
      }

      [TestCase( LogLevel.Error )]
      [TestCase( LogLevel.Warn )]
      [TestCase( LogLevel.Info )]
      [TestCase( LogLevel.Trace )]
      public void after_writing_log_entries_to_fill_the_buffer_a_newly_attached_sink_will_receive_all_entries(
         LogLevel level )
      {
         WriteStringsToLog( level, m_log.LogBufferSize );
         var handledEntries = 0;
         Action<ILogEntry> handler = e => { handledEntries++; };
         Contract.Requires<ArgumentNullException>( handler != null );
         m_log.AddSink( new ActionLogSink( handler ) );
         Assert.That( handledEntries, Is.EqualTo( m_log.LogBufferSize ) );
      }

      [TestCase( LogLevel.Error )]
      [TestCase( LogLevel.Warn )]
      [TestCase( LogLevel.Info )]
      [TestCase( LogLevel.Trace )]
      public void
         after_writing_log_entries_to_fill_the_buffer_a_previously_attached_sink_will_have_received_all_entries(
            LogLevel level )
      {
         var handledEntries = 0;
         Action<ILogEntry> handler = e => { handledEntries++; };
         Contract.Requires<ArgumentNullException>( handler != null );
         m_log.AddSink( new ActionLogSink( handler ) );
         WriteStringsToLog( level, m_log.LogBufferSize );
         Assert.That( handledEntries, Is.EqualTo( m_log.LogBufferSize ) );
      }

      [TestCase( LogLevel.Error )]
      [TestCase( LogLevel.Warn )]
      [TestCase( LogLevel.Info )]
      [TestCase( LogLevel.Trace )]
      public void
         after_writing_log_entries_to_overfill_the_buffer_a_newly_attached_sink_will_receive_bufferlength_most_recent_entries(
            LogLevel level )
      {
         const Int32 count = 72;
         WriteStringsToLog( level, count );
         var handledEntries = 0;
         Action<ILogEntry> handler = e => { handledEntries++; };
         Contract.Requires<ArgumentNullException>( handler != null );
         m_log.AddSink( new ActionLogSink( handler ) );
         Assert.That( handledEntries, Is.EqualTo( m_log.LogBufferSize ) );
      }

      [TestCase( LogLevel.Error )]
      [TestCase( LogLevel.Warn )]
      [TestCase( LogLevel.Info )]
      [TestCase( LogLevel.Trace )]
      public void after_writing_log_entries_to_overfill_the_buffer_an_attached_sink_will_have_received_all_entries(
         LogLevel level )
      {
         const Int32 count = 72;
         var handledEntries = 0;
         Action<ILogEntry> handler = e => { handledEntries++; };
         Contract.Requires<ArgumentNullException>( handler != null );
         m_log.AddSink( new ActionLogSink( handler ) );
         WriteStringsToLog( level, count );
         Assert.That( handledEntries, Is.EqualTo( count ) );
      }

      [TestCase( LogLevel.Error )]
      [TestCase( LogLevel.Warn )]
      [TestCase( LogLevel.Info )]
      [TestCase( LogLevel.Trace )]
      public void
         after_writing_log_entries_to_overfill_the_buffer_a_newly_attached_sink_will_receive_bufferlength_most_recent_entries_in_seq_order(
            LogLevel level )
      {
         const Int32 count = 68;
         WriteStringsToLog( level, count );
         var seqCheck = AddSinkCheckSequence( count - m_log.LogBufferSize );
         Assert.That(
            seqCheck.Item1,
            Is.EqualTo( count ),
            "Expected sequence number {0} and got {1}".F( seqCheck.Item1, seqCheck.Item2 ) );
      }

      private String[] WriteStringsToLog( LogLevel level, Int32 count )
      {
         var results = new String[count];
         for(var x = 0; x < count; ++x)
         {
            var message = TestContext.CurrentContext.Random.GetString( 20 );
            results[x] = "test {0} {1}".F( x, message );
            m_log.Write( level, "test {0} {1}", x, message );
         }
         return results;
      }

      private Tuple<String, String> AddSinkCheckEntryMessages( String[] expectedMessages )
      {
         var index = 0;
         String failedMessage = null;
         String expectedMessage = null;
         Action<ILogEntry> handler = e =>
         {
            if(failedMessage != null)
            {
               // if we've already failed just no-op
               return;
            }
            var msg = e.DebugMessage;
            expectedMessage = expectedMessages[index];
            if(msg != expectedMessages[index])
            {
               failedMessage = msg;
               return;
            }
            index++;
         };
         Contract.Requires<ArgumentNullException>( handler != null );
         m_log.AddSink(
            new ActionLogSink( handler ) );
         return Tuple.Create( expectedMessage, failedMessage );
      }

      private Tuple<Int32, Int32> AddSinkCheckSequence( Int32 initialSequenceNumber )
      {
         var expectedSeqNumber = initialSequenceNumber;
         var failedSeqNumber = -1;
         Action<ILogEntry> handler = e =>
         {
            if(failedSeqNumber != -1)
            {
               // if we've already failed just no-op
               return;
            }
            if(e.SequenceId != expectedSeqNumber)
            {
               failedSeqNumber = e.SequenceId;
               return;
            }
            expectedSeqNumber++;
         };
         Contract.Requires<ArgumentNullException>( handler != null );
         m_log.AddSink(
            new ActionLogSink( handler ) );
         return Tuple.Create( expectedSeqNumber, failedSeqNumber );
      }

      [Test]
      public void default_log_level_is_trace()
      {
         Assert.That( m_log.CurrentLevel, Is.EqualTo( LogLevel.Trace ) );
      }
   }
}